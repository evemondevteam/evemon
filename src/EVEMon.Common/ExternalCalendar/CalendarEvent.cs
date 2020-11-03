using System;
using System.Collections;
using System.Threading.Tasks;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Common Appointment Class used for both instances of the calendar.
    /// </summary>
    public abstract class CalendarEvent
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEvent"/> class.
        /// </summary>
        protected CalendarEvent()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(1);
            Subject = string.Empty;
            ItemReminder = true;
            Minutes = 5;
            AlternateReminder = false;
            EarlyReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            LateReminder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0);
            Events = new ArrayList();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets Start Date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets End Date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment requires a reminder.
        /// </summary>
        public bool ItemReminder { get; set; }

        /// <summary>
        /// Gets or sets Minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment uses the Alternate Reminder.
        /// </summary>
        public bool AlternateReminder { get; set; }

        /// <summary>
        /// Gets or sets Early Reminder for alternate reminders.
        /// </summary>
        public DateTime EarlyReminder { get; set; }

        /// <summary>
        /// Gets or sets LateReminder for alternate reminders.
        /// </summary>
        public DateTime LateReminder { get; set; }

        /// <summary>
        /// Gets the number of events.
        /// </summary>
        public int ItemCount => Events.Count;

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        protected ArrayList Events { get; }

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Method to search for any existing event for this skill.
        /// </summary>
        internal abstract Task ReadEventsAsync();

        /// <summary>
        /// Method to get the relevant event item and populate the details.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an event is found, <c>false</c> otherwise.
        /// </returns>
        internal abstract bool GetEvent();

        /// <summary>
        /// Add a new appointment or Update the appropriate event in the calendar.
        /// </summary>
        /// <param name="eventExists">if set to <c>true</c> the event exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        internal abstract Task AddOrUpdateEventAsync(bool eventExists, int queuePosition, bool lastSkillInQueue);

        /// <summary>
        /// Delete the appropriate event.
        /// </summary>
        /// <param name="eventIndex">The index of the event.</param>
        internal abstract Task DeleteEventAsync(int eventIndex);

        #endregion


        #region Public Methods

        /// <summary>
        /// Figure out whether the appointment falls within the times set that the users will
        /// be away from the PC and return the times they have specified.
        /// </summary>
        /// <returns>
        /// The date and time of the relevant reminder.
        /// </returns>
        protected DateTime WorkOutAlternateReminders()
        {
            // See whether the appointment falls within the middle of the two, it it does, set the early reminder
            if ((StartDate >= EarlyReminder) && (StartDate <= LateReminder))
                return EarlyReminder;

            // If the appointment falls outside the two reminders: if the appointment is before the early reminder, set 
            // the late reminder to the day before....
            if (StartDate >= EarlyReminder)
                return LateReminder;

            DateTime lateRem = LateReminder;
            lateRem = lateRem.AddDays(-1);
            return lateRem;
        }

        #endregion
    }
}