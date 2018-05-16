using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiWalletJournalExtra
    {
        [DataMember(Name = "location_id", EmitDefaultValue = false, IsRequired = false)]
        public long LocationID { get; set; }

        [DataMember(Name = "transaction_id", EmitDefaultValue = false, IsRequired = false)]
        public long TransactionID { get; set; }

        [DataMember(Name = "npc_id", EmitDefaultValue = false, IsRequired = false)]
        public int NpcID { get; set; }

        [DataMember(Name = "npc_name", EmitDefaultValue = false, IsRequired = false)]
        public string NpcName { get; set; }

        [DataMember(Name = "destroyed_ship_type_id", EmitDefaultValue = false, IsRequired = false)]
        public int DestroyedShipTypeID { get; set; }

        [DataMember(Name = "character_id", EmitDefaultValue = false, IsRequired = false)]
        public long CharacterID { get; set; }

        [DataMember(Name = "corporation_id", EmitDefaultValue = false, IsRequired = false)]
        public long CorporationID { get; set; }

        [DataMember(Name = "alliance_id", EmitDefaultValue = false, IsRequired = false)]
        public long AllianceID { get; set; }

        [DataMember(Name = "job_id", EmitDefaultValue = false, IsRequired = false)]
        public int JobID { get; set; }

        [DataMember(Name = "contract_id", EmitDefaultValue = false, IsRequired = false)]
        public int ContractID { get; set; }

        [DataMember(Name = "system_id", EmitDefaultValue = false, IsRequired = false)]
        public int SystemID { get; set; }

        [DataMember(Name = "planet_id", EmitDefaultValue = false, IsRequired = false)]
        public int PlanetID { get; set; }
    }
}
