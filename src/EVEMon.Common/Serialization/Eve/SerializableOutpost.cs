using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableOutpost
    {
        [XmlAttribute("stationID")]
        public long StationID { get; set; }

        [XmlAttribute("stationName")]
        public string StationNameXml
        {
            get { return StationName; }
            set { StationName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlAttribute("stationTypeID")]
        public int StationTypeID { get; set; }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlAttribute("corporationID")]
        public int CorporationID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationNameXml
        {
            get { return CorporationName; }
            set { CorporationName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlIgnore]
        public string StationName { get; set; }

        [XmlIgnore]
        public string CorporationName { get; set; }
    }
}