using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents the balance of a wallet in a corporation; individual wallet balances are
    /// returned as doubles.
    /// </summary>
    [DataContract]
    public sealed class EsiAccountBalanceListItem
    {
        [DataMember(Name = "division")]
        public int AccountID { get; set; }

        [DataMember(Name = "balance")]
        public decimal Balance { get; set; }
    }
}
