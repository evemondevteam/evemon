using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.EveMarketer.MarketPricer
{
    public sealed class SerializableECItemPriceItem
    {
        [XmlElement("avg")]
        public double Average { get; set; }
    }
}
