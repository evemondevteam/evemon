using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryColonyListItem
    {
        public EsiPlanetaryColonyListItem()
        {
            LastUpdate = DateTime.MinValue;
        }

        [DataMember(Name = "planet_id")]
        public int PlanetID { get; set; }

        // One of: temperate, barren, oceanic, ice, gas, lava, storm, plasma
        [DataMember(Name = "planet_type")]
        private string PlanetTypeJson { get; set; }

        [IgnoreDataMember]
        public int PlanetType
        {
            get
            {
                // Determine planet type from type name
                // Planet type is a type ID
                CCPAPIPlanetTypes type = CCPAPIPlanetTypes.Unknown;
                if (!string.IsNullOrEmpty(PlanetTypeJson))
                    Enum.TryParse(PlanetTypeJson, true, out type);
                return (int)type;
            }
        }

        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }

        [DataMember(Name = "owner_id")]
        public long OwnerID { get; set; }

        [DataMember(Name = "last_update")]
        public string LastUpdateJson
        {
            get { return LastUpdate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    LastUpdate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "upgrade_level")]
        public int UpgradeLevel { get; set; }

        [DataMember(Name = "num_pins")]
        public int NumberOfPins { get; set; }

		[IgnoreDataMember]
		public DateTime LastUpdate { get; set; }
	}
}
