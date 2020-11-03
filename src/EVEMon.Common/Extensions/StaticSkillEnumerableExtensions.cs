using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Models;

namespace EVEMon.Common.Extensions
{
    public static class StaticSkillEnumerableExtensions
    {
        /// <summary>
        /// Returns an equivalent enumeration with character skills.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static IEnumerable<Skill> ToCharacter(this IEnumerable<StaticSkill> src, Character character)
            => src.Select(skill => character.Skills[skill.ID]);

        /// <summary>
        /// Gets all the prerequisites. I.e, for eidetic memory, it will return <c>{ instant recall IV }</c>. 
        /// The order matches the hirerarchy but skills are not duplicated and are systematically trained to the highest required level.
        /// For example, if some skill is required to lv3 and, later, to lv4, this first time it is encountered, lv4 is returned.
        /// </summary>
        /// <remarks>Please note they may be redundancies.</remarks>
        public static IEnumerable<StaticSkillLevel> GetAllPrerequisites(this IEnumerable<StaticSkill> src)
        {
            long[] highestLevels = new long[StaticSkills.ArrayIndicesCount];
            List<StaticSkillLevel> list = new List<StaticSkillLevel>();

            // Fill the array
            foreach (StaticSkillLevel prereq in src.SelectMany(skill => skill.Prerequisites))
            {
                FillPrerequisites(highestLevels, list, prereq, true);
            }

            // Return the result
            foreach (StaticSkillLevel newSkill in list.Where(newSkill => highestLevels[newSkill.Skill.ArrayIndex] != 0))
            {
                yield return new StaticSkillLevel(newSkill.Skill, highestLevels[newSkill.Skill.ArrayIndex]);
                highestLevels[newSkill.Skill.ArrayIndex] = 0;
            }
        }

        /// <summary>
        /// Fills the given levels array with the prerequisites and when <c>includeRoots</c> is true, the item level itself.
        /// </summary>
        /// <param name="highestLevels"></param>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="includeRoots"></param>
        internal static void FillPrerequisites(long[] highestLevels, List<StaticSkillLevel> list, StaticSkillLevel item,
            bool includeRoots)
        {
            // Prerequisites
            if (highestLevels[item.Skill.ArrayIndex] == 0)
            {
                foreach (StaticSkillLevel prereq in item.Skill.Prerequisites.Where(prereq => prereq.Skill != item.Skill))
                {
                    FillPrerequisites(highestLevels, list, prereq, true);
                }
            }

            // The very level
            if (!includeRoots || highestLevels[item.Skill.ArrayIndex] >= item.Level)
                return;

            highestLevels[item.Skill.ArrayIndex] = item.Level;
            list.Add(item);
        }
    }
}