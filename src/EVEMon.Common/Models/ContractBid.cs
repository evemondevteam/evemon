using System;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Service;
using EVEMon.Common.Serialization.Esi;

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
        internal ContractBid(EsiContractBidsListItem src)
        {
            src.ThrowIfNull(nameof(src));

            ID = src.ID;
            m_bidderId = src.BidderID;
            m_bidder = EveIDToName.GetIDToName(src.BidderID);
            BidDate = src.DateBid;
            Amount = src.Amount;
        }
        
        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; }
        
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

    }
}
