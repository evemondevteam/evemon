using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models.Collections
{
    /// <summary>
    /// A collection of industry jobs.
    /// </summary>
    public sealed class IndustryJobCollection : ReadonlyCollection<IndustryJob>
    {
        private readonly CCPCharacter m_ccpCharacter;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal IndustryJobCollection(CCPCharacter character)
        {
            m_ccpCharacter = character;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal void Dispose()
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            IQueryMonitor charIndustryJobsMonitor =
                m_ccpCharacter.QueryMonitors.Any(x => (ESIAPICharacterMethods)x.Method == ESIAPICharacterMethods.IndustryJobs)
                    ? m_ccpCharacter.QueryMonitors[ESIAPICharacterMethods.IndustryJobs]
                    : null;
            IQueryMonitor corpIndustryJobsMonitor =
                m_ccpCharacter.QueryMonitors.Any(
                    x => (ESIAPICorporationMethods)x.Method == ESIAPICorporationMethods.CorporationIndustryJobs)
                    ? m_ccpCharacter.QueryMonitors[ESIAPICorporationMethods.CorporationIndustryJobs]
                    : null;

            if ((charIndustryJobsMonitor == null || !charIndustryJobsMonitor.Enabled) &&
                (corpIndustryJobsMonitor == null || !corpIndustryJobsMonitor.Enabled))
            {
                return;
            }

            UpdateOnTimerTick();
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableJob> src)
        {
            Items.Clear();
            foreach (SerializableJob srcJob in src)
            {
                Items.Add(new IndustryJob(srcJob) { InstallerID = m_ccpCharacter.CharacterID });
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable jobs from the API.</param>
        internal void Import(IEnumerable<SerializableJobListItem> src)
        {
            // Mark all jobs for deletion 
            // If they are found again on the API feed, they won't be deleted
            // and those set as ignored will be left as ignored
            foreach (IndustryJob job in Items)
            {
                job.MarkedForDeletion = true;
            }

            // Import the jobs from the API
            List<IndustryJob> newJobs = src.Select(
                srcJob => new
                {
                    srcJob,
                    limit = srcJob.EndDate.AddDays(IndustryJob.MaxEndedDays),
                    state = srcJob.CompletedDate == DateTime.MinValue ? 0 : 1,
                    status = srcJob.Status
                }).Where(
                    job => job.limit >= DateTime.UtcNow ||
                           (job.state == (int)JobState.Active &&
                            job.status != (int)CCPJobCompletedStatus.Ready)).Where(
                                job => !Items.Any(x => x.TryImport(job.srcJob))).Select(
                                    job => new IndustryJob(job.srcJob)).Where(
                                        job => job.InstalledItem != null && job.OutputItem != null).ToList();

            // Add the items that are no longer marked for deletion
            newJobs.AddRange(Items.Where(x => !x.MarkedForDeletion));

            // Replace the old list with the new one
            Items.Clear();
            Items.AddRange(newJobs);
        }

        /// <summary>
        /// Exports only the character issued jobs to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used to export only the corporation jobs issued by the character.</remarks>
        internal IEnumerable<SerializableJob> ExportOnlyIssuedByCharacter()
            => Items.Where(job => job.InstallerID == m_ccpCharacter.CharacterID).Select(job => job.Export());

        /// <summary>
        /// Exports the jobs to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable jobs.</returns>
        /// <remarks>Used to export all jobs of the collection.</remarks>
        internal IEnumerable<SerializableJob> Export() => Items.Select(job => job.Export());

        /// <summary>
        /// Notify the user on a job completion.
        /// </summary>
        private void UpdateOnTimerTick()
        {
            // We exit if there are no jobs
            if (!Items.Any())
                return;

            // Add the not notified "Ready" jobs to the completed list
            List<IndustryJob> jobsCompleted = Items.Where(
                job => job.IsActive && job.TTC.Length == 0 && !job.NotificationSend).ToList();

            jobsCompleted.ForEach(job => job.NotificationSend = true);

            // We exit if no jobs have been completed
            if (!jobsCompleted.Any())
                return;

            // Sends a notification
            if (Items.All(job => job.IssuedFor == IssuedFor.Corporation))
            {
                // Fires the event regarding the corporation industry jobs completion, issued by the character
                List<IndustryJob> characterJobs =
                    jobsCompleted.Where(job => job.InstallerID == m_ccpCharacter.CharacterID).ToList();
                if (characterJobs.Any())
                    EveMonClient.OnCharacterIndustryJobsCompleted(m_ccpCharacter, characterJobs);

                // Fires the event regarding the corporation industry jobs completion
                EveMonClient.OnCorporationIndustryJobsCompleted(m_ccpCharacter, jobsCompleted);
            }
            else
            // Fires the event regarding the character's industry jobs completion
                EveMonClient.OnCharacterIndustryJobsCompleted(m_ccpCharacter, jobsCompleted);
        }
    }
}