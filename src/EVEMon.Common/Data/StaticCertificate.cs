using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate from the datafiles.
    /// </summary>
    public sealed class StaticCertificate
    {
        #region Constructor

        /// <summary>
        /// Constructor from XML.
        /// </summary>
        /// <param name="certClass"></param>
        /// <param name="src"></param>
        internal StaticCertificate(StaticCertificateClass certClass, SerializableCertificate src)
        {
            ID = src.ID;
            Description = src.Description;
            Class = certClass;
            Grades = new Dictionary<CertificateGrade, List<StaticSkillLevel>>();
            PrerequisiteSkills = new Dictionary<CertificateGrade, List<StaticSkillLevel>>();

            // Recommendations
            Recommendations = new StaticRecommendations<Item>();
            if (src.Recommendations == null || StaticItems.ShipsMarketGroup == null)
                return;

            foreach (Ship ship in src.Recommendations
                .Select(recommendation => StaticItems.ShipsMarketGroup.AllItems.OfType<Ship>()
                    .FirstOrDefault(item => item.Name == recommendation.ShipName))
                .Where(ship => ship != null))
            {
                ship.Recommendations.Add(this);
                Recommendations.Add(ship);
            }
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets this certificate's ID.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the certificate's class.
        /// </summary>
        public StaticCertificateClass Class { get; }

        /// <summary>
        /// Gets this certificate's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets this certificate's grades.
        /// </summary>
        public IDictionary<CertificateGrade, List<StaticSkillLevel>> Grades { get; }

        /// <summary>
        /// Gets the ships this certificate is recommended for.
        /// </summary>
        public StaticRecommendations<Item> Recommendations { get; }

        /// <summary>
        /// Gets the prerequisite skills.
        /// </summary>
        public Dictionary<CertificateGrade, List<StaticSkillLevel>> PrerequisiteSkills { get; }

        /// <summary>
        /// Gets all the top-level prerequisite skills
        /// </summary>
        public IEnumerable<StaticSkillLevel> AllTopPrerequisiteSkills
        {
            get
            {
                long[] highestLevels = new long[StaticSkills.ArrayIndicesCount];
                List<StaticSkillLevel> list = new List<StaticSkillLevel>();

                // Collect all prerequisites from skills
                foreach (StaticSkillLevel skillPrereq in PrerequisiteSkills.SelectMany(entry => entry.Value).Where(
                    skillPrereq => skillPrereq.Skill != null && highestLevels[skillPrereq.Skill.ArrayIndex] < skillPrereq.Level))
                {
                    highestLevels[skillPrereq.Skill.ArrayIndex] = skillPrereq.Level;
                    list.Add(skillPrereq);
                }

                // Return the result
                foreach (StaticSkillLevel newItem in list.Where(
                    newItem => highestLevels[newItem.Skill.ArrayIndex] != 0))
                {
                    yield return new StaticSkillLevel(newItem.Skill, highestLevels[newItem.Skill.ArrayIndex]);
                    highestLevels[newItem.Skill.ArrayIndex] = 0;
                }
            }
        }

        # endregion


        # region Helper Methods

        /// <summary>
        /// Completes the initialization by updating the prerequisites.
        /// </summary>
        internal void CompleteInitialization(IEnumerable<SerializableCertificatePrerequisite> prereqs)
        {
            foreach (var prereqGrade in prereqs.GroupBy(x => x.Grade))
            {
                var prereqList = new List<StaticSkillLevel>(32);
                foreach (var prereq in prereqGrade)
                {
                    int level;
                    if (prereq.Level.TryParseInv(out level))
                        prereqList.Add(new StaticSkillLevel(prereq.ID, level));
                }
                PrerequisiteSkills.Add(prereqGrade.Key, prereqList);
            }
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this certificate.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Description;

        #endregion
    }
}
