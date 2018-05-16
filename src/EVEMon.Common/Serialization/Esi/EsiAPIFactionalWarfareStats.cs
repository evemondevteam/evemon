using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a serializable version of factional warfare stats. Used for querying CCP.
    /// </summary>
    [DataContract]
    public sealed class EsiAPIFactionalWarfareStats
    {
        private DateTime enlisted;

        public EsiAPIFactionalWarfareStats()
        {
            enlisted = DateTime.MinValue;
        }

        [DataMember(Name = "faction_id", EmitDefaultValue = false, IsRequired = false)]
        public int FactionID { get; set; }
        
        [DataMember(Name = "enlisted_on", EmitDefaultValue = false, IsRequired = false)]
        private string EnlistedDateJson
        {
            get { return enlisted.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    enlisted = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "current_rank", EmitDefaultValue = false, IsRequired = false)]
        public int CurrentRank { get; set; }

        [DataMember(Name = "highest_rank", EmitDefaultValue = false, IsRequired = false)]
        public int HighestRank { get; set; }

        [DataMember(Name = "kills")]
        public EsiEveFactionWarfareTotals Kills { get; set; }

        [DataMember(Name = "victory_points")]
        public EsiEveFactionWarfareTotals VictoryPoints { get; set; }

        [IgnoreDataMember]
        public DateTime EnlistedDate
        {
            get
            {
                return enlisted;
            }
        }

        public SerializableAPIFactionalWarfareStats ToXMLItem()
        {
            return new SerializableAPIFactionalWarfareStats()
            {
                CurrentRank = CurrentRank,
                EnlistedDate = EnlistedDate,
                FactionID = FactionID,
                HighestRank = HighestRank,
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
