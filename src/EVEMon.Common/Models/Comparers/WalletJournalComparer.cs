using System;
using System.Collections.Generic;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Models.Comparers
{
    public sealed class WalletJournalComparer : Comparer<WalletJournal>
    {
        private readonly WalletJournalColumn m_column;
        private readonly bool m_isAscending;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletJournalComparer"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="isAscending">Is ascending flag.</param>
        public WalletJournalComparer(WalletJournalColumn column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="WalletJournal" /> type and returns a value
        /// indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public override int Compare(WalletJournal x, WalletJournal y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="WalletJournal" /> type and returns a value
        /// indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        private int CompareCore(WalletJournal x, WalletJournal y)
        {
            switch (m_column)
            {
                case WalletJournalColumn.Date:
                    return x.Date.CompareTo(y.Date);
                case WalletJournalColumn.Type:
                    return string.Compare(x.Type, y.Type, StringComparison.CurrentCulture);
                case WalletJournalColumn.Amount:
                    return x.Amount.CompareTo(y.Amount);
                case WalletJournalColumn.Balance:
                    return x.Balance.CompareTo(y.Balance);
                case WalletJournalColumn.Reason:
                    return string.Compare(x.Reason, y.Reason, StringComparison.CurrentCulture);
                case WalletJournalColumn.Issuer:
                    return string.Compare(x.Issuer, y.Issuer, StringComparison.CurrentCulture);
                case WalletJournalColumn.Recipient:
                    return string.Compare(x.Recipient, y.Recipient, StringComparison.CurrentCulture);
                case WalletJournalColumn.TaxReceiver:
                    return string.Compare(x.TaxReceiver, y.TaxReceiver, StringComparison.CurrentCulture);
                case WalletJournalColumn.TaxAmount:
                    return x.TaxAmount.CompareTo(y.TaxAmount);
                case WalletJournalColumn.ID:
                    return x.ID.CompareTo(y.ID);
                default:
                    return 0;
            }
        }
    }
}
