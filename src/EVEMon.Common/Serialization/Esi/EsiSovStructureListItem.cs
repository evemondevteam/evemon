using EVEMon.Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiSovStructureListItem
    {
        private DateTime vulnEnd;
        private DateTime vulnStart;

        public EsiSovStructureListItem()
        {
            vulnEnd = DateTime.MinValue;
            vulnStart = DateTime.MinValue;
        }

        [DataMember(Name = "alliance_id")]
        public long AllianceID { get; set; }

        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }

        [DataMember(Name = "structure_id")]
        public long ID { get; set; }

        [DataMember(Name = "structure_type_id")]
        public int StructureTypeID { get; set; }

        [DataMember(Name = "vulnerability_occupancy_level", EmitDefaultValue = false, IsRequired = false)]
        public double ADM { get; set; }

        [DataMember(Name = "vulnerable_start_time", EmitDefaultValue = false, IsRequired = false)]
        private string VulnStartJson
        {
            get
            {
                return vulnStart.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    vulnStart = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime VulnStart
        {
            get
            {
                return vulnStart;
            }
        }

        [DataMember(Name = "vulnerable_end_time", EmitDefaultValue = false, IsRequired = false)]
        private string VulnEndJson
        {
            get
            {
                return vulnEnd.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    vulnEnd = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime VulnEnd
        {
            get
            {
                return vulnEnd;
            }
        }
    }
}
