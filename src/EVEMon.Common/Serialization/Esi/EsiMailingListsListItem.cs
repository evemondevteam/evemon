using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiMailingListsListItem
    {
        [DataMember(Name = "mailing_list_id")]
        public long ID { get; set; }

        [DataMember(Name = "name")]
        public string DisplayName { get; set; }

        public SerializableMailingListsListItem ToXMLItem()
        {
            return new SerializableMailingListsListItem()
            {
                DisplayName = DisplayName,
                ID = ID
            };
        }
    }
}
