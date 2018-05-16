using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiKillLogAttackersListItem : EsiCharacterBase
    {
        [DataMember(Name = "damage_done")]
        public int DamageDone { get; set; }

        [DataMember(Name = "final_blow")]
        public bool FinalBlow { get; set; }

        [DataMember(Name = "security_status")]
        public double SecurityStatus { get; set; }

        [DataMember(Name = "weapon_type_id", EmitDefaultValue = false, IsRequired = false)]
        public int WeaponTypeID { get; set; }

        public SerializableKillLogAttackersListItem ToXMLItem()
        {
            return new SerializableKillLogAttackersListItem()
            {
                AllianceID = AllianceID,
                CorporationID = CorporationID,
                DamageDone = DamageDone,
                FactionID = FactionID,
                FinalBlow = FinalBlow,
                ID = ID,
                SecurityStatus = SecurityStatus,
                ShipTypeID = ShipTypeID,
                WeaponTypeID = WeaponTypeID
            };
        }
    }
}
