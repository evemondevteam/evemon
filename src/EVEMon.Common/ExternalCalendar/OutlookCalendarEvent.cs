using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Helpers;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;

namespace EVEMon.Common.ExternalCalendar
{
    /// <summary>
    /// Class for handling MS Outlook calendar.
    /// </summary>
    public sealed class OutlookCalendarEvent : CalendarEvent
    {
        private static Application s_outlookApplication;
        private static MAPIFolder s_mapiFolder;


        #region Properties

        /// <summary>
        /// Gets the Outlook application.
        /// </summary>
        /// <value>The outlook application.</value>
        internal static Application OutlookApplication
        {
            get
            {
                try
                {
                    if (s_outlookApplication == null)
                    {
                        s_outlookApplication = new Application();
                        s_outlookApplication.QuitEvent += Outlook_Quit;
                    }
                }
                catch (COMException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                }
                catch (ArgumentException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                }
                return s_outlookApplication;
            }
        }

        #endregion


        #region Event Hanlders

        /// <summary>
        /// Occurs when Outlook quits.
        /// </summary>
        private static void Outlook_Quit()
        {
            s_outlookApplication.QuitEvent -= Outlook_Quit;
            s_outlookApplication.Dispose();
            s_outlookApplication = null;
            s_mapiFolder = null;
        }


        #endregion


        #region Internal Methods

        /// <summary>
        /// Add a new event or Update the appropriate event in the calendar.
        /// </summary>
        /// <param name="eventExists">if set to <c>true</c> the event exists.</param>
        /// <param name="queuePosition">The queue position.</param>
        /// <param name="lastSkillInQueue">if set to <c>true</c> skill is the last in queue.</param>
        internal override Task AddOrUpdateEventAsync(bool eventExists, int queuePosition, bool lastSkillInQueue)
            => TaskHelper.RunIOBoundTaskAsync(() =>
            {
                AppointmentItem eventItem = eventExists
                    ? (AppointmentItem)Events[0]
                    : (AppointmentItem)s_mapiFolder.Items.Add(OlItemType.olAppointmentItem);

                eventItem.Subject = Subject;
                eventItem.Start = StartDate;
                eventItem.End = EndDate;

                string queuePositionText = lastSkillInQueue
                    ? "End Of Queue"
                    : queuePosition.ToString(CultureConstants.DefaultCulture);

                eventItem.Body = eventExists
                    ? $"{eventItem.Body} {Environment.NewLine}Updated: {DateTime.Now} Queue Position: {queuePositionText}"
                    : $"Added: {DateTime.Now} Queue Position: {queuePositionText}";

                eventItem.ReminderSet = ItemReminder || AlternateReminder;
                eventItem.BusyStatus = OlBusyStatus.olBusy;
                eventItem.AllDayEvent = false;
                eventItem.Location = string.Empty;

                if (AlternateReminder)
                {
                    EarlyReminder = new DateTime(StartDate.Year,
                        StartDate.Month,
                        StartDate.Day,
                        EarlyReminder.Hour,
                        EarlyReminder.Minute,
                        EarlyReminder.Second);

                    LateReminder = new DateTime(StartDate.Year,
                        StartDate.Month,
                        StartDate.Day,
                        LateReminder.Hour,
                        LateReminder.Minute,
                        LateReminder.Second);

                    DateTime dateTimeAlternateReminder = WorkOutAlternateReminders();

                    // Subtract the reminder time from the event time
                    TimeSpan timeSpan = eventItem.Start.Subtract(dateTimeAlternateReminder);
                    eventItem.ReminderMinutesBeforeStart = Math.Abs(timeSpan.Hours * 60 + timeSpan.Minutes);
                    Minutes = eventItem.ReminderMinutesBeforeStart;
                }

                eventItem.ReminderMinutesBeforeStart = eventItem.ReminderSet ? Minutes : 0;
                eventItem.Save();
            });

        /// <summary>
        /// Get the relevant event item and populate the details.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if an event is found, <c>false</c> otherwise.
        /// </returns>
        internal override bool GetEvent()
        {
            if (Events.Count < 1)
                return false;

            AppointmentItem eventItem = (AppointmentItem)Events[0];
            StartDate = eventItem.Start;
            EndDate = eventItem.End;
            Subject = eventItem.Subject;
            ItemReminder = eventItem.ReminderSet;
            Minutes = eventItem.ReminderMinutesBeforeStart;
            return true;
        }

        /// <summary>
        /// Pull all the events and populate the event array.
        /// </summary>
        internal override Task ReadEventsAsync()
            => TaskHelper.RunIOBoundTaskAsync(() =>
            {
                Events.Clear();
                Events.AddRange(GetEventItems());
            });

        /// <summary>
        /// Delete the specified event.
        /// </summary>
        /// <param name="eventIndex">The event index.</param>
        internal override Task DeleteEventAsync(int eventIndex)
            => TaskHelper.RunIOBoundTaskAsync(() =>
            {
                ((AppointmentItem)Events[eventIndex]).Delete();
            });

        /// <summary>
        /// Gets true if the Outlook calendar exist.
        /// </summary>
        /// <param name="useDefaultCalendar">if set to <c>true</c> [use default calendar].</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static bool OutlookCalendarExist(bool useDefaultCalendar, string path = null)
        {
            s_mapiFolder = null;
            return GetMapiFolder(OutlookApplication.Session.Folders, useDefaultCalendar, path);
        }

        /// <summary>
        /// Gets the mapi folder.
        /// </summary>
        /// <param name="folders">The folders.</param>
        /// <param name="useDefaultCalendar">if set to <c>true</c> [use default calendar].</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static bool GetMapiFolder(IEnumerable folders, bool useDefaultCalendar = false, string path = null)
        {
            if (useDefaultCalendar)
            {
                s_mapiFolder = OutlookApplication.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);
                return s_mapiFolder != null;
            }

            if (string.IsNullOrWhiteSpace(path))
                path = Settings.Calendar.OutlookCustomCalendarPath;

            if (!path.StartsWith(@"\\", StringComparison.Ordinal))
                return s_mapiFolder != null;

            string pathRoot = GetFolderPathRoot(path);

            foreach (MAPIFolder folder in folders.Cast<MAPIFolder>().TakeWhile(
                folder => s_mapiFolder == null).Select(
                    folder => new { folder, folderRoot = GetFolderPathRoot(folder.FolderPath) }).Where(
                        folder => folder.folderRoot == pathRoot).Select(folder => folder.folder))
            {
                if (folder.DefaultItemType == OlItemType.olAppointmentItem && folder.FolderPath == path)
                {
                    s_mapiFolder = folder;
                    break;
                }

                if (folder.Folders.Any())
                    GetMapiFolder(folder.Folders, path: path);
            }

            return s_mapiFolder != null && s_mapiFolder.FolderPath == path;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get events matching the subject.
        /// </summary>
        /// <returns>
        /// Array of relevant events or empty array.
        /// </returns>
        private ArrayList GetEventItems()
        {
            // Use a Jet Query to filter the details we need initially between the two specified dates
            string dateFilter = $"[Start] >= '{StartDate:g}' and [End] <= '{EndDate:g}'";
            _Items calendarItems = s_mapiFolder.Items.Restrict(dateFilter);
            calendarItems.Sort("[Start]", Type.Missing);
            calendarItems.IncludeRecurrences = true;

            // Must use 'like' comparison for Find/FindNext
            string subjectFilter = !string.IsNullOrEmpty(Subject)
                ? $"@SQL=\"urn:schemas:httpmail:subject\" like '%{Subject.Replace("'", "''")}%'"
                : "@SQL=\"urn:schemas:httpmail:subject\" <> '!@#'";

            // Use Find and FindNext methods to get all the items
            ArrayList resultArray = new ArrayList();
            AppointmentItem eventItem = calendarItems.Find(subjectFilter) as AppointmentItem;
            while (eventItem != null)
            {
                resultArray.Add(eventItem);

                // Find the next event
                eventItem = calendarItems.FindNext() as AppointmentItem;
            }

            return resultArray;
        }

        /// <summary>
        /// Gets the folder path root.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        private static string GetFolderPathRoot(string folderPath)
        {
            // Strip header directory seperator characters
            folderPath = folderPath.Remove(0, 2);

            // Find the index of a directory seperator character
            int index = folderPath.IndexOf(Path.DirectorySeparatorChar, 0);

            // Reconstruct the root path according to the index found
            return $"\\{(index > 0 ? folderPath.Substring(0, index) : folderPath)}";
        }

        #endregion

    }
}