using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIContactsList : List<EsiContactListItem>
    {
        public SerializableAPIContactList ToXMLItem()
        {
            var ret = new SerializableAPIContactList();
            foreach (var contact in this)
                ret.Contacts.Add(contact.ToXMLItem());
            return ret;
        }
    }
}
