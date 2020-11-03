using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Models.Collections;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents the factional warfare statistics of the EVE universe.
    /// </summary>
    public static class EveFactionalWarfareStats
    {
        #region Fields

        private static readonly EveFactionWarfareStatsCollection s_eveFactionalWarfareStats =
            new EveFactionWarfareStatsCollection();

        private static readonly EveFactionWarsCollection s_eveFactionWars = new EveFactionWarsCollection();
        private const string Filename = "FacWarStats";

        private static bool s_isImporting;
        private static bool s_loaded;
        private static bool s_queryPending;
        private static ResponseParams s_statsResponse = null;
        private static ResponseParams s_warsResponse = null;

        private static int s_totalsKillsYesterday;
        private static int s_totalsKillsLastWeek;
        private static int s_totalsKillsTotal;
        private static int s_totalsVictoryPointsYesterday;
        private static int s_totalsVictoryPointsLastWeek;
        private static int s_totalsVictoryPointsTotal;
        private static DateTime s_nextCheckTime = DateTime.MinValue;

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the totals kills yesterday.
        /// </summary>
        public static int TotalsKillsYesterday
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsKillsYesterday;
            }
        }

        /// <summary>
        /// Gets the totals kills last week.
        /// </summary>
        public static int TotalsKillsLastWeek
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsKillsLastWeek;
            }
        }

        /// <summary>
        /// Gets the totals kills total.
        /// </summary>
        public static int TotalsKillsTotal
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsKillsTotal;
            }
        }

        /// <summary>
        /// Gets the totals victory points yesterday.
        /// </summary>
        public static int TotalsVictoryPointsYesterday
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsVictoryPointsYesterday;
            }
        }

        /// <summary>
        /// Gets the totals victory points last week.
        /// </summary>
        public static int TotalsVictoryPointsLastWeek
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsVictoryPointsLastWeek;
            }
        }

        /// <summary>
        /// Gets the totals victory points total.
        /// </summary>
        public static int TotalsVictoryPointsTotal
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting ? 0 : s_totalsVictoryPointsTotal;
            }
        }

        /// <summary>
        /// Gets the factional warfare stats.
        /// </summary>
        public static IEnumerable<EveFactionWarfareStats> FactionalWarfareStats
        {
            get
            {
                // Ensure list importation
                EnsureImportation();

                return s_isImporting
                    ? Enumerable.Empty<EveFactionWarfareStats>()
                    : s_eveFactionalWarfareStats;
            }
        }

        #endregion


        #region File Updating

        /// <summary>
        /// Downloads the faction warfare statistics.
        /// </summary>
        private static void UpdateList()
        {
            var now = DateTime.UtcNow;

            // Quit if the data is fresh
            if ((!s_loaded || now > s_nextCheckTime) && !s_queryPending && !EsiErrors.
                IsErrorCountExceeded)
            {
                // If the request fails, it will only be retried after the next minute
                s_nextCheckTime = now.AddMinutes(1.0);
                s_queryPending = true;

                EveMonClient.APIProviders.CurrentProvider.QueryEsi<EsiAPIEveFactionWars>(
                    ESIAPIGenericMethods.FactionWars, OnFactionWarsUpdated, new ESIParams(
                    s_warsResponse));
            }
        }

        /// <summary>
        /// Processes the faction war list.
        /// </summary>
        private static void OnFactionWarsUpdated(EsiResult<EsiAPIEveFactionWars> result,
            object ignore)
        {
            s_warsResponse = result.Response;
            if (result.HasError)
            {
                // Was there an error ?
                s_queryPending = false;
                EveMonClient.Notifications.NotifyEveFactionWarsError(result);
            }
            if (EsiErrors.IsErrorCountExceeded)
            {
                s_queryPending = false;
                // If error count is exceeded (success or fail), retry when it resets
                s_nextCheckTime = EsiErrors.ErrorCountResetTime;
            }
            else if (!result.HasError)
            {
                // Stage two request for factional warfare stats
                EveMonClient.Notifications.InvalidateAPIError();
                EveMonClient.APIProviders.CurrentProvider.QueryEsi
                    <EsiAPIEveFactionalWarfareStats>(ESIAPIGenericMethods.
                    EVEFactionalWarfareStats, OnWarStatsUpdated, new ESIParams(
                    s_statsResponse), result.HasData ? result.Result : null);
            }
        }

        /// <summary>
        /// Processes the faction war statistics list.
        /// </summary>
        private static void OnWarStatsUpdated(EsiResult<EsiAPIEveFactionalWarfareStats> result,
            object wars)
        {
            var factionWars = wars as EsiAPIEveFactionWars;
            s_statsResponse = result.Response;
            // Was there an error ?
            if (result.HasError)
            {
                s_queryPending = false;
                EveMonClient.Notifications.NotifyEveFactionalWarfareStatsError(result);
            }
            else
            {
                // Set the next update to be after downtime
                s_nextCheckTime = DateTime.Today.AddHours(EveConstants.DowntimeHour).
                    AddMinutes(EveConstants.DowntimeDuration);
                s_queryPending = false;
                EveMonClient.Notifications.InvalidateAPIError();
                if (result.HasData)
                {
                    var fwStats = result.Result.ToXMLItem(factionWars);
                    Import(fwStats);
                    EveMonClient.OnEveFactionalWarfareStatsUpdated();
                    // Save the file to our cache
                    LocalXmlCache.SaveAsync(Filename, Util.SerializeToXmlDocument(fwStats)).
                        ConfigureAwait(false);
                }
            }
        }

        #endregion


        #region Importation

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private static void EnsureImportation()
        {
            UpdateList();
            Import();
        }

        /// <summary>
        /// Deserialize the file and import the stats.
        /// </summary>
        private static void Import()
        {
            // Exit if we have already imported or are in the process of importing the list
            if (!s_loaded && !s_queryPending && !s_isImporting)
            {
                var result = LocalXmlCache.Load<SerializableAPIEveFactionalWarfareStats>(
                    Filename, true);
                if (result == null)
                    s_nextCheckTime = DateTime.UtcNow;
                else
                    // Deserialize the result
                    Import(result);
            }
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(SerializableAPIEveFactionalWarfareStats src)
        {
            s_isImporting = true;

            s_totalsKillsYesterday = src.Totals.KillsYesterday;
            s_totalsKillsLastWeek = src.Totals.KillsLastWeek;
            s_totalsKillsTotal = src.Totals.KillsTotal;
            s_totalsVictoryPointsYesterday = src.Totals.VictoryPointsYesterday;
            s_totalsVictoryPointsLastWeek = src.Totals.VictoryPointsLastWeek;
            s_totalsVictoryPointsTotal = src.Totals.VictoryPointsTotal;

            s_eveFactionalWarfareStats.Import(src.FactionalWarfareStats);
            s_eveFactionWars.Import(src.FactionWars);

            s_loaded = true;
            s_isImporting = false;
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the against faction IDs.
        /// </summary>
        /// <param name="factionID">The faction ID.</param>
        /// <returns></returns>
        public static IEnumerable<int> GetAgainstFactionIDs(long factionID)
        {
            // Ensure list importation
            EnsureImportation();

            if (s_isImporting)
                return Enumerable.Empty<int>();

            List<int> againstIDs = new List<int>();
            foreach (EveFactionWar factionWar in s_eveFactionWars.Where(faction => faction.FactionID == factionID))
            {
                if (factionWar.AgainstID == factionWar.PrimeAgainstID)
                {
                    againstIDs.Insert(0, factionWar.AgainstID);
                    continue;
                }
                againstIDs.Add(factionWar.AgainstID);
            }
            return againstIDs;
        }

        /// <summary>
        /// Gets the factional warfare stats for faction.
        /// </summary>
        /// <param name="factionID">The faction ID.</param>
        /// <returns></returns>
        public static EveFactionWarfareStats GetFactionalWarfareStatsForFaction(int factionID)
        {
            // Ensure list importation
            EnsureImportation();

            return s_isImporting
                ? null
                : s_eveFactionalWarfareStats.FirstOrDefault(factionStats => factionStats.FactionID == factionID);
        }

        #endregion

    }
}
