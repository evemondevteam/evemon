using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Repurposed to serve in two places for stat counters about FW kill statistics.
    /// </summary>
    [DataContract]
    public sealed class EsiEveFactionWarfareTotals
    {
        [DataMember(Name = "yesterday")]
        public int Yesterday { get; set; }

        [DataMember(Name = "last_week")]
        public int LastWeek { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
