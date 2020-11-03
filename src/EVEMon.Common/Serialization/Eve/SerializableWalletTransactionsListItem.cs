using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableWalletTransactionsListItem
    {
        [XmlAttribute("transactionID")]
        public long ID { get; set; }

        [XmlAttribute("journalTransactionID")]
        public long JournalTransactionID { get; set; }

        [XmlAttribute("quantity")]
        public long Quantity { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("typeName")]
        public string TypeName { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("clientID")]
        public long ClientID { get; set; }

        [XmlAttribute("clientName")]
        public string ClientNameXml
        {
            get { return ClientName; }
            set { ClientName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlAttribute("stationID")]
        public long StationID { get; set; }

        [XmlAttribute("stationName")]
        public string StationNameXml
        {
            get { return StationName; }
            set { StationName = value?.HtmlDecode() ?? string.Empty; }
        }

        [XmlAttribute("transactionType")]
        public string TransactionType { get; set; }

        [XmlAttribute("transactionFor")]
        public string TransactionFor { get; set; }

        [XmlAttribute("transactionDateTime")]
        public string TransactionDateXml
        {
            get { return TransactionDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    TransactionDate = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public string ClientName { get; set; }

        [XmlIgnore]
        public string StationName { get; set; }

        [XmlIgnore]
        public DateTime TransactionDate { get; set; }
    }
}