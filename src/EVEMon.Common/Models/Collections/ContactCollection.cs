using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class ContactCollection : ReadonlyCollection<Contact>
    {
        private readonly CCPCharacter m_character;
        
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal ContactCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable contacts from the API.</param>
        internal void Import(IEnumerable<EsiContactListItem> src)
        {
            Items.Clear();

            // Import the contacts from the API
            foreach (EsiContactListItem srcContact in src)
                Items.Add(new Contact(srcContact));
        }
    }
}
