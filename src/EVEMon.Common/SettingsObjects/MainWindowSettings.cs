using System.Xml.Serialization;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MainWindowSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowSettings"/> class.
        /// </summary>
        public MainWindowSettings()
        {
            ShowOverview = true;
            ShowMenuBar = true;
            CombatLog = new CombatLogSettings();
            Assets = new AssetSettings();
            WalletJournal = new WalletJournalSettings();
            WalletTransactions = new WalletTransactionSettings();
            MarketOrders = new MarketOrderSettings();
            Contracts = new ContractSettings();
            IndustryJobs = new IndustryJobSettings();
            Planetary = new PlanetarySettings();
            Research = new ResearchSettings();
            EVEMailMessages = new EveMailMessageSettings();
            EVENotifications = new EveNotificationSettings();
            OverviewItemSize = PortraitSizes.x96;
            TitleFormat = MainWindowTitleFormat.NextCharToFinish;
            ShowOverviewSkillQueueTrainingTime = true;
            ShowOverviewPortrait = true;
            ShowOverviewWallet = true;
            ShowOverviewTotalSkillpoints = true;
            PutTrainingSkillsFirstOnOverview = true;
            SkillQueueWarningThresholdDays = 1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show character info in title bar].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show character info in title bar]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showCharacterInfoInTitleBar")]
        public bool ShowCharacterInfoInTitleBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview].
        /// </summary>
        /// <value><c>true</c> if [show overview]; otherwise, <c>false</c>.</value>
        [XmlElement("showOverview")]
        public bool ShowOverview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show menu bar].
        /// </summary>
        /// <value><c>true</c> if [show menu bar]; otherwise, <c>false</c>.</value>
        [XmlElement("showMenuBar")]
        public bool ShowMenuBar { get; set; }

        /// <summary>
        /// Gets or sets the title format.
        /// </summary>
        /// <value>The title format.</value>
        [XmlElement("titleFormat")]
        public MainWindowTitleFormat TitleFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show skill name in window title].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show skill name in window title]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showSkillNameInWindowTitle")]
        public bool ShowSkillNameInWindowTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show all public skills].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show all public skills]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showAllPublicSkills")]
        public bool ShowAllPublicSkills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show non public skills].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show non public skills]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showNonPublicSkills")]
        public bool ShowNonPublicSkills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show prereq met skills].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show prereq met skills]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showPrereqMetSkills")]
        public bool ShowPrereqMetSkills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [highlight partial skills].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [highlight partial skills]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("highlightPartialSkills")]
        public bool HighlightPartialSkills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [highlight queued skills].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [highlight queued skills]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("highlightQueuedSkills")]
        public bool HighlightQueuedSkills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always show skill queue time].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [always show skill queue time]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("alwaysShowSkillQueueTime")]
        public bool AlwaysShowSkillQueueTime { get; set; }
        
        /// <summary>
        /// Gets or sets the skill queue warning threshold days.
        /// </summary>
        /// <value>
        /// The skill queue warning threshold days.
        /// </value>
        [XmlElement("skillQueueWarningThresholdDays")]
        public int SkillQueueWarningThresholdDays { get; set; }

        /// <summary>
        /// Gets or sets the index of the overview.
        /// </summary>
        /// <value>The index of the overview.</value>
        [XmlElement("overviewIndex")]
        public int OverviewIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the overview item.
        /// </summary>
        /// <value>The size of the overview item.</value>
        [XmlElement("overviewItemSize")]
        public PortraitSizes OverviewItemSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview wallet].
        /// </summary>
        /// <value><c>true</c> if [show overview wallet]; otherwise, <c>false</c>.</value>
        [XmlElement("showWalletOnOverview")]
        public bool ShowOverviewWallet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview skillpoints].
        /// </summary>
        /// <value><c>true</c> if [show overview skillpoints]; otherwise, <c>false</c>.</value>
        [XmlElement("showSkillpointsOnOverview")]
        public bool ShowOverviewTotalSkillpoints { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview portrait].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show overview portrait]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showPortraitOnOverview")]
        public bool ShowOverviewPortrait { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview skill queue training time].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show overview skill queue training time]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showOverviewSkillQueueTrainingTime")]
        public bool ShowOverviewSkillQueueTrainingTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview location].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show overview location]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showOverviewLocation")]
        public bool ShowOverviewLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show overview jobs]. If both this
        /// value and ShowOverviewLocation are <c>true</c>, then ShowOverviewLocation will
        /// take precedence.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show overview jobs]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showOverviewJobs")]
        public bool ShowOverviewJobs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [put training skills first on overview].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [put training skills first on overview]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("putTrainingSkillsFirstOnOverview")]
        public bool PutTrainingSkillsFirstOnOverview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use increased contrast on overview].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use increased contrast on overview]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("useIncreasedContrastOnOverview")]
        public bool UseIncreasedContrastOnOverview { get; set; }

        /// <summary>
        /// Gets or sets the combat log.
        /// </summary>
        /// <value>The combat log.</value>
        [XmlElement("combatLog")]
        public CombatLogSettings CombatLog { get; set; }

        /// <summary>
        /// Gets or sets the assets.
        /// </summary>
        /// <value>The assets.</value>
        [XmlElement("assets")]
        public AssetSettings Assets { get; set; }

        /// <summary>
        /// Gets or sets the market orders.
        /// </summary>
        /// <value>The market orders.</value>
        [XmlElement("marketOrders")]
        public MarketOrderSettings MarketOrders { get; set; }

        /// <summary>
        /// Gets or sets the contracts.
        /// </summary>
        /// <value>The contracts.</value>
        [XmlElement("contracts")]
        public ContractSettings Contracts { get; set; }

        /// <summary>
        /// Gets or sets the wallet journal.
        /// </summary>
        /// <value>The wallet journal.</value>
        [XmlElement("walletJournal")]
        public WalletJournalSettings WalletJournal { get; set; }

        /// <summary>
        /// Gets or sets the wallet transactions.
        /// </summary>
        /// <value>The wallet transactions.</value>
        [XmlElement("walletTransactions")]
        public WalletTransactionSettings WalletTransactions { get; set; }

        /// <summary>
        /// Gets or sets the industry jobs.
        /// </summary>
        /// <value>The industry jobs.</value>
        [XmlElement("industryJobs")]
        public IndustryJobSettings IndustryJobs { get; set; }

        /// <summary>
        /// Gets or sets the planetary.
        /// </summary>
        [XmlElement("planetary")]
        public PlanetarySettings Planetary { get; set; }

        /// <summary>
        /// Gets or sets the research.
        /// </summary>
        /// <value>The research.</value>
        [XmlElement("research")]
        public ResearchSettings Research { get; set; }

        /// <summary>
        /// Gets or sets the EVE mail messages.
        /// </summary>
        /// <value>The EVE mail messages.</value>
        [XmlElement("eveMailMessages")]
        public EveMailMessageSettings EVEMailMessages { get; set; }

        /// <summary>
        /// Gets or sets the EVE notifications.
        /// </summary>
        /// <value>The EVE notifications.</value>
        [XmlElement("eveNotifications")]
        public EveNotificationSettings EVENotifications { get; set; }
    }
}
