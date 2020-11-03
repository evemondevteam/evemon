using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a skill
    /// </summary>
    public sealed class SerializableCharacterSkill
    {
        [XmlAttribute("typeID")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("level")]
        public long Level { get; set; }

        [XmlAttribute("activelevel")]
        public long ActiveLevel { get; set; }

        [XmlAttribute("skillpoints")]
        public long Skillpoints { get; set; }

        [XmlAttribute("ownsBook")]
        public bool OwnsBook { get; set; }

        [XmlAttribute("isKnown")]
        public bool IsKnown { get; set; }
    }
}
