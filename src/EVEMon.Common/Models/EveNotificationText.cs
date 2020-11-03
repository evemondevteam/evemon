using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EVEMon.Common.Collections;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models.Extended;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Models
{
    public sealed class EveNotificationText
    {
        // Regular expression used for matching YAML placeholders
        private static readonly Regex PLACEHOLDER = new Regex("{(?<placeholder>\\w+)}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // Field used to encode the sender name
        private static readonly string SENDER_NAME = "senderName";

        /// <summary>
        /// Converts a YAML node to a key-value dictionary, where the keys are the node names
        /// and the values are the node contents.
        /// </summary>
        /// <param name="pairs">The YAML nodes to convert</param>
        /// <returns>The nodes in a dictionary form</returns>
        private static Dictionary<string, string> GetNodes(YamlMappingNode pairs) => pairs.
            Children.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value.ToString());

        private readonly EveNotification m_notification;
        private readonly EveNotificationTextParser m_parser;
        private string m_parsedText;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationText" /> class.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="typeID">The notification type ID.</param>
        /// <param name="text">The notification text.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public EveNotificationText(EveNotification notification, int typeID, string text)
        {
            NotificationID = typeID;
            NotificationText = text;
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
            string text = NotificationText;
            if (!string.IsNullOrWhiteSpace(textLayout))
            {
                if (!textLayout.Contains("{") || NotificationID == 0)
                    // If nothing to substitute, leave it alone
                    text = textLayout;
                else
                {
                    var pairs = Util.ParseYaml(NotificationText) as YamlMappingNode;
                    if (pairs != null)
                    {
                        // Substitute the {} placeholders with YAML text
                        var parsedDict = GetParsedDictionary(textLayout, pairs);
                        foreach (var pair in pairs)
                            m_parser.Parse(m_notification, pair, parsedDict);
                        string parsedText = parsedDict.Aggregate(textLayout, (current, pair) =>
                            current.Replace("{" + pair.Key + "}", pair.Value.Trim('\'')));
                        text = parsedText.Contains("{") ? NotificationText : parsedText;
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// Gets the parsed dictionary.
        /// </summary>
        /// <param name="textLayout">The text layout.</param>
        /// <param name="pairs">The text pairs.</param>
        /// <returns></returns>
        private IDictionary<string, string> GetParsedDictionary(string textLayout,
            YamlMappingNode pairs)
        {
            var parsedDict = GetNodes(pairs);
            var textLayoutDict = PLACEHOLDER.Matches(textLayout).Cast<Match>().Select(x =>
                x.Groups["placeholder"].Value).ToDictionary(key => key, value => string.Empty);
            parsedDict.AddRange(textLayoutDict.Keys.Except(parsedDict.Keys).ToDictionary(
                key => key, value => string.Empty));
            // Fill in the sender name
            if (parsedDict.ContainsKey(SENDER_NAME) && string.IsNullOrWhiteSpace(parsedDict[
                    SENDER_NAME]))
                parsedDict[SENDER_NAME] = m_notification.SenderName;
            return parsedDict;
        }
    }
}
