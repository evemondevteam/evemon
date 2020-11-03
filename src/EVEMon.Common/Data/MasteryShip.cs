using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;
using System.Collections.Generic;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery ship.
    /// </summary>
    public sealed class MasteryShip : ReadonlyCollection<Mastery>
    {

        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="ship">The ship.</param>
        internal MasteryShip(SerializableMasteryShip src, Ship ship) : base(src.Masteries.
            Count)
        {
            Ship = ship;
            // Add in sorted order 1-5
            var masteriesSorted = new List<Mastery>(src.Masteries.Count);
            foreach (var mastery in src.Masteries)
                masteriesSorted.Add(new Mastery(this, mastery));
            masteriesSorted.Sort();
            Items.AddRange(masteriesSorted);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MasteryShip"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="masteryShip">The mastery ship.</param>
        internal MasteryShip(Character character, MasteryShip masteryShip) : base(masteryShip.
            Count)
        {
            Character = character;
            Ship = masteryShip.Ship;
            // Add in sorted order 1-5
            var masteriesSorted = new List<Mastery>(masteryShip.Count);
            foreach (var mastery in masteryShip)
                masteriesSorted.Add(new Mastery(character, mastery));
            masteriesSorted.Sort();
            Items.AddRange(masteriesSorted);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the character this certificate is bound to.
        /// </summary>
        public Character Character { get; }

        /// <summary>
        /// Gets the ship.
        /// </summary>
        public Ship Ship { get; }

        /// <summary>
        /// Gets the highest trained mastery level.
        /// May be null if no level has been trained.
        /// </summary>
        public Mastery HighestTrainedLevel => Items.LastOrDefault(level => level.IsTrained);

        #endregion

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public Mastery GetLevel(int level) => Items.FirstOrDefault(mastery => mastery.Level ==
            level);

        /// <summary>
        /// Initializes the mastery.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">character</exception>
        internal void Initialize()
        {
            bool updatedAnything;
            do
            {
                updatedAnything = false;
                bool previousUntrained = false;
                // Iterate 1 to 5
                // If a mastery is untrained, then we know that all levels above it are
                // also untrained
                foreach (var mastery in Items)
                {
                    if (!previousUntrained)
                    {
                        updatedAnything = updatedAnything | mastery.TryUpdateMasteryStatus();
                        previousUntrained = mastery.Status == Enumerations.MasteryStatus.
                            Untrained;
                    }
                    else
                        // This method had side effects (setting m_updated to true) which are
                        // also satisfied here
                        updatedAnything = updatedAnything | mastery.SetAsUntrained();
                }
            } while (updatedAnything);
        }
    }
}
