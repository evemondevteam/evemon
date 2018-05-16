using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a serializable version of a planetary colony. Used for querying CCP.
    /// </summary>
    [DataContract]
    public sealed class EsiAPIPlanetaryColony
    {
        [DataMember(Name = "links")]
        public List<EsiPlanetaryLink> Links { get; set; }

        [DataMember(Name = "pins")]
        public List<EsiPlanetaryPin> Pins { get; set; }

        [DataMember(Name = "routes")]
        public List<EsiPlanetaryRoute> Routes { get; set; }
    }
}
