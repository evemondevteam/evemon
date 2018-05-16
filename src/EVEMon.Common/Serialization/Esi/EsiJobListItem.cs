using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiJobListItem
    {
        private DateTime completedDate;
        private DateTime endDate;
        private DateTime pauseDate;
        private CCPJobCompletedStatus status;
        private DateTime startDate;

        public EsiJobListItem()
        {
            completedDate = DateTime.MinValue;
            endDate = DateTime.MaxValue;
            startDate = DateTime.MinValue;
            pauseDate = DateTime.MinValue;
            status = CCPJobCompletedStatus.Ready;
        }

        [DataMember(Name = "job_id")]
        public int JobID { get; set; }

        [DataMember(Name = "installer_id")]
        public long InstallerID { get; set; }

        [DataMember(Name = "facility_id")]
        public long FacilityID { get; set; }

        [DataMember(Name = "activity_id")]
        public int ActivityID { get; set; }

        [DataMember(Name = "blueprint_id")]
        public long BlueprintID { get; set; }

        [DataMember(Name = "blueprint_type_id")]
        public int BlueprintTypeID { get; set; }

        [DataMember(Name = "blueprint_location_id")]
        public long BlueprintLocationID { get; set; }

        [DataMember(Name = "output_location_id")]
        public long OutputLocationID { get; set; }

        [DataMember(Name = "runs")]
        public int Runs { get; set; }

        [DataMember(Name = "cost")]
        public double Cost { get; set; }

        [DataMember(Name = "licensed_runs")]
        public int LicensedRuns { get; set; }

        [DataMember(Name = "probability")]
        public double Probability { get; set; }

        [DataMember(Name = "product_type_id")]
        public int ProductTypeID { get; set; }

        // One of: active, cancelled, delivered, paused, ready, reverted
        [DataMember(Name = "status")]
        private string StatusJson
        {
            get
            {
                return status.ToString().ToLower();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Enum.TryParse(value, true, out status);
            }
        }

        [DataMember(Name = "duration")]
        public int TimeInSeconds { get; set; }

        [DataMember(Name = "completed_character_id")]
        public long CompletedCharacterID { get; set; }

        [DataMember(Name = "successful_runs")]
        public int SuccessfulRuns { get; set; }

        [DataMember(Name = "start_date")]
        private string StartDateJson
        {
            get { return startDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    startDate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "end_date")]
        private string EndDateJson
        {
            get { return endDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    endDate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "pause_date")]
        private string PauseProductionTimeJson
        {
            get { return pauseDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    pauseDate = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "completed_date")]
        private string CompletedDateJson
        {
            get { return completedDate.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    completedDate = value.TimeStringToDateTime();
            }
        }

        /// <summary>
        /// The time this job was installed.
        /// </summary>
        [IgnoreDataMember]
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
        }

        /// <summary>
        /// The time this job will finish.
        /// </summary>
        [IgnoreDataMember]
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
        }

        /// <summary>
        /// The time this job was paused.
        /// </summary>
        [IgnoreDataMember]
        public DateTime PauseDate
        {
            get
            {
                return pauseDate;
            }
        }

        /// <summary>
        /// The time this job was completed.
        /// </summary>
        [IgnoreDataMember]
        public DateTime CompletedDate
        {
            get
            {
                return completedDate;
            }
        }

        [IgnoreDataMember]
        public CCPJobCompletedStatus Status
        {
            get
            {
                return status;
            }
        }

        public SerializableJobListItem ToXMLItem()
        {
            return new SerializableJobListItem()
            {
                ActivityID = ActivityID,
                BlueprintID = BlueprintID,
                BlueprintLocationID = BlueprintLocationID,
                BlueprintTypeID = BlueprintTypeID,
                BlueprintTypeName = StaticItems.GetItemName(BlueprintTypeID),
                CompletedCharacterID = CompletedCharacterID,
                CompletedDate = CompletedDate,
                Cost = Cost,
                EndDate = EndDate,
                FacilityID = FacilityID,
                InstallerID = InstallerID,
                JobID = JobID,
                LicensedRuns = LicensedRuns,
                OutputLocationID = OutputLocationID,
                PauseDate = PauseDate,
                Probability = Probability,
                ProductTypeID = ProductTypeID,
                ProductTypeName = StaticItems.GetItemName(ProductTypeID),
                Runs = Runs,
                StartDate = StartDate,
                StationID = BlueprintLocationID,
                Status = (int)Status,
                SuccessfulRuns = SuccessfulRuns,
                TimeInSeconds = TimeInSeconds
            };
        }
    }
}
