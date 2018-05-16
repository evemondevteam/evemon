using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPINotifications : List<EsiNotificationsListItem>
    {
        public SerializableAPINotifications ToXMLItem()
        {
            var ret = new SerializableAPINotifications();
            foreach (var order in this)
                ret.Notifications.Add(order.ToXMLItem());
            return ret;
        }
    }
}
