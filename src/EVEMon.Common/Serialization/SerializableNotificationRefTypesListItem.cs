using System.Xml.Serialization;

namespace EVEMon.Common.Serialization
{
    public sealed class SerializableNotificationRefTypesListItem
    {
        [XmlAttribute("refTypeID")]
        public int TypeID { get; set; }

        [XmlAttribute("refTypeCode")]
        public string TypeCode { get; set; }

        [XmlAttribute("refTypeName")]
        public string TypeName { get; set; }

        [XmlAttribute("subjectLayout")]
        public string SubjectLayout { get; set; }

        [XmlAttribute("textLayout")]
        public string TextLayout { get; set; }
    }
}
