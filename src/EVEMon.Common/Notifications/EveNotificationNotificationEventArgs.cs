using System;

namespace EVEMon.Common.Notifications
{
    public sealed class EveNotificationEventArgs : NotificationEventArgs
    {
        private int m_newNotificationsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newNotifications">The new notifications.</param>
        public EveNotificationEventArgs(object sender, int newNotifications)
            : base(sender, NotificationCategory.NewEveNotification)
        {
            m_newNotificationsCount = newNotifications;
            UpdateDescription();
        }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => false;

        /// <summary>
        /// Adds the number of new mail messages from the given notification to this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            m_newNotificationsCount += ((EveNotificationEventArgs)other).m_newNotificationsCount;
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = $"{m_newNotificationsCount} new EVE notification{(m_newNotificationsCount > 1 ? "s" : string.Empty)}.";
        }
    }
}