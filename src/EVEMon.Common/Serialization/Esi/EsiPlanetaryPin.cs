using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryPin
    {
        private DateTime expiryTime;
        private DateTime installTime;
        private DateTime lastCycleStart;

        public EsiPlanetaryPin()
        {
            expiryTime = DateTime.MinValue;
            installTime = DateTime.MinValue;
            lastCycleStart = DateTime.MinValue;
        }

        [DataMember(Name = "pin_id")]
        public long PinID { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }
        
        [DataMember(Name = "schematic_id", EmitDefaultValue = false, IsRequired = false)]
        public int SchematicID { get; set; }

        [DataMember(Name = "extractor_details", EmitDefaultValue = false, IsRequired = false)]
        public EsiPlanetaryExtractorDetails ExtractorDetails { get; set; }

        [DataMember(Name = "factory_details", EmitDefaultValue = false, IsRequired = false)]
        public EsiPlanetaryFactoryDetails FactoryDetails { get; set; }

        [DataMember(Name = "contents", EmitDefaultValue = false, IsRequired = false)]
        public List<EsiPlanetaryContentsListItem> Contents { get; set; }

        [DataMember(Name = "last_cycle_start", EmitDefaultValue = false, IsRequired = false)]
        public string LastCycleStartJson
        {
            get { return lastCycleStart.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    lastCycleStart = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "install_time", EmitDefaultValue = false, IsRequired = false)]
        private string InstallTimeJson
        {
            get { return installTime.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    installTime = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "expiry_time", EmitDefaultValue = false, IsRequired = false)]
        private string ExpiryTimeJson
        {
            get { return expiryTime.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    expiryTime = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime LastCycleStart
        {
            get
            {
                return lastCycleStart;
            }
        }

        [IgnoreDataMember]
        public DateTime InstallTime
        {
            get
            {
                return installTime;
            }
        }

        [IgnoreDataMember]
        public DateTime ExpiryTime
        {
            get
            {
                return expiryTime;
            }
        }
    }
}
