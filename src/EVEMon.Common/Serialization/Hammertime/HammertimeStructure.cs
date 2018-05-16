using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Hammertime
{
    [DataContract]
    public sealed class HammertimeStructure
    {
        [DataMember(Name = "name")]
        public string StationName { get; set; }

        [DataMember(Name = "regionId", EmitDefaultValue = false, IsRequired = false)]
        public int RegionID { get; set; }

        [DataMember(Name = "systemId")]
        public int SolarSystemID { get; set; }
        
        [DataMember(Name = "location", EmitDefaultValue = false, IsRequired = false)]
        public EsiPosition Position { get; set; }

        [DataMember(Name = "public", IsRequired = false)]
        public bool IsPublic { get; set; }

        public SerializableOutpost ToXMLItem(long id)
        {
            return new SerializableOutpost()
            {
                // Not available from HammerTime
                CorporationID = 0,
                StationID = id,
                SolarSystemID = SolarSystemID,
                StationTypeID = 0,
                StationName = StationName
            };
        }
    }
}
