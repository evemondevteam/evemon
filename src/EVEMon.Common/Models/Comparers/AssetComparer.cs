using System;
using System.Collections.Generic;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Models.Comparers
{
    public sealed class AssetComparer : Comparer<Asset>
    {
        private readonly AssetColumn m_column;
        private readonly bool m_isAscending;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetComparer"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="isAscending">Is ascending flag.</param>
        public AssetComparer(AssetColumn column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="Asset" /> type and returns a value
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
        public override int Compare(Asset x, Asset y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="Asset" /> type and returns a value
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
        private int CompareCore(Asset x, Asset y)
        {
            switch (m_column)
            {
                case AssetColumn.ItemName:
                    return string.Compare(x.Item.Name, y.Item.Name, StringComparison.CurrentCulture);
                case AssetColumn.Quantity:
                    return x.Quantity.CompareTo(y.Quantity);
                case AssetColumn.UnitaryPrice:
                    return x.Price.CompareTo(y.Price);
                case AssetColumn.TotalPrice:
                    return x.Cost.CompareTo(y.Cost);
                case AssetColumn.Volume:
                    return x.TotalVolume.CompareTo(y.TotalVolume);
                case AssetColumn.BlueprintType:
                    return string.Compare(x.TypeOfBlueprint, y.TypeOfBlueprint, StringComparison.CurrentCulture);
                case AssetColumn.Group:
                    return string.Compare(x.Item.GroupName, y.Item.GroupName, StringComparison.CurrentCulture);
                case AssetColumn.Category:
                    return string.Compare(x.Item.CategoryName, y.Item.CategoryName, StringComparison.CurrentCulture);
                case AssetColumn.Container:
                    return string.Compare(x.Container, y.Container, StringComparison.CurrentCulture);
                case AssetColumn.Flag:
                    return string.Compare(x.Flag, y.Flag, StringComparison.CurrentCulture);
                case AssetColumn.Location:
                    return string.Compare(x.Location, y.Location, StringComparison.CurrentCulture);
                case AssetColumn.FullLocation:
                    return x.SolarSystem.CompareTo(y.SolarSystem);
                case AssetColumn.Region:
                    return x.SolarSystem.Constellation.Region.CompareTo(y.SolarSystem.Constellation.Region);
                case AssetColumn.SolarSystem:
                    return x.SolarSystem.CompareTo(y.SolarSystem);
                case AssetColumn.Jumps:
                    return x.Jumps.CompareTo(y.Jumps);
                default:
                    return 0;
            }
        }
    }
}
