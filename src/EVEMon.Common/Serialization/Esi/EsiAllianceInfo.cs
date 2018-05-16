using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAllianceInfo
    {
        private DateTime startDate;

        public EsiAllianceInfo()
        {
            startDate = DateTime.MinValue;
            Name = "";
        }
        
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "ticker")]
        public string Ticker { get; set; }

        [DataMember(Name = "creator_id")]
        public long CreatorID { get; set; }

        [DataMember(Name = "creator_corporation_id")]
        public long CreatorCorpID { get; set; }

        [DataMember(Name = "executor_corporation_id", EmitDefaultValue = false, IsRequired = false)]
        public long ExecutorCorpID { get; set; }

        [DataMember(Name = "faction_id", EmitDefaultValue = false, IsRequired = false)]
        public int FactionID { get; set; }

        [DataMember(Name = "date_founded")]
        private string StartDateJson
        {
            get
            {
                return startDate.DateTimeToTimeString();
            }
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
