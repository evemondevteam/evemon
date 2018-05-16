using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiPlanetaryContentsListItem
    {
        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }

        [DataMember(Name = "amount")]
        public int Amount { get; set; }
    }
}
