using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Models;

namespace EVEMon.Common.Data
{
    public class MasteryShipCollection : ReadonlyKeyedCollection<int, MasteryShip>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasteryShipCollection"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        public MasteryShipCollection(Character character)
        {
            // Builds the list
            foreach (var masteryShip in StaticMasteries.AllMasteryShips)
                if (masteryShip.Ship != null)
                    Items[masteryShip.Ship.ID] = new MasteryShip(character, masteryShip);
        }

        /// <summary>
        /// Gets the mastery ship by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MasteryShip GetMasteryShipByID(int id) => Items.ContainsKey(id) ? Items[id] :
            null;

        /// <summary>
        /// Initializes each item in the collection.
        /// </summary>
        public void Initialize()
        {
            foreach (KeyValuePair<int, MasteryShip> item in Items)
                item.Value.Initialize();
        }
    }
}
