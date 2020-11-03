using EVEMon.Common.Constants;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            int id;
            EnsureLoaded();
            var type = s_notificationRefTypes.Values.FirstOrDefault(x => x.TypeCode?.Equals(
                name, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (type != null)
                // Found in ref types XML
                id = type.TypeID;
            else if (name == null)
                // Invalid
                id = 0;
            else
            {
                // Create a template notification type; this will probably be disabled once all
                // of the unknown notifications are coded
                int newkey = s_notificationRefTypes.Keys.Max() + 1;
                string subject = Regex.Replace(name, "([A-Z]*)([A-Z][^A-Z$])", "$1 $2").Trim();

                s_notificationRefTypes.Add(newkey, new SerializableNotificationRefTypesListItem()
                {
                    SubjectLayout = subject,
                    TypeID = newkey,
                    TypeCode = name,
                    TextLayout = "",
                    TypeName = name
                });
                id = newkey;
            }
            return id;
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
            var info = LocalXmlCache.GetFileInfo(Filename);
            var result = LocalXmlCache.Load<SerializableNotificationRefTypes>(Filename, true);
            // Update the file if we don't have it or the data have expired
            if (result == null || (s_loaded && s_cachedUntil < DateTime.UtcNow))
                Task.WhenAll(UpdateFileAsync());
            else if (!s_loaded)
            {
                s_cachedUntil = info.Exists ? info.LastWriteTimeUtc.AddDays(1) : DateTime.
                    MinValue;
                if (result == null)
                    s_nextCheckTime = DateTime.UtcNow;
                else
                    // Import the data
                    Import(result);
            }
        }

        /// <summary>
        /// Imports the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void Import(SerializableNotificationRefTypes result)
        {
            if (result == null)
                EveMonClient.Trace("Could not load notification types");
            else
            {
                foreach (var refType in result.Types)
                {
                    int id = refType.TypeID;
                    if (!s_notificationRefTypes.ContainsKey(id))
                        s_notificationRefTypes.Add(id, refType);
                }
                s_loaded = true;
            }
        }

        /// <summary>
        /// Updates the file.
        /// </summary>
        private static async Task UpdateFileAsync()
        {
            // Quit if query is pending
            if (!s_queryPending)
            {
                var url = new Uri(NetworkConstants.BitBucketWikiBase +
                    NetworkConstants.NotificationRefTypes);
                s_queryPending = true;

                var result = await Util.DownloadAPIResultAsync<SerializableNotificationRefTypes>(
                    url, new RequestParams()
                    {
                        AcceptEncoded = true
                    }, transform: APIProvider.RowsetsTransform);
                OnDownloaded(result);
            }
        }

        /// <summary>
        /// Processes the queried notification ref type.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnDownloaded(CCPAPIResult<SerializableNotificationRefTypes> result)
        {
            s_queryPending = false;
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                EveMonClient.Trace("Error loading notification types: " + result.ErrorMessage);
                // Fallback
                EnsureInitialized();
            }
            else
            {
                s_cachedUntil = DateTime.UtcNow.AddDays(1);
                Import(result.Result);
                EveMonClient.OnNotificationRefTypesUpdated();
                // Save the file in cache
                LocalXmlCache.SaveAsync(Filename, result.XmlDocument).ConfigureAwait(false);
            }
        }

        #endregion

    }
}
