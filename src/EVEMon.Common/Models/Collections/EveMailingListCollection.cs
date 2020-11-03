using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class EveMailingListCollection : ReadonlyCollection<EveMailingList>
    {
        private readonly CCPCharacter m_ccpCharacter;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal EveMailingListCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The source.</param>
        internal void Import(IEnumerable<EsiMailingListsListItem> src)
        {
            Items.Clear();

            // Import the mail messages from the API
            foreach (EsiMailingListsListItem srcEVEMailingList in src)
            {
                Items.Add(new EveMailingList(srcEVEMailingList));
            }

            // Fires the event regarding EVE mailing lists update
            EveMonClient.OnCharacterEVEMailingListsUpdated(m_ccpCharacter);
        }
    }
}
