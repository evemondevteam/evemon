using System;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    public sealed class OutputAttributeEnhancer
    {
        [XmlAttribute("attribute")]
        public ImplantSlots Attribute { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("bonus")]
        public long Bonus { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}