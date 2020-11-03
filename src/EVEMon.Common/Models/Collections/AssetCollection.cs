using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class AssetCollection : ReadonlyCollection<Asset>
    {
        private readonly CCPCharacter m_character;
        private readonly Dictionary<SolarSystem, int> m_jumps = new Dictionary<SolarSystem, int>();

        private SolarSystem m_lastStoredCharacterKnownSolarSystem;
        private bool m_isImporting;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal AssetCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable assets from the API.</param>
        internal void Import(IEnumerable<EsiAssetListItem> src)
        {
            var newAssets = src.Select((asset) => new Asset(asset, m_character)).ToList();
            // Create a lookup table to find asset containers by ID
            var lookup = new Dictionary<long, Asset>(newAssets.Count);
            foreach (var asset in src)
            {
                long id = asset.ItemID;
                if (!lookup.ContainsKey(id))
                    // Create asset objects, this can be done outside of the importing process
                    lookup.Add(id, new Asset(asset, m_character));
            }
            // Step 2: Match the items to their containers
            Asset root, container;
            int levels;
            foreach (Asset asset in newAssets)
            {
                // Try to find the parent in the assets list
                root = asset;
                for (levels = 0; levels < 2 && lookup.TryGetValue(root.LocationID, out
                    container); levels++)
                {
                    root = container;
                }
                if (levels > 0)
                {
                    // If it succeeded at least once, put root location ID as the location
                    asset.Container = root.Item.Name;
                    asset.LocationID = root.LocationID;
                }
                asset.UpdateLocation();
            }
            // Step 3: Initial jump totals
            foreach (Asset asset in newAssets)
                asset.Jumps = GetJumps(asset);

            m_isImporting = true;
            Items.Clear();
            foreach (Asset asset in newAssets)
                Items.Add(asset);
            m_isImporting = false;
        }

        /// <summary>
        /// Updates the location.
        /// </summary>
        public void UpdateLocation()
        {
            foreach (Asset asset in Items.TakeWhile(asset => !m_isImporting))
            {
                asset.UpdateLocation();
                asset.Jumps = GetJumps(asset);
            }
        }

        /// <summary>
        /// Gets the jumps.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        private int GetJumps(Asset asset)
        {
            SolarSystem lastKnownSolarSystem = m_character.LastKnownSolarSystem ??
                (m_character.LastKnownStation?.SolarSystem);

            // When data to calculate jumps are insufficient return a default value
            if (lastKnownSolarSystem == null || asset.SolarSystem == null)
                return -1;

            // Reset everything if the character changed solar system
            if (m_lastStoredCharacterKnownSolarSystem != lastKnownSolarSystem)
            {
                m_jumps.Clear();
                m_lastStoredCharacterKnownSolarSystem = lastKnownSolarSystem;
            }

            // If the asset's solar sytem is known return the stored jumps
            if (m_jumps.ContainsKey(asset.SolarSystem))
                return m_jumps[asset.SolarSystem];

            // Calculate the jumps between the character and the asset
            int jumps = lastKnownSolarSystem.GetFastestPathTo(asset.SolarSystem,
                PathSearchCriteria.FewerJumps).Count(system => system != lastKnownSolarSystem);

            // Store the calculated jumps
            m_jumps[asset.SolarSystem] = jumps;

            // Return value
            return jumps;
        }
    }
}
