using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiNotificationsListItem
    {
        private CCPAPIContactType senderType;
        private DateTime sentDate;

        public EsiNotificationsListItem()
        {
            senderType = CCPAPIContactType.Other;
            sentDate = DateTime.MinValue;
        }

        [DataMember(Name = "notification_id")]
        public long NotificationID { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "sender_id")]
        public int SenderID { get; set; }

        [DataMember(Name = "is_read", IsRequired = false)]
        public bool Read { get; set; }

        [DataMember(Name = "text", EmitDefaultValue = false, IsRequired = false)]
        public string NotificationText { get; set; }

        [DataMember(Name = "sender_type")]
        private string SenderTypeJson
        {
            get
            {
                return senderType.ToString().ToLower();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Enum.TryParse(value, true, out senderType);
            }
        }

        [IgnoreDataMember]
        public CCPAPIContactType SenderType
        {
            get
            {
                return senderType;
            }
        }
        
        [DataMember(Name = "timestamp")]
        private string SentDateJson
        {
            get { return sentDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    sentDate = value.TimeStringToDateTime();
            }
        }
        
        [IgnoreDataMember]
        public DateTime SentDate
        {
            get
            {
                return sentDate;
            }
        }

        public SerializableNotificationsListItem ToXMLItem()
        {
            return new SerializableNotificationsListItem()
            {
                NotificationID = NotificationID,
                Read = Read,
                SenderID = SenderID,
                SentDate = SentDate,
                TypeID = EveNotificationType.GetID(Type)
            };
        }
    }
}
