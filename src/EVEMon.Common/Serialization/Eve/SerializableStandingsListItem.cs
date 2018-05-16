using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableStandingsListItem
    {
        [XmlAttribute("fromID")]
        public long ID { get; set; }

        [XmlAttribute("fromName")]
        public string Name { get; set; }

        [XmlAttribute("standing")]
        public double StandingValue { get; set; }

        [XmlIgnore]
        public StandingGroup Group { get; set; }
    }
}
