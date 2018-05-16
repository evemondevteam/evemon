using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Data;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryColonyListItem
    {
        private DateTime lastUpdate;

        public EsiPlanetaryColonyListItem()
        {
            lastUpdate = DateTime.MinValue;
        }

        [DataMember(Name = "planet_id")]
        public long PlanetID { get; set; }

        // One of: temperate, barren, oceanic, ice, gas, lava, storm, plasma
        [DataMember(Name = "planet_type")]
        public string PlanetType { get; set; }

        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }

        [DataMember(Name = "owner_id")]
        public long OwnerID { get; set; }

        [DataMember(Name = "last_update")]
        public string LastUpdateJson
        {
            get { return lastUpdate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    lastUpdate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "upgrade_level")]
        public int UpgradeLevel { get; set; }

        [DataMember(Name = "num_pins")]
        public int NumberOfPins { get; set; }

        [IgnoreDataMember]
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }
        }

        public SerializablePlanetaryColony ToXMLItem()
        {
            // Determine planet type from type name
            // Planet type is a type ID
            CCPAPIPlanetTypes type = CCPAPIPlanetTypes.Unknown;
            if (!string.IsNullOrEmpty(PlanetType))
                Enum.TryParse(PlanetType, true, out type);
            int planetType = (int)type;

            var ret = new SerializablePlanetaryColony()
            {
                LastUpdate = LastUpdate,
                NumberOfPins = NumberOfPins,
                OwnerID = OwnerID,
                PlanetID = PlanetID,
                SolarSystemID = SolarSystemID,
                SolarSystemName = StaticGeography.GetSolarSystemName(SolarSystemID),
                PlanetTypeID = planetType,
                PlanetTypeName = StaticItems.GetItemName(planetType),
                UpgradeLevel = UpgradeLevel
            };
            return ret;
        }
    }
}
