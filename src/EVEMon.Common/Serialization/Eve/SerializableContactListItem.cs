using System;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableContactListItem
    {
        [XmlAttribute("contactID")]
        public long ContactID { get; set; }

        [XmlAttribute("contactName")]
        public string ContactNameXml
        {
            get { return ContactName; }
            set { ContactName = value?.HtmlDecode() ?? String.Empty; }
        }

        [XmlAttribute("inWatchlist")]
        public string InWatchlistXml
        {
            get { return InWatchlist.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    InWatchlist = Convert.ToBoolean(value, CultureConstants.InvariantCulture);
            }
        }

        [XmlAttribute("standing")]
        public float Standing { get; set; }

        [XmlAttribute("contactTypeID")]
        public long ContactTypeID { get; set; }

        [XmlIgnore]
        public ContactGroup Group { get; set; }

        [XmlIgnore]
        public bool InWatchlist { get; set; }

        [XmlIgnore]
        public string ContactName { get; set; }
    }
}
