using System;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// A class for the arguments of notifications invalidation.
    /// </summary>
    public sealed class NotificationInvalidationEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor with a key identifying a sender/category pair.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="category"></param>
        public NotificationInvalidationEventArgs(object sender, NotificationCategory category)
        {
            Key = NotificationEventArgs.GetKey(sender, category);
        }

        /// <summary>
        /// Constructor with a key gotten from a notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        public NotificationInvalidationEventArgs(NotificationEventArgs notification)
        {
            notification.ThrowIfNull(nameof(notification));

            Key = notification.InvalidationKey;
        }

        /// <summary>
        /// Gets a key identifying the notifications to invalidate.
        /// </summary>
        public long Key { get; private set; }
    }
}