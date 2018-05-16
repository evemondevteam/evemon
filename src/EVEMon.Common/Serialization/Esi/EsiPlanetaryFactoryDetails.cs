using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryFactoryDetails
    {
        [DataMember(Name = "schematic_id")]
        public int SchematicID { get; set; }
    }
}
