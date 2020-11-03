using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableCharacterNameListItem
    {
        [XmlAttribute("characterID")]
        public long ID { get; set; }

        [XmlAttribute("name")]
        public string NameXml
        {
            get { return Name; }
            set { Name = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlIgnore]
        public string Name { get; set; }
    }
}