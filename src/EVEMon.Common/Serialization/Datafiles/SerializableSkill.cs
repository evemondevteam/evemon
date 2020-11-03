using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a skill in our datafile
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableSkill
    {
        private readonly Collection<SerializableSkillPrerequisite> m_skillPrereqs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableSkill"/> class.
        /// </summary>
        public SerializableSkill()
        {
            m_skillPrereqs = new Collection<SerializableSkillPrerequisite>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SerializableSkill"/> is public.
        /// </summary>
        /// <value><c>true</c> if public; otherwise, <c>false</c>.</value>
        [XmlAttribute("public")]
        public bool Public { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the primary attribute.
        /// </summary>
        /// <value>The primary attribute.</value>
        [XmlAttribute("primaryAttr")]
        public EveAttribute PrimaryAttribute { get; set; }

        /// <summary>
        /// Gets or sets the secondary attribute.
        /// </summary>
        /// <value>The secondary attribute.</value>
        [XmlAttribute("secondaryAttr")]
        public EveAttribute SecondaryAttribute { get; set; }

        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        /// <value>The rank.</value>
        [XmlAttribute("rank")]
        public long Rank { get; set; }

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>The cost.</value>
        [XmlAttribute("cost")]
        public long Cost { get; set; }

        /// <summary>
        /// Gets the skill prerequisites.
        /// </summary>
        /// <value>The skill prerequisites.</value>
        [XmlElement("prereq")]
        public Collection<SerializableSkillPrerequisite> SkillPrerequisites => m_skillPrereqs;

        /// <summary>
        /// The highest level the skill can be trained on an alpha clone.
        /// </summary>
        /// <value>The level.</value>
        public int AlphaLimit { get; set; }
    }
}
