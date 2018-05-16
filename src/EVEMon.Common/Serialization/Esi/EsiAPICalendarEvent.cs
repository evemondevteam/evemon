using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPICalendarEvent : EsiCalendarEventListItem
    {
        private CCPAPIContactType ownerType;

        public EsiAPICalendarEvent()
        {
            ownerType = CCPAPIContactType.Other;
        }

        [DataMember(Name = "owner_id")]
        public long OwnerID { get; set; }

        [DataMember(Name = "owner_name")]
        public string OwnerName { get; set; }

        [DataMember(Name = "text")]
        public string EventText { get; set; }

        [DataMember(Name = "owner_type")]
        private string OwnerTypeJson
        {
            get
            {
                return ownerType.ToString().ToLower();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Enum.TryParse(value, true, out ownerType);
            }
        }

        [IgnoreDataMember]
        public CCPAPIContactType OwnerType
        {
            get
            {
                return ownerType;
            }
        }

        // in minutes
        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        public SerializableUpcomingCalendarEventsListItem ToXMLItem()
        {
            return new SerializableUpcomingCalendarEventsListItem()
            {
                Duration = Duration,
                EventDate = EventDate,
                EventID = EventID,
                EventText = EventText,
                EventTitle = EventTitle,
                Importance = Importance != 0,
                OwnerID = OwnerID,
                OwnerName = OwnerName,
                Response = Response
            };
        }
    }
}
