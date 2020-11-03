using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a character's plan
    /// </summary>
    [EnforceUIThreadAffinity]
    public abstract class BasePlan : ReadonlyCollection<PlanEntry>
    {
        private readonly PlanEntry[] m_lookup;
        private string m_name;
        private string m_description;

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        protected BasePlan(BaseCharacter character)
        {
            m_lookup = new PlanEntry[StaticSkills.ArrayIndicesCount * 5];
            Character = character;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the plan's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                if (IsConnected)
                    EveMonClient.OnPlanNameChanged((Plan)this);
            }
        }

        /// <summary>
        /// Gets or sets the plan's description.
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                if (IsConnected)
                    EveMonClient.OnPlanNameChanged((Plan)this);
            }
        }

        /// <summary>
        /// Gets or sets true if the plan is connected to a character and will send notifications.
        /// When false, it's just a computing helper.
        /// </summary>
        internal bool IsConnected { get; set; }
        
        /// <summary>
        /// Gets or sets the implant set chosen by the user.
        /// </summary>
        public ImplantSet ChosenImplantSet { get; set; }

        /// <summary>
        /// Gets the owner of this plan.
        /// </summary>
        public BaseCharacter Character { get; }

        /// <summary>
        /// Does the plan contain obsolete entries.
        /// </summary>
        public bool ContainsObsoleteEntries
        {
            get
            {
                using (SuspendingEvents())
                {
                    if (Items.Any(pe => Character.GetSkillLevel(pe.Skill) >= pe.Level))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// List of Obsolete Entires.
        /// </summary>
        public IEnumerable<PlanEntry> ObsoleteEntries
        {
            get
            {
                using (SuspendingEvents())
                {
                    foreach (PlanEntry pe in Items.Where(pe => Character.GetSkillLevel(pe.Skill) >= pe.Level))
                    {
                        yield return pe;
                    }
                }
            }
        }

        #endregion


        #region Event firing and suppression

        /// <summary>
        /// Returns an <see cref="IDisposable"/> object which suspends events notification and will resume them once disposed.
        /// </summary>
        /// <returns></returns>
        public abstract IDisposable SuspendingEvents();

        /// <summary>
        /// Notify changes happened in the entries
        /// </summary>
        internal abstract void OnChanged(PlanChange change);

        #endregion


        #region Statistics

        /// <summary>
        /// Gets the total training time for this plan
        /// </summary>
        public TimeSpan TotalTrainingTime => GetTotalTime(null, true);

        /// <summary>
        /// Gets the total training time for this plan, using the given scratchpad.
        /// </summary>
        /// <param name="scratchpad">The scratchpad to use for the computation, may be null.</param>
        /// <param name="applyRemappingPoints"></param>
        /// <returns></returns>
        public TimeSpan GetTotalTime(CharacterScratchpad scratchpad, bool applyRemappingPoints)
        {
            // No scratchpad ? Let's create one
            if (scratchpad == null)
                scratchpad = new CharacterScratchpad(Character);

            // Train entries
            TimeSpan time = TimeSpan.Zero;
            scratchpad.TrainEntries(Items, applyRemappingPoints);
            return scratchpad.TrainingTime - time;
        }

        /// <summary>
        /// Gets the total number of unique skills (two levels of same skill counts for one unique skill).
        /// </summary>
        public int UniqueSkillsCount => Items.GetUniqueSkillsCount();

        /// <summary>
        /// Gets the number of not known skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        public int NotKnownSkillsCount => Items.GetNotKnownSkillsCount();

        /// <summary>
        /// Gets the total cost of the skill books, in ISK
        /// </summary>
        public long TotalBooksCost => Items.GetTotalBooksCost();

        /// <summary>
        /// Gets the cost of the not known skill books, in ISK
        /// </summary>
        public long NotKnownSkillBooksCost => Items.GetNotKnownSkillBooksCost();

        /// <summary>
        /// Gets the skill points of the planned skill levels
        /// </summary>
        public long TotalSkillPoints => Items.GetTotalSkillPoints();

        #endregion


        #region General purpose methods

        /// <summary>
        /// Gets the entry matching the given parameters.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        public PlanEntry GetEntry(StaticSkill skill, long level)
        {
            skill.ThrowIfNull(nameof(skill));

            long index = skill.ArrayIndex * 5 + level - 1;

            return level == 0 || index > m_lookup.LongLength ? null : m_lookup[index];
        }

        /// <summary>
        /// Adds the given entry into the items list and the lookup.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        protected void AddCore(PlanEntry entry)
        {
            entry.ThrowIfNull(nameof(entry));

            Items.Add(entry);
            m_lookup[entry.Skill.ArrayIndex * 5 + entry.Level - 1] = entry;
            OnChanged(PlanChange.All);
        }

        /// <summary>
        /// Inserts the given entry into the items list and the lookup.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        protected void InsertCore(int index, PlanEntry entry)
        {
            entry.ThrowIfNull(nameof(entry));

            Items.Insert(index, entry);
            m_lookup[entry.Skill.ArrayIndex * 5 + entry.Level - 1] = entry;
            OnChanged(PlanChange.All);
        }

        /// <summary>
        /// Inserts the given entry into the items list and the lookup.
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveCore(int index)
        {
            PlanEntry entry = Items[index];
            Items.RemoveAt(index);
            m_lookup[entry.Skill.ArrayIndex * 5 + entry.Level - 1] = null;
            OnChanged(PlanChange.All);
        }

        /// <summary>
        /// Move the given entry from the specified index to the provided target index.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="targetIndex"></param>
        protected void MoveCore(int startIndex, int targetIndex)
        {
            PlanEntry entry = Items[startIndex];
            Items.RemoveAt(startIndex);
            Items.Insert(targetIndex, entry);
            OnChanged(PlanChange.All);
        }

        /// <summary>
        /// Gets the index of the matching entry.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <returns>The index of the matching entry when found, -1 otherwise.</returns>
        protected int IndexOf(StaticSkill skill, long level)
        {
            PlanEntry entry = GetEntry(skill, level);
            if (entry == null)
                return -1;
            return Items.IndexOf(entry);
        }

        /// <summary>
        /// Gets true if the given skill is planned.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool IsPlanned(StaticSkill skill)
        {
            for (int i = 0; i <= 5; i++)
            {
                if (IsPlanned(skill, i))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets true if the skill is planned at the given level.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public bool IsPlanned(StaticSkill skill, long level) => GetEntry(skill, level) != null;

        /// <summary>
        /// Gets the highest planned level of the given skill.
        /// </summary>
        /// <param name="staticSkill"></param>
        /// <returns>The highest planned level, or 0 if the skill is not planned.</returns>
        public int GetPlannedLevel(StaticSkill staticSkill)
        {
            for (int i = 5; i > 0; i--)
            {
                if (IsPlanned(staticSkill, i))
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Checks whether the given skill level can be planned. Used to enable or disable the "Plan To N" and "Remove" menu options.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">A integer between 0 (remove all entries for this skill) and 5.</param>
        /// <returns></returns>
        internal bool EnablePlanTo(Skill skill, int level)
        {
            // The entry actually wants to remove the item
            if (level == 0)
                return IsPlanned(skill);

            // The entry is already known
            if (skill.Level >= level)
                return false;

            // The entry is already planned at this very level ?
            return GetPlannedLevel(skill) != level;
        }

        #endregion


        #region Core methods for dealing with prerequisites and priorities

        /// <summary>
        /// Adds the missing prerequisites and fix the prerequisites order
        /// </summary>
        /// <remarks>Complexity is O(n²) on the average and O(n^3) on the worst-case.</remarks>
        public void FixPrerequisites()
        {
            if (!Items.Any() || (Items.Last().CharacterSkill != null && Items.Last().CharacterSkill.IsKnown))
                return;

            // Scroll through entries
            for (int i = 0; i < Items.Count; i++)
            {
                PlanEntry entry = Items[i];
                bool jumpBack = false;

                // Scroll through prerequisites
                if (entry.Skill.Prerequisites.Any(
                    prereq => !EnsurePrerequisiteExistBefore(prereq.Skill, prereq.Level, i, entry.Priority)))
                {
                    jumpBack = true;
                    i--;
                }

                // We went through all the prerequisites, we're now left with the previous level of this skill.
                if (jumpBack)
                    continue;

                // Did we have to insert or move an entry for this previous level ?
                if (!EnsurePrerequisiteExistBefore(entry.Skill, entry.Level - 1, i, entry.Priority))
                {
                    // Then, we jump back to this new entry
                    i--;
                }
            }
        }

        /// <summary>
        /// Checks whether a matching entry exists between before the provided <c>insertionIndex</c>.
        /// If the entry exist and is trained, remove it.
        /// If the entry exist later, it is moved to this <c>insertionIndex</c>.
        /// If the entry does not exit, it is inserted at <c>insertionIndex</c>.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="insertionIndex"></param>
        /// <param name="newEntriesPriority"></param>
        /// <returns>True if the searched entry existed or is already trained; false if an insertion or a move was required.</returns>
        private bool EnsurePrerequisiteExistBefore(StaticSkill skill, long level, int insertionIndex, int newEntriesPriority)
        {
            int skillIndex = IndexOf(skill, level);

            // Is the level already trained by the character ?
            if (Character.GetSkillLevel(skill) >= level)
            {
                // If the level is planned, remove it
                if (skillIndex != -1)
                    RemoveCore(skillIndex);

                return true;
            }

            // Is the prerequisite already planned before this very entry ?
            // Then we continue the foreach loop to the next prereq
            if (skillIndex != -1 && skillIndex < insertionIndex)
                return true;

            // The prerequisite is not planned yet, we insert it just before this very entry
            if (skillIndex == -1)
            {
                PlanEntry newEntry = new PlanEntry(this, skill, level)
                { Type = PlanEntryType.Prerequisite, Priority = newEntriesPriority };

                InsertCore(insertionIndex, newEntry);
                return false;
            }

            // The prerequisite exists but it's located later in the plan
            // So we move it at the insertion index
            MoveCore(skillIndex, insertionIndex);
            return false;
        }

        /// <summary>
        /// Check that the plan has a consistant set of priorities (i.e. pre-reqs have a higher priority
        /// than dependant skills
        /// </summary>
        /// <remarks>This methods relies on the assumption that prerequisites order is correct.</remarks>
        /// <param name="fixConflicts">When true, conflicts will be fixed.</param>
        /// <param name="loweringPriorities">When true, conflicts are solved by setting all dependant skills to the priority of their prerequisites.
        /// <para>When false, conflicts are solved by setting the priority of the prerequisites to the same as the highest priority of any dependant skill.</para></param>
        /// <returns>True whether the priorities were ok, false otherwise</returns>
        protected bool FixPrioritiesOrder(bool fixConflicts, bool loweringPriorities)
        {
            bool planOK = true;

            // Check all plan entries
            for (int i = 0; i < Items.Count; i++)
            {
                PlanEntry pe = Items[i];
                int highestDepPriority = GetHighestDependencyPriority(i);

                // Find all dependants on this skill and get the highest priority
                if (pe.Priority <= highestDepPriority)
                    continue;

                if (!fixConflicts)
                    return false;

                if (loweringPriorities)
                    LowerDependenciesPriorities(i);
                else
                    pe.Priority = highestDepPriority;

                planOK = false;
            }
            return planOK;
        }

        /// <summary>
        /// Gets the highest priority of all dependants of a skill (skills who has the given parameter as a prerequisite).
        /// </summary>
        /// <remarks>This methods relies on the assumption that prerequisites order is correct.</remarks>
        /// <param name="posSkillToCheck">Position of the prerequisite skill</param>
        private int GetHighestDependencyPriority(int posSkillToCheck)
        {
            int highestDepPriority = 99;
            PlanEntry pEntry = Items[posSkillToCheck];

            // Scroll through successive skills
            for (int j = posSkillToCheck + 1; j < Items.Count; j++)
            {
                PlanEntry entry = Items[j];

                // Is it either a prerequisite or a previous level ?
                if (entry.IsDependentOf(pEntry))
                    highestDepPriority = Math.Min(entry.Priority, highestDepPriority);
            }

            return highestDepPriority;
        }

        /// <summary>
        /// Lower priorities of all dependant skills to match parent skill
        /// </summary>
        /// <remarks>This methods relies on the assumption that prerequisites order is correct.</remarks>
        /// <param name="posSkill">Position of parent skill</param>
        private void LowerDependenciesPriorities(int posSkill)
        {
            PlanEntry entry = Items[posSkill];

            // Scroll through successive skills
            for (int j = posSkill + 1; j < Items.Count; ++j)
            {
                PlanEntry pEntry = Items[j];

                // Is it either a prerequisite or a previous level ?
                if (pEntry.IsDependentOf(entry))
                    pEntry.Priority = Math.Max(pEntry.Priority, entry.Priority);
            }
        }

        /// <summary>
        /// Gets the minimum level the given skill is required by other entries.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns>The minimum required level, between 0 and 5.</returns>
        public long GetMinimumLevel(IStaticSkill skill)
        {
            // Search the minimum level this skill is required by other entries
            long minNeeded = 0;
            foreach (PlanEntry pe in Items)
            {
                long required;
                StaticSkill tSkill = pe.Skill;

                if (!tSkill.HasAsPrerequisite(skill, out required) || tSkill == skill)
                    continue;

                // All 5 levels are needed, fail now
                if (required == 5)
                    return 5;
                minNeeded = Math.Max(minNeeded, required);
            }
            return minNeeded;
        }

        #endregion


        #region Insertion and removal

        /// <summary>
        /// Gets true whether a skill set is already planned
        /// </summary>
        /// <param name="skillsToAdd"></param>
        /// <returns></returns>
        public bool AreSkillsPlanned(IEnumerable<StaticSkillLevel> skillsToAdd)
            => skillsToAdd.All(item => IsPlanned(item.Skill, item.Level));

        /// <summary>
        /// Rebuild the plan from the given entries enumeration.
        /// </summary>
        /// <remarks>Entries from another plan will be cloned.</remarks>
        /// <param name="entries"></param>
        /// <exception cref="System.ArgumentNullException">entries</exception>
        public void RebuildPlanFrom(IEnumerable<PlanEntry> entries)
        {
            entries.ThrowIfNull(nameof(entries));

            using (SuspendingEvents())
            {
                Items.Clear();
                for (int i = 0; i < m_lookup.Length; i++)
                {
                    m_lookup[i] = null;
                }

                foreach (PlanEntry entry in entries)
                {
                    AddCore(entry.Plan != this ? entry.Clone(this) : entry);
                }
            }
        }

        /// <summary>
        /// Rebuild the plan from the given entries enumeration.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="preserveOldEntries">When true, old entries will be reused as often as possible, preserving their statistics.</param>
        /// <exception cref="System.ArgumentNullException">entries</exception>
        /// <remarks>
        /// Entries from another plan will be cloned.
        /// </remarks>
        public void RebuildPlanFrom(IEnumerable<PlanEntry> entries, bool preserveOldEntries)
        {
            entries.ThrowIfNull(nameof(entries));

            if (!preserveOldEntries)
            {
                RebuildPlanFrom(entries);
                return;
            }

            using (SuspendingEvents())
            {
                // Save the old entries
                SkillLevelSet<PlanEntry> set = new SkillLevelSet<PlanEntry>();
                foreach (PlanEntry entry in Items)
                {
                    set[entry.Skill, entry.Level] = entry;
                }

                // Clear items
                Items.Clear();
                for (int i = 0; i < m_lookup.Length; i++)
                {
                    m_lookup[i] = null;
                }

                // Add the new entries
                foreach (PlanEntry entry in entries)
                {
                    PlanEntry oldEntry = set[entry.Skill, entry.Level];

                    PlanEntry entryToAdd;
                    if (entry.Plan != this)
                        entryToAdd = entry.Clone(this);
                    else if (oldEntry != null)
                        entryToAdd = oldEntry;
                    else
                        entryToAdd = entry;

                    AddCore(entryToAdd);
                }
            }
        }

        /// <summary>
        /// Given a list of skill to remove, we return a list of entries also including all dependencies. No entry is removed by this method.
        /// </summary>
        /// <returns>A list of all the entries to remove.</returns>
        protected IEnumerable<PlanEntry> GetAllEntriesToRemove<T>(IEnumerable<T> skillsToRemove)
            where T : ISkillLevel
        {
            SkillLevelSet<PlanEntry> entriesSet = new SkillLevelSet<PlanEntry>();
            List<PlanEntry> planEntries = new List<PlanEntry>();

            // For every items to add
            foreach (T itemToRemove in skillsToRemove.Where(
                itemToRemove => IsPlanned(itemToRemove.Skill, itemToRemove.Level)).Where(
                    itemToRemove => !entriesSet.Contains(itemToRemove)))
            {
                // Let's first gather dependencies
                foreach (PlanEntry dependencyEntry in Items.Where(
                    dependencyEntry => !entriesSet.Contains(dependencyEntry)).Where(
                        dependencyEntry => dependencyEntry.IsDependentOf(itemToRemove)))
                {
                    // Gather this entry
                    planEntries.Add(dependencyEntry);
                    entriesSet.Set(dependencyEntry);
                }

                // Then add the item itself
                PlanEntry entryToRemove = GetEntry(itemToRemove.Skill, itemToRemove.Level);
                planEntries.Add(entryToRemove);
                entriesSet.Set(entryToRemove);
            }

            return planEntries;
        }

        /// <summary>
        /// Given a list of skill to add, we return a list of all entries to add, also including all dependencies. No entry is added by this method.
        /// </summary>
        /// <param name="skillsToAdd">The enumerations of skills to add.</param>
        /// <param name="note">The note for new entries.</param>
        /// <param name="lowestPrereqPriority">The lowest priority (highest number) among all the prerequisites.</param>
        /// <returns>A list of all the entries to add.</returns>
        protected IEnumerable<PlanEntry> GetAllEntriesToAdd<T>(IEnumerable<T> skillsToAdd, string note,
            out int lowestPrereqPriority)
            where T : ISkillLevel
        {
            SkillLevelSet<PlanEntry> entriesSet = new SkillLevelSet<PlanEntry>();
            List<PlanEntry> planEntries = new List<PlanEntry>();
            lowestPrereqPriority = 1;

            // For every items to add
            foreach (T itemToAdd in skillsToAdd.Where(
                itemToAdd => Character.GetSkillLevel(itemToAdd.Skill) < itemToAdd.Level))
            {
                // Already planned ? We update the lowestPrereqPriority and skip it.
                if (IsPlanned(itemToAdd.Skill, itemToAdd.Level))
                {
                    lowestPrereqPriority = Math.Max(GetEntry(itemToAdd.Skill, itemToAdd.Level).Priority, lowestPrereqPriority);
                    continue;
                }

                // Let's first add dependencies excluding those that the dependent skill is already trained
                StaticSkillLevel item = new StaticSkillLevel(itemToAdd);

                if (Character.GetSkillLevel(itemToAdd.Skill) < 1)
                {
                    foreach (StaticSkillLevel dependency in item.AllDependencies.Where(
                        dependency => !entriesSet.Contains(dependency) && dependency.Skill != item.Skill &&
                                      Character.GetSkillLevel(dependency.Skill) < dependency.Level)
                        .Select(dependency => new
                        {
                            dependency,
                            depItems = item.AllDependencies.Where(
                                dep => item.Skill != dep.Skill &&
                                       dep.Skill.Prerequisites.Any(
                                           prereq => prereq.Skill == dependency.Skill))
                        })
                        .Where(dep => !dep.depItems.Any() ||
                                      !dep.depItems.All(depItem => Character.GetSkillLevel(depItem.Skill) >= depItem.Level))
                        .Select(dep => dep.dependency))
                    {
                        // Create an entry (even for existing ones, we will update them later from those new entries)
                        PlanEntry dependencyEntry = CreateEntryToAdd(dependency.Skill, dependency.Level,
                            PlanEntryType.Prerequisite, note, ref lowestPrereqPriority);
                        planEntries.Add(dependencyEntry);
                        entriesSet.Set(dependencyEntry);
                    }

                    // Already in the "entries to add" list ? We skip it (done at this point only because of recursive prereqs)
                    if (entriesSet.Contains(itemToAdd))
                        continue;
                }

                // Then add the item itself
                PlanEntry entry = CreateEntryToAdd(itemToAdd.Skill, itemToAdd.Level,
                    PlanEntryType.Planned, note, ref lowestPrereqPriority);
                planEntries.Add(entry);
                entriesSet.Set(entry);
            }

            return planEntries;
        }

        /// <summary>
        /// Creates an entry that should be later added
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="type"></param>
        /// <param name="note"></param>
        /// <param name="lowestPrereqPriority"></param>
        /// <returns></returns>
        private PlanEntry CreateEntryToAdd(StaticSkill skill, long level, PlanEntryType type, string note,
            ref int lowestPrereqPriority)
        {
            PlanEntry entry = GetEntry(skill, level);

            // So we have to create a new entry. We first check it's not already learned or something
            if (entry == null)
            {
                return new PlanEntry(null, skill, level)
                {
                    Type = type,
                    Notes = note
                };
            }

            // If the entry is already in the plan, we create an entry that will never be added to the plan.
            // However, the existing entry's notes and priotity will be updated from this new entry

            // Is the priority the lowest so far (higher numbers = lower priority) ?
            int priority = entry.Priority;
            lowestPrereqPriority = Math.Max(priority, lowestPrereqPriority);

            //Skill at this level is planned - just update the Note.
            entry = new PlanEntry(null, skill, level) { Priority = priority, Type = type, Notes = note };
            return entry;
        }

        #endregion


        #region Priorities changes

        /// <summary>
        /// Set the priorities by force, fixing conflicts when required.
        /// </summary>
        /// <param name="displayPlan">The display plan.</param>
        /// <param name="entries">The list of entries to change priority of</param>
        /// <param name="priority">The new priority to set</param>
        /// <exception cref="System.ArgumentNullException">entries</exception>
        public void SetPriority(IEnumerable<PlanEntry> displayPlan, IEnumerable<PlanEntry> entries, int priority)
        {
            entries.ThrowIfNull(nameof(entries));

            // Change priorities and determine how conflicts are going to be fixed
            bool loweringPriorities = true;
            foreach (PlanEntry entry in entries)
            {
                loweringPriorities &= priority > entry.Priority;
                entry.Priority = priority;
            }

            // We are rebuilding the plan with the new priorities
            RebuildPlanFrom(displayPlan, true);

            // Fix things up
            FixPrioritiesOrder(true, loweringPriorities);
            OnChanged(PlanChange.Notification);
        }

        /// <summary>
        /// Removes completed skills
        /// </summary>
        public void CleanObsoleteEntries(ObsoleteRemovalPolicy policy)
        {
            using (SuspendingEvents())
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    PlanEntry pe = Items[i];
                    if (Character.GetSkillLevel(pe.Skill) < pe.Level)
                        continue;

                    // Confirmed by API?
                    if (policy == ObsoleteRemovalPolicy.ConfirmedOnly &&
                        pe.CharacterSkill.LastConfirmedLvl < pe.Level)
                        continue;

                    Items.RemoveAt(i);
                    m_lookup[pe.Skill.ArrayIndex * 5 + pe.Level - 1] = null;

                    i--;
                }
            }
        }

        #endregion


        #region Certificates

        /// <summary>
        /// Checks whether, after this plan, the owner will be eligible to the provided certificate
        /// </summary>
        /// <param name="certLevel">The cert level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">certLevel</exception>
        public bool WillGrantEligibilityFor(CertificateLevel certLevel)
        {
            certLevel.ThrowIfNull(nameof(certLevel));

            if (certLevel.IsTrained)
                return true;

            // We check every prerequisite is trained
            return !certLevel.PrerequisiteSkills.Select(
                skillToTrain => new { skillToTrain, skill = skillToTrain.Skill }).Where(
                    skillToTrain => skillToTrain.skill.Level < skillToTrain.skillToTrain.Level).Where(
                        skillToTrain => !IsPlanned(skillToTrain.skill, skillToTrain.skillToTrain.Level)).Select(
                            skill => skill.skillToTrain).Any();
        }

        #endregion


        #region Masteries

        /// <summary>
        /// Checks whether, after this plan, the owner will be eligible to the provided mastery
        /// </summary>
        /// <param name="masteryLevel">The mastery level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">masteryLevel</exception>
        public bool WillGrantEligibilityFor(Mastery masteryLevel)
        {
            masteryLevel.ThrowIfNull(nameof(masteryLevel));

            if (masteryLevel.IsTrained)
                return true;

            Character character = Character as Character;

            // We check every prerequisite is trained
            return !masteryLevel.Select(mcert => mcert.ToCharacter(character).GetCertificateLevel(masteryLevel.Level))
                .SelectMany(cert => cert.PrerequisiteSkills)
                .Select(skillToTrain => new { skillToTrain, skill = skillToTrain.Skill })
                .Where(skillToTrain => skillToTrain.skill.Level < skillToTrain.skillToTrain.Level)
                .Where(skillToTrain => !IsPlanned(skillToTrain.skill, skillToTrain.skillToTrain.Level))
                .Select(skill => skill.skillToTrain)
                .Any();
        }

        #endregion


        #region Sort

        /// <summary>
        /// Sort this plan
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="reverseOrder"></param>
        /// <param name="groupByPriority"></param>
        private void Sort(PlanEntrySort sort, bool reverseOrder, bool groupByPriority)
        {
            // Perform the sort
            IEnumerable<PlanEntry> entries = new PlanEntrySorter(Character, Items, sort, reverseOrder, groupByPriority).Sort();

            // Update plan
            RebuildPlanFrom(entries);
        }

        /// <summary>
        /// Sorts a plan according to the given settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">settings</exception>
        public void Sort(PlanSorting settings)
        {
            settings.ThrowIfNull(nameof(settings));

            PlanEntrySort criteria = settings.Order == ThreeStateSortOrder.None ? PlanEntrySort.None : settings.Criteria;
            Sort(criteria, settings.Order == ThreeStateSortOrder.Descending, settings.GroupByPriority);
        }

        #endregion


        #region UpdateTrainingTimes

        /// <summary>
        /// Updates the statistics of the entries in the same way this character would train this plan.
        /// </summary>
        public void UpdateStatistics()
        {
            UpdateStatistics(new CharacterScratchpad(Character), true, true);
        }

        /// <summary>
        /// Updates the statistics of the entries in the same way the given character would train this plan.
        /// </summary>
        /// <param name="scratchpad">The scratchpad.</param>
        /// <param name="applyRemappingPoints">if set to <c>true</c> [apply remapping points].</param>
        /// <param name="trainSkills">When true, the character will train every skill, increasing SP, etc.</param>
        /// <exception cref="System.ArgumentNullException">scratchpad</exception>
        public void UpdateStatistics(CharacterScratchpad scratchpad, bool applyRemappingPoints, bool trainSkills)
        {
            scratchpad.ThrowIfNull(nameof(scratchpad));

            CharacterScratchpad scratchpadWithoutImplants = scratchpad.Clone();
            scratchpadWithoutImplants.ClearImplants();
            DateTime time = DateTime.Now;

            // Update the statistics
            foreach (PlanEntry entry in Items)
            {
                // Apply the remapping
                if (applyRemappingPoints && entry.Remapping != null &&
                    entry.Remapping.Status == RemappingPointStatus.UpToDate)
                {
                    scratchpad.Remap(entry.Remapping);
                    scratchpadWithoutImplants.Remap(entry.Remapping);
                }

                // Update entry's statistics
                entry.UpdateStatistics(scratchpad, scratchpadWithoutImplants, ref time);

                // Update the scratchpad
                if (!trainSkills)
                    continue;

                scratchpad.Train(entry.Skill, entry.Level);
                scratchpadWithoutImplants.Train(entry.Skill, entry.Level);
            }
        }

        /// <summary>
        /// Updates the statistics of the entries in the same way this character would train this plan.
        /// </summary>
        public void UpdateOldTrainingTimes()
        {
            UpdateOldTrainingTimes(new CharacterScratchpad(Character.After(ChosenImplantSet)), true, true);
        }

        /// <summary>
        /// Updates the statistics of the entries in the same way the given character would train this plan.
        /// </summary>
        /// <param name="scratchpad">The scratchpad.</param>
        /// <param name="applyRemappingPoints">if set to <c>true</c> [apply remapping points].</param>
        /// <param name="trainSkills">When true, the character will train every skill, increasing SP, etc.</param>
        /// <exception cref="System.ArgumentNullException">scratchpad</exception>
        public void UpdateOldTrainingTimes(CharacterScratchpad scratchpad, bool applyRemappingPoints, bool trainSkills)
        {
            scratchpad.ThrowIfNull(nameof(scratchpad));

            // Update the statistics
            foreach (PlanEntry entry in Items)
            {
                // Apply the remapping
                if (applyRemappingPoints && entry.Remapping != null &&
                    entry.Remapping.Status == RemappingPointStatus.UpToDate)
                    scratchpad.Remap(entry.Remapping);

                // Update entry's statistics
                entry.UpdateOldTrainingTime(scratchpad);

                // Update the scratchpad
                if (trainSkills)
                    scratchpad.Train(entry.Skill, entry.Level);
            }
        }

        #endregion

    }
}