using System;

namespace EVEMon.Common.Notifications
{
    public sealed class EveMailMessageNotificationEventArgs : NotificationEventArgs
    {
        private int m_newMailMessagesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailMessageNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newMessages">The new messages.</param>
        public EveMailMessageNotificationEventArgs(object sender, int newMessages)
            : base(sender, NotificationCategory.NewEveMailMessage)
        {
            m_newMailMessagesCount = newMessages;
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
            m_newMailMessagesCount += ((EveMailMessageNotificationEventArgs)other).m_newMailMessagesCount;
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = $"{m_newMailMessagesCount} new EVE mail message{(m_newMailMessagesCount > 1 ? "s" : string.Empty)}.";
        }
    }
}