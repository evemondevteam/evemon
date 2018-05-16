using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryExtractorDetails
    {
        [DataMember(Name = "product_type_id", EmitDefaultValue = false, IsRequired = false)]
        public int ProductTypeID { get; set; }

        [DataMember(Name = "heads")]
        public List<EsiPlanetaryExtractorHead> Heads { get; set; }

        // in seconds
        [DataMember(Name = "cycle_time", EmitDefaultValue = false, IsRequired = false)]
        public int CycleTime { get; set; }

        [DataMember(Name = "head_radius", EmitDefaultValue = false, IsRequired = false)]
        public double HeadRadius { get; set; }

        [DataMember(Name = "qty_per_cycle", EmitDefaultValue = false, IsRequired = false)]
        public int QuantityPerCycle { get; set; }
    }
}
