using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiContactNotifyListItem
    {
        private DateTime sentDate;

        public EsiContactNotifyListItem()
        {
            sentDate = DateTime.MinValue;
        }

        [DataMember(Name = "notification_id")]
        public long NotificationID { get; set; }

        [DataMember(Name = "message", EmitDefaultValue = true, IsRequired = false)]
        public string Message { get; set; }

        [DataMember(Name = "standing_level")]
        public double StandingLevel { get; set; }

        [DataMember(Name = "sender_character_id")]
        public long SenderID { get; set; }
        
        [DataMember(Name = "send_date")]
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
    }
}
