using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a blueprint in our datafile
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableBlueprint
    {
        private readonly Collection<SerializablePrereqSkill> m_prereqSkills;
        private readonly Collection<SerializableRequiredMaterial> m_requiredMaterials;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableBlueprint"/> class.
        /// </summary>
        public SerializableBlueprint()
        {
            InventionTypeIDs = new ModifiedSerializableDictionary<int, decimal>();
            m_prereqSkills = new Collection<SerializablePrereqSkill>();
            m_requiredMaterials = new Collection<SerializableRequiredMaterial>();
        }

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
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the meta group.
        /// </summary>
        /// <value>The meta group.</value>
        [XmlAttribute("metaGroup")]
        public ItemMetaGroup MetaGroup { get; set; }

        /// <summary>
        /// Gets or sets the produce item ID.
        /// </summary>
        /// <value>The produce item ID.</value>
        [XmlAttribute("productTypeID")]
        public int ProduceItemID { get; set; }

        /// <summary>
        /// Gets or sets the production time.
        /// </summary>
        /// <value>The production time.</value>
        [XmlAttribute("productionTime")]
        public int ProductionTime { get; set; }

        /// <summary>
        /// Gets or sets the research productivity time.
        /// </summary>
        /// <value>The research productivity time.</value>
        [XmlAttribute("researchProductivityTime")]
        public int ResearchProductivityTime { get; set; }

        /// <summary>
        /// Gets or sets the research material time.
        /// </summary>
        /// <value>The research material time.</value>
        [XmlAttribute("researchMaterialTime")]
        public int ResearchMaterialTime { get; set; }

        /// <summary>
        /// Gets or sets the research copy time.
        /// </summary>
        /// <value>The research copy time.</value>
        [XmlAttribute("researchCopyTime")]
        public int ResearchCopyTime { get; set; }

        /// <summary>
        /// Gets or sets the reverse engineering time.
        /// </summary>
        /// <value>The reverse engineering time.</value>
        [XmlAttribute("reverseEngineeringTime")]
        public int ReverseEngineeringTime { get; set; }

        /// <summary>
        /// Gets or sets the invention time.
        /// </summary>
        /// <value>The invention time.</value>
        [XmlAttribute("inventionTime")]
        public int InventionTime { get; set; }

        /// <summary>
        /// Gets or sets the reaction time.
        /// </summary>
        /// <value>The reaction time.</value>
        [XmlAttribute("reactionTime")]
        public int ReactionTime { get; set; }

        /// <summary>
        /// Gets or sets the max production limit.
        /// </summary>
        /// <value>The max production limit.</value>
        [XmlAttribute("maxProductionLimit")]
        public int MaxProductionLimit { get; set; }

        /// <summary>
        /// Gets or sets the invention type ID.
        /// </summary>
        /// <value>The invention type ID.</value>
        [XmlElement("inventTypeIDs")]
        public ModifiedSerializableDictionary<int, decimal> InventionTypeIDs { get; set; }

        /// <summary>
        /// Gets or sets the reaction outcome.
        /// </summary>
        /// <value>The reaction outcome.</value>
        [XmlElement("reactionOutcome")]
        public SerializableMaterialQuantity ReactionOutcome { get; set; }

        /// <summary>
        /// Gets the prereq skill.
        /// </summary>
        /// <value>The prereq skill.</value>
        [XmlElement("s")]
        public Collection<SerializablePrereqSkill> PrereqSkill => m_prereqSkills;

        /// <summary>
        /// Gets the req material.
        /// </summary>
        /// <value>The req material.</value>
        [XmlElement("m")]
        public Collection<SerializableRequiredMaterial> ReqMaterial => m_requiredMaterials;
    }
}
