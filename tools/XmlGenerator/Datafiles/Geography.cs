using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.Interfaces;
using EVEMon.XmlGenerator.Providers;
using EVEMon.XmlGenerator.Utils;

namespace EVEMon.XmlGenerator.Datafiles
{
    internal static class Geography
    {
        private const double BaseDistance = 1.0E14;

        /// <summary>
        /// Generates the geography datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write(@"Generating geography datafile... ");

            // Regions
            IEnumerable<SerializableRegion> regions = Database.MapRegionsTable.Select(
                srcRegion =>
                {
                    Util.UpdatePercentDone(Database.GeographyTotalCount);
                    SerializableRegion region = new SerializableRegion
                    {
                        ID = srcRegion.ID,
                        Name = srcRegion.Name
                    };

                    // Constellations
                    region.Constellations.AddRange(ExportConstellations(srcRegion).OrderBy(x => x.Name));
                    return region;
                });

            // Jumps
            IEnumerable<SerializableJump> jumps = Database.MapSolarSystemJumpsTable.Where(srcJump => srcJump.A < srcJump.B)
                .Select(srcJump => new SerializableJump
                {
                    FirstSystemID = srcJump.A,
                    SecondSystemID = srcJump.B
                });

            // Serialize
            GeoDatafile datafile = new GeoDatafile();
            datafile.Regions.AddRange(regions.OrderBy(x => x.Name));
            datafile.Jumps.AddRange(jumps);

            Util.DisplayEndTime(stopwatch);

            Util.SerializeXml(datafile, DatafileConstants.GeographyDatafile);
        }

        /// <summary>
        /// Exports the constellations.
        /// </summary>
        /// <param name="srcRegion">The source region.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableConstellation> ExportConstellations(IHasID srcRegion)
            => Database.MapConstellationsTable.Where(x => x.RegionID == srcRegion.ID)
                .Select(srcConstellation =>
                {
                    SerializableConstellation constellation = new SerializableConstellation
                    {
                        ID = srcConstellation.ID,
                        Name = srcConstellation.Name
                    };

                    // Systems
                    constellation.Systems.AddRange(ExportSystems(srcConstellation).OrderBy(x => x.Name));
                    return constellation;
                });

        /// <summary>
        /// Exports the systems.
        /// </summary>
        /// <param name="srcConstellation">The source constellation.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableSolarSystem> ExportSystems(IHasID srcConstellation)
            => Database.MapSolarSystemsTable.Where(x => x.ConstellationID == srcConstellation.ID)
                .Select(srcSystem =>
                {
                    SerializableSolarSystem system = new SerializableSolarSystem
                    {
                        ID = srcSystem.ID,
                        Name = srcSystem.Name,
                        X = (int)(srcSystem.X / BaseDistance),
                        Y = (int)(srcSystem.Y / BaseDistance),
                        Z = (int)(srcSystem.Z / BaseDistance),
                        SecurityLevel = srcSystem.SecurityLevel
                    };

                    // Stations
                    system.Stations.AddRange(ExportStations(srcSystem).OrderBy(x => x.Name));
                    return system;
                });

        /// <summary>
        /// Exports the stations.
        /// </summary>
        /// <param name="srcSystem">The SRC system.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableStation> ExportStations(IHasID srcSystem)
            => Database.StaStationsTable.Where(x => x.SolarSystemID == srcSystem.ID)
                .Select(srcStation =>
                {
                    SerializableStation station = new SerializableStation
                    {
                        ID = srcStation.ID,
                        Name = srcStation.Name,
                        CorporationID = srcStation.CorporationID,
                        CorporationName = Database.InvNamesTable[srcStation.CorporationID].Name,
                        ReprocessingEfficiency = srcStation.ReprocessingEfficiency,
                        ReprocessingStationsTake = srcStation.ReprocessingStationsTake
                    };

                    // Agents
                    station.Agents.AddRange(ExportAgents(srcStation));
                    return station;
                });

        /// <summary>
        /// Exports the agents.
        /// </summary>
        /// <param name="srcStation">The station.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableAgent> ExportAgents(IHasLongID srcStation)
            => Database.AgtAgentsTable
                .Where(x => x.LocationID == srcStation.ID)
                .Select(agent => new SerializableAgent
                {
                    ID = agent.ID,
                    Level = agent.Level,
                    Quality = agent.Quality,
                    Name = Database.InvNamesTable[agent.ID].Name,
                    DivisionName = Database.CrpNPCDivisionsTable[agent.DivisionID].DivisionName,
                    AgentType = Database.AgtAgentTypesTable[agent.AgentTypeID].AgentType,
                    ResearchSkillID = Database.AgtResearchAgentsTable.Any(x => x.ID == agent.ID)
                        ? Database.AgtResearchAgentsTable[agent.ID].ResearchSkillID
                        : 0,
                    LocatorService = agent.IsLocator
                });
    }
}
