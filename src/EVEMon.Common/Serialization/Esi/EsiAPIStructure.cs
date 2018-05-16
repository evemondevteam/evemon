using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIStructure
    {
        [DataMember(Name = "name")]
        public string StationName { get; set; }

        [DataMember(Name = "type_id", EmitDefaultValue = false, IsRequired = false)]
        public int StationTypeID { get; set; }

        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }
        
        [DataMember(Name = "position", EmitDefaultValue = false, IsRequired = false)]
        public EsiPosition Position { get; set; }

        public SerializableOutpost ToXMLItem(long id)
        {
            return new SerializableOutpost()
            {
                // Not yet available from the API
                CorporationID = 0,
                StationID = id,
                SolarSystemID = SolarSystemID,
                StationTypeID = StationTypeID,
                StationName = StationName
            };
        }
    }
}
