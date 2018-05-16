using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIEmploymentHistory : List<EsiEmploymentHistoryListItem>
    {
        public IEnumerable<SerializableEmploymentHistoryListItem> ToXMLItem()
        {
            var ret = new LinkedList<SerializableEmploymentHistoryListItem>();
            foreach (var history in this)
                ret.AddLast(history.ToXMLItem());
            return ret;
        }
    }
}
