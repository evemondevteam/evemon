using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIMailMessages : List<EsiMailMessagesListItem>
    {
        public SerializableAPIMailMessages ToXMLItem()
        {
            var ret = new SerializableAPIMailMessages();
            foreach (var mail in this)
                ret.Messages.Add(mail.ToXMLItem());
            return ret;
        }
    }
}
