using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiLoyaltyListItem
    {
        [DataMember(Name = "corporation_id")]
        public int CorpID { get; set; }

        [DataMember(Name = "loyalty_points")]
        public int LoyaltyPoints { get; set; }
    }
}
