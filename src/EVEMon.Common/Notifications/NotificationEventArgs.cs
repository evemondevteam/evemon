using System;
using EVEMon.Common.Models;
using EVEMon.Common.Threading;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// Represents an argument for a notification invalidation.
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sender"></param>
        public NotificationEventArgs(Object sender, NotificationCategory category)
        {
            Sender = sender;
            Category = category;
        }

        /// <summary>
        /// Gets this category's notification.
        /// </summary>
        public NotificationCategory Category { get; }

        /// <summary>
        /// Gets the sender of this notification.
        /// </summary>
        public object Sender { get; }

        /// <summary>
        /// Gets the API key that sent this notification, or null if the sender was not an account.
        /// </summary>
        public ESIKey SenderAPIKey => Sender as ESIKey;

        /// <summary>
        /// Gets the character that sent this notification, or null if the sender was not a character.
        /// </summary>
        public Character SenderCharacter => Sender as Character;

        /// <summary>
        /// Gets the corporation that sent this notification, or null if the sender was not a corporation.
        /// </summary>
        public Corporation SenderCorporation => Sender as Corporation;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the priority for this notification.
        /// </summary>
        public NotificationPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the behaviour of this notification regarding other notifications with the same validation key.
        /// </summary>
        public NotificationBehaviour Behaviour { get; set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public virtual bool HasDetails => false;

        /// <summary>
        /// Gets a key, identifying a category/sender pair, that will be used for invalidation.
        /// </summary>
        public long InvalidationKey => GetKey(Sender, Category);

        /// <summary>
        /// Gets the key for the given sender and category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static long GetKey(Object sender, NotificationCategory category)
        {
            long left = (long)category << 32;
            int right = sender?.GetHashCode() ?? 0;
            return left | unchecked((uint)right);
        }

        /// <summary>
        /// Appends a given notification to this one.
        /// </summary>
        /// <param name="other"></param>
        public virtual void Append(NotificationEventArgs other)
        {
            // Must have to be implemented by inheritors
            throw new NotImplementedException();
        }

        /// <summary>
        /// Schedules an action on this notification.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="callback">The callback.</param>
        public static void ScheduleAction(TimeSpan time, Action callback)
        {
            Dispatcher.Schedule(time, callback.Invoke);
        }

        /// <summary>
        /// Gets the description for this notification.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Description;
    }
}