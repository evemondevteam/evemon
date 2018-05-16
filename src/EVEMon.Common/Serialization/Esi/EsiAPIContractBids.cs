using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIContractBids : List<EsiContractBidsListItem>
    {
        public SerializableAPIContractBids ToXMLItem(long contract)
        {
            var ret = new SerializableAPIContractBids();
            foreach (var bid in this)
                ret.ContractBids.Add(bid.ToXMLItem(contract));
            return ret;
        }
    }
}
