using System;
using EVEMon.Common.Data;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Extensions
{
    public static class StaticSkillExtensions
    {
        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill.
        /// The check is performed recursively through all prerequisites.
        /// </summary>
        /// <param name="thisSkill">This skill.</param>
        /// <param name="skill">Skill to check.</param>
        /// <returns>
        /// 	<code>true</code> if it is a prerequisite.
        /// </returns>
        public static bool HasAsPrerequisite(this IStaticSkill thisSkill, IStaticSkill skill)
        {
            long neededLevel = 0;
            return thisSkill.HasAsPrerequisite(skill, ref neededLevel, true);
        }

        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill, and what level it needs.
        /// The check is performed recursively through all prerequisites.
        /// </summary>
        /// <param name="thisSkill">This skill.</param>
        /// <param name="skill">Skill to check.</param>
        /// <param name="neededLevel">The level that is needed. Out parameter.</param>
        /// <returns>
        /// 	<code>true</code> if it is a prerequisite, needed level in <var>neededLevel</var> out parameter.
        /// </returns>
        public static bool HasAsPrerequisite(this IStaticSkill thisSkill, IStaticSkill skill, out long neededLevel)
        {
            neededLevel = 0;
            return thisSkill.HasAsPrerequisite(skill, ref neededLevel, true);
        }

        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill, and what level it needs.
        /// Find the highest level needed by searching entire prerequisite tree.
        /// </summary>
        /// <param name="thisSkill">This skill.</param>
        /// <param name="skill">Skill to check.</param>
        /// <param name="neededLevel">The level that is needed. Out parameter.</param>
        /// <param name="recurse">Pass <code>true</code> to check recursively.</param>
        /// <returns>
        /// 	<code>true</code> if it is a prerequisite, needed level in <var>neededLevel</var> out parameter.
        /// </returns>
        private static bool HasAsPrerequisite(this IStaticSkill thisSkill, IStaticSkill skill, ref long neededLevel, bool recurse)
        {
            long thisID = thisSkill.ID;

            foreach (StaticSkillLevel prereq in thisSkill.Prerequisites)
            {
                if (prereq.Skill == skill)
                    neededLevel = Math.Max(prereq.Level, neededLevel);

                if (recurse && neededLevel < 5 && prereq.Skill.ID != thisID)
                    // check for neededLevel fixes recursive skill bug (e.g polaris )
                    prereq.Skill.HasAsPrerequisite(skill, ref neededLevel, true);
            }
            return neededLevel > 0;
        }

        /// <summary>
        /// Checks whether a certain skill is an immediate prerequisite of this skill,
        /// and the level needed
        /// </summary>
        /// <param name="thisSkill">This skill.</param>
        /// <param name="skill">Skill that may be an immediate prereq</param>
        /// <param name="neededLevel">needed level of skill</param>
        /// <returns>
        /// Skill gs is an immediate prereq of this skill
        /// </returns>
        public static bool HasAsImmediatePrereq(this IStaticSkill thisSkill, IStaticSkill skill, out long neededLevel)
        {
            neededLevel = 0;
            return thisSkill.HasAsPrerequisite(skill, ref neededLevel, false);
        }
    }
}