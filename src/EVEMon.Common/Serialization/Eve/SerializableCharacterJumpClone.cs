using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableCharacterJumpClone
    {
        [XmlAttribute("jumpCloneID")]
        public long JumpCloneID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [XmlAttribute("cloneName")]
        public string CloneNameXml
        {
            get { return CloneName; }
            set
            {
                CloneName = value;
            }
        }

        [XmlIgnore]
        public string CloneName { get; set; }
    }
}
