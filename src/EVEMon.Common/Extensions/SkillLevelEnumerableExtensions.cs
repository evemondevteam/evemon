using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Models;

namespace EVEMon.Common.Extensions
{
    public static class SkillLevelEnumerableExtensions
    {
        /// <summary>
        /// Returns an enumeration of the static equivalent of the items.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IEnumerable<StaticSkillLevel> ToStatic(this IEnumerable<SkillLevel> src)
            => src.Select(item => new StaticSkillLevel(item));

        /// <summary>
        /// Gets all the dependencies, in a way matching the hierachical order and without redudancies.
        /// I.e, for eidetic memory II, it will return <c>{ instant recall I, instant recall II, instant recall III, instant recall IV,  eidetic memory I, eidetic memory II }</c>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="includeRoots">When true, the levels in this enumeration are also included.</param>
        public static IEnumerable<SkillLevel> GetAllDependencies(this IEnumerable<SkillLevel> src, bool includeRoots)
        {
            var skillLevels = src as IList<SkillLevel> ?? src.ToList();
            SkillLevel first = skillLevels.FirstOrDefault();
            return first == null || first.Skill == null
                ? Enumerable.Empty<SkillLevel>()
                : skillLevels.ToStatic().GetAllDependencies(includeRoots).ToCharacter(first.Skill.Character);
        }

        /// <summary>
        /// Gets true if all the levels are known
        /// </summary>
        /// <param name="src"></param>
        public static bool AreTrained(this IEnumerable<SkillLevel> src) => src.All(x => x.IsTrained);

        /// <summary>
        /// Checks whether those prerequisites contains the provided skill, returning the need level
        /// </summary>
        /// <param name="src"></param>
        /// <param name="skill"></param>
        /// <param name="neededLevel"></param>
        /// <returns></returns>
        public static bool Contains(this IEnumerable<SkillLevel> src, Skill skill, out long neededLevel)
        {
            neededLevel = 0;
            foreach (SkillLevel prereq in src.Where(prereq => prereq.Skill == skill))
            {
                neededLevel = prereq.Level;
                return true;
            }
            return false;
        }


        #region GetTotalTrainingTime

        /// <summary>
        /// Gets the points required to train all the prerequisites
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns></returns>
        public static TimeSpan GetTotalTrainingTime(this IEnumerable<SkillLevel> src)
        {
            bool junk = false;
            return src.GetTotalTrainingTime(new Dictionary<Skill, long>(), ref junk);
        }

        /// <summary>
        /// Gets the points required to train all the prerequisites
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="alreadyCountedList">The already counted list.</param>
        /// <returns></returns>
        public static TimeSpan GetTotalTrainingTime(this IEnumerable<SkillLevel> src, Dictionary<Skill, long> alreadyCountedList)
        {
            bool junk = false;
            return src.GetTotalTrainingTime(alreadyCountedList, ref junk);
        }

        /// <summary>
        /// Gets the points required to train all the prerequisites
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="isCurrentlyTraining">if set to <c>true</c> [is currently training].</param>
        /// <returns></returns>
        public static TimeSpan GetTotalTrainingTime(this IEnumerable<SkillLevel> src, ref bool isCurrentlyTraining)
            => src.GetTotalTrainingTime(new Dictionary<Skill, long>(), ref isCurrentlyTraining);

        /// <summary>
        /// Gets the time required to train all the prerequisites
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="alreadyCountedList">The already counted list.</param>
        /// <param name="isCurrentlyTraining">if set to <c>true</c> [is currently training].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">src or alreadyCountedList</exception>
        public static TimeSpan GetTotalTrainingTime(this IEnumerable<SkillLevel> src, Dictionary<Skill, long> alreadyCountedList,
            ref bool isCurrentlyTraining)
        {
            src.ThrowIfNull(nameof(src));

            alreadyCountedList.ThrowIfNull(nameof(alreadyCountedList));

            TimeSpan result = TimeSpan.Zero;
            foreach (SkillLevel item in src)
            {
                Skill skill = item.Skill;
                isCurrentlyTraining |= skill.IsTraining;

                // Gets the number of points we're starting from
                long fromPoints = skill.SkillPoints;
                if (alreadyCountedList.ContainsKey(skill))
                    fromPoints = alreadyCountedList[skill];

                // Gets the number of points we're targeting
                long toPoints = skill.GetLeftPointsRequiredToLevel(item.Level);
                if (fromPoints < toPoints)
                    result += skill.GetTimeSpanForPoints(toPoints - fromPoints);

                // Updates the alreadyCountedList
                alreadyCountedList[skill] = Math.Max(fromPoints, toPoints);

                // Recursive
                if (fromPoints < toPoints)
                    result += skill.Prerequisites.GetTotalTrainingTime(alreadyCountedList, ref isCurrentlyTraining);
            }
            return result;
        }

        #endregion
    }
}