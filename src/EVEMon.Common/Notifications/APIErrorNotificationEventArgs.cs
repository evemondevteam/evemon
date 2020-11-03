using System;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Notifications
{
    public sealed class APIErrorNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        public APIErrorNotificationEventArgs(object sender, IAPIResult result)
            : base(sender, NotificationCategory.QueryingError)
        {
            Result = result;
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public IAPIResult Result { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;
    }
}