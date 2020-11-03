using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common.Models.Extended
{
    public abstract class EveNotificationTextParser
    {
        public static event EventHandler<EventArgs> NotificationTextParserUpdated;
        
        private static EveNotificationTextParser s_parser;
        private static DateTime s_cachedUntil;
        private static bool s_queryPending;

        /// <summary>
        /// Parses the notification text.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="parsedDict">The parsed dictionary.</param>
        public abstract void Parse(EveNotification notification, KeyValuePair<YamlNode, YamlNode> pair,
            IDictionary<string, string> parsedDict);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <returns></returns>
        internal static EveNotificationTextParser GetParser()
        {
            if (s_parser != null && s_cachedUntil > DateTime.UtcNow)
                return s_parser;

            // GetExternalParserAsync();
            // The external parser is very out of date
            return (s_parser = new InternalEveNotificationTextParser());
        }

        /// <summary>
        /// Asynchronously gets the external parser.
        /// </summary>
        private static void GetExternalParserAsync()
        {
            if (s_queryPending)
                return;

            Uri url = new Uri(NetworkConstants.BitBucketWikiBase + NetworkConstants.
                ExternalEveNotificationTextParser);

            s_queryPending = true;
            HttpWebClientService.DownloadStringAsync(url).ContinueWith(task =>
            {
                OnDownloaded(task.Result);
            });
        }

        /// <summary>
        /// Processes the queried notification text parser.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnDownloaded(DownloadResult<string> result)
        {
            if (result.Error != null)
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(result.Error.Message);
                return;
            }
            
            string[] referenceAssemblies =
            {
                typeof(Enumerable).Assembly.Location,
                typeof(YamlNode).Assembly.Location,
            };

            // Revert to internal parser if the compilation fails for any reason
            s_parser = CodeCompiler.GenerateAssembly<EveNotificationTextParser>(
                referenceAssemblies, result.Result) ?? new InternalEveNotificationTextParser();

            // Reset query pending flag
            // after we compiled the parser
            s_queryPending = false;
            s_cachedUntil = DateTime.UtcNow.AddHours(12);

            // Notify the subscribers
            NotificationTextParserUpdated?.ThreadSafeInvoke(null, EventArgs.Empty);
        }
    }
}
