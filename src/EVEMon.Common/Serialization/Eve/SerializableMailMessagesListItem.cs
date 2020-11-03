using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Collections;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableMailMessagesListItem
    {
        private readonly Collection<long> m_toCharacterIDs;
        private readonly Collection<long> m_toListID;

        public SerializableMailMessagesListItem()
        {
            m_toCharacterIDs = new Collection<long>();
            m_toListID = new Collection<long>();
        }

        [XmlAttribute("messageID")]
        public long MessageID { get; set; }

        [XmlAttribute("senderID")]
        public long SenderID { get; set; }

        [XmlAttribute("senderName")]
        public string SenderName { get; set; }

        [XmlAttribute("sentDate")]
        public string SentDateXml
        {
            get { return SentDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    SentDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("title")]
        public string TitleXml
        {
            get { return Title; }
            set { Title = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlAttribute("toCorpOrAllianceID")]
        public long ToCorpOrAllianceID { get; set; }
        
        [XmlAttribute("toCharacterIDs")]
        public string ToCharacterIDsXml
        {
            get { return string.Join(",", m_toCharacterIDs); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Parse one by one into IDs
                    long id;
                    foreach (string idStr in value.Split(','))
                        if (idStr.TryParseInv(out id))
                            m_toCharacterIDs.Add(id);
                }
            }
        }

        [XmlAttribute("toListID")]
        public string ToListIDXml
        {
            get { return string.Join(",", m_toListID); }
            set
            {
                // Parse one by one into IDs
                long id;
                foreach (string idStr in value.Split(','))
                    if (idStr.TryParseInv(out id))
                        m_toListID.Add(id);
            }
        }

        [XmlIgnore]
        public string Title { get; set; }

        [XmlIgnore]
        public DateTime SentDate { get; set; }

        [XmlIgnore]
        public Collection<long> ToCharacterIDs => m_toCharacterIDs;

        [XmlIgnore]
        public Collection<long> ToListID => m_toListID;
    }
}
