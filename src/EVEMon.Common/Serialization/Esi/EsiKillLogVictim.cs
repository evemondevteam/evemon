using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public class EsiKillLogVictim : EsiCharacterBase
    {
        public EsiKillLogVictim()
        {
            Position = EsiPosition.ORIGIN;
        }

        [DataMember(Name = "damage_taken")]
        public int DamageTaken { get; set; }

        [DataMember(Name = "items", EmitDefaultValue = false, IsRequired = false)]
        public List<EsiKillLogItemListItem> Items { get; set; }

        [DataMember(Name = "position", IsRequired = false)]
        public EsiPosition Position { get; set; }

        public SerializableKillLogVictim ToXMLItem()
        {
            // This object has more data, which is handled by EsiAPIKillMail
            return new SerializableKillLogVictim()
            {
                AllianceID = AllianceID,
                CorporationID = CorporationID,
                DamageTaken = DamageTaken,
                FactionID = FactionID,
                ID = ID,
                ShipTypeID = ShipTypeID
            };
        }
    }
}
