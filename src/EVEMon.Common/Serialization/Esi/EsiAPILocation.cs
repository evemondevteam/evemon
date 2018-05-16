using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a serializable version of a character's location.
    /// </summary>
    [DataContract]
    public sealed class EsiAPILocation
    {
        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }

        [IgnoreDataMember]
        public string SolarSystemName
        {
            get
            {
                return StaticGeography.GetSolarSystemName(SolarSystemID);
            }
        }

        [DataMember(Name = "station_id", EmitDefaultValue = false, IsRequired = false)]
        public int StationID { get; set; }

        [DataMember(Name = "structure_id", EmitDefaultValue = false, IsRequired = false)]
        public long StructureID { get; set; }

        public SerializableLocation ToXMLItem()
        {
            return new SerializableLocation()
            {
                SolarSystemID = SolarSystemID,
                StationID = StationID,
                StructureID = StructureID
            };
        }
    }
}
