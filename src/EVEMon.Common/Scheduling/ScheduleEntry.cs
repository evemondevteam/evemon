using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Scheduling
{
    /// <summary>
    /// Represents a schedule entry.
    /// </summary>
    public abstract class ScheduleEntry
    {
        public const int TitleMaxLength = 56;

        protected ScheduleEntry()
        {
            Title = string.Empty;
            Options = ScheduleEntryOptions.None;
            EndDate = DateTime.MinValue;
            StartDate = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the start date for the validity of this schedule entry
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the validity of this schedule entry
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets this entry's options
        /// </summary>
        public ScheduleEntryOptions Options { get; set; }

        /// <summary>
        /// Gets or sets this entry's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets true if it is expired.
        /// </summary>
        public bool Expired => DateTime.UtcNow > EndDate.ToUniversalTime();

        /// <summary>
        /// Checks whether this entry forces EVEMon to run in silent mode (no tray tooltips nor sounds).
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        public bool Silent(DateTime timeToTest) => (Options & ScheduleEntryOptions.Quiet) != 0 && Clash(timeToTest);

        /// <summary>
        /// Checks whether this entry intersects with a blocking option with the given time while its options.
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        public bool Blocking(DateTime timeToTest) => (Options & ScheduleEntryOptions.Blocking) != 0 && Clash(timeToTest);

        /// <summary>
        /// Checks whether the given time is contained within this entry
        /// </summary>
        /// <param name="checkDateTime"></param>
        /// <returns></returns>
        public abstract bool Contains(DateTime checkDateTime);

        /// <summary>
        /// Gets an enumeration of ranges for this entry within the given interval.
        /// </summary>
        /// <param name="fromDt"></param>
        /// <param name="toDt"></param>
        /// <returns></returns>
        public abstract IEnumerable<ScheduleDateTimeRange> GetRangesInPeriod(DateTime fromDt, DateTime toDt);

        /// <summary>
        /// Checks whether the given time intersects with this entry.
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        protected abstract bool Clash(DateTime timeToTest);

        /// <summary>
        /// Checks whether this entry occurs on the given day.
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        public abstract bool IsToday(DateTime timeToTest);

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal abstract SerializableScheduleEntry Export();

        /// <summary>
        /// Gets the entry's title.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Title;
    }
}