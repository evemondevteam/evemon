using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryLink
    {
        [DataMember(Name = "source_pin_id")]
        public long SourcePinID { get; set; }

        [DataMember(Name = "destination_pin_id")]
        public long DestinationPinID { get; set; }

        [DataMember(Name = "link_level")]
        public short LinkLevel { get; set; }
    }
}
