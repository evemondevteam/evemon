using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.PatchXml;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    /// <summary>
    /// Takes care of looking for new versions of EVEMon and its datafiles.
    /// </summary>
    public static class UpdateManager
    {
        private static readonly TimeSpan s_frequency = TimeSpan.FromMinutes(Settings.Updates.
            UpdateFrequency);

        private static bool s_checkScheduled;
        private static bool s_enabled;

        /// <summary>
        /// Gets or sets whether the autoupdater is enabled.
        /// </summary>
        public static bool Enabled
        {
            get { return s_enabled; }
            set
            {
                s_enabled = value;

                if (!s_enabled)
                    return;

                if (s_checkScheduled)
                    return;

                // Schedule a check in 10 seconds
                ScheduleCheck(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Deletes the installation files.
        /// </summary>
        public static void DeleteInstallationFiles()
        {
            foreach (string file in Directory.GetFiles(EveMonClient.EVEMonDataDir,
                "EVEMon-install-*.exe", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    FileInfo installationFile = new FileInfo(file);
                    if (!installationFile.Exists)
                        continue;

                    FileHelper.DeleteFile(installationFile.FullName);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }

        /// <summary>
        /// Deletes the data files.
        /// </summary>
        public static void DeleteDataFiles()
        {
            foreach (string file in Datafile.GetFilesFrom(EveMonClient.EVEMonDataDir,
                Datafile.DatafilesExtension).Concat(Datafile.GetFilesFrom(EveMonClient.
                EVEMonDataDir, Datafile.OldDatafileExtension)))
            {
                try
                {
                    FileInfo dataFile = new FileInfo(file);
                    if (dataFile.Exists)
                        FileHelper.DeleteFile(dataFile.FullName);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }

        /// <summary>
        /// Schedules a check a specified time period in the future.
        /// </summary>
        /// <param name="time">Time period in the future to start check.</param>
        private static void ScheduleCheck(TimeSpan time)
        {
            s_checkScheduled = true;
            Dispatcher.Schedule(time, () => BeginCheckAsync().ConfigureAwait(false));
            EveMonClient.Trace("in " + time);
        }

        /// <summary>
        /// Performs the check asynchronously.
        /// </summary>
        /// <remarks>
        /// Invoked on the UI thread the first time and on some background thread the rest of the time.
        /// </remarks>
        private static async Task BeginCheckAsync()
        {
            // If update manager has been disabled since the last
            // update was triggered quit out here
            if (!s_enabled)
                s_checkScheduled = false;
            // No connection ? Recheck in one minute
            else if (!NetworkMonitor.IsNetworkAvailable)
                ScheduleCheck(TimeSpan.FromMinutes(1));
            else
            {
                string updateAddress = NetworkConstants.GitHubBase + NetworkConstants.
                    EVEMonUpdates;
                string emergAddress = updateAddress.Replace(".xml", string.Empty) +
                    "-emergency.xml";
                // Otherwise, query for the patch file
                // First look up for an emergency patch
                await Util.DownloadXmlAsync<SerializablePatch>(new Uri(emergAddress)).
                    ContinueWith(async task =>
                    {
                        var result = task.Result;
                        // If no emergency patch found proceed with the regular
                        if (result.Error != null)
                            result = await Util.DownloadXmlAsync<SerializablePatch>(new Uri(
                                updateAddress));
                        // Proccess the result
                        OnCheckCompleted(result);
                    }, EveMonClient.CurrentSynchronizationContext).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Called when patch file check completed.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnCheckCompleted(DownloadResult<SerializablePatch> result)
        {
            // If update manager has been disabled since the last
            // update was triggered quit out here
            if (!s_enabled)
            {
                s_checkScheduled = false;
                return;
            }

            // Was there an error ?
            if (result.Error != null)
            {
                // Logs the error and reschedule
                EveMonClient.Trace($"UpdateManager - {result.Error.Message}",
                    printMethod: false);
                ScheduleCheck(TimeSpan.FromMinutes(1));
                return;
            }

            try
            {
                // No error, let's try to deserialize
                ScanUpdateFeed(result.Result);
            }
            catch (InvalidOperationException exc)
            {
                // An error occurred during the deserialization
                ExceptionHandler.LogException(exc, true);
            }
            finally
            {
                EveMonClient.Trace();

                // Reschedule
                ScheduleCheck(s_frequency);
            }
        }

        /// <summary>
        /// Scans the update feed.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void ScanUpdateFeed(SerializablePatch result)
        {
            Version currentVersion = Version.Parse(EveMonClient.FileVersionInfo.FileVersion);
            SerializableRelease newestRelease = result.Releases?.FirstOrDefault(
                release => Version.Parse(release.Version).Major == currentVersion.Major);

            Version newestVersion = (newestRelease != null) ? Version.Parse(newestRelease.
                Version) : currentVersion;
            Version mostRecentDeniedVersion = !string.IsNullOrEmpty(Settings.Updates.
                MostRecentDeniedUpgrade) ? new Version(Settings.Updates.
                MostRecentDeniedUpgrade) : new Version();

            // Is the program out of date and user has not previously denied this version?
            if (currentVersion < newestVersion & mostRecentDeniedVersion < newestVersion)
            {
                // Quit if newest release is null
                // (Shouldn't happen but it's nice to be prepared)
                if (newestRelease == null)
                    return;
                // Reset the most recent denied version
                Settings.Updates.MostRecentDeniedUpgrade = string.Empty;

                Uri forumUrl = new Uri(newestRelease.TopicAddress);
                Uri installerUrl = new Uri(newestRelease.PatchAddress);
                string updateMessage = newestRelease.Message;
                string installArgs = newestRelease.InstallerArgs;
                string md5Sum = newestRelease.MD5Sum;
                string additionalArgs = newestRelease.AdditionalArgs;
                bool canAutoInstall = !string.IsNullOrEmpty(installerUrl.AbsoluteUri) &&
                    !string.IsNullOrEmpty(installArgs);

                if (!string.IsNullOrEmpty(additionalArgs) && additionalArgs.Contains(
                    "%EVEMON_EXECUTABLE_PATH%"))
                {
                    string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                    installArgs = $"{installArgs} {additionalArgs}";
                    installArgs = installArgs.Replace("%EVEMON_EXECUTABLE_PATH%", appPath);
                }

                // Requests a notification to subscribers and quit
                EveMonClient.OnUpdateAvailable(forumUrl, installerUrl, updateMessage,
                    currentVersion, newestVersion, md5Sum, canAutoInstall, installArgs);
                return;
            }

            // New data files released?
            if (result.FilesHaveChanged)
            {
                // Requests a notification to subscribers
                EveMonClient.OnDataUpdateAvailable(result.ChangedDatafiles);
                return;
            }

            // Notify about a new major version
            Version newestMajorVersion = result.Releases?.Max(release => Version.Parse(
                release.Version)) ?? new Version();
            SerializableRelease newestMajorRelease = result.Releases?.FirstOrDefault(release =>
                Version.Parse(release.Version) == newestMajorVersion);
            if (newestMajorRelease == null)
                return;
            newestVersion = Version.Parse(newestMajorRelease.Version);
            Version mostRecentDeniedMajorUpgrade = !string.IsNullOrEmpty(Settings.Updates.
                MostRecentDeniedMajorUpgrade)
                ? new Version(Settings.Updates.MostRecentDeniedMajorUpgrade)
                : new Version();

            // Is there is a new major version and the user has not previously denied it?
            if (currentVersion >= newestVersion | mostRecentDeniedMajorUpgrade >= newestVersion)
                return;
            // Reset the most recent denied version
            Settings.Updates.MostRecentDeniedMajorUpgrade = string.Empty;
            EveMonClient.OnUpdateAvailable(null, null, null, currentVersion, newestVersion,
                null, false, null);
        }

        /// <summary>
        /// Replaces the datafile.
        /// </summary>
        /// <param name="oldFilename">The old filename.</param>
        /// <param name="newFilename">The new filename.</param>
        public static void ReplaceDatafile(string oldFilename, string newFilename)
        {
            try
            {
                FileHelper.DeleteFile($"{oldFilename}.bak");
                File.Copy(oldFilename, $"{oldFilename}.bak");
                FileHelper.DeleteFile(oldFilename);
                File.Move(newFilename, oldFilename);
            }
            catch (ArgumentException ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
            catch (IOException ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
            catch (UnauthorizedAccessException ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
        }
    }
}
