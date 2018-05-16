using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIMarketOrders : List<EsiOrderListItem>
    {
        public SerializableAPIMarketOrders ToXMLItem(long ownerID)
        {
            var ret = new SerializableAPIMarketOrders();
            foreach (var order in this)
                ret.Orders.Add(order.ToXMLItem(ownerID));
            return ret;
        }
    }
}
