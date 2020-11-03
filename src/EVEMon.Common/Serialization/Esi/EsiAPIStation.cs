using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIStation
    {
        [DataMember(Name = "station_id")]
        public long ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "owner", EmitDefaultValue = false, IsRequired = false)]
        public int CorporationID { get; set; }

        [DataMember(Name = "system_id")]
        public int SolarSystemID { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }

        [DataMember(Name = "race_id", EmitDefaultValue = false, IsRequired = false)]
        public int RaceID { get; set; }

        [DataMember(Name = "position")]
        public EsiPosition Position { get; set; }

        [DataMember(Name = "reprocessing_efficiency")]
        public float ReprocessingEfficiency { get; set; }

        [DataMember(Name = "reprocessing_stations_take")]
        public float ReprocessingStationsTake { get; set; }

        [DataMember(Name = "max_dockable_ship_volume")]
        public float MaxDockableShipVolume { get; set; }

        [DataMember(Name = "office_rental_cost")]
        public decimal OfficeRentalCost { get; set; }

        public SerializableStation ToSerializableStation()
        {
            return new SerializableStation()
            {
                CorporationID = CorporationID,
                ID = ID,
                Name = Name,
                ReprocessingEfficiency = ReprocessingEfficiency,
                ReprocessingStationsTake = ReprocessingStationsTake
            };
        }

        public SerializableOutpost ToSerializableOutpost()
        {
            return new SerializableOutpost()
            {
                CorporationID = CorporationID,
                SolarSystemID = SolarSystemID,
                StationID = ID,
                StationTypeID = TypeID,
                StationName = Name
            };
        }
    }
}
