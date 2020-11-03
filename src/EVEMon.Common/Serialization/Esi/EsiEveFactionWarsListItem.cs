using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiEveFactionWarsListItem
    {
        [DataMember(Name = "faction_id")]
        public int FactionID { get; set; }
        
        [DataMember(Name = "against_id")]
        public int AgainstID { get; set; }

        public SerializableEveFactionWarsListItem ToXMLItem()
        {
            return new SerializableEveFactionWarsListItem()
            {
                FactionID = FactionID,
                AgainstID = AgainstID
            };
        }
    }
}
