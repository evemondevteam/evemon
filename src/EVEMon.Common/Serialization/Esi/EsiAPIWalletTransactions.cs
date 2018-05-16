using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIWalletTransactions : List<EsiWalletTransactionsListItem>
    {
        public SerializableAPIWalletTransactions ToXMLItem()
        {
            var ret = new SerializableAPIWalletTransactions();
            foreach (var entry in this)
                ret.WalletTransactions.Add(entry.ToXMLItem());
            return ret;
        }
    }
}
