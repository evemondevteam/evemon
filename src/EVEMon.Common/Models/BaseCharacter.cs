using System;
using System.Collections.Generic;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Models
{
    public abstract class BaseCharacter
    {
        #region Abstract methods and properties

        protected abstract Int64 TotalSkillPoints { get; }
        protected abstract ICharacterAttribute GetAttribute(EveAttribute attribute);

        internal abstract void Dispose();

        public abstract Int64 GetSkillLevel(StaticSkill skill);
        public abstract Int64 GetSkillPoints(StaticSkill skill);

        #endregion


        #region Computation methods

        /// <summary>
        /// Gets the total skill points for this character.
        /// </summary>
        public Int64 SkillPoints => TotalSkillPoints;

        /// <summary>
        /// Computes the SP per hour for the given skill, without factoring out the newbies bonus.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        public virtual float GetBaseSPPerHour(StaticSkill skill)
        {
            return GetOmegaSPPerHour(skill);
        }

        /// <summary>
        /// Computes the SP per hour for the given skill, if the account status is Omega (subscribed)
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        public float GetOmegaSPPerHour(StaticSkill skill)
        {
            skill.ThrowIfNull(nameof(skill));

            if (skill.PrimaryAttribute == EveAttribute.None || skill.SecondaryAttribute == EveAttribute.None)
                return 0.0f;

            float primAttr = GetAttribute(skill.PrimaryAttribute).EffectiveValue;
            float secondaryAttr = GetAttribute(skill.SecondaryAttribute).EffectiveValue;
            return primAttr * 60.0f + secondaryAttr * 30.0f;
        }

        /// <summary>
        /// Gets a character scratchpad representing this character after the provided skill levels trainings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trainings"></param>
        /// <returns></returns>
        public CharacterScratchpad After<T>(IEnumerable<T> trainings)
            where T : ISkillLevel
        {
            CharacterScratchpad scratchpad = new CharacterScratchpad(this);
            scratchpad.Train(trainings);
            return scratchpad;
        }

        /// <summary>
        /// Gets a character scratchpad representing this character after a switch to the provided implant set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">set</exception>
        public CharacterScratchpad After(ImplantSet set)
        {
            set.ThrowIfNull(nameof(set));

            CharacterScratchpad scratchpad = new CharacterScratchpad(this);
            for (int i = 0; i < 5; i++)
            {
                EveAttribute attribute = (EveAttribute)i;
                scratchpad[attribute].ImplantBonus = set[attribute].Bonus;
            }
            return scratchpad;
        }

        /// <summary>
        /// Gets the time span for a specific number of skill points.
        /// </summary>
        /// <param name="points">The points to calculate points.</param>
        /// <param name="skill">The skill to train.</param>
        /// <returns></returns>
        public TimeSpan GetTimeSpanForPoints(StaticSkill skill, Int64 points) 
            => GetTrainingTime(points, GetBaseSPPerHour(skill));

        /// <summary>
        /// Gets the required skill injectors for the specified skill points.
        /// </summary>
        /// <param name="skillPoints">The skill points.</param>
        /// <returns></returns>
        public int GetRequiredSkillInjectorsForSkillPoints(long skillPoints)
        {
            long remainingSkillPoints = skillPoints;
            int injectorsCount = 0;

            while (remainingSkillPoints > 0)
            {
                long projectedSkillPoints = SkillPoints + skillPoints - remainingSkillPoints;
                remainingSkillPoints -= GetSkillPointsGainedFromInjector(projectedSkillPoints);
                injectorsCount++;
            }

            return injectorsCount;
        }

        /// <summary>
        /// Gets the skill points gained from injector.
        /// </summary>
        /// <param name="skillPoints">The start skill points.</param>
        /// <returns></returns>
        private static int GetSkillPointsGainedFromInjector(long skillPoints)
        {
            double sp = skillPoints / 1000000d;
            const int Multiplier = 100000;

            if (sp < 5)
                return 5 * Multiplier;
            if (sp < 50)
                return 4 * Multiplier;
            if (sp < 80)
                return 3 * Multiplier;
            return (int)(1.5 * Multiplier);
        }

        #endregion


        #region GetSPToTrain

        /// <summary>
        /// Computes the number of SP to train.
        /// </summary>
        /// <param name="skillLevel">The skill level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skillLevel</exception>
        public Int64 GetSPToTrain(ISkillLevel skillLevel)
        {
            skillLevel.ThrowIfNull(nameof(skillLevel));

            return GetSPToTrain(skillLevel.Skill, skillLevel.Level);
        }

        /// <summary>
        /// Computes the number of SP to train.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        private Int64 GetSPToTrain(StaticSkill skill, Int64 level, TrainingOrigin origin = TrainingOrigin.FromCurrent)
        {
            if (level == 0)
                return 0;
            Int64 sp = skill.GetPointsRequiredForLevel(level);

            // Deals with the origin
            Int64 result;
            switch (origin)
            {
                // Include current SP
                case TrainingOrigin.FromCurrent:
                    result = sp - GetSkillPoints(skill);
                    break;

                // This level only (previous are known)
                case TrainingOrigin.FromPreviousLevel:
                    result = sp - skill.GetPointsRequiredForLevel(level - 1);
                    break;

                case TrainingOrigin.FromPreviousLevelOrCurrent:
                    result = sp - Math.Max(GetSkillPoints(skill), skill.GetPointsRequiredForLevel(level - 1));
                    break;

                // Include nothing
                default:
                    result = sp;
                    break;
            }

            // Returns result
            return result < 0 ? 0 : result;
        }

        #endregion


        #region GetTrainingTime & GetTrainingTimeToMultipleSkills

        /// <summary>
        /// Computes the training time for the given skill.
        /// </summary>
        /// <param name="skillLevel">The skill level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skillLevel</exception>
        public TimeSpan GetTrainingTime(ISkillLevel skillLevel)
        {
            skillLevel.ThrowIfNull(nameof(skillLevel));

            return GetTrainingTime(skillLevel.Skill, skillLevel.Level);
        }

        /// <summary>
        /// Computes the training time for the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public TimeSpan GetTrainingTime(StaticSkill skill, Int64 level, TrainingOrigin origin = TrainingOrigin.FromCurrent)
        {
            float spPerHour = GetBaseSPPerHour(skill);
            Int64 sp = GetSPToTrain(skill, level, origin);
            return GetTrainingTime(sp, spPerHour);
        }

        /// <summary>
        /// Gets the time to train the given SP at the provided speed.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="spPerHour"></param>
        /// <returns></returns>
        private static TimeSpan GetTrainingTime(Int64 sp, float spPerHour)
            => Math.Abs(spPerHour) < float.Epsilon ? TimeSpan.FromDays(999.0) : TimeSpan.FromHours(sp / spPerHour);

        /// <summary>
        /// Gets the time require to train the given skills and their prerequisites.
        /// </summary>
        /// <param name="trainings">A sequence of pairs of skills and the target levels.</param>
        /// <returns></returns>
        public TimeSpan GetTrainingTimeToMultipleSkills(IEnumerable<ISkillLevel> trainings)
            => After(trainings).TrainingTime;

        #endregion


        #region ICharacter non-abstract explicit members

        /// <summary>
        /// Gets the <see cref="ICharacterAttribute"/> with the specified attribute.
        /// </summary>
        /// <value></value>
        public ICharacterAttribute this[EveAttribute attribute] => GetAttribute(attribute);

        /// <summary>
        /// Gets the intelligence.
        /// </summary>
        /// <value>The intelligence.</value>
        public ICharacterAttribute Intelligence => GetAttribute(EveAttribute.Intelligence);

        /// <summary>
        /// Gets the perception.
        /// </summary>
        /// <value>The perception.</value>
        public ICharacterAttribute Perception => GetAttribute(EveAttribute.Perception);

        /// <summary>
        /// Gets the willpower.
        /// </summary>
        /// <value>The willpower.</value>
        public ICharacterAttribute Willpower => GetAttribute(EveAttribute.Willpower);

        /// <summary>
        /// Gets the charisma.
        /// </summary>
        /// <value>The charisma.</value>
        public ICharacterAttribute Charisma => GetAttribute(EveAttribute.Charisma);

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <value>The memory.</value>
        public ICharacterAttribute Memory => GetAttribute(EveAttribute.Memory);

        #endregion
    }
}