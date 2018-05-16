#define HAMMERTIME

using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Hammertime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using HammertimeStructureList = System.Collections.Generic.Dictionary<string, EVEMon.Common.
    Serialization.Hammertime.HammertimeStructure>;

namespace EVEMon.Common.Service
{
    public static class EveIDToStation
    {
        private const string Filename = "ConquerableStationList";

        // Cache used to return all data, this is saved and loaded into the file
        private static readonly Dictionary<long, SerializableOutpost> s_cacheList = new Dictionary<long, SerializableOutpost>();

        // Provider for conquerable stations (NPC stations go through staticgeography)
        private static readonly IDToObjectProvider<SerializableOutpost> s_conq = new ConquerableStationProvider(s_cacheList);

        // Provider for citadels
        private static readonly IDToObjectProvider<SerializableOutpost> s_cita = new CitadelStationProvider(s_cacheList);

        private static bool s_savePending;
        private static DateTime s_lastSaveTime;

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static EveIDToStation()
        {
            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static async void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            await UpdateOnOneSecondTickAsync();
        }
        
        /// <summary>
        /// Gets the station information from its ID. Works on NPC, conquerable, and citadel stations.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The station information</returns>
        internal static Station GetIDToStation(long id)
        {
            var station = StaticGeography.GetStationByID(id);
            if (station == null && id != 0L)
            {
                SerializableOutpost serStation;

                // Citadels have ID over maximum int value
                if (id < int.MaxValue)
                    serStation = s_conq.LookupID(id);
                else
                    serStation = s_cita.LookupID(id);

                if (serStation != null)
                    station = new Station(serStation);
            }
            return station;
        }
        
        /// <summary>
        /// Initializes the cache from file.
        /// </summary>
        public static void InitializeFromFile()
        {
            // Quit if the client has been shut down
            if (EveMonClient.Closed)
                return;

            string file = LocalXmlCache.GetFileInfo(Filename).FullName;

            if (!File.Exists(file) || s_cacheList.Any())
                return;

            // Deserialize the file
            SerializableStationList cache = Util.DeserializeXmlFromFile<SerializableStationList>(file);

            // Reset the cache if anything went wrong
            if (cache == null || cache.Stations.Any(x => x.StationID == 0) ||
                cache.Stations.Any(x => x.StationName.Length == 0))
            {
                EveMonClient.Trace("Station and citadel deserialization failed; deleting file.");
                FileHelper.DeleteFile(file);
                return;
            }

            // Add the data to the cache
            Import(cache.Stations);
        }
        
        /// <summary>
        /// Imports the data from the query result.
        /// </summary>
        /// <param name="entities">The entities.</param>
        private static void Import(IEnumerable<SerializableOutpost> entities)
        {
            foreach (SerializableOutpost entity in entities)
            {
                long id = entity.StationID;

                // Add the query result to our cache list if it doesn't exist already
                if (!s_cacheList.ContainsKey(id))
                    s_cacheList.Add(id, entity);
            }
        }

        /// <summary>
        /// Every timer tick, checks whether we should save the list every 10s.
        /// 
        /// While this looks similar to EveIDToName.UpdateOnOneSecondTickAsync, the methods
        /// are static, so there is little to gain by inheriting from an abstract superclass.
        /// </summary>
        private static Task UpdateOnOneSecondTickAsync()
        {
            // Is a save requested and is the last save older than 10s ?
            if (s_savePending && DateTime.UtcNow > s_lastSaveTime.AddSeconds(10))
                return SaveImmediateAsync();

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Saves this cache list to a file.
        /// </summary>
        public static async Task SaveImmediateAsync()
        {
            // Save in file
            await LocalXmlCache.SaveAsync(Filename, Util.SerializeToXmlDocument(Export()));

            // Reset savePending flag
            s_lastSaveTime = DateTime.UtcNow;
            s_savePending = false;
        }

        /// <summary>
        /// Exports the cache list to a serializable object.
        /// </summary>
        /// <returns></returns>
        private static SerializableStationList Export()
        {
            var serial = new SerializableStationList();

            lock (s_cacheList)
            {
                serial.Stations.AddRange(s_cacheList.Values);
            }

            return serial;
        }

        /// <summary>
        /// Provides station ID lookups. Uses the NPC/conquerable station endpoint.
        /// </summary>
        private class ConquerableStationProvider : IDToObjectProvider<SerializableOutpost>
        {
            public ConquerableStationProvider(IDictionary<long, SerializableOutpost> cacheList) : base(cacheList) { }

            protected override void FetchIDs()
            {
                long id = 0L;
                lock (m_pendingIDs)
                {
                    using (var it = m_pendingIDs.GetEnumerator())
                    {
                        // Fetch the next ID if it is available
                        if (it.MoveNext())
                        {
                            id = it.Current;
                            m_pendingIDs.Remove(id);
                        }
                    }
                }
                if (id != 0L)
                    EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIStation>(
                        ESIAPIGenericMethods.StationInfo, id, OnQueryStationUpdated, id);
            }

            private void OnQueryStationUpdated(EsiResult<EsiAPIStation> result, object ignore)
            {
                // Bail if there is an error
                if (result.HasError)
                {
                    EveMonClient.Notifications.NotifyStationQueryError(result);
                    m_queryPending = false;
                    return;
                }

                EveMonClient.Notifications.InvalidateAPIError();

                lock (s_cacheList)
                {
                    // Add resulting names to the cache; duplicates should not occur, but
                    // guard against them defensively
                    var station = result.Result.ToSerializableOutpost();
                    long id = station.StationID;

                    if (s_cacheList.ContainsKey(id))
                        s_cacheList[id] = station;
                    else
                        s_cacheList.Add(id, station);
                    m_requested.Add(id);
                }
                OnLookupComplete();
            }

            protected override void TriggerEvent() {
                EveMonClient.OnConquerableStationListUpdated();
            }
        }

        /// <summary>
        /// Provides citadel ID lookups. Uses the citadel info endpoint.
        /// </summary>
        private class CitadelStationProvider : IDToObjectProvider<SerializableOutpost>
        {
            public CitadelStationProvider(IDictionary<long, SerializableOutpost> cacheList) : base(cacheList) { }

            protected override void FetchIDs()
            {
                long id = 0L;
                lock (m_pendingIDs)
                {
                    using (var it = m_pendingIDs.GetEnumerator())
                    {
                        // Fetch the next ID if it is available
                        if (it.MoveNext())
                        {
                            id = it.Current;
                            m_pendingIDs.Remove(id);
                        }
                    }
                }
                if (id != 0L)
                {
#if HAMMERTIME
                    // Download data from hammertime citadel hunt project
                    // Avoids some access and API key problems on private citadels
                    var url = new Uri(string.Format(NetworkConstants.HammertimeCitadel, id));
                    Util.DownloadJsonAsync<HammertimeStructureList>(url, null).ContinueWith((task) =>
                    {
                        OnQueryStationUpdated(task, id);
                    });
#else
                    EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIStructure>(
                        ESIAPIGenericMethods.CitadelInfo, id, OnQueryStationUpdated, id);
#endif
                }
            }

#if HAMMERTIME
            private void OnQueryStationUpdated(Task<JsonResult<HammertimeStructureList>> result, long id)
            {
                JsonResult<HammertimeStructureList> jsonResult;

                // Bail if there is an error
                if (result.IsFaulted || (jsonResult = result.Result).HasError)
                {
                    EveMonClient.Notifications.NotifyCitadelQueryError(null);
                    m_queryPending = false;
                    return;
                }

                EveMonClient.Notifications.InvalidateAPIError();

                // Should only have one result, with an integer key
                var citInfo = jsonResult.Result;
                if (citInfo.Count == 1)
                    AddToCache(id, citInfo.Values.First().ToXMLItem(id));
                else
                    // Requested, but failed
                    AddToCache(id, null);
            }
#else
            private void OnQueryStationUpdated(EsiResult<EsiAPIStructure> result, object idObject)
            {
                long id = (idObject as long?) ?? 0L;

                // Bail if there is an error
                if (result.HasError || id == 0L)
                {
                    EveMonClient.Notifications.NotifyCitadelQueryError(result);
                    m_queryPending = false;
                    return;
                }

                EveMonClient.Notifications.InvalidateAPIError();

                AddToCache(id, result.Result.ToXMLItem(id));
            }
#endif

            private void AddToCache(long id, SerializableOutpost station)
            {
                lock (s_cacheList)
                {
                    // Add resulting names to the cache; duplicates should not occur, but
                    // guard against them defensively
                    if (s_cacheList.ContainsKey(id))
                        s_cacheList[id] = station;
                    else
                        s_cacheList.Add(id, station);
                    m_requested.Add(id);
                }
                OnLookupComplete();
            }

            protected override void TriggerEvent()
            {
                EveMonClient.OnConquerableStationListUpdated();
            }
        }
    }
}
