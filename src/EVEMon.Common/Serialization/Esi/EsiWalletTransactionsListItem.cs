using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiWalletTransactionsListItem
    {
        private DateTime transactionDate;

        public EsiWalletTransactionsListItem()
        {
            transactionDate = DateTime.MinValue;
        }

        [DataMember(Name = "transaction_id")]
        public long ID { get; set; }

        [DataMember(Name = "journal_ref_id")]
        public long JournalTransactionID { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }

        [DataMember(Name = "unit_price")]
        public decimal Price { get; set; }

        [DataMember(Name = "client_id")]
        public int ClientID { get; set; }

        [DataMember(Name = "location_id")]
        public long StationID { get; set; }
        
        [DataMember(Name = "is_buy")]
        public bool Buy { get; set; }

        [DataMember(Name = "is_personal")]
        public bool Personal { get; set; }

        [DataMember(Name = "date")]
        private string TransactionDateJson
        {
            get { return transactionDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    transactionDate = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime TransactionDate
        {
            get
            {
                return transactionDate;
            }
        }

        public SerializableWalletTransactionsListItem ToXMLItem()
        {
            return new SerializableWalletTransactionsListItem()
            {
                ClientID = ClientID,
                ID = ID,
                JournalTransactionID = JournalTransactionID,
                Price = Price,
                Quantity = Quantity,
                StationID = StationID,
                TransactionDate = TransactionDate,
                TransactionType = Buy ? "buy" : "sell",
                TransactionFor = Personal ? "personal" : "corporate",
                TypeID = TypeID,
                TypeName = StaticItems.GetItemName(TypeID)
            };
        }
    }
}
