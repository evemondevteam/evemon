using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableEmploymentHistory
    {
        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationNameXml
        {
            get { return CorporationName; }
            set { CorporationName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlAttribute("startDate")]
        public string StartDateXml
        {
            get { return StartDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    StartDate = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public string CorporationName { get; set; }

        [XmlIgnore]
        public DateTime StartDate { get; set; }
    }
}