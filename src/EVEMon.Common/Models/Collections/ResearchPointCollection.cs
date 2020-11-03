using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class ResearchPointCollection : ReadonlyCollection<ResearchPoint>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal ResearchPointCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable research points from the API.</param>
        internal void Import(IEnumerable<EsiResearchListItem> src)
        {
            Items.Clear();

            // Import the research points from the API
            foreach (EsiResearchListItem srcResearchPoint in src)
            {
                Items.Add(new ResearchPoint(srcResearchPoint, m_character));
            }
        }
    }
}
