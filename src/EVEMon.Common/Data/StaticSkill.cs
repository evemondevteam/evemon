using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a skill definition. Characters use their own representation, through <see cref="Skill"/>.
    /// </summary>
    public sealed class StaticSkill : IStaticSkill
    {
        private static StaticSkill s_unknownStaticSkill;


        #region Constructors

        /// <summary>
        /// Constructor for an unknown static skill.
        /// </summary>
        private StaticSkill()
        {
            ID = int.MaxValue;
            Name = $"{EveMonConstants.UnknownText} Skill";
            Description = "An unknown skill.";
            ArrayIndex = short.MaxValue;
            Prerequisites = new Collection<StaticSkillLevel>();
            PrimaryAttribute = EveAttribute.None;
            SecondaryAttribute = EveAttribute.None;
            Group = StaticSkillGroup.UnknownStaticSkillGroup;
            FormattedCost = Cost.ToNumericString(0);
            AlphaLimit = 0;
       }

        /// <summary>
        /// Deserialization constructor from datafiles.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="src"></param>
        /// <param name="arrayIndex"></param>
        internal StaticSkill(StaticSkillGroup group, SerializableSkill src, int arrayIndex)
        {
            ID = src.ID;
            Cost = src.Cost;
            Rank = src.Rank;
            IsPublic = src.Public;
            Name = src.Name;
            Description = src.Description;
            PrimaryAttribute = src.PrimaryAttribute;
            SecondaryAttribute = src.SecondaryAttribute;
            ArrayIndex = arrayIndex;
            Group = group;
            Prerequisites = new Collection<StaticSkillLevel>();
            FormattedCost = Cost.ToNumericString(0);
            AlphaLimit = src.AlphaLimit;
        }

        #endregion


        #region Initialization

        /// <summary>
        /// Completes the initialization by updating the prequisites and checking trainability on trial account.
        /// </summary>
        internal void CompleteInitialization(IEnumerable<SerializableSkillPrerequisite> prereqs)
        {
            if (prereqs == null)
                return;

            // Create the prerequisites list
            Prerequisites.AddRange(prereqs.Select(x => new StaticSkillLevel(x.GetSkill(), x.Level)));
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ID of this skill.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets a zero-based index for skills (allow the use of arrays to optimize computations).
        /// </summary>
        public int ArrayIndex { get; }

        /// <summary>
        /// Gets the name of this skill (interned).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of this skill.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the rank of this skill.
        /// </summary>
        public long Rank { get; }

        /// <summary>
        /// Gets the skill's cost.
        /// </summary>
        public long Cost { get; }

        /// <summary>
        /// Gets the skill group this skill is part of.
        /// </summary>
        public StaticSkillGroup Group { get; }

        /// <summary>
        /// Gets false when the skill is not for sale by any NPC (CCP never published it or removed it from the game, it's inactive).
        /// </summary>
        public bool IsPublic { get; }

        /// <summary>
        /// Gets the primary attribute of this skill.
        /// </summary>
        public EveAttribute PrimaryAttribute { get; }

        /// <summary>
        /// Gets the secondary attribute of this skill.
        /// </summary>
        public EveAttribute SecondaryAttribute { get; }

        /// <summary>
        /// Get whether skill is trainable on a alpha account.
        /// </summary>
        public bool AlphaFriendly { get { return AlphaLimit > 0; } }

        /// <summary>
        /// Get the level limit for an alpha clone.
        /// </summary>
        public long AlphaLimit { get; private set; }

        /// <summary>
        /// Gets the prerequisites a character must satisfy before it can be trained.
        /// </summary>
        public Collection<StaticSkillLevel> Prerequisites { get; }

        /// <summary>
        /// Gets a formatted representation of the price.
        /// </summary>
        public string FormattedCost { get; }

        /// <summary>
        /// Gets all the prerequisites. I.e, for eidetic memory, it will return <c>{ instant recall IV }</c>.
        /// The order matches the hierarchy but skills are not duplicated and are systematically trained to the highest required level.
        /// For example, if some skill is required to lv3 and, later, to lv4, this first time it is encountered, lv4 is returned.
        /// </summary>
        /// <value>All prerequisites.</value>
        /// <remarks>Please note they may be redundancies.</remarks>
        public IEnumerable<StaticSkillLevel> AllPrerequisites
        {
            get
            {
                long[] highestLevels = new long[StaticSkills.ArrayIndicesCount];
                List<StaticSkillLevel> list = new List<StaticSkillLevel>();

                // Fill the array
                foreach (StaticSkillLevel prereq in Prerequisites)
                {
                    StaticSkillEnumerableExtensions.FillPrerequisites(highestLevels, list, prereq, true);
                }

                // Return the result
                foreach (StaticSkillLevel newItem in list.Where(x => highestLevels[x.Skill.ArrayIndex] != 0))
                {
                    yield return new StaticSkillLevel(newItem.Skill, highestLevels[newItem.Skill.ArrayIndex]);
                    highestLevels[newItem.Skill.ArrayIndex] = 0;
                }
            }
        }

        /// <summary>
        /// Gets the unknown static skill.
        /// </summary>
        /// <value>
        /// The unknown static skill.
        /// </value>
        public static StaticSkill UnknownStaticSkill => s_unknownStaticSkill ?? (s_unknownStaticSkill = new StaticSkill());

        #endregion


        #region Public Methods - Computations

        /// <summary>
        /// Calculates the cumulative points required for a level of this skill (starting from a zero level).
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public long GetPointsRequiredForLevel(long level)
        {
            // Much faster than the old formula. This one too may have 1pt difference here and there, only on the lv2 skills
            switch (level)
            {
                case -1:
                case 0:
                    return 0;
                case 1:
                    return 250 * Rank;
                case 2:
                    switch (Rank)
                    {
                        case 1:
                            return 1415;
                        default:
                            return (int)(Rank * 1414.3f + 0.5f);
                    }
                case 3:
                    return 8000 * Rank;
                case 4:
                    return Convert.ToInt32(Math.Ceiling(Math.Pow(2, 2.5 * level - 2.5) * 250 * Rank));
                case 5:
                    return 256000 * Rank;
                default:
                    throw new NotImplementedException($"One of our devs messed up. Skill level was {level} ?!");
            }
        }

        /// <summary>
        /// Calculates the cumulative points required for a level of this skill (starting from a zero level).
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        public long GetPointsRequiredForLevelOnly(int level)
        {
            if (level == 0)
                return 0;

            return GetPointsRequiredForLevel(level) - GetPointsRequiredForLevelOnly(level - 1);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Gets this skill's representation for the provided character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public Skill ToCharacter(Character character)
        {
            character.ThrowIfNull(nameof(character));

            return character.Skills.GetByArrayIndex(ArrayIndex);
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation for this skill (the name of the skill).
        /// </summary>
        /// <returns>Name of the Static Skill.</returns>
        public override string ToString() => Name;

        #endregion
    }
}
