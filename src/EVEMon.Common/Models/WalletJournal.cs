using System;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class WalletJournal
    {
        private readonly long m_taxReceiverID;
        private readonly long m_ownerID1;
        private readonly long m_ownerID2;
        private readonly int m_refTypeID;
        private string m_taxReceiver;
        private string m_ownerName1;
        private string m_ownerName2;
        private string m_refType;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletJournal" /> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal WalletJournal(SerializableWalletJournalListItem src)
        {
            src.ThrowIfNull(nameof(src));

            m_refTypeID = src.RefTypeID;
            m_taxReceiverID = src.TaxReceiverID;

            ID = src.ID;
            Date = src.Date;
            Amount = src.Amount;
            Balance = src.Balance;
            m_ownerID1 = src.OwnerID1;
            m_ownerName1 = EveIDToName.GetIDToName(m_ownerID1);
            m_ownerID2 = src.OwnerID2;
            m_ownerName2 = EveIDToName.GetIDToName(m_ownerID2);
            TaxAmount = src.TaxAmount;

            Reason = ParseReason(src.Reason ?? string.Empty);
            m_refType = EveRefType.GetRefTypeIDToName(src.RefTypeID);
            m_taxReceiver = GetTaxReceiver();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Gets the balance.
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        public string Issuer => m_ownerName1.IsEmptyOrUnknown() ?
            (m_ownerName1 = EveIDToName.GetIDToName(m_ownerID1)) : m_ownerName1;

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        public string Recipient => m_ownerName2.IsEmptyOrUnknown() ?
            (m_ownerName2 = EveIDToName.GetIDToName(m_ownerID2)) : m_ownerName2;

        /// <summary>
        /// Gets the tax amount.
        /// </summary>
        public decimal TaxAmount { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type => m_refType.IsEmptyOrUnknown() ? (m_refType = EveRefType.
            GetRefTypeIDToName(m_refTypeID)) : m_refType;

        /// <summary>
        /// Gets the tax receiver.
        /// </summary>
        public string TaxReceiver => m_taxReceiver.IsEmptyOrUnknown() ? (m_taxReceiver =
            GetTaxReceiver()) : m_taxReceiver;

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the tax receiver.
        /// </summary>
        /// <returns></returns>
        private string GetTaxReceiver() => (m_taxReceiverID == 0) ? string.Empty :
            EveIDToName.GetIDToName(m_taxReceiverID);

        /// <summary>
        /// Parses the reason text.
        /// </summary>
        /// <param name="reasonText">The reason text.</param>
        /// <returns></returns>
        // If RefType is of type "Bounty Prizes" return a generic message,
        // otherwise clean the header of a player entered text if it exists
        private string ParseReason(string reasonText) => m_refTypeID == 85 ?
            "Killing NPC entities" : reasonText.Replace("DESC: ", string.Empty);

        #endregion
    }
}
