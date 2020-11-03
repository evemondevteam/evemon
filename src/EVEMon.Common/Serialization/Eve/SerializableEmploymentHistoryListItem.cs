using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableEmploymentHistoryListItem
    {
        [XmlAttribute("recordID")]
        public long RecordID { get; set; }

        [XmlAttribute("corporationID")]
        public int CorporationID { get; set; }

        [XmlAttribute("corporationName")]
        public string CorporationName { get; set; }

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
        public DateTime StartDate { get; set; }
    }
}