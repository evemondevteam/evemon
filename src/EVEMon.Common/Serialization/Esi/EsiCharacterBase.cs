using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a reference to a character used in kill mails and other character lists.
    /// </summary>
    [DataContract]
    public abstract class EsiCharacterBase
    {
        [DataMember(Name = "character_id")]
        public long ID { get; set; }
        
        [DataMember(Name = "corporation_id")]
        public long CorporationID { get; set; }
        
        [DataMember(Name = "alliance_id")]
        public long AllianceID { get; set; }
        
        [DataMember(Name = "faction_id")]
        public int FactionID { get; set; }
        
        [DataMember(Name = "ship_type_id")]
        public int ShipTypeID { get; set; }
    }
}
