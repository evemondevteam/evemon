using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Notifications
{
    public sealed class MarketOrdersNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrdersNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orders">The orders.</param>
        /// <exception cref="System.ArgumentNullException">orders</exception>
        public MarketOrdersNotificationEventArgs(object sender, IEnumerable<MarketOrder> orders)
            : base(sender, NotificationCategory.MarketOrdersEnding)
        {
            orders.ThrowIfNull(nameof(orders));

            Orders = new Collection<MarketOrder>();
            foreach (MarketOrder order in orders)
            {
                Orders.Add(order);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<MarketOrder> Orders { get; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;

        /// <summary>
        /// Enqueue the orders from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            foreach (MarketOrder order in ((MarketOrdersNotificationEventArgs)other).Orders)
            {
                Orders.Add(order);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = $"{Orders.Count} market order{(Orders.Count > 1 ? "s" : string.Empty)} expired or fulfilled.";
        }
    }
}