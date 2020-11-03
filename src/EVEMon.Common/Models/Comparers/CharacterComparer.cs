using System;
using System.Collections.Generic;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.Extensions;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Models.Comparers
{
    /// <summary>
    /// Represents a customizable characters comparer
    /// </summary>
    public struct CharacterComparer : IComparer<Character>
    {
        #region Constructors

        /// <summary>
        /// Constructor for an ascending sort along the given criteria.
        /// </summary>
        public CharacterComparer(CharacterSortCriteria criteria)
            : this()
        {
            Criteria = criteria;
            Order = SortOrder.Ascending;
        }

        /// <summary>
        /// Constructor with custom parameters.
        /// </summary>
        public CharacterComparer(CharacterSortCriteria criteria, SortOrder order)
            : this()
        {
            Criteria = criteria;
            Order = order;
        }

        /// <summary>
        /// Constructor from a tray popup setting
        /// </summary>
        /// <param name="criteria"></param>
        public CharacterComparer(TrayPopupSort criteria)
            : this()
        {
            switch (criteria)
            {
                case TrayPopupSort.NameASC:
                    Criteria = CharacterSortCriteria.Name;
                    Order = SortOrder.Ascending;
                    break;
                case TrayPopupSort.NameDESC:
                    Criteria = CharacterSortCriteria.Name;
                    Order = SortOrder.Descending;
                    break;
                case TrayPopupSort.TrainingCompletionTimeASC:
                    Criteria = CharacterSortCriteria.TrainingCompletion;
                    Order = SortOrder.Ascending;
                    break;
                case TrayPopupSort.TrainingCompletionTimeDESC:
                    Criteria = CharacterSortCriteria.TrainingCompletion;
                    Order = SortOrder.Descending;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the sort order
        /// </summary>
        public SortOrder Order { get; }

        /// <summary>
        /// Gets or sets the sort criteria
        /// </summary>
        public CharacterSortCriteria Criteria { get; }

        #endregion


        #region Compare Methods

        /// <summary>
        /// Performs the comparison
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Character x, Character y)
        {
            // Exchange items when descending sort
            if (Order == SortOrder.Descending)
            {
                Character temp = x;
                x = y;
                y = temp;
            }

            // Deal with the criteria
            switch (Criteria)
            {
                case CharacterSortCriteria.TrainingCompletion:
                    return CompareByCompletionTime(x, y);
                case CharacterSortCriteria.Name:
                    return CompareByName(x, y);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Compares the two characters by their training completion time or, when not in training their names
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// x
        /// or
        /// y
        /// </exception>
        public static int CompareByCompletionTime(Character x, Character y)
        {
            x.ThrowIfNull(nameof(x));

            y.ThrowIfNull(nameof(y));

            // Get their training skills
            QueuedSkill skillX = x.CurrentlyTrainingSkill;
            QueuedSkill skillY = y.CurrentlyTrainingSkill;
            if (skillX == null && skillY == null)
                return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
            if (skillX == null || skillY == null)
                return -1;

            // Compare end time
            return DateTime.Compare(skillX.EndTime, skillY.EndTime);
        }

        /// <summary>
        /// Compare the given characters by their names
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// x
        /// or
        /// y
        /// </exception>
        public static int CompareByName(Character x, Character y)
        {
            x.ThrowIfNull(nameof(x));

            y.ThrowIfNull(nameof(y));

            return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
        }

        #endregion
    }
}