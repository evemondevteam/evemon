using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable location containing a solar system and station / structure.
    /// </summary>
    public sealed class SerializableLocation
    {
        [XmlElement("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlElement("stationID")]
        public int StationID { get; set; }

        [XmlElement("structureID")]
        public long StructureID { get; set; }
    }
}
