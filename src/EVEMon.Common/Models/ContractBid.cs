using System;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class ContractBid
    {
        private readonly long m_bidderId;
        private string m_bidder;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal ContractBid(SerializableContractBidsListItem src)
        {
            src.ThrowIfNull(nameof(src));

            ID = src.ID;
            ContractID = src.ContractID;
            m_bidderId = src.BidderID;
            m_bidder = EveIDToName.GetIDToName(src.BidderID);
            BidDate = src.DateBid;
            Amount = src.Amount;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal ContractBid(SerializableContractBid src)
        {
            src.ThrowIfNull(nameof(src));

            ID = src.BidID;
            ContractID = src.ContractID;
            m_bidder = src.Bidder;
            BidDate = src.BidDate;
            Amount = src.Amount;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// Gets the contract ID.
        /// </summary>
        public long ContractID { get; }

        /// <summary>
        /// Gets the bidder.
        /// </summary>
        public string Bidder => m_bidder.IsEmptyOrUnknown() ? (m_bidder = EveIDToName.
            GetIDToName(m_bidderId)) : m_bidder;

        /// <summary>
        /// Gets the bid date.
        /// </summary>
        public DateTime BidDate { get; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        public decimal Amount { get; }

        #endregion


        #region Exportation

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableContractBid Export() => new SerializableContractBid
        {
            BidID = ID,
            ContractID = ContractID,
            Bidder = Bidder,
            BidDate = BidDate,
            Amount = Amount
        };

        #endregion
    }
}
