using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryExtractorHead
    {
        [DataMember(Name = "head_id")]
        public int HeadID { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }
    }
}
