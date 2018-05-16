using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiContractBidsListItem
    {
        private DateTime bidDate;

        public EsiContractBidsListItem()
        {
            bidDate = DateTime.MinValue;
        }

        [DataMember(Name = "bid_id")]
        public int ID { get; set; }

        [DataMember(Name = "bidder_id")]
        public long BidderID { get; set; }

        [DataMember(Name = "date_bid")]
        private string DateBidJson
        {
            get
            {
                return bidDate.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    bidDate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [IgnoreDataMember]
        public DateTime DateBid
        {
            get
            {
                return bidDate;
            }
        }

        public SerializableContractBidsListItem ToXMLItem(long contract)
        {
            // We need the contract that was requested so that we can match the bids
            return new SerializableContractBidsListItem()
            {
                BidderID = BidderID,
                ContractID = contract,
                DateBid = DateBid,
                Amount = Amount,
            };
        }
    }
}
