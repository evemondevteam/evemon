using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiKillLogItemListItem
    {
        [DataMember(Name = "item_type_id")]
        public int TypeID { get; set; }

        [DataMember(Name = "flag")]
        public short EVEFlag { get; set; }

        [DataMember(Name = "quantity_dropped", EmitDefaultValue = true, IsRequired = false)]
        public int QtyDropped { get; set; }

        [DataMember(Name = "quantity_destroyed", EmitDefaultValue = true, IsRequired = false)]
        public int QtyDestroyed { get; set; }

        [DataMember(Name = "singleton")]
        public byte Singleton { get; set; }

        // Items inside containers etc, can only be nested one deep
        [DataMember(Name = "items", EmitDefaultValue = false, IsRequired = false)]
        public List<EsiKillLogItemListItem> Items { get; set; }

        public SerializableKillLogItemListItem ToXMLItem()
        {
            var ret = new SerializableKillLogItemListItem()
            {
                TypeID = TypeID,
                QtyDestroyed = QtyDestroyed,
                QtyDropped = QtyDropped,
                Singleton = Singleton,
                EVEFlag = EVEFlag
            };

            // Nested items
            if (Items != null)
                foreach (var item in Items)
                    ret.Items.Add(item.ToXMLItem());

            return ret;
        }
    }
}
