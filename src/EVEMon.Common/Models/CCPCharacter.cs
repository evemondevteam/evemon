using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models.Collections;
using EVEMon.Common.Models.Extended;
using EVEMon.Common.QueryMonitor;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a character from CCP, with additional capacities for training and such.
    /// </summary>
    public sealed class CCPCharacter : Character
    {
        #region Fields

        private readonly List<MarketOrder> m_endedOrdersForCharacter;
        private readonly List<MarketOrder> m_endedOrdersForCorporation;
        private readonly List<Contract> m_endedContractsForCharacter;
        private readonly List<Contract> m_endedContractsForCorporation;
        private readonly List<IndustryJob> m_jobsCompletedForCharacter;

        private CharacterDataQuerying m_characterDataQuerying;
        private CorporationDataQuerying m_corporationDataQuerying;
        private List<SerializableAPIUpdate> m_lastAPIUpdates;

        private Enum m_errorNotifiedMethod = CCPAPIMethodsEnum.None;
        private bool m_isFwEnlisted;

        #endregion


        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="guid"></param>
        private CCPCharacter(CharacterIdentity identity, Guid guid)
            : base(identity, guid)
        {
            QueryMonitors = new QueryMonitorCollection();
            SkillQueue = new SkillQueue(this);
            Standings = new StandingCollection(this);
            Assets = new AssetCollection(this);
            WalletJournal = new WalletJournalCollection(this);
            WalletTransactions = new WalletTransactionsCollection(this);
            CharacterMarketOrders = new MarketOrderCollection(this);
            CorporationMarketOrders = new MarketOrderCollection(this);
            CharacterContracts = new ContractCollection(this);
            CorporationContracts = new ContractCollection(this);
            CharacterContractBids = new ContractBidCollection(this);
            CorporationContractBids = new ContractBidCollection(this);
            CharacterIndustryJobs = new IndustryJobCollection(this);
            CorporationIndustryJobs = new IndustryJobCollection(this);
            ResearchPoints = new ResearchPointCollection(this);
            EVEMailMessages = new EveMailMessageCollection(this);
            EVEMailingLists = new EveMailingListCollection(this);
            EVENotifications = new EveNotificationCollection(this);
            Contacts = new ContactCollection(this);
            CharacterMedals = new MedalCollection(this);
            CorporationMedals = new MedalCollection(this);
            UpcomingCalendarEvents = new UpcomingCalendarEventCollection(this);
            KillLog = new KillLogCollection(this);
            PlanetaryColonies = new PlanetaryColonyCollection(this);

            m_endedOrdersForCharacter = new List<MarketOrder>();
            m_endedOrdersForCorporation = new List<MarketOrder>();

            m_endedContractsForCharacter = new List<Contract>();
            m_endedContractsForCorporation = new List<Contract>();

            m_jobsCompletedForCharacter = new List<IndustryJob>();

            EveMonClient.CharacterAssetsUpdated += EveMonClient_CharacterAssetsUpdated;
            EveMonClient.CharacterMarketOrdersUpdated += EveMonClient_CharacterMarketOrdersUpdated;
            EveMonClient.CorporationMarketOrdersUpdated += EveMonClient_CorporationMarketOrdersUpdated;
            EveMonClient.CharacterContractsUpdated += EveMonClient_CharacterContractsUpdated;
            EveMonClient.CorporationContractsUpdated += EveMonClient_CorporationContractsUpdated;
            EveMonClient.CharacterIndustryJobsUpdated += EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CorporationIndustryJobsUpdated += EveMonClient_CorporationIndustryJobsUpdated;
            EveMonClient.CharacterIndustryJobsCompleted += EveMonClient_CharacterIndustryJobsCompleted;
            EveMonClient.CorporationIndustryJobsCompleted += EveMonClient_CorporationIndustryJobsCompleted;
            EveMonClient.CharacterPlaneteryPinsCompleted += EveMonClient_CharacterPlaneteryPinsCompleted;
            EveMonClient.APIKeyInfoUpdated += EveMonClient_APIKeyInfoUpdated;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="identity">The identity for this character</param>
        /// <param name="serial">A deserialization object for characters</param>
        internal CCPCharacter(CharacterIdentity identity, SerializableCCPCharacter serial)
            : this(identity, serial.Guid)
        {
            Import(serial);
            m_lastAPIUpdates = serial.LastUpdates.ToList();
        }

        /// <summary>
        /// Constructor for a new CCP character on a new identity.
        /// </summary>
        /// <param name="identity"></param>
        internal CCPCharacter(CharacterIdentity identity)
            : this(identity, Guid.NewGuid())
        {
            ForceUpdateBasicFeatures = true;
            m_lastAPIUpdates = new List<SerializableAPIUpdate>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an adorned name, with (file), (url) or (cached) labels.
        /// </summary>
        public override string AdornedName
            => !Identity.APIKeys.Any() || Identity.APIKeys.All(apiKey => !apiKey.Monitored) ||
               (m_characterDataQuerying != null && m_characterDataQuerying.CharacterSheetMonitor.HasError)
                ? $"{Name} (cached)"
                : Name;

        /// <summary>
        /// Gets the skill queue for this character.
        /// </summary>
        public SkillQueue SkillQueue { get; }

        /// <summary>
        /// Gets the standings for this character.
        /// </summary>
        public StandingCollection Standings { get; }

        /// <summary>
        /// Gets the assets for this character.
        /// </summary>
        public AssetCollection Assets { get; }

        /// <summary>
        /// Gets the factional warfare stats for this character.
        /// </summary>
        public FactionalWarfareStats FactionalWarfareStats { get; internal set; }

        /// <summary>
        /// Gets the wallet journal for this character.
        /// </summary>
        public WalletJournalCollection WalletJournal { get; }

        /// <summary>
        /// Gets the wallet transactions for this character.
        /// </summary>
        public WalletTransactionsCollection WalletTransactions { get; }

        /// <summary>
        /// Gets the collection of market orders.
        /// </summary>
        public IEnumerable<MarketOrder> MarketOrders
            => CharacterMarketOrders
                .Concat(CorporationMarketOrders.Where(order => order.OwnerID == CharacterID));

        /// <summary>
        /// Gets or sets the character market orders.
        /// </summary>
        /// <value>The character market orders.</value>
        public MarketOrderCollection CharacterMarketOrders { get; }

        /// <summary>
        /// Gets or sets the corporation market orders.
        /// </summary>
        /// <value>The corporation market orders.</value>
        public MarketOrderCollection CorporationMarketOrders { get; }

        /// <summary>
        /// Gets the collection of contracts.
        /// </summary>
        /// <remarks>Excludes contracts that appear in both collections</remarks>
        public IEnumerable<Contract> Contracts
            => CharacterContracts
                .Where(charContract => CorporationContracts
                    .All(corpContract => corpContract.ID != charContract.ID))
                .Concat(CorporationContracts
                    .Where(contract => contract.IssuerID == CharacterID));

        /// <summary>
        /// Gets or sets the character contracts.
        /// </summary>
        /// <value>The character contracts.</value>
        public ContractCollection CharacterContracts { get; }

        /// <summary>
        /// Gets or sets the corporation contracts.
        /// </summary>
        /// <value>The character contracts.</value>
        public ContractCollection CorporationContracts { get; }

        /// <summary>
        /// Gets or sets the character contract bids.
        /// </summary>
        /// <value>The character contract bids.</value>
        public ContractBidCollection CharacterContractBids { get; }

        /// <summary>
        /// Gets or sets the corporation contract bids.
        /// </summary>
        /// <value>The character contract bids.</value>
        public ContractBidCollection CorporationContractBids { get; }

        /// <summary>
        /// Gets the collection of industry jobs.
        /// </summary>
        public IEnumerable<IndustryJob> IndustryJobs
            => CharacterIndustryJobs
                .Concat(CorporationIndustryJobs.Where(job => job.InstallerID == CharacterID));

        /// <summary>
        /// Gets or sets the character industry jobs.
        /// </summary>
        /// <value>The character industry jobs.</value>
        public IndustryJobCollection CharacterIndustryJobs { get; }

        /// <summary>
        /// Gets or sets the corporation industry jobs.
        /// </summary>
        /// <value>The corporation industry jobs.</value>
        public IndustryJobCollection CorporationIndustryJobs { get; }

        /// <summary>
        /// Gets the collection of research points.
        /// </summary>
        public ResearchPointCollection ResearchPoints { get; }

        /// <summary>
        /// Gets the collection of EVE mail messages.
        /// </summary>
        public EveMailMessageCollection EVEMailMessages { get; }

        /// <summary>
        /// Gets the collection of EVE mail messages.
        /// </summary>
        public EveMailingListCollection EVEMailingLists { get; }

        /// <summary>
        /// Gets the collection of EVE notifications.
        /// </summary>
        public EveNotificationCollection EVENotifications { get; }

        /// <summary>
        /// Gets the collection of contacts.
        /// </summary>
        public ContactCollection Contacts { get; }

        /// <summary>
        /// Gets the collection of character medals.
        /// </summary>
        public MedalCollection CharacterMedals { get; }

        /// <summary>
        /// Gets the collection of corporation medals.
        /// </summary>
        public MedalCollection CorporationMedals { get; }

        /// <summary>
        /// Gets the collection of upcoming calendar events.
        /// </summary>
        public UpcomingCalendarEventCollection UpcomingCalendarEvents { get; }

        /// <summary>
        /// Gets the collection of kill logs.
        /// </summary>
        public KillLogCollection KillLog { get; }

        /// <summary>
        /// Gets the collection of planetary colonies.
        /// </summary>
        public PlanetaryColonyCollection PlanetaryColonies { get; }

        /// <summary>
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors { get; }

        /// <summary>
        /// Gets true when the character is currently actively training, false otherwise.
        /// </summary>
        public override bool IsTraining => SkillQueue.IsTraining;

        /// <summary>
        /// Gets the skill currently in training, even when it is paused.
        /// </summary>
        public override QueuedSkill CurrentlyTrainingSkill => SkillQueue?.CurrentlyTraining;

        /// <summary>
        /// Gets a value indicating whether the character has insufficient balance to complete its buy orders.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the character has sufficient balance; otherwise, <c>false</c>.
        /// </value>
        public bool HasSufficientBalance
        {
            get
            {
                IEnumerable<BuyOrder> activeBuyOrders = MarketOrders.OfType<BuyOrder>().Where(
                    order => (order.State == OrderState.Active || order.State == OrderState.Modified) &&
                             order.IssuedFor == IssuedFor.Character).ToList();

                decimal additionalToCover = activeBuyOrders.Sum(x => x.TotalPrice) - activeBuyOrders.Sum(order => order.Escrow);

                return Balance >= additionalToCover;
            }

        }

        /// <summary>
        /// Gets a value indicating whether the character is enlisted in factional warfare.
        /// </summary>
        /// <value>
        ///   <c>true</c> if character is not enlisted in factional warfare; otherwise, <c>false</c>.
        /// </value>
        public bool IsFactionalWarfareNotEnlisted
        {
            get { return FactionID == 0 || m_isFwEnlisted; }
            internal set { m_isFwEnlisted = value; }
        }

        /// <summary>
        /// Gets true when a new character is created.
        /// </summary>
        public bool ForceUpdateBasicFeatures { get; }

        #endregion


        #region Importing & Exporting

        /// <summary>
        /// Create a serializable character sheet for this character.
        /// </summary>
        /// <returns></returns>
        public override SerializableSettingsCharacter Export()
        {
            SerializableCCPCharacter serial = new SerializableCCPCharacter();
            Export(serial);

            // Skill queue
            serial.SkillQueue.AddRange(SkillQueue.Export());

            // Market orders
            serial.MarketOrders.AddRange(MarketOrdersExport());

            // Contracts
            serial.Contracts.AddRange(ContractsExport());

            // ContractBids
            serial.ContractBids.AddRange(CharacterContractBids.Export());

            // Industry jobs
            serial.IndustryJobs.AddRange(IndustryJobsExport());

            // Eve mail messages IDs
            serial.EveMailMessagesIDs = EVEMailMessages.Export();

            // Eve notifications IDs
            serial.EveNotificationsIDs = EVENotifications.Export();

            // Last API updates
            if (QueryMonitors.Any())
            {
                m_lastAPIUpdates = QueryMonitors.Select(
                    monitor => new SerializableAPIUpdate
                                   {
                                       Method = monitor.Method.ToString(),
                                       Time = monitor.LastUpdate
                                   }).ToList();
            }

            serial.LastUpdates.AddRange(m_lastAPIUpdates);

            return serial;
        }

        /// <summary>
        /// Exports the market orders.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SerializableOrderBase> MarketOrdersExport()
        {
            // Until we can determine what data the character's API keys can query,
            // we have to keep the data unprocessed. Once we know, we filter them

            IEnumerable<SerializableOrderBase> corporationMarketOrdersExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_corporationDataQuerying != null
                    ? CorporationMarketOrders.ExportOnlyIssuedByCharacter()
                    : new List<SerializableOrderBase>();

            IEnumerable<SerializableOrderBase> characterMarketOrdersExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_characterDataQuerying != null
                    ? CharacterMarketOrders.Export()
                    : new List<SerializableOrderBase>();

            return characterMarketOrdersExport.Concat(corporationMarketOrdersExport);
        }

        /// <summary>
        /// Exports the contracts.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Excludes contracts that appear in both collections.</remarks>
        private IEnumerable<SerializableContract> ContractsExport()
        {
            // Until we can determine what data the character's API keys can query,
            // we have to keep the data unprocessed. Once we know, we filter them

            IEnumerable<SerializableContract> corporationContractsExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_corporationDataQuerying != null
                    ? CorporationContracts.ExportOnlyIssuedByCharacter()
                    : new List<SerializableContract>();

            IEnumerable<SerializableContract> characterContractsExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_characterDataQuerying != null
                    ? CharacterContracts.Export().Where(charContract => corporationContractsExport.All(
                        corpContract => corpContract.ContractID != charContract.ContractID))
                    : new List<SerializableContract>();

            return characterContractsExport.Concat(corporationContractsExport);
        }

        /// <summary>
        /// Exports the industry jobs.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SerializableJob> IndustryJobsExport()
        {
            // Until we can determine what data the character's API keys can query,
            // we have to keep the data unprocessed. Once we know, we filter them

            IEnumerable<SerializableJob> corporationIndustryJobsExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_corporationDataQuerying != null
                    ? CorporationIndustryJobs.ExportOnlyIssuedByCharacter()
                    : new List<SerializableJob>();

            IEnumerable<SerializableJob> characterIndustryJobsExport =
                EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed) || m_characterDataQuerying != null
                    ? CharacterIndustryJobs.Export()
                    : new List<SerializableJob>();

            return characterIndustryJobsExport.Concat(corporationIndustryJobsExport);
        }

        /// <summary>
        /// Imports data from a serialization object.
        /// </summary>
        /// <param name="serial"></param>
        private void Import(SerializableCCPCharacter serial)
        {
            Import((SerializableSettingsCharacter)serial);

            // Skill queue
            SkillQueue.Import(serial.SkillQueue);

            // Market orders
            MarketOrdersImport(serial.MarketOrders);

            // Contracts
            ContractsImport(serial.Contracts);

            // Contract Bids
            CharacterContractBids.Import(serial.ContractBids);

            // Industry jobs
            IndustryJobsImport(serial.IndustryJobs);

            // EVE mail messages IDs
            EVEMailMessages.Import(serial.EveMailMessagesIDs);

            // EVE notifications IDs
            EVENotifications.Import(serial.EveNotificationsIDs);

            // Kill Logs
            KillLog.ImportFromCacheFile();

            // Fire the global event
            EveMonClient.OnCharacterUpdated(this);
        }

        /// <summary>
        /// Imports the market orders.
        /// </summary>
        /// <param name="marketOrders">The market orders.</param>
        private void MarketOrdersImport(IList<SerializableOrderBase> marketOrders)
        {
            CharacterMarketOrders.Import(marketOrders.Where(job => job.IssuedFor == IssuedFor.Character));
            CorporationMarketOrders.Import(marketOrders.Where(job => job.IssuedFor == IssuedFor.Corporation));
        }

        /// <summary>
        /// Imports the contracts.
        /// </summary>
        /// <param name="contracts">The contracts.</param>
        private void ContractsImport(IList<SerializableContract> contracts)
        {
            CharacterContracts.Import(contracts.Where(contract => contract.IssuedFor == IssuedFor.Character));
            CorporationContracts.Import(contracts.Where(contract => contract.IssuedFor == IssuedFor.Corporation));
        }

        /// <summary>
        /// Imports the industry jobs.
        /// </summary>
        /// <param name="industryJobs">The industry jobs.</param>
        private void IndustryJobsImport(IList<SerializableJob> industryJobs)
        {
            CharacterIndustryJobs.Import(industryJobs.Where(job => job.IssuedFor == IssuedFor.Character));
            CorporationIndustryJobs.Import(industryJobs.Where(job => job.IssuedFor == IssuedFor.Corporation));
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal override void Dispose()
        {
            // Unsubscribe events
            EveMonClient.CharacterAssetsUpdated -= EveMonClient_CharacterAssetsUpdated;
            EveMonClient.CharacterMarketOrdersUpdated -= EveMonClient_CharacterMarketOrdersUpdated;
            EveMonClient.CorporationMarketOrdersUpdated -= EveMonClient_CorporationMarketOrdersUpdated;
            EveMonClient.CharacterContractsUpdated -= EveMonClient_CharacterContractsUpdated;
            EveMonClient.CorporationContractsUpdated -= EveMonClient_CorporationContractsUpdated;
            EveMonClient.CharacterIndustryJobsUpdated -= EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CorporationIndustryJobsUpdated -= EveMonClient_CorporationIndustryJobsUpdated;
            EveMonClient.CharacterIndustryJobsCompleted -= EveMonClient_CharacterIndustryJobsCompleted;
            EveMonClient.CorporationIndustryJobsCompleted -= EveMonClient_CorporationIndustryJobsCompleted;
            EveMonClient.CharacterPlaneteryPinsCompleted -= EveMonClient_CharacterPlaneteryPinsCompleted;
            EveMonClient.APIKeyInfoUpdated -= EveMonClient_APIKeyInfoUpdated;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;

            // Unsubscribe events
            SkillQueue.Dispose();
            CharacterIndustryJobs.Dispose();
            CorporationIndustryJobs.Dispose();
            PlanetaryColonies.Dispose();

            // Unsubscribe events
            if (m_characterDataQuerying != null)
            {
                m_characterDataQuerying.Dispose();
                m_characterDataQuerying = null;
            }

            if (m_corporationDataQuerying != null)
            {
                m_corporationDataQuerying.Dispose();
                m_corporationDataQuerying = null;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks whether we should notify an error.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal bool ShouldNotifyError(IAPIResult result, Enum method)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return false;

            // We don't want to be notified about corp roles error
            if (result.CCPError != null && result.CCPError.IsCorpRolesError)
                return false;

            // We don't want to be notified about kill log exhausted error
            if (result.CCPError != null && result.CCPError.IsKillLogExhaustedError)
                return false;

            // We don't want to be notified about not enlisted in factional warfare error
            if (result.CCPError != null && result.CCPError.IsFactionalWarfareEnlistedError)
                return false;

            // Notify an error occurred
            if (result.HasError)
            {
                if (!m_errorNotifiedMethod.Equals(CCPAPIMethodsEnum.None))
                    return false;

                m_errorNotifiedMethod = method;
                return true;
            }

            // Removes the previous error notification
            if (!m_errorNotifiedMethod.Equals(method))
                return false;

            EveMonClient.Notifications.InvalidateCharacterAPIError(this);
            m_errorNotifiedMethod = CCPAPIMethodsEnum.None;

            return false;
        }

        /// <summary>
        /// Notifies for market orders related events.
        /// </summary>
        private void NotifyForMarketOrdersRelatedEvents()
        {
            // Notify for ended orders
            NotifyEndedOrders();

            // Notify for insufficient balance
            NotifyInsufficientBalance();

            // Reset helper lists
            m_endedOrdersForCharacter.Clear();
            m_endedOrdersForCorporation.Clear();

            // Fires the event regarding market orders update
            EveMonClient.OnMarketOrdersUpdated(this);
        }

        /// <summary>
        /// Notifies for ended orders.
        /// </summary>
        private void NotifyEndedOrders()
        {
            // Notify ended orders issued by the character
            if (m_endedOrdersForCharacter.Any())
                EveMonClient.Notifications.NotifyCharacterMarkerOrdersEnded(this, m_endedOrdersForCharacter);

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify ended orders issued for the corporation
            //if (m_endedOrdersForCorporation.Any())
            //    EveMonClient.Notifications.NotifyCorporationMarketOrdersEnded(Corporation, m_endedOrdersForCorporation);
        }

        /// <summary>
        /// Notifies for contracts related events.
        /// </summary>
        private void NotifyForContractsRelatedEvents()
        {
            // Notify for ended contracts
            NotifyEndedContracts();

            // Notify for assigned contracts
            NotifyAssignedContracts();

            // Reset helper lists
            // Note: Special condition logic is applied due to the fact that CCP
            // includes coproration related contracts in character API feed
            if (m_characterDataQuerying != null && m_corporationDataQuerying != null &&
                m_corporationDataQuerying.CorporationContractsQueried)
            {
                m_endedContractsForCharacter.Clear();
            }

            if (m_corporationDataQuerying != null && m_characterDataQuerying != null &&
                m_characterDataQuerying.CharacterContractsQueried)
            {
                m_endedContractsForCorporation.Clear();
            }

            // Fires the event regarding contracts update
            EveMonClient.OnContractsUpdated(this);
        }

        /// <summary>
        /// Notifies for ended contracts.
        /// </summary>
        private void NotifyEndedContracts()
        {
            // Notify ended contracts issued by the character
            if (m_endedContractsForCharacter.Any(x => !x.NotificationSend))
            {
                EveMonClient.Notifications.NotifyCharacterContractsEnded(this, m_endedContractsForCharacter);
                m_endedContractsForCharacter.ForEach(x => x.NotificationSend = true);
            }

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify ended contracts issued for the corporation
            //if (m_endedContractsForCorporation.All(x => x.NotificationSend))
            //    return;

            //EveMonClient.Notifications.NotifyCorporationContractsEnded(Corporation, m_endedContractsForCorporation);
            //m_endedContractsForCorporation.ForEach(x => x.NotificationSend = true);
        }

        /// <summary>
        /// Notifies for assigned contracts.
        /// </summary>
        private void NotifyAssignedContracts()
        {
            if (Contracts.Any(contract => contract.State == ContractState.Assigned))
            {
                int assignedContracts = Contracts.Count(contracts => contracts.State == ContractState.Assigned);
                EveMonClient.Notifications.NotifyCharacterContractsAssigned(this, assignedContracts);
                return;
            }

            EveMonClient.Notifications.InvalidateCharacterContractsAssigned(this);
        }

        /// <summary>
        /// Notifies for insufficient balance.
        /// </summary>
        internal void NotifyInsufficientBalance()
        {
            // Check the character has sufficient balance
            // for its buying orders and send a notification if not
            if (!HasSufficientBalance)
            {
                EveMonClient.Notifications.NotifyInsufficientBalance(this);
                return;
            }

            EveMonClient.Notifications.InvalidateInsufficientBalance(this);
        }

        /// <summary>
        /// Notifies for industry jobs related events.
        /// </summary>
        private void NotifyForIndustryJobsRelatedEvents()
        {
            // Fires the event regarding industry jobs update
            EveMonClient.OnIndustryJobsUpdated(this);
        }

        /// <summary>
        /// Resets the last API updates.
        /// </summary>
        /// <param name="lastUpdates">The last updates.</param>
        private void ResetLastAPIUpdates(IEnumerable<SerializableAPIUpdate> lastUpdates)
        {
            foreach (SerializableAPIUpdate lastUpdate in lastUpdates)
            {
                Enum method = APIMethods.Methods.FirstOrDefault(apiMethod => apiMethod.ToString() == lastUpdate.Method);
                if (method == null)
                    continue;

                IQueryMonitorEx monitor = QueryMonitors[method] as IQueryMonitorEx;
                monitor?.Reset(lastUpdate.Time);
            }
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            // Force update a monitor if the last update exceed the current datetime
            foreach (IQueryMonitorEx monitor in QueryMonitors.Where(
                monitor => !monitor.IsUpdating && monitor.LastUpdate > DateTime.UtcNow).Cast<IQueryMonitorEx>())
            {
                monitor.ForceUpdate(true);
            }
        }

        /// <summary>
        /// Handles the APIKeyInfoUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_APIKeyInfoUpdated(object sender, EventArgs e)
        {
            if (EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed))
                return;

            if (!Identity.APIKeys.Any() || Identity.APIKeys.Any(apiKey => apiKey.Type == CCPAPIKeyType.Unknown))
                return;

            if (m_characterDataQuerying == null && Identity.APIKeys.Any(apiKey => apiKey.IsCharacterOrAccountType))
            {
                m_characterDataQuerying = new CharacterDataQuerying(this);
                ResetLastAPIUpdates(m_lastAPIUpdates.Where(lastUpdate => Enum.IsDefined(typeof(CCPAPICharacterMethods),
                                                                                        lastUpdate.Method)));
            }

            if (m_corporationDataQuerying == null && Identity.APIKeys.Any(apiKey => apiKey.IsCorporationType))
            {
                m_corporationDataQuerying = new CorporationDataQuerying(this);
                ResetLastAPIUpdates(m_lastAPIUpdates.Where(lastUpdate => Enum.IsDefined(typeof(CCPAPICorporationMethods),
                                                                                        lastUpdate.Method)));
            }
        }

        /// <summary>
        /// Handles the CharacterAssetsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Triggering a settings exportation to update the character owned skillbooks found in Assets</remarks>
        private void EveMonClient_CharacterAssetsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            Export();
        }

        /// <summary>
        /// Handles the CharacterMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.MarketOrdersEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterMarketOrdersUpdated(object sender, MarketOrdersEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedOrdersForCharacter.AddRange(e.EndedOrders);

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationMarketOrdersQueried)
            {
                return;
            }

            NotifyForMarketOrdersRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.MarketOrdersEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationMarketOrdersUpdated(object sender, MarketOrdersEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedOrdersForCorporation.AddRange(e.EndedOrders);
            m_endedOrdersForCharacter.AddRange(e.EndedOrders.Where(order => order.OwnerID == CharacterID));

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterMarketOrdersQueried)
            {
                return;
            }

            NotifyForMarketOrdersRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterContractsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.ContractsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterContractsUpdated(object sender, ContractsEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedContractsForCharacter.AddRange(e.EndedContracts.Where(
                charEndedContract => m_endedContractsForCorporation.All(
                    corpEndedContract => corpEndedContract.ID != charEndedContract.ID)));

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationContractsQueried)
            {
                return;
            }

            NotifyForContractsRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationContractsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.ContractsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationContractsUpdated(object sender, ContractsEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedContractsForCorporation.AddRange(e.EndedContracts);
            m_endedContractsForCharacter.AddRange(e.EndedContracts.Where(contract => contract.IssuerID == CharacterID).Where(
                corpEndedContract => m_endedContractsForCharacter.All(
                    charEndedContract => charEndedContract.ID != corpEndedContract.ID)));

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterContractsQueried)
            {
                return;
            }

            NotifyForContractsRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterIndustryJobsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterIndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationIndustryJobsQueried)
            {
                return;
            }

            NotifyForIndustryJobsRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationIndustryJobsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationIndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterIndustryJobsQueried)
            {
                return;
            }

            NotifyForIndustryJobsRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterIndustryJobsCompleted event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.IndustryJobsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterIndustryJobsCompleted(object sender, IndustryJobsEventArgs e)
        {
            if (e.Character != this)
                return;

            // Add the completed jobs to a list
            m_jobsCompletedForCharacter.AddRange(e.CompletedJobs);

            // If character has completed corporation issued jobs, we wait till we gather those too
            if (CorporationIndustryJobs.Any(job => job.ActiveJobState == ActiveJobState.Ready && !job.NotificationSend))
                return;

            // Notify completed jobs issued by the character
            EveMonClient.Notifications.NotifyCharacterIndustryJobCompletion(this, m_jobsCompletedForCharacter);

            // Now that we have send the notification clear the list
            m_jobsCompletedForCharacter.Clear();
        }

        /// <summary>
        /// Handles the CorporationIndustryJobsCompleted event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.IndustryJobsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationIndustryJobsCompleted(object sender, IndustryJobsEventArgs e)
        {
            if (e.Character != this)
                return;

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify completed jobs issued for the corporation
            //EveMonClient.Notifications.NotifyCorporationIndustryJobCompletion(Corporation, e.CompletedJobs);
        }

        /// <summary>
        /// Handles the CharacterPlaneteryPinsCompleted event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlanetaryPinsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterPlaneteryPinsCompleted(object sender, PlanetaryPinsEventArgs e)
        {
            if (e.Character != this)
                return;

            // Notify completed pins the character
            EveMonClient.Notifications.NotifyCharacterPlanetaryPinCompleted(this, e.CompletedPins);
        }

        #endregion
    }
}