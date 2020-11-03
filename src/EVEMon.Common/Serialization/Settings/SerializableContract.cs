using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableContract
    {
        public SerializableContract()
        {
            ContractType = ContractType.None;
        }

        [XmlAttribute("contractID")]
        public long ContractID { get; set; }

        [XmlAttribute("contractState")]
        public ContractState ContractState { get; set; }

        [XmlAttribute("contractType")]
        public ContractType ContractType { get; set; }

        [XmlAttribute("issuedFor")]
        public IssuedFor IssuedFor { get; set; }
    }
}
