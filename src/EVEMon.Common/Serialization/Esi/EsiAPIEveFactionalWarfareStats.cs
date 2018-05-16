using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIEveFactionalWarfareStats : List<EsiEveFactionalWarfareStatsListItem>
    {
        public SerializableAPIEveFactionalWarfareStats ToXMLItem(EsiAPIEveFactionWars wars)
        {
            var totals = new SerializableEveFacWarfareTotals();
            var ret = new SerializableAPIEveFactionalWarfareStats()
            {
                Totals = totals
            };

            // Manually compute the totals and convert individual war counts
            foreach (var warStats in this)
            {
                var kills = warStats.Kills;
                var vp = warStats.VictoryPoints;

                totals.KillsLastWeek += kills.LastWeek;
                totals.KillsTotal += kills.Total;
                totals.KillsYesterday += kills.Yesterday;

                totals.VictoryPointsLastWeek += vp.LastWeek;
                totals.VictoryPointsTotal += vp.Total;
                totals.VictoryPointsYesterday += vp.Yesterday;

                ret.FactionalWarfareStats.Add(warStats.ToXMLItem());
            }

            // Add the war declarations
            foreach (var war in wars)
                ret.FactionWars.Add(war.ToXMLItem());

            return ret;
        }
    }
}
