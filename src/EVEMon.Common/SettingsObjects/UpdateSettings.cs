using System;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for the updates from CCP and others
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class UpdateSettings
    {
        private int m_updateFrequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSettings"/> class.
        /// </summary>
        public UpdateSettings()
        {
            CheckTimeOnStartup = true;
            CheckEVEMonVersion = true;
            HttpTimeout = 20;
            Periods = new ModifiedSerializableDictionary<string, UpdatePeriod>();
            IgnoreNetworkStatus = false;
            UpdateFrequency = 720;
        }

        /// <summary>
        /// When true, EVEMon will check its version from repo
        /// </summary>
        [XmlElement("checkEVEMonVersion")]
        public bool CheckEVEMonVersion { get; set; }

        /// <summary>
        /// When true, EVEMon will check its time from NIST
        /// </summary>
        [XmlElement("checkTimeOnStartup")]
        public bool CheckTimeOnStartup { get; set; }

        /// <summary>
        /// Gets or sets the latest upgrade version the user choose to reject.
        /// </summary>
        [XmlElement("mostRecentDeniedUpdgrade")]
        public string MostRecentDeniedUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the latest upgrade major version the user choose to reject.
        /// </summary>
        [XmlElement("mostRecentDeniedMajorUpdgrade")]
        public string MostRecentDeniedMajorUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the length of time between updates in minutes.
        /// </summary>
        /// <remarks>
        /// Hidden Setting. The value of this setting must be equal to or higher than 720 minutes, the default is 720 minutes (12 hours).
        /// </remarks>
        [XmlElement("updateFrequency")]
        public int UpdateFrequency
        {
            get { return m_updateFrequency < 720 ? 720 : m_updateFrequency; }
            set { m_updateFrequency = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP timeout.
        /// </summary>
        /// <value>The HTTP timeout.</value>
        [XmlElement("httpTimeout")]
        public int HttpTimeout { get; set; }

        /// <summary>
        /// Short circuit the check for network connectivity and try and connect anyway.
        /// </summary>
        /// <value><c>true</c> if [ignore network status]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Hidden setting, no UI. Used for the hand full of people using Wine/Darwine with a broken .NET Network Stack.
        /// </remarks>
        [XmlElement("ignoreNetworkStatus")]
        public bool IgnoreNetworkStatus { get; set; }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        /// <value>The periods.</value>
        [XmlElement("periods")]
        public ModifiedSerializableDictionary<string, UpdatePeriod> Periods { get; set; }
    }
}