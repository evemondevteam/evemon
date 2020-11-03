using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Used for both wallet and hangar divisions.
    /// </summary>
    [DataContract]
    public class EsiDivisionListItem
    {
        // 1 through 7 inclusive
        [DataMember(Name = "division")]
        public int Division { get; set; }
        
        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }
    }
}
