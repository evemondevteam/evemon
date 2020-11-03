using System.Collections.Generic;
using EVEMon.Common.Collections.Global;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// The static list of the masteries.
    /// </summary>
    public static class StaticMasteries
    {
        private static readonly Dictionary<int, MasteryShip> s_masteryShipsByID =
            new Dictionary<int, MasteryShip>();


        #region Initialization

        /// <summary>
        /// Initialize static masteries.
        /// </summary>
        internal static void Load()
        {
            MasteriesDatafile datafile = Util.DeserializeDatafile<MasteriesDatafile>(
                DatafileConstants.MasteriesDatafile);

            foreach (SerializableMasteryShip srcShip in datafile.MasteryShips)
            {
                Ship ship = StaticItems.GetItemByID(srcShip.ID) as Ship;
                if (ship != null)
                    s_masteryShipsByID[ship.ID] = new MasteryShip(srcShip, ship);
            }

            GlobalDatafileCollection.OnDatafileLoaded();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an enumeration of all the mastery ships.
        /// </summary>
        public static IEnumerable<MasteryShip> AllMasteryShips => s_masteryShipsByID.Values;

        #endregion

    }
}
