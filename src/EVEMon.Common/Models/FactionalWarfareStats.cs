using System;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Service;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Models
{
    public sealed class FactionalWarfareStats
    {
        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal FactionalWarfareStats(EsiAPIFactionalWarfareStats src)
        {
            src.ThrowIfNull(nameof(src));

            FactionID = src.FactionID;
            FactionName = EveIDToName.GetIDToName(FactionID);
            EnlistedDate = src.EnlistedDate;
            CurrentRank = src.CurrentRank;
            HighestRank = src.HighestRank;
            KillsYesterday = src.Kills?.Yesterday ?? 0;
            KillsLastWeek = src.Kills?.LastWeek ?? 0;
            KillsTotal = src.Kills?.Total ?? 0;
            VictoryPointsYesterday = src.VictoryPoints?.Yesterday ?? 0;
            VictoryPointsLastWeek = src.VictoryPoints?.LastWeek ?? 0;
            VictoryPointsTotal = src.VictoryPoints?.Total ?? 0;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the faction ID.
        /// </summary>
        public int FactionID { get; }

        /// <summary>
        /// Gets the name of the faction.
        /// </summary>
        public string FactionName { get; }

        /// <summary>
        /// Gets the enlisted date.
        /// </summary>
        public DateTime EnlistedDate { get; }

        /// <summary>
        /// Gets the current rank.
        /// </summary>
        public int CurrentRank { get; }

        /// <summary>
        /// Gets the highest rank.
        /// </summary>
        public int HighestRank { get; }

        /// <summary>
        /// Gets the kills yesterday.
        /// </summary>
        public int KillsYesterday { get; }

        /// <summary>
        /// Gets the kills last week.
        /// </summary>
        public int KillsLastWeek { get; }

        /// <summary>
        /// Gets the kills total.
        /// </summary>
        public int KillsTotal { get; }

        /// <summary>
        /// Gets the victory points yesterday.
        /// </summary>
        public int VictoryPointsYesterday { get; }

        /// <summary>
        /// Gets the victory points last week.
        /// </summary>
        public int VictoryPointsLastWeek { get; }

        /// <summary>
        /// Gets the victory points total.
        /// </summary>
        public int VictoryPointsTotal { get; }

        #endregion
    }
}
