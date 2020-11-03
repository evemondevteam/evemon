using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Constants;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Scheduling
{
    /// <summary>
    /// Holds the scheduling entries
    /// </summary>
    [EnforceUIThreadAffinity]
    public static class Scheduler
    {
        private static readonly List<ScheduleEntry> s_schedule = new List<ScheduleEntry>();

        /// <summary>
        /// Gets the scheduled entries
        /// </summary>
        public static IEnumerable<ScheduleEntry> Entries => s_schedule.Where(entry => !entry.Expired);

        /// <summary>
        /// Checks whether EVEMon is currently running in silent mode (no tooltips nor sounds).
        /// </summary>
        /// <returns></returns>
        public static bool SilentMode => s_schedule.Any(x => x.Silent(DateTime.Now));

        /// <summary>
        /// Add the given entry
        /// </summary>
        /// <param name="entry"></param>
        public static void Add(ScheduleEntry entry)
        {
            s_schedule.Add(entry);

            // Notify to subscribers
            EveMonClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Add the given entry
        /// </summary>
        /// <param name="entry"></param>
        public static void Remove(ScheduleEntry entry)
        {
            s_schedule.Remove(entry);

            // Notify to subscribers
            EveMonClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Checks whether a certain datetime will fall on a time where the user won't be able to log in.
        /// </summary>
        /// <remarks>Checks both scheduling entries and downtimes.</remarks>
        /// <param name="time"></param>
        /// <param name="blockingEntry"></param>
        /// <param name="isAutoBlocking"></param>
        /// <returns></returns>
        public static bool SkillIsBlockedAt(DateTime time, out string blockingEntry, out bool isAutoBlocking)
        {
            blockingEntry = string.Empty;
            isAutoBlocking = false;

            // Checks schedule entries to see if they overlap the input time
            foreach (ScheduleEntry entry in s_schedule.Where(entry => entry.Blocking(time)))
            {
                blockingEntry = entry.Title;
                return true;
            }

            // Checks whether it will be on downtime
            if (time.ToUniversalTime().Hour != EveConstants.DowntimeHour ||
                time.ToUniversalTime().Minute >= EveConstants.DowntimeDuration)
            {
                return false;
            }

            blockingEntry = EveMonConstants.DowntimeText;
            isAutoBlocking = true;
            return true;
        }

        /// <summary>
        /// Imports data from the given serialization object.
        /// </summary>
        /// <param name="serial"></param>
        internal static void Import(SchedulerSettings serial)
        {
            s_schedule.Clear();
            foreach (SerializableScheduleEntry serialEntry in serial.Entries)
            {
                SerializableRecurringScheduleEntry serialReccuringEntry= serialEntry as SerializableRecurringScheduleEntry;
                if (serialReccuringEntry != null)
                    s_schedule.Add(new RecurringScheduleEntry(serialReccuringEntry));
                else
                    s_schedule.Add(new SimpleScheduleEntry(serialEntry));
            }

            // Notify to subscribers
            EveMonClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Exports data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal static SchedulerSettings Export()
        {
            SchedulerSettings serial = new SchedulerSettings();
            foreach (ScheduleEntry entry in s_schedule.Where(entry => !entry.Expired))
            {
                serial.Entries.Add(entry.Export());
            }
            return serial;
        }

        /// <summary>
        /// Clears all the expired entries.
        /// </summary>
        public static void ClearExpired()
        {
            // Removed the expired entries
            int i = 0;
            while (i < s_schedule.Count)
            {
                ScheduleEntry entry = s_schedule[i];
                if (entry.Expired)
                    s_schedule.RemoveAt(i);
                else
                    i++;
            }

            // Notify to subscribers
            EveMonClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Tries the parse time.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        public static bool TryParseTime(string text, out int seconds)
        {
            try
            {
                DateTimeFormatInfo dtfi = CultureConstants.DefaultCulture.DateTimeFormat;
                DateTime res = DateTime.ParseExact(text, dtfi.ShortTimePattern, dtfi);
                seconds = Convert.ToInt32(Math.Round(res.Subtract(DateTime.Today).TotalSeconds));
                return true;
            }
            catch (FormatException)
            {
                seconds = 0;
                return false;
            }
        }
    }
}