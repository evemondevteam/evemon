using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;

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
        internal void Import(IEnumerable<SerializableAssetListItem> src)
        {
            m_isImporting = true;

            Items.Clear();

            // Import the assets from the API
            foreach (SerializableAssetListItem srcAsset in src)
            {
                Asset asset = new Asset(srcAsset);
                asset.Jumps = GetJumps(asset);
                Items.Add(asset);

                Items.AddRange(srcAsset.Contents.Select(content => new Asset(content)
                {
                    LocationID = asset.LocationID,
                    Container = asset.Item.Name,
                    Jumps = asset.Jumps
                }));
            }

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
            SolarSystem lastKnownSolaSystem = m_character.LastKnownSolarSystem ?? m_character.LastKnownStation?.SolarSystem;

            // When data to calculate jumps are insufficient return a default value
            if (lastKnownSolaSystem == null || asset.SolarSystem == null)
                return -1;

            // Reset everything if the character changed solar system
            if (m_lastStoredCharacterKnownSolarSystem != lastKnownSolaSystem)
            {
                m_jumps.Clear();
                m_lastStoredCharacterKnownSolarSystem = lastKnownSolaSystem;
            }

            // If the asset's solar sytem is known return the stored jumps
            if (m_jumps.ContainsKey(asset.SolarSystem))
                return m_jumps[asset.SolarSystem];

            // Calculate the jumps between the character and the asset
            int jumps = lastKnownSolaSystem.GetFastestPathTo(asset.SolarSystem, PathSearchCriteria.FewerJumps)
                .Count(system => system != lastKnownSolaSystem);

            // Store the calculated jumps
            m_jumps[asset.SolarSystem] = jumps;

            // Return value
            return jumps;
        }
    }
}
