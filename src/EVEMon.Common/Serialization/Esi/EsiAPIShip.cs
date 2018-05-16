using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a serializable version of a character's current ship.
    /// </summary>
    [DataContract]
    public sealed class EsiAPIShip
    {
        [DataMember(Name = "ship_type_id")]
        public int ShipTypeID { get; set; }

        // Unique to a particular ship until repackaged
        [DataMember(Name = "ship_item_id")]
        public long ShipItemID { get; set; }

        [DataMember(Name = "ship_name")]
        public string ShipName { get; set; }
    }
}
