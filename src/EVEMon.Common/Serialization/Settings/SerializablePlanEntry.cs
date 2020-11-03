using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a plan entry
    /// </summary>
    public sealed class SerializablePlanEntry
    {
        private readonly Collection<string> m_planGroups;

        public SerializablePlanEntry()
        {
            m_planGroups = new Collection<string>();
            Priority = 3;
        }

        [XmlAttribute("skillID")]
        public int ID { get; set; }

        [XmlAttribute("skill")]
        public string SkillName { get; set; }

        [XmlAttribute("level")]
        public long Level { get; set; }

        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlAttribute("type")]
        public PlanEntryType Type { get; set; }

        [XmlElement("notes")]
        public string Notes { get; set; }

        [XmlElement("group")]
        public Collection<string> PlanGroups => m_planGroups;

        [XmlElement("remapping")]
        public SerializableRemappingPoint Remapping { get; set; }
    }
}