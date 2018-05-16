using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiEveFactionalWarfareStatsListItem
    {
        [DataMember(Name = "faction_id")]
        public int FactionID { get; set; }
        
        [DataMember(Name = "pilots")]
        public int Pilots { get; set; }

        [DataMember(Name = "systems_controlled")]
        public int SystemsControlled { get; set; }

        [DataMember(Name = "kills")]
        public EsiEveFactionWarfareTotals Kills { get; set; }
        
        [DataMember(Name = "victory_points")]
        public EsiEveFactionWarfareTotals VictoryPoints { get; set; }

        public SerializableEveFactionalWarfareStatsListItem ToXMLItem()
        {
            return new SerializableEveFactionalWarfareStatsListItem()
            {
                FactionID = FactionID,
                Pilots = Pilots,
                SystemsControlled = SystemsControlled,
                KillsLastWeek = Kills?.LastWeek ?? 0,
                KillsTotal = Kills?.Total ?? 0,
                KillsYesterday = Kills?.Yesterday ?? 0,
                VictoryPointsLastWeek = VictoryPoints?.LastWeek ?? 0,
                VictoryPointsTotal = VictoryPoints?.Total ?? 0,
                VictoryPointsYesterday = VictoryPoints?.Yesterday ?? 0
            };
        }
    }
}
