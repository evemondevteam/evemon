using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Models.Comparers
{
    /// <summary>
    /// Performs a comparison between two <see cref="IndustryJob" /> types.
    /// </summary>
    public sealed class PlanetaryPinComparer : Comparer<PlanetaryPin>
    {
        private readonly PlanetaryColumn m_column;
        private readonly bool m_isAscending;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryPinComparer"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="isAscending">Is ascending flag.</param>
        public PlanetaryPinComparer(PlanetaryColumn column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="PlanetaryColony" /> type and returns a value
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
        public override int Compare(PlanetaryPin x, PlanetaryPin y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="PlanetaryColony" /> type and returns a value
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
        private int CompareCore(PlanetaryPin x, PlanetaryPin y)
        {
            switch (m_column)
            {
                case PlanetaryColumn.State:
                    return x.State.CompareTo(y.State);
                case PlanetaryColumn.TTC:
                    return x.ExpiryTime.CompareTo(y.ExpiryTime);
                case PlanetaryColumn.TypeName:
                    return string.Compare(x.TypeName, y.TypeName, StringComparison.CurrentCulture);
                case PlanetaryColumn.ContentTypeName:
                    return string.Compare(x.ContentTypeName, y.ContentTypeName, StringComparison.CurrentCulture);
                case PlanetaryColumn.InstallTime:
                    return x.InstallTime.CompareTo(y.InstallTime);
                case PlanetaryColumn.EndTime:
                    return x.ExpiryTime.CompareTo(y.ExpiryTime);
                case PlanetaryColumn.PlanetName:
                    return string.Compare(x.Colony.PlanetName, y.Colony.PlanetName, StringComparison.CurrentCulture);
                case PlanetaryColumn.PlanetTypeName:
                    return string.Compare(x.Colony.PlanetTypeName, y.Colony.PlanetTypeName, StringComparison.CurrentCulture);
                case PlanetaryColumn.SolarSystem:
                    return x.Colony.SolarSystem.CompareTo(y.Colony.SolarSystem);
                case PlanetaryColumn.Location:
                    return string.Compare(x.Colony.FullLocation, y.Colony.FullLocation, StringComparison.CurrentCulture);
                case PlanetaryColumn.Region:
                    return x.Colony.SolarSystem.Constellation.Region.CompareTo(y.Colony.SolarSystem.Constellation.Region);
                case PlanetaryColumn.Quantity:
                    return x.ContentQuantity.CompareTo(y.ContentQuantity);
                case PlanetaryColumn.QuantityPerCycle:
                    return x.QuantityPerCycle.CompareTo(y.QuantityPerCycle);
                case PlanetaryColumn.CycleTime:
                    return x.CycleTime.CompareTo(y.CycleTime);
                case PlanetaryColumn.Volume:
                    return x.ContentVolume.CompareTo(y.ContentVolume);
                case PlanetaryColumn.LinkedTo:
                    return string.Compare(string.Join(", ", x.LinkedTo.Select(z => z.TypeName).Distinct()),
                        string.Join(", ", y.LinkedTo.Select(z => z.TypeName).Distinct()), StringComparison.CurrentCulture);
                case PlanetaryColumn.RoutedTo:
                    return string.Compare(string.Join(", ", x.RoutedTo.Select(z => z.TypeName).Distinct()),
                        string.Join(", ", y.RoutedTo.Select(z => z.TypeName).Distinct()), StringComparison.CurrentCulture);
                case PlanetaryColumn.GroupName:
                    return string.Compare(x.GroupName, y.GroupName, StringComparison.CurrentCulture);
                default:
                    return 0;
            }
        }
    }
}
