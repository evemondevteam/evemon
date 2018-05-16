using System;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;


namespace EVEMon.Common.Models
{
    public sealed class WalletTransaction
    {
        private readonly long m_stationID;
        private readonly long m_clientID;
        private string m_clientName;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletTransaction" /> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        internal WalletTransaction(SerializableWalletTransactionsListItem src)
        {
            src.ThrowIfNull(nameof(src));

            ID = src.ID;
            JournalID = src.JournalTransactionID;
            Date = src.TransactionDate;
            ItemName = src.TypeName;
            Quantity = src.Quantity;
            Price = src.Price;
            m_clientID = src.ClientID;
            m_clientName = EveIDToName.GetIDToName(m_clientID);
            TransactionType = src.TransactionType == "buy" ? TransactionType.Buy : TransactionType.Sell;
            TransactionFor = src.TransactionFor == "personal" ? IssuedFor.Character : IssuedFor.Corporation;
            m_stationID = src.StationID;
            UpdateStation();

            Credit = GetCredit();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// Gets the journal ID.
        /// </summary>
        public long JournalID { get; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        /// <value>
        /// The name of the item.
        /// </value>
        public string ItemName { get; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public long Quantity { get; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Gets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName => m_clientName.IsEmptyOrUnknown() ?
            (m_clientName = EveIDToName.GetIDToName(m_clientID)) : m_clientName;

        /// <summary>
        /// Gets the station.
        /// </summary>
        public Station Station { get; private set; }

        /// <summary>
        /// Gets the type of the transaction.
        /// </summary>
        /// <value>
        /// The type of the transaction.
        /// </value>
        public TransactionType TransactionType { get; }

        /// <summary>
        /// Gets the transaction for.
        /// </summary>
        public IssuedFor TransactionFor { get; }

        /// <summary>
        /// Gets the credit.
        /// </summary>
        public decimal Credit { get; }

        #endregion


        #region Helper Methods

        private decimal GetCredit()
        {

            decimal credit = Quantity * Price;
            return TransactionType == TransactionType.Buy ? -credit : credit;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Updates the station.
        /// </summary>
        public void UpdateStation()
        {
            Station = EveIDToStation.GetIDToStation(m_stationID);
        }

        #endregion

    }
}
