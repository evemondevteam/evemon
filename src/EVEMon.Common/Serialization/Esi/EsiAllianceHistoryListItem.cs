using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAllianceHistoryListItem
    {
        private DateTime startDate;

        public EsiAllianceHistoryListItem()
        {
            startDate = DateTime.MinValue;
        }

        [DataMember(Name = "record_id")]
        public int RecordID { get; set; }

        [DataMember(Name = "alliance_id", EmitDefaultValue = false, IsRequired = false)]
        public long AllianceID { get; set; }

        [DataMember(Name = "is_deleted", IsRequired = false)]
        public bool Closed { get; set; }

        [DataMember(Name = "start_date")]
        public string StartDateJson
        {
            get { return startDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    startDate = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
        }
    }
}
