using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIMailingLists : List<EsiMailingListsListItem>
    {
        public SerializableAPIMailingLists ToXMLItem()
        {
            var ret = new SerializableAPIMailingLists();
            foreach (var list in this)
                ret.MailingLists.Add(list.ToXMLItem());
            return ret;
        }
    }
}
