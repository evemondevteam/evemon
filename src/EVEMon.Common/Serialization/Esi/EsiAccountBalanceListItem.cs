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

        public SerializableAccountBalanceListItem ToXMLItem()
        {
            return new SerializableAccountBalanceListItem()
            {
                // Character balances are returned as a double, this is only for corps
                // Index is 1-7 on ESI but 1000 to 1006 on XML
                AccountID = AccountID,
                AccountKey = AccountID + 999L,
                Balance = Balance
            };
        }
    }
}
