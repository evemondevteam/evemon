using System;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableJobListItem
    {
        [XmlAttribute("jobID")]
        public long JobID { get; set; }

        [XmlAttribute("installerID")]
        public long InstallerID { get; set; }

        // Never referenced by EVEMon!
        [XmlAttribute("installerName")]
        public string InstallerName { get; set; }

        [XmlAttribute("facilityID")]
        public long FacilityID { get; set; }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlAttribute("solarSystemName")]
        public string SolarSystemName { get; set; }

        [XmlAttribute("stationID")]
        public long StationID { get; set; }

        [XmlAttribute("activityID")]
        public int ActivityID { get; set; }

        [XmlAttribute("blueprintID")]
        public long BlueprintID { get; set; }

        [XmlAttribute("blueprintTypeID")]
        public int BlueprintTypeID { get; set; }

        [XmlAttribute("blueprintTypeName")]
        public string BlueprintTypeName { get; set; }

        [XmlAttribute("blueprintLocationID")]
        public long BlueprintLocationID { get; set; }

        [XmlAttribute("outputLocationID")]
        public long OutputLocationID { get; set; }

        [XmlAttribute("runs")]
        public int Runs { get; set; }

        [XmlAttribute("cost")]
        public double Cost { get; set; }

        [XmlAttribute("teamID")]
        public long TeamID { get; set; }

        [XmlAttribute("licensedRuns")]
        public int LicensedRuns { get; set; }

        [XmlAttribute("probability")]
        public double Probability { get; set; }

        [XmlAttribute("productTypeID")]
        public int ProductTypeID { get; set; }

        [XmlAttribute("productTypeName")]
        public string ProductTypeName { get; set; }

        [XmlAttribute("status")]
        public int Status { get; set; }

        [XmlAttribute("timeInSeconds")]
        public int TimeInSeconds { get; set; }

        [XmlAttribute("completedCharacterID")]
        public long CompletedCharacterID { get; set; }

        [XmlAttribute("successfulRuns")]
        public int SuccessfulRuns { get; set; }

        [XmlAttribute("startDate")]
        public string StartDateXml
        {
            get { return StartDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    StartDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("endDate")]
        public string EndDateXml
        {
            get { return EndDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    EndDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("pauseDate")]
        public string PauseProductionTimeXml
        {
            get { return PauseDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    PauseDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("completedDate")]
        public string CompletedDateXml
        {
            get { return CompletedDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    CompletedDate = value.TimeStringToDateTime();
            }
        }

        /// <summary>
        /// The time this job was installed.
        /// </summary>
        [XmlIgnore]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The time this job will finish.
        /// </summary>
        [XmlIgnore]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The time this job was paused.
        /// </summary>
        [XmlIgnore]
        public DateTime PauseDate { get; set; }

        /// <summary>
        /// The time this job was completed.
        /// </summary>
        [XmlIgnore]
        public DateTime CompletedDate { get; set; }

        /// <summary>
        /// Which this job was issued for.
        /// </summary>
        [XmlIgnore]
        public IssuedFor IssuedFor { get; set; }
    }
}
