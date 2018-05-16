using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIStandings : List<EsiStandingsListItem>
    {
        public SerializableAPIStandings ToXMLItem()
        {
            var ret = new SerializableAPIStandings();
            var standings = new SerializableStandings();
            ret.CharacterNPCStandings = standings;

            foreach (var standing in this)
            {
                var xml = standing.ToXMLItem();

                // Split by type (yeah the UI recombines them...)
                switch (xml.Group)
                {
                case StandingGroup.Agents:
                    standings.AgentStandings.Add(xml);
                    break;
                case StandingGroup.Factions:
                    standings.FactionStandings.Add(xml);
                    break;
                case StandingGroup.NPCCorporations:
                    standings.NPCCorporationStandings.Add(xml);
                    break;
                default:
                    // Ignore
                    break;
                }
            }

            return ret;
        }
    }
}
