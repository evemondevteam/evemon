using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Settings;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIMarketOrders : List<EsiOrderListItem>
    {
        /// <summary>
        /// Sets all market orders in this collection to be issued by the specified character
        /// ID. Used to apply the right ID to character market orders (corp market orders
        /// already have an IssuedBy field)
        /// </summary>
        /// <param name="id">The issuing character ID.</param>
        public void SetAllIssuedBy(long id)
        {
            foreach (EsiOrderListItem item in this)
                item.IssuedBy = id;
        }
    }
}
