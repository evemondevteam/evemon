using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPICorpAccountBalance : List<EsiAccountBalanceListItem>
    {
        public SerializableAPIAccountBalance ToXMLItem()
        {
            var ret = new SerializableAPIAccountBalance();
            foreach (var balance in this)
                ret.Accounts.Add(balance.ToXMLItem());
            return ret;
        }
    }
}
