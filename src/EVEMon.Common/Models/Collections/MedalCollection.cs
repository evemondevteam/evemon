using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Models.Collections
{
    public sealed class MedalCollection : ReadonlyCollection<Medal>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal MedalCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable medals from the API.</param>
        /// <param name="isCharMedal">If true, the medals are for a character (and will be
        /// assigned to group CurrentCorporation or OtherCorporation); if false, the medals are
        /// for a corporation and will all be assigned group Corporation.</param>
        internal void Import(IEnumerable<EsiMedalsListItem> src, bool isCharMedal)
        {
            Items.Clear();

            // Import the medals from the API
            foreach (EsiMedalsListItem srcMedal in src)
            {
                MedalGroup group;
                if (!isCharMedal)
                    group = MedalGroup.Corporation;
                else if (m_character.CorporationID == srcMedal.CorporationID)
                    group = MedalGroup.CurrentCorporation;
                else
                    group = MedalGroup.OtherCorporation;
                Items.Add(new Medal(m_character, srcMedal, group));
            }

            // Assign the 'number of times awarded'
            foreach (Medal medal in Items.ToList())
                medal.TimesAwarded = Items.Count(x => x.ID == medal.ID);
        }
    }
}
