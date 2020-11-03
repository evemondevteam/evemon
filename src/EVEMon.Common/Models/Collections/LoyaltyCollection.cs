using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class LoyaltyCollection : ReadonlyCollection<Loyalty>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal LoyaltyCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable loyalty point info from the API.</param>
        internal void Import(IEnumerable<EsiLoyaltyListItem> src)
        {
            Items.Clear();

            // Import the loyalty point info from the API
            foreach (EsiLoyaltyListItem srcloyalty in src)
            {
                Items.Add(new Loyalty(m_character, srcloyalty));
            }
        }
    }
}
