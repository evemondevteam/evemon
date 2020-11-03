using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections.Global;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents the list of all static skills.
    /// </summary>
    public static class StaticSkills
    {
        private static int s_arrayIndicesCount;
        private static StaticSkill[] s_skills;
        private static readonly Dictionary<long, StaticSkill> s_skillsByID = new Dictionary<long, StaticSkill>();
        private static readonly Dictionary<string, StaticSkill> s_skillsByName = new Dictionary<string, StaticSkill>();
        private static readonly Dictionary<int, StaticSkillGroup> s_skillGroupsByID = new Dictionary<int, StaticSkillGroup>();


        #region Initialization

        /// <summary>
        /// Initialize static skills.
        /// </summary>
        internal static void Load()
        {
            SkillsDatafile datafile = Util.DeserializeDatafile<SkillsDatafile>(DatafileConstants.SkillsDatafile,
                Util.LoadXslt(Properties.Resources.DatafilesXSLT));

            // Fetch deserialized data
            s_arrayIndicesCount = 0;
            List<Collection<SerializableSkillPrerequisite>> prereqs = new List<Collection<SerializableSkillPrerequisite>>();
            foreach (SerializableSkillGroup srcGroup in datafile.SkillGroups)
            {
                StaticSkillGroup group = new StaticSkillGroup(srcGroup, ref s_arrayIndicesCount);
                s_skillGroupsByID[@group.ID] = @group;

                // Store skills
                foreach (StaticSkill skill in @group)
                {
                    s_skillsByID[skill.ID] = skill;
                    s_skillsByName[skill.Name] = skill;
                }

                // Store prereqs
                prereqs.AddRange(srcGroup.Skills.Select(serialSkill => serialSkill.SkillPrerequisites));
            }

            // Complete initialization
            s_skills = new StaticSkill[s_arrayIndicesCount];
            foreach (StaticSkill staticSkill in s_skillsByID.Values)
            {
                staticSkill.CompleteInitialization(prereqs[staticSkill.ArrayIndex]);
                s_skills[staticSkill.ArrayIndex] = staticSkill;
            }

            GlobalDatafileCollection.OnDatafileLoaded();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the total number of zero-based indices given to skills (for optimization purposes, it allows the use of arrays for computations).
        /// </summary>
        public static int ArrayIndicesCount => s_arrayIndicesCount;

        /// <summary>
        /// Gets the list of groups.
        /// </summary>
        public static IEnumerable<StaticSkillGroup> AllGroups => s_skillGroupsByID.Values;

        /// <summary>
        /// Gets the list of groups.
        /// </summary>
        public static IEnumerable<StaticSkill> AllSkills => s_skillGroupsByID.Values.SelectMany(group => group);

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets a skill by its id or its name.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns>The static skill</returns>
        /// <remarks>
        /// This method exists for backwards compatibility
        /// with settings that don't contain the skill's id.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public static StaticSkill GetSkill(this SerializableSkillPrerequisite src)
        {
            src.ThrowIfNull(nameof(src));

            return GetSkillByID(src.ID) ?? GetSkillByName(src.Name) ?? StaticSkill.UnknownStaticSkill;
        }

        /// <summary>
        /// Gets the name of the skill.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The skill name or <see cref="EveMonConstants.UnknownText"/> if the is no such skill in our data.</returns>
        public static string GetSkillName(int id)
        {
            if (id == 0)
                return string.Empty;

            StaticSkill skill = GetSkillByID(id);
            return skill?.Name ?? EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets a skill by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillByName(string name)
        {
            StaticSkill skill;
            s_skillsByName.TryGetValue(name, out skill);
            return skill;
        }

        /// <summary>
        /// Gets a skill by its identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillByID(long id)
        {
            StaticSkill skill;
            s_skillsByID.TryGetValue(id, out skill);
            return skill;
        }

        /// <summary>
        /// Gets a skill by its array index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillByArrayIndex(int index) => s_skills[index];

        /// <summary>
        /// Gets a group by its name.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static StaticSkillGroup GetSkillGroupByID(int groupId)
        {
            StaticSkillGroup group;
            s_skillGroupsByID.TryGetValue(groupId, out group);
            return group;
        }

        #endregion
    }
}