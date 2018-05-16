using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIContractItems : List<EsiContractItemsListItem>
    {
        public SerializableAPIContractItems ToXMLItem()
        {
            var ret = new SerializableAPIContractItems();
            foreach (var item in this)
                ret.ContractItems.Add(item.ToXMLItem());
            return ret;
        }
    }
}
