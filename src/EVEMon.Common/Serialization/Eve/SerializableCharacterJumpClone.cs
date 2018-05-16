using System;
using System.Xml.Serialization;
using EVEMon.Common.Service;

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
                if (!string.IsNullOrEmpty(value))
                    CloneName = value;

                var station = EveIDToStation.GetIDToStation(LocationID);

                CloneName = string.Format("Clone in {0}", station != null ? station.Name :
                    string.Format("Clone Vat Bay ({0})", LocationID));
            }
        }

        [XmlIgnore]
        public string CloneName { get; set; }

    }
}
