using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiStandingsListItem
    {
        [DataMember(Name = "from_id")]
        public int ID { get; set; }

        // One of: agent, npc_corp, faction
        [DataMember(Name = "from_type")]
        private string GroupJson
        {
            get
            {
                // Convert from EVEmon enumeration
                switch (Group)
                {
                case StandingGroup.NPCCorporations:
                    return "npc_corp";
                case StandingGroup.Agents:
                    return "agent";
                case StandingGroup.Factions:
                    return "faction";
                default:
                    return string.Empty;
                }
            }
            set
            {
                // Convert to EVEmon enumeration
                switch (value)
                {
                case "npc_corp":
                    Group = StandingGroup.NPCCorporations;
                    break;
                case "agent":
                    Group = StandingGroup.Agents;
                    break;
                case "faction":
                    Group = StandingGroup.Factions;
                    break;
                default:
                    break;
                }
            }
        }

        [DataMember(Name = "standing")]
        public double StandingValue { get; set; }

        [IgnoreDataMember]
        public StandingGroup Group { get; set; }

        public SerializableStandingsListItem ToXMLItem()
        {
            return new SerializableStandingsListItem()
            {
                ID = ID,
                Group = Group,
                StandingValue = StandingValue
            };
        }
    }
}
