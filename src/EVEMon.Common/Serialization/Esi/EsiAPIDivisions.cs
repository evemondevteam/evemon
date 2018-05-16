using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIDivisions
    {
        [DataMember(Name = "hangar", EmitDefaultValue = false, IsRequired = false)]
        public List<EsiDivisionListItem> Hangar { get; set; }

        [DataMember(Name = "wallet", EmitDefaultValue = false, IsRequired = false)]
        public List<EsiDivisionListItem> Wallet { get; set; }
    }
}
