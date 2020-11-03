using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections.Global;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Constants;
using System;
using System.Globalization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Stores all the data regarding geography.
    /// </summary>
    public static class StaticGeography
    {
        #region Fields

        private static readonly Dictionary<int, Faction> s_factionsByID = new Dictionary<int, Faction>();
        private static readonly Dictionary<int, Region> s_regionsByID = new Dictionary<int, Region>();
        private static readonly Dictionary<int, Constellation> s_constellationsByID = new Dictionary<int, Constellation>();
        private static readonly Dictionary<long, Planet> s_planetsByID = new Dictionary<long, Planet>();
        private static readonly Dictionary<int, SolarSystem> s_solarSystemsByID = new Dictionary<int, SolarSystem>();
        private static readonly Dictionary<long, Station> s_stationsByID = new Dictionary<long, Station>();
        private static readonly Dictionary<int, NPCCorporation> s_corporationsByID = new Dictionary<int, NPCCorporation>();
        private static readonly Dictionary<int, Agent> s_agentsByID = new Dictionary<int, Agent>();

        #endregion


        #region Initialization

        /// <summary>
        /// Initialize static geography.
        /// </summary>
        internal static void Load()
        {
            GeoDatafile datafile = LoadGeoData();
            LoadFactions();

            CompleteInitialization(datafile);

            GlobalDatafileCollection.OnDatafileLoaded();
        }

        /// <summary>
        /// Initialize the NPC factions.
        /// </summary>
        private static void LoadFactions()
        {
            // This is a workaround until XmlGenerator can be updated
            foreach (string factionInfo in Properties.Resources.chrFactions.Split('\n'))
            {
                string[] entries = factionInfo.Split(',');
                NPCCorporation baseCorp = null, militiaCorp = null;
                if (entries.Length > 9)
                {
                    // factionID,factionName,description,raceIDs,solarSystemID,corporationID,
                    // sizeFactor,stationCount,stationSystemCount,militiaCorporationID,iconID
                    int id, end = entries.Length, corpID, militiaID;
                    string factionName = entries[1].Trim();
                    // Find executor and militia corps (also NPC)
                    if (entries[end - 2].TryParseInv(out militiaID))
                        militiaCorp = GetCorporationByID(militiaID);
                    if (entries[end - 6].TryParseInv(out corpID))
                        baseCorp = GetCorporationByID(corpID);
                    if (entries[0].TryParseInv(out id) && !string.IsNullOrEmpty(factionName) &&
                            id > 0 && baseCorp != null)
                        s_factionsByID.Add(id, new Faction(id, baseCorp, militiaCorp,
                            factionName));
                }
            }
        }

        /// <summary>
        /// Initializes the geography gzip data file.
        /// </summary>
        private static GeoDatafile LoadGeoData()
        {
            var datafile = Util.DeserializeDatafile<GeoDatafile>(DatafileConstants.GeographyDatafile,
                Util.LoadXslt(Properties.Resources.DatafilesXSLT));

            // Generate the nodes
            foreach (SerializableRegion srcRegion in datafile.Regions)
            {
                Region region = new Region(srcRegion);
                s_regionsByID[srcRegion.ID] = region;

                // Store the children into their dictionaries
                foreach (Constellation constellation in region)
                {
                    s_constellationsByID[constellation.ID] = constellation;

                    foreach (SolarSystem solarSystem in constellation)
                    {
                        s_solarSystemsByID[solarSystem.ID] = solarSystem;

                        // Add planets to global lookup
                        var systemPlanets = solarSystem.Planets;
                        if (systemPlanets != null)
                            foreach (var planet in systemPlanets)
                                s_planetsByID[planet.ID] = planet;

                        foreach (Station station in solarSystem)
                        {
                            s_stationsByID[station.ID] = station;

                            s_corporationsByID[station.CorporationID] = new NPCCorporation(station);

                            foreach (Agent agent in station)
                                s_agentsByID[agent.ID] = agent;
                        }
                    }
                }
            }

            return datafile;
        }

        /// <summary>
        /// Completes the initialization.
        /// </summary>
        /// <param name="datafile">The datafile.</param>
        private static void CompleteInitialization(GeoDatafile datafile)
        {
            // Connects the systems
            foreach (SerializableJump srcJump in datafile.Jumps)
            {
                SolarSystem a = GetSolarSystemByID(srcJump.FirstSystemID);
                SolarSystem b = GetSolarSystemByID(srcJump.SecondSystemID);

                if (a == null || b == null)
                    continue;

                a.AddNeighbor(b);
                b.AddNeighbor(a);
            }

            foreach (SolarSystem system in s_solarSystemsByID.Values)
            {
                system.TrimNeighbors();
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all the regions in the universe.
        /// </summary>
        public static IEnumerable<Region> AllRegions => s_regionsByID.Values;

        /// <summary>
        /// Gets an enumeration of all the constellations in the universe.
        /// </summary>
        public static IEnumerable<Constellation> AllConstellations => s_constellationsByID.Values;

        /// <summary>
        /// Gets an enumeration of all the solar systems in the universe.
        /// </summary>
        public static IEnumerable<SolarSystem> AllSolarSystems => s_solarSystemsByID.Values;

        /// <summary>
        /// Gets an enumeration of all the stations in the universe.
        /// </summary>
        public static IEnumerable<Station> AllStations => s_stationsByID.Values;

        /// <summary>
        /// Gets an enumeration of all the NPCCorporations in the universe.
        /// </summary>
        public static IEnumerable<NPCCorporation> AllNPCCorporations => s_corporationsByID.Values;

        /// <summary>
        /// Gets an enumeration of all the agents in the universe.
        /// </summary>
        public static IEnumerable<Agent> AllAgents => s_agentsByID.Values;

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the region with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Region GetRegionByID(int id)
        {
            Region result;
            s_regionsByID.TryGetValue(id, out result);
            return result;
        }
        
        /// <summary>
        /// Gets the constellation with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Constellation GetConstellationByID(int id)
        {
            Constellation result;
            s_constellationsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the planet with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Planet GetPlanetByID(int id)
        {
            Planet result;
            s_planetsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the system with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static SolarSystem GetSolarSystemByID(int id)
        {
            SolarSystem result;
            s_solarSystemsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the system name with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The system name, or EveMonConstants.UnknownText if no system has this ID</returns>
        public static string GetSolarSystemName(int id)
        {
            return GetSolarSystemByID(id)?.Name ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the system with the provided name. Slow!
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static SolarSystem GetSolarSystemByName(string name) => s_solarSystemsByID.
            Values.FirstOrDefault(system => system.Name == name);

        /// <summary>
        /// Gets the station with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Station GetStationByID(long id)
        {
            Station result;
            s_stationsByID.TryGetValue(id, out result);
            return result;
        }
        
        /// <summary>
        /// Gets the NPC Corporation with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static NPCCorporation GetCorporationByID(int id)
        {
            NPCCorporation result;
            s_corporationsByID.TryGetValue(id, out result);
            return result;
        }
        
        /// <summary>
        /// Gets the agent with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Agent GetAgentByID(int id)
        {
            Agent result;
            s_agentsByID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets the agent with the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Agent GetAgentByName(string name) => s_agentsByID.Values.FirstOrDefault(
            agent => agent.Name.Equals(name, StringComparison.InvariantCulture));

        /// <summary>
        /// Gets the faction with the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Faction GetFactionByID(int id)
        {
            Faction result;
            s_factionsByID.TryGetValue(id, out result);
            return result;
        }

        #endregion


        /// <summary>
        /// The description of the range.
        /// </summary>
        public static string GetRange(int range)
        {
            switch (range)
            {
                case 0:
                    return "stations";
                case 1:
                    return "solar systems";
                case 2:
                    return "5 jumps";
                case 3:
                    return "10 jumps";
                case 4:
                    return "20 jumps";
                case 5:
                    return "regions";
                default:
                    return string.Empty;
            }
        }
    }
}
