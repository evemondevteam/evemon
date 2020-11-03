using System;
using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a readonly skill group definition. Characters have their own implementations, <see cref="SkillGroup"/>
    /// </summary>
    public sealed class StaticSkillGroup : ReadonlyKeyedCollection<int, StaticSkill>
    {
        private static StaticSkillGroup s_unknownStaticSkillGroup;


        #region Constructors

        /// <summary>
        /// Constructor for an unknown static skill group.
        /// </summary>
        private StaticSkillGroup()
        {
            ID = int.MaxValue;
            Name = EveMonConstants.UnknownText;
        }

        /// <summary>
        /// Deserialization constructor from datafiles.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="skillArrayIndex"></param>
        internal StaticSkillGroup(SerializableSkillGroup src, ref int skillArrayIndex)
        {
            ID = src.ID;
            Name = src.Name;
            foreach (SerializableSkill srcSkill in src.Skills)
            {
                Items[srcSkill.ID] = new StaticSkill(this, srcSkill, skillArrayIndex);
                skillArrayIndex++;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the group's identifier.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the group's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unknown static skill group.
        /// </summary>
        /// <value>
        /// The unknown static skill group.
        /// </value>
        public static StaticSkillGroup UnknownStaticSkillGroup
            => s_unknownStaticSkillGroup ?? (s_unknownStaticSkillGroup = new StaticSkillGroup());

        #endregion


        #region Indexers

        /// <summary>
        /// Gets a skill from this group by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StaticSkill this[int id] => GetByKey(id);

        #endregion


        #region Public Methods

        /// <summary>
        /// Checks whether this group contains the specified skill.
        /// </summary>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public bool Contains(int skillID) => Items.ContainsKey(skillID);

        #endregion


        #region IEnumerable<StaticSkill> Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StaticSkill> GetEnumerator() => Items.Values.GetEnumerator();

        #endregion
    }
}