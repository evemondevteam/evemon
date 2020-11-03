using System;
using System.Collections.Generic;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Represents a plan made for computation, without the automatic priorities fixing and prerequisites checks. 
    /// It therefore provides different methods from <see cref="Plan"/> for insertions and removals, to allow usage of this class as a regular collection.
    /// It also does not fire events.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class PlanScratchpad : BasePlan
    {
        #region Disposable

        /// <summary>
        /// A stub disposable object.
        /// </summary>
        private sealed class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion


        private static readonly Disposable s_disposable = new Disposable();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public PlanScratchpad(BaseCharacter character)
            : base(character)
        {
        }

        /// <summary>
        /// Constructor from an enumeration.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="entries"></param>
        public PlanScratchpad(BaseCharacter character, IEnumerable<PlanEntry> entries)
            : this(character)
        {
            AddRange(entries);
        }


        #region Event firing and suppression

        /// <summary>
        /// Returns an <see cref="IDisposable"/> object which suspends events notification and will resume them once disposed.
        /// </summary>
        /// <remarks>Use the returned object in a <c>using</c> block to ensure the disposal of the object even when exceptions are thrown.</remarks>
        /// <returns></returns>
        public override IDisposable SuspendingEvents() => s_disposable;

        /// <summary>
        /// Notify changes happened in the entries
        /// </summary>
        internal override void OnChanged(PlanChange change)
        {
        }

        #endregion


        #region Insertions and removals

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(PlanEntry entry)
        {
            AddCore(entry);
        }

        /// <summary>
        /// Adds the provided items.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <exception cref="System.ArgumentNullException">entries</exception>
        public void AddRange(IEnumerable<PlanEntry> entries)
        {
            entries.ThrowIfNull(nameof(entries));

            foreach (PlanEntry entry in entries)
            {
                AddCore(entry);
            }
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        public void Remove(PlanEntry entry)
        {
            entry.ThrowIfNull(nameof(entry));

            int index = IndexOf(entry.Skill, entry.Level);
            if (index != -1)
                RemoveCore(index);
        }

        #endregion


        #region Simple sort

        /// <summary>
        /// Performs a simple ordering by the given sort criteria, based on the latest statistics.
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="reverseOrder"></param>
        internal void SimpleSort(PlanEntrySort sort, bool reverseOrder)
        {
            // Apply simple sort operators
            switch (sort)
            {
                case PlanEntrySort.None:
                    break;
                case PlanEntrySort.Name:
                    Items.StableSort(PlanEntrySorter.CompareByName);
                    break;
                case PlanEntrySort.Cost:
                    Items.StableSort(PlanEntrySorter.CompareByCost);
                    break;
                case PlanEntrySort.PrimaryAttribute:
                    Items.StableSort(PlanEntrySorter.CompareByPrimaryAttribute);
                    break;
                case PlanEntrySort.SecondaryAttribute:
                    Items.StableSort(PlanEntrySorter.CompareBySecondaryAttribute);
                    break;
                case PlanEntrySort.Priority:
                    Items.StableSort(PlanEntrySorter.CompareByPriority);
                    break;
                case PlanEntrySort.PlanGroup:
                    Items.StableSort(PlanEntrySorter.CompareByPlanGroup);
                    break;
                case PlanEntrySort.PercentCompleted:
                    Items.StableSort(PlanEntrySorter.CompareByPercentCompleted);
                    break;
                case PlanEntrySort.Rank:
                    Items.StableSort(PlanEntrySorter.CompareByRank);
                    break;
                case PlanEntrySort.Notes:
                    Items.StableSort(PlanEntrySorter.CompareByNotes);
                    break;
                case PlanEntrySort.PlanType:
                    Items.StableSort(PlanEntrySorter.CompareByPlanType);
                    break;
                case PlanEntrySort.TimeDifference:
                    Items.StableSort(PlanEntrySorter.CompareByTimeDifference);
                    break;
                case PlanEntrySort.TrainingTime:
                    Items.StableSort(PlanEntrySorter.CompareByTrainingTime);
                    break;
                case PlanEntrySort.TrainingTimeNatural:
                    Items.StableSort(PlanEntrySorter.CompareByTrainingTimeNatural);
                    break;
                case PlanEntrySort.SkillGroupDuration:
                    Dictionary<StaticSkillGroup, TimeSpan> skillGroupsDurations = new Dictionary<StaticSkillGroup, TimeSpan>();
                    Items.StableSort((x, y) => PlanEntrySorter.CompareBySkillGroupDuration(x, y, Items, skillGroupsDurations));
                    break;
                case PlanEntrySort.SPPerHour:
                    Items.StableSort(PlanEntrySorter.CompareBySPPerHour);
                    break;
                case PlanEntrySort.SkillPointsRequired:
                    Items.StableSort(PlanEntrySorter.CompareBySkillPointsRequired);
                    break;
                case PlanEntrySort.OmegaRequired:
                    Items.StableSort(PlanEntrySorter.CompareByOmegaRequired);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Reverse order
            if (reverseOrder)
                Items.Reverse();
        }

        #endregion
    }
}
