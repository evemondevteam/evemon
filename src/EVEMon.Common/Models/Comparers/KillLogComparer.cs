using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EVEMon.Common.Models.Comparers
{
    /// <summary>
    /// Performs a comparison between two <see cref="KillLog"/> types.
    /// </summary>
    public sealed class KillLogComparer : Comparer<KillLog>
    {
        private readonly ColumnHeader m_column;
        private readonly bool m_isAscending;

        /// <summary>
        /// Initializes a new instance of the <see cref="KillLogComparer"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="isAscending">Is ascending flag.</param>
        public KillLogComparer(ColumnHeader column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="KillLog" /> type and returns a value
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
        public override int Compare(KillLog x, KillLog y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="KillLog" /> type and returns a value
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
        private int CompareCore(KillLog x, KillLog y)
        {
            switch (m_column.Index)
            {
                case 0:
                    return x.KillTime.CompareTo(y.KillTime);
                case 1:
                    return string.Compare(x.Victim.ShipTypeName, y.Victim.ShipTypeName, StringComparison.CurrentCulture);
                case 2:
                    return string.Compare(x.Victim.Name, y.Victim.Name, StringComparison.CurrentCulture);
                case 3:
                    return string.Compare(x.Victim.CorporationName, y.Victim.CorporationName, StringComparison.CurrentCulture);
                case 4:
                    return string.Compare(x.Victim.AllianceName, y.Victim.AllianceName, StringComparison.CurrentCulture);
                case 5:
                    return string.Compare(x.Victim.FactionName, y.Victim.FactionName, StringComparison.CurrentCulture);
                default:
                    return 0;
            }
        }
    }
}

