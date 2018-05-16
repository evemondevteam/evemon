using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIContracts : List<EsiContractListItem>
    {
        public SerializableAPIContracts ToXMLItem()
        {
            var ret = new SerializableAPIContracts();
            foreach (var contract in this)
                ret.Contracts.Add(contract.ToXMLItem());
            return ret;
        }
    }
}
