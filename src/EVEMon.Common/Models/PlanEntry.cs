using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a plan's entry.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class PlanEntry : ISkillLevel
    {
        public const int DefaultPriority = 3;

        private readonly Collection<string> m_planGroups = new Collection<string>();
        private readonly StaticSkill m_skill;
        private readonly BasePlan m_owner;
        private readonly long m_level;

        private RemappingPoint m_remapping;
        private PlanEntryType m_entryType;
        private int m_priority;
        private string m_notes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        public PlanEntry(BasePlan owner, StaticSkill skill, long level)
        {
            m_owner = owner;
            m_skill = skill;
            m_level = level;

            m_priority = DefaultPriority;
            m_notes = string.Empty;

            OldTrainingTime = TimeSpan.Zero;
            TrainingTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        public PlanEntry(StaticSkill skill, long level)
            : this(null, skill, level)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="serial">The serial.</param>
        internal PlanEntry(BasePlan owner, SerializablePlanEntry serial)
        {
            m_owner = owner;
            m_entryType = serial.Type;
            m_skill = GetSkill(serial);
            m_level = serial.Level;
            m_notes = serial.Notes;
            m_priority = serial.Priority;

            m_planGroups.AddRange(serial.PlanGroups);

            if (serial.Remapping != null)
                m_remapping = new RemappingPoint(serial.Remapping);
        }

        /// <summary>
        /// Gets the character this entry is bound to.
        /// </summary>
        public BaseCharacter Character => m_owner.Character;

        /// <summary>
        /// Gets the owner.
        /// </summary>
        public BasePlan Plan => m_owner;

        /// <summary>
        /// Gets the skill of this entry.
        /// </summary>
        public StaticSkill Skill => m_skill;

        /// <summary>
        /// Gets the character's skill of this entry.
        /// </summary>
        public Skill CharacterSkill
        {
            get
            {
                Character character = m_owner.Character as Character;
                return character == null ? null : m_skill.ToCharacter(character);
            }
        }

        /// <summary>
        /// Gets the skill level of this plan entry.
        /// </summary>
        public long Level => m_level;

        /// <summary>
        /// Gets the entry's priority.
        /// </summary>
        public int Priority
        {
            get { return m_priority; }
            internal set { m_priority = value; }
        }

        /// <summary>
        /// Gets the entry type.
        /// </summary>
        public PlanEntryType Type
        {
            get { return m_entryType; }
            internal set { m_entryType = value; }
        }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        public string Notes
        {
            get { return m_notes; }
            set
            {
                m_notes = value;
                m_owner?.OnChanged(PlanChange.Notification);
            }
        }

        /// <summary>
        /// Gets or sets the remapping point to apply before that skill is trained.
        /// </summary>
        public RemappingPoint Remapping
        {
            get { return m_remapping; }
            set
            {
                m_remapping = value;
                m_owner?.OnChanged(PlanChange.Notification);
            }
        }

        /// <summary>
        /// Gets the names of the plans this entry was taken from when those plans were merged.
        /// </summary>
        public Collection<string> PlanGroups => m_planGroups;

        /// <summary>
        /// Gets a description of the plans groups ("none", "multiple (2)" or the plan's name).
        /// </summary>
        public string PlanGroupsDescription
        {
            get
            {
                if (m_planGroups == null || m_planGroups.Count == 0)
                    return "None";

                return m_planGroups.Count == 1
                    ? m_planGroups[0]
                    : $"Multiple ({m_planGroups.Count})";
            }
        }

        /// <summary>
        /// Gets true if the character already know all the prerequisites.
        /// </summary>
        public bool CanTrainNow
        {
            get
            {
                BaseCharacter character = Character;

                // Checks all the prerequisites are trained
                bool prereqMet = m_skill.Prerequisites.All(x => character.GetSkillLevel(x.Skill) >= x.Level);

                // Checks the skill has the previous level
                return prereqMet && m_level != 0 && character.GetSkillLevel(m_skill) >= m_level - 1;
            }
        }

        /// <summary>
        /// Gets a skill by its ID or its name.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <returns></returns>
        private static StaticSkill GetSkill(SerializablePlanEntry serial)
        {
            // Try get skill by its ID
            StaticSkill skill = StaticSkills.GetSkillByID(serial.ID) ?? StaticSkills.GetSkillByName(serial.SkillName);

            // We failed? Try get skill by its name

            return skill;
        }

        /// <summary>
        /// Gets true if this skill level is, in any way, dependent of the provided skill level.
        /// Checks prerequisites but also same skill's lower levels.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True if the given item's skill is a prerequisite of this one or if it is a lower level of the same skill.</returns>
        public bool IsDependentOf(ISkillLevel level) => ((StaticSkillLevel)this).IsDependentOf(level);

        public bool OmegaRequired
        {
            get
            {
                return m_level > m_skill.AlphaLimit;
            }
        }

        #region Computations done when UpdateTrainingTime is called

        /// <summary>
        /// Gets the training time computed the last time the <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public TimeSpan TrainingTime { get; private set; }

        /// <summary>
        /// Gets the backup of the training time made just before <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public TimeSpan OldTrainingTime { get; private set; }

        /// <summary>
        /// Gets the training time without implants, as computed the last time <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public TimeSpan NaturalTrainingTime { get; private set; }

        /// <summary>
        /// Gets the skill points total at the end of the training, as computed the last time <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public long EstimatedTotalSkillPoints { get; private set; }

        /// <summary>
        /// Gets the SP/Hour, as computed the last time <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public int SpPerHour { get; private set; }

        /// <summary>
        /// Gets the training start time, as computed the last time <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the training end time, as computed the last time <see cref="PlanEntry.UpdateStatistics"/> was called.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Represents the progress towards completion.
        /// </summary>
        public float FractionCompleted => m_level == CharacterSkill.Level + 1
            ? CharacterSkill.FractionCompleted
            : m_level == CharacterSkill.Level
                ? 1f
                : 0f;

        /// <summary>
        /// How many skill points are required to train this skill.
        /// </summary>
        public long SkillPointsRequired { get; private set; }

        /// <summary>
        /// Updates the column statistics (with the exception of the <see cref="UpdateOldTrainingTime"/>) from the given scratchpad.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="characterWithoutImplants"></param>
        /// <param name="time"></param>
        internal void UpdateStatistics(BaseCharacter character, BaseCharacter characterWithoutImplants, ref DateTime time)
        {
            SkillPointsRequired = m_skill.GetPointsRequiredForLevel(m_level) - character.GetSkillPoints(m_skill);
            EstimatedTotalSkillPoints = character.SkillPoints + SkillPointsRequired;
            TrainingTime = character.GetTrainingTime(m_skill, m_level);
            NaturalTrainingTime = characterWithoutImplants.GetTrainingTime(m_skill, m_level);
            SpPerHour = (int)Math.Round(character.GetBaseSPPerHour(m_skill));
            EndTime = time + TrainingTime;
            StartTime = time;
            time = EndTime;
        }

        /// <summary>
        /// Updates the <see cref="OldTrainingTime"/> statistic.
        /// </summary>
        /// <param name="character"></param>
        internal void UpdateOldTrainingTime(BaseCharacter character)
        {
            OldTrainingTime = character.GetTrainingTime(m_skill, m_level);
        }

        #endregion


        /// <summary>
        /// Gets a hash code from the level and skill ID.
        /// </summary>
        /// <returns></returns>
        // After the switch to 64-bit integers this line was throwing a
        // warning. GetHashCode can't possibly be unique for every object
        // there is, additionally GetHashCode() should not be used for
        // equality only grouping; or at least Google says so...
        public override int GetHashCode() => m_skill.ID << 3 | Convert.ToInt32(m_level);

        /// <summary>
        /// Returns a string representation of entry - eases debugging.
        /// </summary>
        /// <returns>Hull Upgrades IV</returns>
        public override string ToString()
            => $"{m_skill.Name} {Models.Skill.GetRomanFromInt(m_level)}";

        /// <summary>
        /// Creates a clone of this entry for the given plan.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        internal PlanEntry Clone(BasePlan plan)
        {
            // We need a skill for the plan's character
            PlanEntry clone = new PlanEntry(plan, m_skill, m_level)
            {
                m_entryType = m_entryType,
                m_priority = m_priority,
                m_notes = m_notes,
                m_remapping = m_remapping?.Clone(),
                OldTrainingTime = OldTrainingTime,
                TrainingTime = TrainingTime
            };
            clone.m_planGroups.AddRange(m_planGroups);

            return clone;
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static implicit operator StaticSkillLevel(PlanEntry entry)
            => entry == null ? null : new StaticSkillLevel(entry.Skill, entry.Level);
    }
}
