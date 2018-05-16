using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiContractItemsListItem
    {
        [DataMember(Name = "record_id")]
        public long RecordID { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }

        // Max stack size is int32
        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        // -1 is singleton or BPO, -2 is blueprint copy
        [DataMember(Name = "raw_quantity")]
        public int RawQuantity { get; set; }

        [DataMember(Name = "is_singleton")]
        public bool Singleton { get; set; }

        [DataMember(Name = "is_included")]
        public bool Included { get; set; }

        public SerializableContractItemsListItem ToXMLItem()
        {
            return new SerializableContractItemsListItem()
            {
                TypeID = TypeID,
                RecordID = RecordID,
                Included = Included,
                Quantity = Quantity,
                RawQuantity = RawQuantity,
                Singleton = Singleton
            };
        }
    }
}
