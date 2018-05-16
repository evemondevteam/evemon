using System;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiContractListItem
    {
        private DateTime dateAccepted;
        private DateTime dateCompleted;
        private DateTime dateExpired;
        private DateTime dateIssued;
        private ContractAvailability availability;

        public EsiContractListItem()
        {
            dateAccepted = DateTime.MinValue;
            dateCompleted = DateTime.MinValue;
            dateExpired = DateTime.MinValue;
            dateIssued = DateTime.MinValue;
            availability = ContractAvailability.None;
        }

        [DataMember(Name = "contract_id")]
        public long ContractID { get; set; }

        [DataMember(Name = "issuer_id")]
        public long IssuerID { get; set; }

        [DataMember(Name = "issuer_corporation_id")]
        public long IssuerCorpID { get; set; }

        [DataMember(Name = "assignee_id")]
        public long AssigneeID { get; set; }

        [DataMember(Name = "acceptor_id")]
        public long AcceptorID { get; set; }

        [DataMember(Name = "start_location_id")]
        public long StartStationID { get; set; }

        [DataMember(Name = "end_location_id")]
        public long EndStationID { get; set; }

        // One of: unknown, item_exchange, auction, courier, loan
        [DataMember(Name = "type")]
        public string Type { get; set; }

        // One of: outstanding, in_progress, finished_issuer, finished_contractor, finished,
        // cancelled, rejected, failed, deleted, reversed
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "for_corporation")]
        public bool ForCorp { get; set; }

        // One of: public, personal, corporation, alliance
        [DataMember(Name = "availability")]
        private string AvailabilityJson
        {
            get
            {
                return availability.ToString();
            }
            set
            {
                switch (value)
                {
                case "public":
                    availability = ContractAvailability.Public;
                    break;
                case "corporation":
                case "personal":
                case "alliance":
                    availability = ContractAvailability.Private;
                    break;
                default:
                    availability = ContractAvailability.None;
                    break;
                }
            }
        }

        [DataMember(Name = "date_issued")]
        private string DateIssuedJson
        {
            get { return dateIssued.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    dateIssued = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "date_expired")]
        private string DateExpiredJson
        {
            get { return dateExpired.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    dateExpired = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "date_accepted")]
        private string DateAcceptedJson
        {
            get { return dateAccepted.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    dateAccepted = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "days_to_complete")]
        public int NumDays { get; set; }

        [DataMember(Name = "date_completed")]
        private string DateCompletedJson
        {
            get { return dateCompleted.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    dateCompleted = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "reward")]
        public decimal Reward { get; set; }

        [DataMember(Name = "collateral")]
        public decimal Collateral { get; set; }

        [DataMember(Name = "buyout")]
        public decimal Buyout { get; set; }

        [DataMember(Name = "volume")]
        public decimal Volume { get; set; }

        [IgnoreDataMember]
        public DateTime DateIssued
        {
            get
            {
                return dateIssued;
            }
        }

        [IgnoreDataMember]
        public DateTime DateExpired
        {
            get
            {
                return dateExpired;
            }
        }

        [IgnoreDataMember]
        public DateTime DateAccepted
        {
            get
            {
                return dateAccepted;
            }
        }

        [IgnoreDataMember]
        public DateTime DateCompleted
        {
            get
            {
                return dateCompleted;
            }
        }

        [IgnoreDataMember]
        public ContractAvailability Availability
        {
            get
            {
                return availability;
            }
        }
        
        // Converts ESI status to the old XML status
        private CCPContractStatus ContractStatus
        {
            get
            {
                CCPContractStatus ccpStatus;
                switch (Status)
                {
                case "outstanding":
                    ccpStatus = CCPContractStatus.Outstanding;
                    break;
                case "in_progress":
                    ccpStatus = CCPContractStatus.InProgress;
                    break;
                case "finished_issuer":
                    ccpStatus = CCPContractStatus.CompletedByIssuer;
                    break;
                case "finished_contractor":
                    ccpStatus = CCPContractStatus.CompletedByContractor;
                    break;
                case "finished":
                    ccpStatus = CCPContractStatus.Completed;
                    break;
                case "cancelled":
                    ccpStatus = CCPContractStatus.Canceled;
                    break;
                case "rejected":
                    ccpStatus = CCPContractStatus.Rejected;
                    break;
                case "failed":
                    ccpStatus = CCPContractStatus.Failed;
                    break;
                case "deleted":
                    ccpStatus = CCPContractStatus.Deleted;
                    break;
                case "reversed":
                    ccpStatus = CCPContractStatus.Reversed;
                    break;
                default:
                    ccpStatus = CCPContractStatus.None;
                    break;
                }
                return ccpStatus;
            }
        }

        // Converts ESI contrac type to the old XML type
        private string XMLType
        {
            get
            {
                string type;
                switch (Type)
                {
                case "item_exchange":
                    type = "ItemExchange";
                    break;
                case "auction":
                case "courier":
                case "loan":
                    type = Type.ToTitleCase();
                    break;
                default:
                    type = "None";
                    break;
                }
                return type;
            }
        }

        public SerializableContractListItem ToXMLItem()
        {
            return new SerializableContractListItem()
            {
                AcceptorID = AcceptorID,
                AssigneeID = AssigneeID,
                // Yes, it converts to and from, but all in the name of serialization
                Availability = Availability.ToString(),
                Buyout = Buyout,
                Collateral = Collateral,
                ContractID = ContractID,
                DateAccepted = DateAccepted,
                DateCompleted = DateCompleted,
                DateExpired = DateExpired,
                DateIssued = DateIssued,
                EndStationID = EndStationID,
                ForCorp = ForCorp,
                IssuerCorpID = IssuerCorpID,
                IssuerID = IssuerID,
                NumDays = NumDays,
                Price = Price,
                Reward = Reward,
                Status = ContractStatus.ToString(),
                StartStationID = StartStationID,
                Title = Title,
                Type = XMLType,
                Volume = Volume
            };
        }
    }
}
