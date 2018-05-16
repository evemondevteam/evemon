using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;

namespace EVEMon.Common.Service
{
    public static class EveNotificationType
    {
        private static Dictionary<int, SerializableNotificationRefTypesListItem> s_notificationRefTypes =
            new Dictionary<int, SerializableNotificationRefTypesListItem>(128);
        private static DateTime s_cachedUntil;
        private static DateTime s_nextCheckTime;
        private static bool s_queryPending;
        private static bool s_loaded;

        private const string Filename = "NotificationRefTypes";

        #region Helper Methods

        /// <summary>
        /// Gets the description of the notification type.
        /// </summary>
        /// <param name="typeID">The type ID.</param>
        /// <returns></returns>
        internal static string GetName(int typeID)
        {
            EnsureLoaded();

            SerializableNotificationRefTypesListItem type;
            s_notificationRefTypes.TryGetValue(typeID, out type);
            return type?.TypeName ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the ID of the notification.
        /// </summary>
        /// <param name="typeID">The type name.</param>
        /// <returns>The type ID.</returns>
        internal static int GetID(string name)
        {
            EnsureLoaded();

            SerializableNotificationRefTypesListItem type = s_notificationRefTypes.Values.
                FirstOrDefault(x => x.TypeCode?.Equals(name, StringComparison.
                InvariantCultureIgnoreCase) ?? false);
            return type?.TypeID ?? 0;
        }

        /// <summary>
        /// Gets the subject layout.
        /// </summary>
        /// <param name="typeID">The type identifier.</param>
        /// <returns></returns>
        internal static string GetSubjectLayout(int typeID)
        {
            EnsureLoaded();

            SerializableNotificationRefTypesListItem type;
            s_notificationRefTypes.TryGetValue(typeID, out type);
            return type?.SubjectLayout ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the text layout.
        /// </summary>
        /// <param name="typeID">The type identifier.</param>
        /// <returns></returns>
        internal static string GetTextLayout(int typeID)
        {
            if (EveMonClient.IsDebugBuild)
                EnsureInitialized();
            else
                EnsureImportation();

            SerializableNotificationRefTypesListItem type;
            s_notificationRefTypes.TryGetValue(typeID, out type);
            return type?.TextLayout ?? string.Empty;
        }

        #endregion


        #region Importation

        /// <summary>
        /// Ensrues the notification types data has been loaded from the proper source.
        /// </summary>
        private static void EnsureLoaded()
        {
            /*if (EveMonClient.IsDebugBuild)
                EnsureInitialized();
            else
                EnsureImportation();*/
            // Unable to find notification ref types in the SDE, and ESI has swapped from the
            // old ints to a new naming scheme. Updated the definition of the ref types XML
            // and use only the local version...
            EnsureInitialized();
        }

        /// <summary>
        /// Ensures the notification types data have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_loaded)
                return;

            var result = Util.DeserializeAPIResultFromString<SerializableNotificationRefTypes>(
                Properties.Resources.NotificationRefTypes, APIProvider.RowsetsTransform);

            Import(result.Result);
        }

        /// <summary>
        /// Ensures the importation.
        /// </summary>
        private static void EnsureImportation()
        {
            // Quit if we already checked a minute ago or query is pending
            if (s_nextCheckTime > DateTime.UtcNow || s_queryPending)
                return;

            s_nextCheckTime = DateTime.UtcNow.AddMinutes(1);

            string filename = LocalXmlCache.GetFileInfo(Filename).FullName;

            // Update the file if we don't have it or the data have expired
            if (!File.Exists(filename) || (s_loaded && s_cachedUntil < DateTime.UtcNow))
            {
                Task.WhenAll(UpdateFileAsync());
                return;
            }

            // Exit if we have already imported the list
            if (s_loaded)
                return;

            s_cachedUntil = File.GetLastWriteTimeUtc(filename).AddDays(1);
            
            // Deserialize the xml file
            CCPAPIResult<SerializableNotificationRefTypes> result = Util.
                DeserializeAPIResultFromFile<SerializableNotificationRefTypes>(filename,
                APIProvider.RowsetsTransform);

            // In case the file has an error we prevent the importation
            if (result.HasError)
            {
                EveMonClient.Trace("Error importing EVE notification types, deleting file");

                FileHelper.DeleteFile(filename);

                s_nextCheckTime = DateTime.UtcNow;

                return;
            }

            // Import the data
            Import(result.Result);
        }

        /// <summary>
        /// Imports the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void Import(SerializableNotificationRefTypes result)
        {
            if (result == null)
            {
                EveMonClient.Trace("failed");
                return;
            }

            foreach (var refType in result.Types)
            {
                int id = refType.TypeID;
                if (!s_notificationRefTypes.ContainsKey(id))
                    s_notificationRefTypes.Add(id, refType);
            }
            s_loaded = true;
        }

        /// <summary>
        /// Updates the file.
        /// </summary>
        private static async Task UpdateFileAsync()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            var url = new Uri(NetworkConstants.BitBucketWikiBase +
                NetworkConstants.NotificationRefTypes);

            s_queryPending = true;

            CCPAPIResult<SerializableNotificationRefTypes> result = await Util.
                DownloadAPIResultAsync<SerializableNotificationRefTypes>(url, acceptEncoded: true,
                transform: APIProvider.RowsetsTransform);
            OnDownloaded(result);
        }

        /// <summary>
        /// Processes the queried notification ref type.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnDownloaded(CCPAPIResult<SerializableNotificationRefTypes> result)
        {
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace("Error loading notification types: " + result.ErrorMessage);

                // Fallback
                EnsureInitialized();
                return;
            }

            s_cachedUntil = DateTime.UtcNow.AddDays(1);

            // Import the list
            Import(result.Result);

            // Reset query pending flag
            s_queryPending = false;

            // Notify the subscribers
            EveMonClient.OnNotificationRefTypesUpdated();

            // Save the file in cache
            LocalXmlCache.SaveAsync(Filename, result.XmlDocument).ConfigureAwait(false);
        }

        #endregion

    }
}
