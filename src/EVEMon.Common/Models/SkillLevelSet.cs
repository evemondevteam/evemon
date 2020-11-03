using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a lisht and fast dictionary for skilllevels, focused on 
    /// </summary>
    internal sealed class SkillLevelSet<T> : IReadonlyCollection<T>
        where T : class, ISkillLevel
    {
        private readonly T[] m_items;

        /// <summary>
        /// Constructor
        /// </summary>
        public SkillLevelSet()
        {
            m_items = new T[StaticSkills.ArrayIndicesCount * 5];
        }

        /// <summary>
        /// Gets the number of items in the set
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Adds the given item in the list
        /// </summary>
        /// <param name="item"></param>
        public void Set(T item)
        {
            this[item.Skill.ArrayIndex, item.Level] = item;
        }


        #region Indexors

        /// <summary>
        /// Gets or sets the item for the given skill array index and level
        /// </summary>
        /// <param name="skillArrayIndex"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public T this[long skillArrayIndex, long level]
        {
            get
            {
                if (level <= 0 || level > 5)
                    throw new ArgumentOutOfRangeException(nameof(level), @"Level must be greater than 0 and lesser or equal than 5.");

                return m_items[skillArrayIndex * 5 + level - 1];
            }
            set
            {
                if (level <= 0 || level > 5)
                    throw new ArgumentOutOfRangeException(nameof(level), @"Level must be greater than 0 and lesser or equal than 5.");

                T oldValue = m_items[skillArrayIndex * 5 + level - 1];

                if (value == null || value.Skill == null)
                    Count--;
                else if (oldValue == null || oldValue.Skill == null)
                    Count++;
                m_items[skillArrayIndex * 5 + level - 1] = value;
            }
        }

        /// <summary>
        /// Gets or sets the item for the given skill and level
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public T this[StaticSkill skill, long level]
        {
            get { return this[skill.ArrayIndex, level]; }
            set { this[skill.ArrayIndex, level] = value; }
        }

        #endregion


        #region Contains overloads

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <param name="skillArrayindex"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Contains(int skillArrayindex, int level)
        {
            if (skillArrayindex > m_items.Length)
                return true;

            T result = this[skillArrayindex, level];
            return result != null && result.Skill != null;
        }

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Contains(StaticSkill skill, int level)
        {
            if (skill.ArrayIndex > m_items.Length)
                return true;

            T result = this[skill.ArrayIndex, level];
            return result != null && result.Skill != null;
        }

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Contains(Skill skill, int level)
        {
            if (skill.ArrayIndex > m_items.Length)
                return true;

            T result = this[skill.ArrayIndex, level];
            return result != null && result.Skill != null;
        }

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <remarks>The comparison is only based on skill array index and level, the two objects may be actually different references</remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ISkillLevel item)
        {
            if (item.Skill.ArrayIndex > m_items.Length)
                return true;

            T result = this[item.Skill.ArrayIndex, item.Level];
            return result != null && result.Skill != null;
        }

        #endregion


        #region Remove overloads

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <param name="skillArrayindex"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public void Remove(int skillArrayindex, int level)
        {
            this[skillArrayindex, level] = default(T);
        }

        /// <summary>
        /// Gets true if a matching item is already contained.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public void Remove(StaticSkill skill, int level)
        {
            this[skill.ArrayIndex, level] = default(T);
        }

        /// <summary>
        /// Remove the matching item
        /// </summary>
        /// <remarks>The comparison is only based on skill array index and level, the two objects may be actually different references</remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Remove(ISkillLevel item)
        {
            this[item.Skill.ArrayIndex, item.Level] = default(T);
        }

        #endregion


        #region GetLevelsOf() overloads

        /// <summary>
        /// Gets the levels of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public IEnumerable<T> GetLevelsOf(Skill skill) => GetLevelsOf(skill.ArrayIndex);

        /// <summary>
        /// Gets the levels of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public IEnumerable<T> GetLevelsOf(StaticSkill skill) => GetLevelsOf(skill.ArrayIndex);

        /// <summary>
        /// Gets the levels of the skill represented by the given index.
        /// </summary>
        /// <param name="skillArrayIndex"></param>
        /// <returns></returns>
        public IEnumerable<T> GetLevelsOf(int skillArrayIndex)
        {
            for (int i = 0; i < 5; i++)
            {
                T item = m_items[skillArrayIndex * 5 + i];
                if (item != null && item.Skill != null)
                    yield return item;
            }
        }

        #endregion


        #region IEnumerable<T> Members

        private IEnumerable<T> Enumerate() => m_items.Where(item => item != null);

        public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Enumerate()).GetEnumerator();

        #endregion
    }
}