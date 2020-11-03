using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a mastery level.
    /// </summary>
    public sealed class Mastery : ReadonlyCollection<MasteryCertificate>, IComparable<Mastery>
    {
        private bool m_updated;
        private readonly Character m_character;


        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="masteryShip">The mastery ship.</param>
        /// <param name="src">The source.</param>
        internal Mastery(MasteryShip masteryShip, SerializableMastery src)
            : base(src?.Certificates.Count ?? 0)
        {
            if (src != null)
            {
                MasteryShip = masteryShip;
                Level = src.Grade;
                Status = MasteryStatus.Untrained;
                foreach (SerializableMasteryCertificate certificate in src.Certificates)
                    Items.Add(new MasteryCertificate(this, certificate));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mastery"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="mastery">The mastery.</param>
        internal Mastery(Character character, Mastery mastery)
            : base(mastery.Count)
        {
            m_character = character;
            MasteryShip = mastery.MasteryShip;
            Level = mastery.Level;
            Status = MasteryStatus.Untrained;
            foreach (MasteryCertificate masteryCertificate in mastery)
                Items.Add(new MasteryCertificate(character, masteryCertificate));
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the mastery ship.
        /// </summary>
        public MasteryShip MasteryShip { get; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public MasteryStatus Status { get; private set; }

        /// <summary>
        /// Gets true whether the mastery is trained.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mastery is trained; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrained => Status == MasteryStatus.Trained;

        /// <summary>
        /// Gets true whether the mastery is partially trained.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mastery is partilly trained; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartiallyTrained => Status == MasteryStatus.PartiallyTrained;

        /// <summary>
        /// Gets the prerequisite skills.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SkillLevel> GetPrerequisiteSkills() => Items.SelectMany(cert =>
            cert.Certificate.PrerequisiteSkills.Where(level => (int)level.Key == Level).
            SelectMany(level => level.Value.ToCharacter(m_character))).Distinct();

        /// <summary>
        /// Gets the training time.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTrainingTime() => m_character.GetTrainingTimeToMultipleSkills(
            GetPrerequisiteSkills());

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries to update the mastery level status.
        /// </summary>
        public bool TryUpdateMasteryStatus()
        {
            if (m_updated)
                return false;

            bool noPrereq = true;
            bool trained = true;

            // Scan prerequisite skills
            foreach (SkillLevel prereqSkill in GetPrerequisiteSkills())
            {
                // Trained only if the skill's level is greater or equal to the minimum level
                trained = trained && prereqSkill.Skill.Level >= prereqSkill.Level;
                noPrereq = noPrereq && prereqSkill.AllDependencies.All(x => !x.IsTrained);
            }

            // Updates status
            if (trained)
                Status = MasteryStatus.Trained;
            else if (noPrereq)
                Status = MasteryStatus.Untrained;
            else
                Status = MasteryStatus.PartiallyTrained;

            m_updated = true;
            return true;
        }

        /// <summary>
        /// Sets this mastery as "untrained", a useful optimization for low-SP characters
        /// (which are more numerous than maxed chars)
        /// </summary>
        public bool SetAsUntrained()
        {
            if (m_updated)
                return false;

            m_updated = true;
            Status = MasteryStatus.Untrained;
            return true;
        }

        #endregion

        public int CompareTo(Mastery other)
        {
            MasteryShip sm = MasteryShip, osm = other.MasteryShip;
            int comparison;
            if (sm == null)
                // NULL versions should not be intermixed, but if they are, put them last
                comparison = (osm == null) ? 0 : 1;
            else if (osm == null)
                comparison = -1;
            else
            {
                // Both are not null
                string shipOne = sm.Ship?.Name ?? string.Empty, shipTwo = osm.Ship?.Name ??
                    string.Empty;
                comparison = shipOne.CompareTo(shipTwo);
                if (comparison == 0)
                    // Levels are 1 to 5, no overflow can occur here
                    comparison = Level - other.Level;
            }
            return comparison;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => "Level " + Skill.GetRomanFromInt(Level);

    }
}
