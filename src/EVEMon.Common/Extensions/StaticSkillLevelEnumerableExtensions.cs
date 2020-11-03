using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Models;
using EVEMon.Common.Models.Collections;

namespace EVEMon.Common.Extensions
{
    /// <summary>
    /// Extension methods for objects of <see cref="StaticSkillLevel">StaticSkillLevel</see> objects.
    /// </summary>
    public static class StaticSkillLevelEnumerableExtensions
    {
        #region Methods on Enumerations of StaticSkillLevel

        /// <summary>
        /// Returns an equivalent enumeration with character skills.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static IEnumerable<SkillLevel> ToCharacter(this IEnumerable<StaticSkillLevel> src, Character character)
            => src.Where(item => item.Skill != null).Select(item => new SkillLevel(character?.
            Skills[item.Skill.ID] ?? SkillCollection.Skills[item.Skill.ID], item.Level));

        /// <summary>
        /// Gets all the dependencies, in a way matching the hierachical order and without redudancies.
        /// I.e, for eidetic memory II, it will return <c>{ instant recall I, instant recall II, instant recall III, instant recall IV,  eidetic memory I, eidetic memory II }</c>.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="includeRoots">When true, the levels in this enumeration are also included.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public static IEnumerable<StaticSkillLevel> GetAllDependencies(this IEnumerable<StaticSkillLevel> src, bool includeRoots)
        {
            src.ThrowIfNull(nameof(src));

            SkillLevelSet<StaticSkillLevel> set = new SkillLevelSet<StaticSkillLevel>();
            List<StaticSkillLevel> list = new List<StaticSkillLevel>();

            // Fill the set and list
            foreach (StaticSkillLevel item in src)
            {
                list.FillDependencies(set, item, includeRoots);
            }

            // Return the results
            return list;
        }

        /// <summary>
        /// Add the item, its previous levels and its prerequisites to the given set and list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="set">The set.</param>
        /// <param name="item">The item.</param>
        /// <param name="includeRoots">if set to <c>true</c> [include roots].</param>
        internal static void FillDependencies(this IList<StaticSkillLevel> list, SkillLevelSet<StaticSkillLevel> set,
            StaticSkillLevel item, bool includeRoots)
        {
            StaticSkill skill = item.Skill;

            // Add first level and prerequisites
            if (!set.Contains(skill, 1))
            {
                // Prerequisites
                foreach (StaticSkillLevel prereq in skill.Prerequisites.Where(prereq => skill != prereq.Skill))
                {
                    list.FillDependencies(set, prereq, true);
                }

                // Include the first level
                StaticSkillLevel newItem = new StaticSkillLevel(skill, 1);
                list.Add(newItem);
                set.Set(newItem);
            }

            // Add greater levels
            long max = includeRoots ? item.Level : item.Level - 1;
            for (int i = 2; i <= max; i++)
            {
                if (set.Contains(skill, i))
                    continue;

                StaticSkillLevel newItem = new StaticSkillLevel(skill, i);
                list.Add(newItem);
                set.Set(newItem);
            }
        }

        #endregion
    }
}
