using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EVEMon.Common.Collections;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models.Extended;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Models
{
    public sealed class EveNotificationText
    {
        private readonly EveNotification m_notification;
        private readonly EveNotificationTextParser m_parser;
        private string m_parsedText;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationText" /> class.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public EveNotificationText(EveNotification notification, SerializableNotificationTextsListItem src)
        {
            src.ThrowIfNull(nameof(src));

            NotificationID = src.NotificationID;
            NotificationText = src.NotificationText;
            m_notification = notification;
            m_parser = EveNotificationTextParser.GetParser();
        }


        #region Public Properties

        /// <summary>
        /// Gets or sets the notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; }

        /// <summary>
        /// Gets the notification text.
        /// </summary>
        /// <value>
        /// The notification text.
        /// </value>
        internal string NotificationText { get; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        /// <value>The notification text.</value>
        internal string ParsedText => m_parsedText = (string.IsNullOrEmpty(m_parsedText) ||
            m_parsedText.Contains(EveMonConstants.UnknownText)) ? Parse(
            EveNotificationType.GetTextLayout(m_notification.TypeID)).NewLinesToBreakLines().
            DecodeUnicodeCharacters().Normalize() : m_parsedText;

        #endregion


        /// <summary>
        /// Parses the specified notification text.
        /// </summary>
        /// <param name="textLayout">The text layout.</param>
        /// <returns></returns>
        internal string Parse(string textLayout = null)
        {
            if (String.IsNullOrWhiteSpace(textLayout))
                return NotificationText;

            if (!textLayout.Contains("{") || NotificationID == 0)
                return textLayout;

            var pairs = Util.ParseYaml(NotificationText) as YamlMappingNode;

            IDictionary<string, string> parsedDict = GetParsedDictionary(textLayout, pairs);

            foreach (KeyValuePair<YamlNode, YamlNode> pair in pairs)
            {
                m_parser.Parse(m_notification, pair, parsedDict);
            }

            string parsedText = parsedDict.Aggregate(textLayout, (current, pair) =>
                current.Replace("{" + pair.Key + "}", pair.Value.Trim('\'')));

            return parsedText.Contains("{") ? NotificationText : parsedText;
        }

        /// <summary>
        /// Gets the parsed dictionary.
        /// </summary>
        /// <param name="textLayout">The text layout.</param>
        /// <param name="pairs">The text pairs.</param>
        /// <returns></returns>
        private IDictionary<string, string> GetParsedDictionary(string textLayout, YamlMappingNode pairs)
        {
            IDictionary<string, string> parsedDict = pairs.Children.ToDictionary(
                pair => pair.Key.ToString(), pair => pair.Value.ToString());

            IDictionary<string, string> textLayoutDict = Regex.Matches(textLayout,
                "{(?<placeholder>\\w+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase).
                Cast<Match>().Select(x => x.Groups["placeholder"].Value).
                ToDictionary(key => key, value => string.Empty);

            parsedDict.AddRange(textLayoutDict.Keys.Except(parsedDict.Keys).ToDictionary(
                key => key, value => string.Empty));

            if (parsedDict.ContainsKey("senderName") && string.IsNullOrWhiteSpace(
                    parsedDict["senderName"]))
                parsedDict["senderName"] = m_notification.SenderName;

            return parsedDict;
        }
    }
}
