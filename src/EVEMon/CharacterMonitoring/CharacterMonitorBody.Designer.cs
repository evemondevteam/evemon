using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.CharacterMonitoring
{
    internal sealed partial class CharacterMonitorBody
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterMonitorBody));
            this.toolstripPanel = new System.Windows.Forms.Panel();
            this.toolStripContextual = new System.Windows.Forms.ToolStrip();
            this.preferencesMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.columnSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSizeColumnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsColumnSettingsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.hideInactiveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberAbsFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsOptionsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.showOnlyCharMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyCorpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsReadingPaneSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.readingPaneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paneRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paneBottomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paneOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combatLogSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.combatLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPlanetarySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.showOnlyExtractorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextDel = new System.Windows.Forms.ToolStripButton();
            this.searchTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.groupMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.walletJournalCharts = new System.Windows.Forms.ToolStripButton();
            this.allContacts = new System.Windows.Forms.ToolStripButton();
            this.contactsExcellent = new System.Windows.Forms.ToolStripButton();
            this.contactsGood = new System.Windows.Forms.ToolStripButton();
            this.contactsNeutral = new System.Windows.Forms.ToolStripButton();
            this.contactsBad = new System.Windows.Forms.ToolStripButton();
            this.contactsTerrible = new System.Windows.Forms.ToolStripButton();
            this.inWatchList = new System.Windows.Forms.ToolStripButton();
            this.toolStripFeatures = new System.Windows.Forms.ToolStrip();
            this.skillsIcon = new System.Windows.Forms.ToolStripButton();
            this.skillQueueIcon = new System.Windows.Forms.ToolStripButton();
            this.employmentIcon = new System.Windows.Forms.ToolStripButton();
            this.standingsIcon = new System.Windows.Forms.ToolStripButton();
            this.contactsIcon = new System.Windows.Forms.ToolStripButton();
            this.factionalWarfareStatsIcon = new System.Windows.Forms.ToolStripButton();
            this.medalsIcon = new System.Windows.Forms.ToolStripButton();
            this.killLogIcon = new System.Windows.Forms.ToolStripButton();
            this.assetsIcon = new System.Windows.Forms.ToolStripButton();
            this.ordersIcon = new System.Windows.Forms.ToolStripButton();
            this.contractsIcon = new System.Windows.Forms.ToolStripButton();
            this.walletJournalIcon = new System.Windows.Forms.ToolStripButton();
            this.walletTransactionsIcon = new System.Windows.Forms.ToolStripButton();
            this.jobsIcon = new System.Windows.Forms.ToolStripButton();
            this.loyaltyIcon = new System.Windows.Forms.ToolStripButton();
            this.planetaryIcon = new System.Windows.Forms.ToolStripButton();
            this.researchIcon = new System.Windows.Forms.ToolStripButton();
            this.mailMessagesIcon = new System.Windows.Forms.ToolStripButton();
            this.eveNotificationsIcon = new System.Windows.Forms.ToolStripButton();
            this.calendarEventsIcon = new System.Windows.Forms.ToolStripButton();
            this.featuresMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.EnableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsPagesSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toggleSkillsIcon = new System.Windows.Forms.ToolStripButton();
            this.tsToggleSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.searchTextTimer = new System.Windows.Forms.Timer(this.components);
            this.borderPanel = new EVEMon.Common.Controls.BorderPanel();
            this.corePanel = new System.Windows.Forms.Panel();
            this.multiPanel = new EVEMon.Common.Controls.MultiPanel.MultiPanel();
            this.loyaltyPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.loyaltyList = new EVEMon.CharacterMonitoring.CharacterLoyaltyList();
            this.standingsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.standingsList = new EVEMon.CharacterMonitoring.CharacterStandingsList();
            this.skillsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.skillsList = new EVEMon.CharacterMonitoring.CharacterSkillsList();
            this.ordersPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.ordersList = new EVEMon.CharacterMonitoring.CharacterMarketOrdersList();
            this.skillQueuePage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.skillQueueList = new EVEMon.CharacterMonitoring.CharacterSkillsQueueList();
            this.jobsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.jobsList = new EVEMon.CharacterMonitoring.CharacterIndustryJobsList();
            this.researchPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.researchList = new EVEMon.CharacterMonitoring.CharacterResearchPointsList();
            this.mailMessagesPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.mailMessagesList = new EVEMon.CharacterMonitoring.CharacterEveMailMessagesList();
            this.eveNotificationsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.eveNotificationsList = new EVEMon.CharacterMonitoring.CharacterEveNotificationsList();
            this.employmentPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.employmentList = new EVEMon.CharacterMonitoring.CharacterEmploymentHistoryList();
            this.contractsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.contractsList = new EVEMon.CharacterMonitoring.CharacterContractsList();
            this.assetsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.assetsList = new EVEMon.CharacterMonitoring.CharacterAssetsList();
            this.walletJournalPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.walletJournalList = new EVEMon.CharacterMonitoring.CharacterWalletJournalList();
            this.walletTransactionsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.walletTransactionsList = new EVEMon.CharacterMonitoring.CharacterWalletTransactionsList();
            this.factionalWarfareStatsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.factionalWarfareStatsList = new EVEMon.CharacterMonitoring.CharacterFactionalWarfareStatsList();
            this.contactsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.contactsList = new EVEMon.CharacterMonitoring.CharacterContactList();
            this.medalsPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.medalsList = new EVEMon.CharacterMonitoring.CharacterMedalsList();
            this.killLogPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.killLogList = new EVEMon.CharacterMonitoring.CharacterKillLogList();
            this.planetaryPage = new EVEMon.Common.Controls.MultiPanel.MultiPanelPage();
            this.planetaryList = new EVEMon.CharacterMonitoring.CharacterPlanetaryList();
            this.warningLabel = new System.Windows.Forms.Label();
            this.notificationList = new EVEMon.Controls.NotificationList();
            this.toolstripPanel.SuspendLayout();
            this.toolStripContextual.SuspendLayout();
            this.toolStripFeatures.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.corePanel.SuspendLayout();
            this.multiPanel.SuspendLayout();
            this.loyaltyPage.SuspendLayout();
            this.standingsPage.SuspendLayout();
            this.skillsPage.SuspendLayout();
            this.ordersPage.SuspendLayout();
            this.skillQueuePage.SuspendLayout();
            this.jobsPage.SuspendLayout();
            this.researchPage.SuspendLayout();
            this.mailMessagesPage.SuspendLayout();
            this.eveNotificationsPage.SuspendLayout();
            this.employmentPage.SuspendLayout();
            this.contractsPage.SuspendLayout();
            this.assetsPage.SuspendLayout();
            this.walletJournalPage.SuspendLayout();
            this.walletTransactionsPage.SuspendLayout();
            this.factionalWarfareStatsPage.SuspendLayout();
            this.contactsPage.SuspendLayout();
            this.medalsPage.SuspendLayout();
            this.killLogPage.SuspendLayout();
            this.planetaryPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolstripPanel
            // 
            this.toolstripPanel.AutoSize = true;
            this.toolstripPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toolstripPanel.Controls.Add(this.toolStripContextual);
            this.toolstripPanel.Controls.Add(this.toolStripFeatures);
            this.toolstripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolstripPanel.Location = new System.Drawing.Point(0, 0);
            this.toolstripPanel.Name = "toolstripPanel";
            this.toolstripPanel.Size = new System.Drawing.Size(614, 56);
            this.toolstripPanel.TabIndex = 17;
            // 
            // toolStripContextual
            // 
            this.toolStripContextual.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripContextual.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesMenu,
            this.searchTextDel,
            this.searchTextBox,
            this.groupMenu,
            this.walletJournalCharts,
            this.allContacts,
            this.contactsExcellent,
            this.contactsGood,
            this.contactsNeutral,
            this.contactsBad,
            this.contactsTerrible,
            this.inWatchList});
            this.toolStripContextual.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripContextual.Location = new System.Drawing.Point(0, 31);
            this.toolStripContextual.Name = "toolStripContextual";
            this.toolStripContextual.Size = new System.Drawing.Size(614, 25);
            this.toolStripContextual.TabIndex = 15;
            // 
            // preferencesMenu
            // 
            this.preferencesMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.preferencesMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.preferencesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnSettingsMenuItem,
            this.autoSizeColumnMenuItem,
            this.tsColumnSettingsSeparator,
            this.hideInactiveMenuItem,
            this.numberAbsFormatMenuItem,
            this.tsOptionsSeparator,
            this.showOnlyCharMenuItem,
            this.showOnlyCorpMenuItem,
            this.tsReadingPaneSeparator,
            this.readingPaneMenuItem,
            this.combatLogSeparator,
            this.combatLogMenuItem,
            this.tsPlanetarySeparator,
            this.showOnlyExtractorMenuItem});
            this.preferencesMenu.Image = ((System.Drawing.Image)(resources.GetObject("preferencesMenu.Image")));
            this.preferencesMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.preferencesMenu.Name = "preferencesMenu";
            this.preferencesMenu.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.preferencesMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.preferencesMenu.Size = new System.Drawing.Size(29, 22);
            this.preferencesMenu.Text = "Preferences";
            this.preferencesMenu.ToolTipText = "Preferences";
            this.preferencesMenu.DropDownOpening += new System.EventHandler(this.preferencesMenu_DropDownOpening);
            // 
            // columnSettingsMenuItem
            // 
            this.columnSettingsMenuItem.Name = "columnSettingsMenuItem";
            this.columnSettingsMenuItem.Size = new System.Drawing.Size(252, 22);
            this.columnSettingsMenuItem.Text = "Column Settings";
            this.columnSettingsMenuItem.Click += new System.EventHandler(this.columnSettingsMenuItem_Click);
            // 
            // autoSizeColumnMenuItem
            // 
            this.autoSizeColumnMenuItem.Name = "autoSizeColumnMenuItem";
            this.autoSizeColumnMenuItem.Size = new System.Drawing.Size(252, 22);
            this.autoSizeColumnMenuItem.Text = "Auto-Size Columns";
            this.autoSizeColumnMenuItem.Click += new System.EventHandler(this.autoSizeColumnMenuItem_Click);
            // 
            // tsColumnSettingsSeparator
            // 
            this.tsColumnSettingsSeparator.Name = "tsColumnSettingsSeparator";
            this.tsColumnSettingsSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // hideInactiveMenuItem
            // 
            this.hideInactiveMenuItem.Name = "hideInactiveMenuItem";
            this.hideInactiveMenuItem.Size = new System.Drawing.Size(252, 22);
            this.hideInactiveMenuItem.Text = "Hide Inactive Orders";
            this.hideInactiveMenuItem.Click += new System.EventHandler(this.hideInactiveMenuItem_Click);
            // 
            // numberAbsFormatMenuItem
            // 
            this.numberAbsFormatMenuItem.Name = "numberAbsFormatMenuItem";
            this.numberAbsFormatMenuItem.Size = new System.Drawing.Size(252, 22);
            this.numberAbsFormatMenuItem.Text = "Number Abbreviating Format";
            this.numberAbsFormatMenuItem.Click += new System.EventHandler(this.numberAbsFormatMenuItem_Click);
            // 
            // tsOptionsSeparator
            // 
            this.tsOptionsSeparator.Name = "tsOptionsSeparator";
            this.tsOptionsSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // showOnlyCharMenuItem
            // 
            this.showOnlyCharMenuItem.CheckOnClick = true;
            this.showOnlyCharMenuItem.Name = "showOnlyCharMenuItem";
            this.showOnlyCharMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showOnlyCharMenuItem.Text = "Show Only Issued for Character";
            this.showOnlyCharMenuItem.Click += new System.EventHandler(this.showOnlyCharMenuItem_Click);
            // 
            // showOnlyCorpMenuItem
            // 
            this.showOnlyCorpMenuItem.CheckOnClick = true;
            this.showOnlyCorpMenuItem.Name = "showOnlyCorpMenuItem";
            this.showOnlyCorpMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showOnlyCorpMenuItem.Text = "Show Only Issued for Corporation";
            this.showOnlyCorpMenuItem.Click += new System.EventHandler(this.showOnlyCorpMenuItem_Click);
            // 
            // tsReadingPaneSeparator
            // 
            this.tsReadingPaneSeparator.Name = "tsReadingPaneSeparator";
            this.tsReadingPaneSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // readingPaneMenuItem
            // 
            this.readingPaneMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paneRightMenuItem,
            this.paneBottomMenuItem,
            this.paneOffMenuItem});
            this.readingPaneMenuItem.Name = "readingPaneMenuItem";
            this.readingPaneMenuItem.Size = new System.Drawing.Size(252, 22);
            this.readingPaneMenuItem.Text = "Reading Pane";
            this.readingPaneMenuItem.DropDownOpening += new System.EventHandler(this.readingPaneMenuItem_DropDownOpening);
            // 
            // paneRightMenuItem
            // 
            this.paneRightMenuItem.CheckOnClick = true;
            this.paneRightMenuItem.Name = "paneRightMenuItem";
            this.paneRightMenuItem.Size = new System.Drawing.Size(114, 22);
            this.paneRightMenuItem.Tag = "Right";
            this.paneRightMenuItem.Text = "Right";
            this.paneRightMenuItem.Click += new System.EventHandler(this.paneRightMenuItem_Click);
            // 
            // paneBottomMenuItem
            // 
            this.paneBottomMenuItem.CheckOnClick = true;
            this.paneBottomMenuItem.Name = "paneBottomMenuItem";
            this.paneBottomMenuItem.Size = new System.Drawing.Size(114, 22);
            this.paneBottomMenuItem.Tag = "Bottom";
            this.paneBottomMenuItem.Text = "Bottom";
            this.paneBottomMenuItem.Click += new System.EventHandler(this.paneBottomMenuItem_Click);
            // 
            // paneOffMenuItem
            // 
            this.paneOffMenuItem.CheckOnClick = true;
            this.paneOffMenuItem.Name = "paneOffMenuItem";
            this.paneOffMenuItem.Size = new System.Drawing.Size(114, 22);
            this.paneOffMenuItem.Tag = "Off";
            this.paneOffMenuItem.Text = "Off";
            this.paneOffMenuItem.Click += new System.EventHandler(this.paneOffMenuItem_Click);
            // 
            // combatLogSeparator
            // 
            this.combatLogSeparator.Name = "combatLogSeparator";
            this.combatLogSeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // combatLogMenuItem
            // 
            this.combatLogMenuItem.CheckOnClick = true;
            this.combatLogMenuItem.Name = "combatLogMenuItem";
            this.combatLogMenuItem.Size = new System.Drawing.Size(252, 22);
            this.combatLogMenuItem.Text = "Show Condensed Combat Logs";
            this.combatLogMenuItem.Click += new System.EventHandler(this.combatLogMenuItem_Click);
            // 
            // tsPlanetarySeparator
            // 
            this.tsPlanetarySeparator.Name = "tsPlanetarySeparator";
            this.tsPlanetarySeparator.Size = new System.Drawing.Size(249, 6);
            // 
            // showOnlyExtractorMenuItem
            // 
            this.showOnlyExtractorMenuItem.CheckOnClick = true;
            this.showOnlyExtractorMenuItem.Name = "showOnlyExtractorMenuItem";
            this.showOnlyExtractorMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showOnlyExtractorMenuItem.Text = "Show Only Extractor Control Unit";
            this.showOnlyExtractorMenuItem.Click += new System.EventHandler(this.showOnlyExtractorMenuItem_Click);
            // 
            // searchTextDel
            // 
            this.searchTextDel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchTextDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchTextDel.Image = ((System.Drawing.Image)(resources.GetObject("searchTextDel.Image")));
            this.searchTextDel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.searchTextDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchTextDel.Name = "searchTextDel";
            this.searchTextDel.Size = new System.Drawing.Size(23, 22);
            this.searchTextDel.Text = "searchTextDel";
            this.searchTextDel.ToolTipText = "Delete search text";
            this.searchTextDel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.searchTextDel_MouseUp);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.searchTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.searchTextBox.AutoSize = false;
            this.searchTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.searchTextBox.Size = new System.Drawing.Size(120, 21);
            this.searchTextBox.ToolTipText = "Search";
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // groupMenu
            // 
            this.groupMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.groupMenu.Image = ((System.Drawing.Image)(resources.GetObject("groupMenu.Image")));
            this.groupMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupMenu.Name = "groupMenu";
            this.groupMenu.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.groupMenu.Size = new System.Drawing.Size(94, 22);
            this.groupMenu.Text = "Group By...";
            this.groupMenu.DropDownOpening += new System.EventHandler(this.groupMenu_DropDownOpening);
            this.groupMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.groupMenu_DropDownItemClicked);
            // 
            // walletJournalCharts
            // 
            this.walletJournalCharts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.walletJournalCharts.Image = ((System.Drawing.Image)(resources.GetObject("walletJournalCharts.Image")));
            this.walletJournalCharts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.walletJournalCharts.Name = "walletJournalCharts";
            this.walletJournalCharts.Size = new System.Drawing.Size(23, 22);
            this.walletJournalCharts.Text = "Wallet Journal Charts";
            this.walletJournalCharts.ToolTipText = "Charts";
            this.walletJournalCharts.Click += new System.EventHandler(this.walletJournalCharts_Click);
            // 
            // allContacts
            // 
            this.allContacts.Checked = true;
            this.allContacts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allContacts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.allContacts.Image = ((System.Drawing.Image)(resources.GetObject("allContacts.Image")));
            this.allContacts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.allContacts.Name = "allContacts";
            this.allContacts.Size = new System.Drawing.Size(23, 22);
            this.allContacts.Text = "All contacts";
            this.allContacts.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // contactsExcellent
            // 
            this.contactsExcellent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsExcellent.Image = ((System.Drawing.Image)(resources.GetObject("contactsExcellent.Image")));
            this.contactsExcellent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsExcellent.Name = "contactsExcellent";
            this.contactsExcellent.Size = new System.Drawing.Size(23, 22);
            this.contactsExcellent.Text = "Excellent standing";
            this.contactsExcellent.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // contactsGood
            // 
            this.contactsGood.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsGood.Image = ((System.Drawing.Image)(resources.GetObject("contactsGood.Image")));
            this.contactsGood.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsGood.Name = "contactsGood";
            this.contactsGood.Size = new System.Drawing.Size(23, 22);
            this.contactsGood.Tag = "Good";
            this.contactsGood.Text = "Good standing";
            this.contactsGood.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // contactsNeutral
            // 
            this.contactsNeutral.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsNeutral.Image = ((System.Drawing.Image)(resources.GetObject("contactsNeutral.Image")));
            this.contactsNeutral.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsNeutral.Name = "contactsNeutral";
            this.contactsNeutral.Size = new System.Drawing.Size(23, 22);
            this.contactsNeutral.Text = "Neutral standing";
            this.contactsNeutral.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // contactsBad
            // 
            this.contactsBad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsBad.Image = ((System.Drawing.Image)(resources.GetObject("contactsBad.Image")));
            this.contactsBad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsBad.Name = "contactsBad";
            this.contactsBad.Size = new System.Drawing.Size(23, 22);
            this.contactsBad.Text = "Bad standing";
            this.contactsBad.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // contactsTerrible
            // 
            this.contactsTerrible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsTerrible.Image = ((System.Drawing.Image)(resources.GetObject("contactsTerrible.Image")));
            this.contactsTerrible.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsTerrible.Name = "contactsTerrible";
            this.contactsTerrible.Size = new System.Drawing.Size(23, 22);
            this.contactsTerrible.Text = "Terrible standing";
            this.contactsTerrible.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // inWatchList
            // 
            this.inWatchList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.inWatchList.Image = ((System.Drawing.Image)(resources.GetObject("inWatchList.Image")));
            this.inWatchList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inWatchList.Name = "inWatchList";
            this.inWatchList.Size = new System.Drawing.Size(23, 22);
            this.inWatchList.Text = "Watch List";
            this.inWatchList.Click += new System.EventHandler(this.contactsToolbarIcon_Click);
            // 
            // toolStripFeatures
            // 
            this.toolStripFeatures.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFeatures.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripFeatures.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skillsIcon,
            this.skillQueueIcon,
            this.employmentIcon,
            this.standingsIcon,
            this.contactsIcon,
            this.factionalWarfareStatsIcon,
            this.medalsIcon,
            this.killLogIcon,
            this.assetsIcon,
            this.ordersIcon,
            this.contractsIcon,
            this.walletJournalIcon,
            this.walletTransactionsIcon,
            this.jobsIcon,
            this.loyaltyIcon,
            this.planetaryIcon,
            this.researchIcon,
            this.mailMessagesIcon,
            this.eveNotificationsIcon,
            this.calendarEventsIcon,
            this.featuresMenu,
            this.tsPagesSeparator,
            this.toggleSkillsIcon,
            this.tsToggleSeparator});
            this.toolStripFeatures.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripFeatures.Location = new System.Drawing.Point(0, 0);
            this.toolStripFeatures.Name = "toolStripFeatures";
            this.toolStripFeatures.Size = new System.Drawing.Size(614, 31);
            this.toolStripFeatures.TabIndex = 13;
            // 
            // skillsIcon
            // 
            this.skillsIcon.Checked = true;
            this.skillsIcon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.skillsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.skillsIcon.Image = ((System.Drawing.Image)(resources.GetObject("skillsIcon.Image")));
            this.skillsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillsIcon.Name = "skillsIcon";
            this.skillsIcon.Size = new System.Drawing.Size(28, 28);
            this.skillsIcon.Tag = "skillsPage";
            this.skillsIcon.Text = "Skills";
            this.skillsIcon.ToolTipText = "Display skills list";
            this.skillsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // skillQueueIcon
            // 
            this.skillQueueIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.skillQueueIcon.Image = ((System.Drawing.Image)(resources.GetObject("skillQueueIcon.Image")));
            this.skillQueueIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.skillQueueIcon.Name = "skillQueueIcon";
            this.skillQueueIcon.Size = new System.Drawing.Size(28, 28);
            this.skillQueueIcon.Tag = "skillQueuePage";
            this.skillQueueIcon.Text = "Queue";
            this.skillQueueIcon.ToolTipText = "Display skills in queue";
            this.skillQueueIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // employmentIcon
            // 
            this.employmentIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.employmentIcon.Image = ((System.Drawing.Image)(resources.GetObject("employmentIcon.Image")));
            this.employmentIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.employmentIcon.Name = "employmentIcon";
            this.employmentIcon.Size = new System.Drawing.Size(28, 28);
            this.employmentIcon.Tag = "employmentPage";
            this.employmentIcon.Text = "Employment History";
            this.employmentIcon.ToolTipText = "Display employment history";
            this.employmentIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // standingsIcon
            // 
            this.standingsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.standingsIcon.Image = ((System.Drawing.Image)(resources.GetObject("standingsIcon.Image")));
            this.standingsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.standingsIcon.Name = "standingsIcon";
            this.standingsIcon.Size = new System.Drawing.Size(28, 28);
            this.standingsIcon.Tag = "standingsPage";
            this.standingsIcon.Text = "Standings";
            this.standingsIcon.ToolTipText = "Display standings";
            this.standingsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // contactsIcon
            // 
            this.contactsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contactsIcon.Image = ((System.Drawing.Image)(resources.GetObject("contactsIcon.Image")));
            this.contactsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contactsIcon.Name = "contactsIcon";
            this.contactsIcon.Size = new System.Drawing.Size(28, 28);
            this.contactsIcon.Tag = "contactsPage";
            this.contactsIcon.Text = "Contacts";
            this.contactsIcon.ToolTipText = "Display contacts";
            this.contactsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // factionalWarfareStatsIcon
            // 
            this.factionalWarfareStatsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.factionalWarfareStatsIcon.Image = ((System.Drawing.Image)(resources.GetObject("factionalWarfareStatsIcon.Image")));
            this.factionalWarfareStatsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.factionalWarfareStatsIcon.Name = "factionalWarfareStatsIcon";
            this.factionalWarfareStatsIcon.Size = new System.Drawing.Size(28, 28);
            this.factionalWarfareStatsIcon.Tag = "factionalWarfareStatsPage";
            this.factionalWarfareStatsIcon.Text = "Factional Warfare";
            this.factionalWarfareStatsIcon.ToolTipText = "Display factional warfare stats";
            this.factionalWarfareStatsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // medalsIcon
            // 
            this.medalsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.medalsIcon.Image = ((System.Drawing.Image)(resources.GetObject("medalsIcon.Image")));
            this.medalsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.medalsIcon.Name = "medalsIcon";
            this.medalsIcon.Size = new System.Drawing.Size(28, 28);
            this.medalsIcon.Tag = "medalsPage";
            this.medalsIcon.Text = "Medals";
            this.medalsIcon.ToolTipText = "Display medals";
            this.medalsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // killLogIcon
            // 
            this.killLogIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.killLogIcon.Image = ((System.Drawing.Image)(resources.GetObject("killLogIcon.Image")));
            this.killLogIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.killLogIcon.Name = "killLogIcon";
            this.killLogIcon.Size = new System.Drawing.Size(28, 28);
            this.killLogIcon.Tag = "killLogPage";
            this.killLogIcon.Text = "Combat Log";
            this.killLogIcon.ToolTipText = "Display combat log";
            this.killLogIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // assetsIcon
            // 
            this.assetsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.assetsIcon.Image = ((System.Drawing.Image)(resources.GetObject("assetsIcon.Image")));
            this.assetsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.assetsIcon.Name = "assetsIcon";
            this.assetsIcon.Size = new System.Drawing.Size(28, 28);
            this.assetsIcon.Tag = "assetsPage";
            this.assetsIcon.Text = "Assets";
            this.assetsIcon.ToolTipText = "Display assets";
            this.assetsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // ordersIcon
            // 
            this.ordersIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ordersIcon.Image = ((System.Drawing.Image)(resources.GetObject("ordersIcon.Image")));
            this.ordersIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ordersIcon.Name = "ordersIcon";
            this.ordersIcon.Size = new System.Drawing.Size(28, 28);
            this.ordersIcon.Tag = "ordersPage";
            this.ordersIcon.Text = "Market";
            this.ordersIcon.ToolTipText = "Display market orders";
            this.ordersIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // contractsIcon
            // 
            this.contractsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.contractsIcon.Image = ((System.Drawing.Image)(resources.GetObject("contractsIcon.Image")));
            this.contractsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.contractsIcon.Name = "contractsIcon";
            this.contractsIcon.Size = new System.Drawing.Size(28, 28);
            this.contractsIcon.Tag = "contractsPage";
            this.contractsIcon.Text = "Contracts";
            this.contractsIcon.ToolTipText = "Display contracts";
            this.contractsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // walletJournalIcon
            // 
            this.walletJournalIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.walletJournalIcon.Image = ((System.Drawing.Image)(resources.GetObject("walletJournalIcon.Image")));
            this.walletJournalIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.walletJournalIcon.Name = "walletJournalIcon";
            this.walletJournalIcon.Size = new System.Drawing.Size(28, 28);
            this.walletJournalIcon.Tag = "walletJournalPage";
            this.walletJournalIcon.Text = "Wallet Journal";
            this.walletJournalIcon.ToolTipText = "Display wallet journal";
            this.walletJournalIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // walletTransactionsIcon
            // 
            this.walletTransactionsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.walletTransactionsIcon.Image = ((System.Drawing.Image)(resources.GetObject("walletTransactionsIcon.Image")));
            this.walletTransactionsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.walletTransactionsIcon.Name = "walletTransactionsIcon";
            this.walletTransactionsIcon.Size = new System.Drawing.Size(28, 28);
            this.walletTransactionsIcon.Tag = "walletTransactionsPage";
            this.walletTransactionsIcon.Text = "Wallet Transactions";
            this.walletTransactionsIcon.ToolTipText = "Display wallet transactions";
            this.walletTransactionsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // jobsIcon
            // 
            this.jobsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.jobsIcon.Image = ((System.Drawing.Image)(resources.GetObject("jobsIcon.Image")));
            this.jobsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jobsIcon.Name = "jobsIcon";
            this.jobsIcon.Size = new System.Drawing.Size(28, 28);
            this.jobsIcon.Tag = "jobsPage";
            this.jobsIcon.Text = "Industry";
            this.jobsIcon.ToolTipText = "Display industry jobs";
            this.jobsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // loyaltyIcon
            // 
            this.loyaltyIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loyaltyIcon.Image = ((System.Drawing.Image)(resources.GetObject("loyaltyIcon.Image")));
            this.loyaltyIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loyaltyIcon.Name = "loyaltyIcon";
            this.loyaltyIcon.Size = new System.Drawing.Size(28, 28);
            this.loyaltyIcon.Tag = "loyaltyPage";
            this.loyaltyIcon.Text = "Loyalty Points";
            this.loyaltyIcon.ToolTipText = "Display loyalty point balances";
            this.loyaltyIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // planetaryIcon
            // 
            this.planetaryIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.planetaryIcon.Image = ((System.Drawing.Image)(resources.GetObject("planetaryIcon.Image")));
            this.planetaryIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.planetaryIcon.Name = "planetaryIcon";
            this.planetaryIcon.Size = new System.Drawing.Size(28, 28);
            this.planetaryIcon.Tag = "planetaryPage";
            this.planetaryIcon.Text = "Planetary Colonies";
            this.planetaryIcon.ToolTipText = "Display planetary colonies";
            this.planetaryIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // researchIcon
            // 
            this.researchIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.researchIcon.Image = ((System.Drawing.Image)(resources.GetObject("researchIcon.Image")));
            this.researchIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.researchIcon.Name = "researchIcon";
            this.researchIcon.Size = new System.Drawing.Size(28, 28);
            this.researchIcon.Tag = "researchPage";
            this.researchIcon.Text = "Research";
            this.researchIcon.ToolTipText = "Display research points";
            this.researchIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // mailMessagesIcon
            // 
            this.mailMessagesIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mailMessagesIcon.Image = ((System.Drawing.Image)(resources.GetObject("mailMessagesIcon.Image")));
            this.mailMessagesIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mailMessagesIcon.Name = "mailMessagesIcon";
            this.mailMessagesIcon.Size = new System.Drawing.Size(28, 28);
            this.mailMessagesIcon.Tag = "mailMessagesPage";
            this.mailMessagesIcon.Text = "Mail";
            this.mailMessagesIcon.ToolTipText = "Display EVE mails";
            this.mailMessagesIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // eveNotificationsIcon
            // 
            this.eveNotificationsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eveNotificationsIcon.Image = ((System.Drawing.Image)(resources.GetObject("eveNotificationsIcon.Image")));
            this.eveNotificationsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eveNotificationsIcon.Name = "eveNotificationsIcon";
            this.eveNotificationsIcon.Size = new System.Drawing.Size(28, 28);
            this.eveNotificationsIcon.Tag = "eveNotificationsPage";
            this.eveNotificationsIcon.Text = "Notification";
            this.eveNotificationsIcon.ToolTipText = "Display EVE notifications";
            this.eveNotificationsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // calendarEventsIcon
            // 
            this.calendarEventsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.calendarEventsIcon.Image = ((System.Drawing.Image)(resources.GetObject("calendarEventsIcon.Image")));
            this.calendarEventsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.calendarEventsIcon.Name = "calendarEventsIcon";
            this.calendarEventsIcon.Size = new System.Drawing.Size(28, 28);
            this.calendarEventsIcon.Tag = "calendarEventsPage";
            this.calendarEventsIcon.Text = "Calendar";
            this.calendarEventsIcon.ToolTipText = "Display calendar events";
            this.calendarEventsIcon.Click += new System.EventHandler(this.toolbarIcon_Click);
            // 
            // featuresMenu
            // 
            this.featuresMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.featuresMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.featuresMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EnableAllToolStripMenuItem,
            this.DisableAllToolStripMenuItem,
            this.SelectionToolStripSeparator});
            this.featuresMenu.Image = ((System.Drawing.Image)(resources.GetObject("featuresMenu.Image")));
            this.featuresMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.featuresMenu.Name = "featuresMenu";
            this.featuresMenu.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.featuresMenu.Size = new System.Drawing.Size(37, 28);
            this.featuresMenu.Text = "More features";
            this.featuresMenu.ToolTipText = "Advanced features";
            this.featuresMenu.DropDownOpening += new System.EventHandler(this.featureMenu_DropDownOpening);
            this.featuresMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.featuresMenu_DropDownItemClicked);
            // 
            // EnableAllToolStripMenuItem
            // 
            this.EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem";
            this.EnableAllToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.EnableAllToolStripMenuItem.Text = "Enable All";
            this.EnableAllToolStripMenuItem.Click += new System.EventHandler(this.EnableAllToolStripMenuItem_Click);
            // 
            // DisableAllToolStripMenuItem
            // 
            this.DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem";
            this.DisableAllToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.DisableAllToolStripMenuItem.Text = "Disable All";
            this.DisableAllToolStripMenuItem.Click += new System.EventHandler(this.DisableAllToolStripMenuItem_Click);
            // 
            // SelectionToolStripSeparator
            // 
            this.SelectionToolStripSeparator.Name = "SelectionToolStripSeparator";
            this.SelectionToolStripSeparator.Size = new System.Drawing.Size(126, 6);
            // 
            // tsPagesSeparator
            // 
            this.tsPagesSeparator.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsPagesSeparator.Name = "tsPagesSeparator";
            this.tsPagesSeparator.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsPagesSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // toggleSkillsIcon
            // 
            this.toggleSkillsIcon.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleSkillsIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleSkillsIcon.Image = ((System.Drawing.Image)(resources.GetObject("toggleSkillsIcon.Image")));
            this.toggleSkillsIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleSkillsIcon.Name = "toggleSkillsIcon";
            this.toggleSkillsIcon.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toggleSkillsIcon.Size = new System.Drawing.Size(28, 28);
            this.toggleSkillsIcon.Text = "Toggle All Skills";
            this.toggleSkillsIcon.ToolTipText = "Toggle all skills";
            this.toggleSkillsIcon.Click += new System.EventHandler(this.toggleSkillsIcon_Click);
            // 
            // tsToggleSeparator
            // 
            this.tsToggleSeparator.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsToggleSeparator.Name = "tsToggleSeparator";
            this.tsToggleSeparator.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsToggleSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // searchTextTimer
            // 
            this.searchTextTimer.Interval = 300;
            this.searchTextTimer.Tick += new System.EventHandler(this.searchTextTimer_Tick);
            // 
            // borderPanel
            // 
            this.borderPanel.AutoSize = true;
            this.borderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.borderPanel.BackColor = System.Drawing.SystemColors.Window;
            this.borderPanel.Controls.Add(this.corePanel);
            this.borderPanel.Controls.Add(this.notificationList);
            this.borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderPanel.Location = new System.Drawing.Point(0, 56);
            this.borderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(2, 2, 1, 2);
            this.borderPanel.Size = new System.Drawing.Size(614, 275);
            this.borderPanel.TabIndex = 18;
            // 
            // corePanel
            // 
            this.corePanel.Controls.Add(this.multiPanel);
            this.corePanel.Controls.Add(this.warningLabel);
            this.corePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.corePanel.Location = new System.Drawing.Point(2, 74);
            this.corePanel.Margin = new System.Windows.Forms.Padding(0);
            this.corePanel.Name = "corePanel";
            this.corePanel.Padding = new System.Windows.Forms.Padding(1, 1, 2, 0);
            this.corePanel.Size = new System.Drawing.Size(611, 199);
            this.corePanel.TabIndex = 14;
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.loyaltyPage);
            this.multiPanel.Controls.Add(this.standingsPage);
            this.multiPanel.Controls.Add(this.skillsPage);
            this.multiPanel.Controls.Add(this.ordersPage);
            this.multiPanel.Controls.Add(this.skillQueuePage);
            this.multiPanel.Controls.Add(this.jobsPage);
            this.multiPanel.Controls.Add(this.researchPage);
            this.multiPanel.Controls.Add(this.mailMessagesPage);
            this.multiPanel.Controls.Add(this.eveNotificationsPage);
            this.multiPanel.Controls.Add(this.employmentPage);
            this.multiPanel.Controls.Add(this.contractsPage);
            this.multiPanel.Controls.Add(this.assetsPage);
            this.multiPanel.Controls.Add(this.walletJournalPage);
            this.multiPanel.Controls.Add(this.walletTransactionsPage);
            this.multiPanel.Controls.Add(this.factionalWarfareStatsPage);
            this.multiPanel.Controls.Add(this.contactsPage);
            this.multiPanel.Controls.Add(this.medalsPage);
            this.multiPanel.Controls.Add(this.killLogPage);
            this.multiPanel.Controls.Add(this.planetaryPage);
            this.multiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel.Location = new System.Drawing.Point(1, 18);
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.skillsPage;
            this.multiPanel.Size = new System.Drawing.Size(608, 181);
            this.multiPanel.TabIndex = 14;
            // 
            // loyaltyPage
            // 
            this.loyaltyPage.Controls.Add(this.loyaltyList);
            this.loyaltyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loyaltyPage.Location = new System.Drawing.Point(0, 0);
            this.loyaltyPage.Name = "loyaltyPage";
            this.loyaltyPage.Size = new System.Drawing.Size(608, 181);
            this.loyaltyPage.TabIndex = 18;
            this.loyaltyPage.Tag = "LoyaltyPoints";
            this.loyaltyPage.Text = "loyaltyPage";
            // 
            // loyaltyList
            // 
            this.loyaltyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loyaltyList.Location = new System.Drawing.Point(0, 0);
            this.loyaltyList.Name = "loyaltyList";
            this.loyaltyList.Size = new System.Drawing.Size(608, 181);
            this.loyaltyList.TabIndex = 0;
            // 
            // standingsPage
            // 
            this.standingsPage.Controls.Add(this.standingsList);
            this.standingsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.standingsPage.Location = new System.Drawing.Point(0, 0);
            this.standingsPage.Name = "standingsPage";
            this.standingsPage.Size = new System.Drawing.Size(608, 181);
            this.standingsPage.TabIndex = 7;
            this.standingsPage.Tag = "Standings";
            this.standingsPage.Text = "standingsPage";
            // 
            // standingsList
            // 
            this.standingsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.standingsList.Location = new System.Drawing.Point(0, 0);
            this.standingsList.Name = "standingsList";
            this.standingsList.Size = new System.Drawing.Size(608, 181);
            this.standingsList.TabIndex = 0;
            // 
            // skillsPage
            // 
            this.skillsPage.Controls.Add(this.skillsList);
            this.skillsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsPage.Location = new System.Drawing.Point(0, 0);
            this.skillsPage.Name = "skillsPage";
            this.skillsPage.Size = new System.Drawing.Size(608, 181);
            this.skillsPage.TabIndex = 0;
            this.skillsPage.Tag = "CharacterSheet";
            this.skillsPage.Text = "skillsPage";
            // 
            // skillsList
            // 
            this.skillsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillsList.Location = new System.Drawing.Point(0, 0);
            this.skillsList.Margin = new System.Windows.Forms.Padding(0);
            this.skillsList.Name = "skillsList";
            this.skillsList.Size = new System.Drawing.Size(608, 181);
            this.skillsList.TabIndex = 12;
            // 
            // ordersPage
            // 
            this.ordersPage.Controls.Add(this.ordersList);
            this.ordersPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersPage.Location = new System.Drawing.Point(0, 0);
            this.ordersPage.Name = "ordersPage";
            this.ordersPage.Size = new System.Drawing.Size(564, 181);
            this.ordersPage.TabIndex = 2;
            this.ordersPage.Tag = "MarketOrders";
            this.ordersPage.Text = "ordersPage";
            // 
            // ordersList
            // 
            this.ordersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersList.Grouping = MarketOrderGrouping.State;
            this.ordersList.Location = new System.Drawing.Point(0, 0);
            this.ordersList.Name = "ordersList";
            this.ordersList.Size = new System.Drawing.Size(564, 181);
            this.ordersList.TabIndex = 13;
            this.ordersList.TextFilter = "";
            // 
            // skillQueuePage
            // 
            this.skillQueuePage.Controls.Add(this.skillQueueList);
            this.skillQueuePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueuePage.Location = new System.Drawing.Point(0, 0);
            this.skillQueuePage.Name = "skillQueuePage";
            this.skillQueuePage.Size = new System.Drawing.Size(568, 86);
            this.skillQueuePage.TabIndex = 1;
            this.skillQueuePage.Tag = "SkillQueue";
            this.skillQueuePage.Text = "skillQueuePage";
            // 
            // skillQueueList
            // 
            this.skillQueueList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillQueueList.Location = new System.Drawing.Point(0, 0);
            this.skillQueueList.Name = "skillQueueList";
            this.skillQueueList.Size = new System.Drawing.Size(568, 86);
            this.skillQueueList.TabIndex = 0;
            // 
            // jobsPage
            // 
            this.jobsPage.Controls.Add(this.jobsList);
            this.jobsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobsPage.Location = new System.Drawing.Point(0, 0);
            this.jobsPage.Name = "jobsPage";
            this.jobsPage.Size = new System.Drawing.Size(608, 181);
            this.jobsPage.TabIndex = 3;
            this.jobsPage.Tag = "IndustryJobs";
            this.jobsPage.Text = "jobsPage";
            // 
            // jobsList
            // 
            this.jobsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobsList.Grouping = IndustryJobGrouping.State;
            this.jobsList.Location = new System.Drawing.Point(0, 0);
            this.jobsList.Name = "jobsList";
            this.jobsList.Size = new System.Drawing.Size(608, 181);
            this.jobsList.TabIndex = 0;
            this.jobsList.TextFilter = "";
            // 
            // researchPage
            // 
            this.researchPage.Controls.Add(this.researchList);
            this.researchPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.researchPage.Location = new System.Drawing.Point(0, 0);
            this.researchPage.Name = "researchPage";
            this.researchPage.Size = new System.Drawing.Size(568, 86);
            this.researchPage.TabIndex = 4;
            this.researchPage.Tag = "ResearchPoints";
            this.researchPage.Text = "researchPage";
            // 
            // researchList
            // 
            this.researchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.researchList.Grouping = null;
            this.researchList.Location = new System.Drawing.Point(0, 0);
            this.researchList.Name = "researchList";
            this.researchList.Size = new System.Drawing.Size(568, 86);
            this.researchList.TabIndex = 0;
            this.researchList.TextFilter = "";
            // 
            // mailMessagesPage
            // 
            this.mailMessagesPage.Controls.Add(this.mailMessagesList);
            this.mailMessagesPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mailMessagesPage.Location = new System.Drawing.Point(0, 0);
            this.mailMessagesPage.Name = "mailMessagesPage";
            this.mailMessagesPage.Size = new System.Drawing.Size(608, 181);
            this.mailMessagesPage.TabIndex = 5;
            this.mailMessagesPage.Tag = "MailMessages";
            this.mailMessagesPage.Text = "mailMessagesPage";
            // 
            // mailMessagesList
            // 
            this.mailMessagesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mailMessagesList.Grouping = EVEMailMessagesGrouping.State;
            this.mailMessagesList.Location = new System.Drawing.Point(0, 0);
            this.mailMessagesList.Name = "mailMessagesList";
            this.mailMessagesList.Size = new System.Drawing.Size(608, 181);
            this.mailMessagesList.TabIndex = 0;
            this.mailMessagesList.TextFilter = "";
            // 
            // eveNotificationsPage
            // 
            this.eveNotificationsPage.Controls.Add(this.eveNotificationsList);
            this.eveNotificationsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eveNotificationsPage.Location = new System.Drawing.Point(0, 0);
            this.eveNotificationsPage.Name = "eveNotificationsPage";
            this.eveNotificationsPage.Size = new System.Drawing.Size(568, 86);
            this.eveNotificationsPage.TabIndex = 6;
            this.eveNotificationsPage.Tag = "Notifications";
            this.eveNotificationsPage.Text = "eveNotificationsPage";
            // 
            // eveNotificationsList
            // 
            this.eveNotificationsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eveNotificationsList.Grouping = EVENotificationsGrouping.Type;
            this.eveNotificationsList.Location = new System.Drawing.Point(0, 0);
            this.eveNotificationsList.Name = "eveNotificationsList";
            this.eveNotificationsList.Size = new System.Drawing.Size(568, 86);
            this.eveNotificationsList.TabIndex = 0;
            this.eveNotificationsList.TextFilter = "";
            // 
            // employmentPage
            // 
            this.employmentPage.Controls.Add(this.employmentList);
            this.employmentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employmentPage.Location = new System.Drawing.Point(0, 0);
            this.employmentPage.Name = "employmentPage";
            this.employmentPage.Size = new System.Drawing.Size(568, 156);
            this.employmentPage.TabIndex = 8;
            this.employmentPage.Tag = "EmploymentHistory";
            this.employmentPage.Text = "employmentPage";
            // 
            // employmentList
            // 
            this.employmentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employmentList.Location = new System.Drawing.Point(0, 0);
            this.employmentList.Name = "employmentList";
            this.employmentList.Size = new System.Drawing.Size(568, 156);
            this.employmentList.TabIndex = 0;
            // 
            // contractsPage
            // 
            this.contractsPage.Controls.Add(this.contractsList);
            this.contractsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contractsPage.Location = new System.Drawing.Point(0, 0);
            this.contractsPage.Name = "contractsPage";
            this.contractsPage.Size = new System.Drawing.Size(564, 181);
            this.contractsPage.TabIndex = 9;
            this.contractsPage.Tag = "Contracts";
            this.contractsPage.Text = "contractsPage";
            // 
            // contractsList
            // 
            this.contractsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contractsList.Grouping = ContractGrouping.State;
            this.contractsList.Location = new System.Drawing.Point(0, 0);
            this.contractsList.Name = "contractsList";
            this.contractsList.Size = new System.Drawing.Size(564, 181);
            this.contractsList.TabIndex = 0;
            this.contractsList.TextFilter = "";
            // 
            // assetsPage
            // 
            this.assetsPage.Controls.Add(this.assetsList);
            this.assetsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetsPage.Location = new System.Drawing.Point(0, 0);
            this.assetsPage.Name = "assetsPage";
            this.assetsPage.Size = new System.Drawing.Size(568, 308);
            this.assetsPage.TabIndex = 10;
            this.assetsPage.Tag = "AssetList";
            this.assetsPage.Text = "assetsPage";
            // 
            // assetsList
            // 
            this.assetsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetsList.Grouping = AssetGrouping.None;
            this.assetsList.Location = new System.Drawing.Point(0, 0);
            this.assetsList.Name = "assetsList";
            this.assetsList.Size = new System.Drawing.Size(568, 308);
            this.assetsList.TabIndex = 0;
            this.assetsList.TextFilter = "";
            // 
            // walletJournalPage
            // 
            this.walletJournalPage.Controls.Add(this.walletJournalList);
            this.walletJournalPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walletJournalPage.Location = new System.Drawing.Point(0, 0);
            this.walletJournalPage.Name = "walletJournalPage";
            this.walletJournalPage.Size = new System.Drawing.Size(568, 86);
            this.walletJournalPage.TabIndex = 11;
            this.walletJournalPage.Tag = "WalletJournal";
            this.walletJournalPage.Text = "walletJournalPage";
            // 
            // walletJournalList
            // 
            this.walletJournalList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walletJournalList.Grouping = WalletJournalGrouping.None;
            this.walletJournalList.Location = new System.Drawing.Point(0, 0);
            this.walletJournalList.Name = "walletJournalList";
            this.walletJournalList.Size = new System.Drawing.Size(568, 86);
            this.walletJournalList.TabIndex = 0;
            this.walletJournalList.TextFilter = "";
            // 
            // walletTransactionsPage
            // 
            this.walletTransactionsPage.Controls.Add(this.walletTransactionsList);
            this.walletTransactionsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walletTransactionsPage.Location = new System.Drawing.Point(0, 0);
            this.walletTransactionsPage.Name = "walletTransactionsPage";
            this.walletTransactionsPage.Size = new System.Drawing.Size(568, 86);
            this.walletTransactionsPage.TabIndex = 12;
            this.walletTransactionsPage.Tag = "WalletTransactions";
            this.walletTransactionsPage.Text = "walletTransactionsPage";
            // 
            // walletTransactionsList
            // 
            this.walletTransactionsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.walletTransactionsList.Grouping = WalletTransactionGrouping.None;
            this.walletTransactionsList.Location = new System.Drawing.Point(0, 0);
            this.walletTransactionsList.Name = "walletTransactionsList";
            this.walletTransactionsList.Size = new System.Drawing.Size(568, 86);
            this.walletTransactionsList.TabIndex = 0;
            this.walletTransactionsList.TextFilter = "";
            // 
            // factionalWarfareStatsPage
            // 
            this.factionalWarfareStatsPage.Controls.Add(this.factionalWarfareStatsList);
            this.factionalWarfareStatsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factionalWarfareStatsPage.Location = new System.Drawing.Point(0, 0);
            this.factionalWarfareStatsPage.Name = "factionalWarfareStatsPage";
            this.factionalWarfareStatsPage.Size = new System.Drawing.Size(568, 156);
            this.factionalWarfareStatsPage.TabIndex = 13;
            this.factionalWarfareStatsPage.Tag = "FactionalWarfareStats";
            this.factionalWarfareStatsPage.Text = "factionalWarfareStatsPage";
            // 
            // factionalWarfareStatsList
            // 
            this.factionalWarfareStatsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factionalWarfareStatsList.Location = new System.Drawing.Point(0, 0);
            this.factionalWarfareStatsList.Name = "factionalWarfareStatsList";
            this.factionalWarfareStatsList.Size = new System.Drawing.Size(568, 156);
            this.factionalWarfareStatsList.TabIndex = 0;
            // 
            // contactsPage
            // 
            this.contactsPage.Controls.Add(this.contactsList);
            this.contactsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contactsPage.Location = new System.Drawing.Point(0, 0);
            this.contactsPage.Name = "contactsPage";
            this.contactsPage.Size = new System.Drawing.Size(564, 181);
            this.contactsPage.TabIndex = 14;
            this.contactsPage.Tag = "ContactList";
            this.contactsPage.Text = "contactsPage";
            // 
            // contactsList
            // 
            this.contactsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contactsList.Location = new System.Drawing.Point(0, 0);
            this.contactsList.Name = "contactsList";
            this.contactsList.Size = new System.Drawing.Size(564, 181);
            this.contactsList.TabIndex = 0;
            // 
            // medalsPage
            // 
            this.medalsPage.Controls.Add(this.medalsList);
            this.medalsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.medalsPage.Location = new System.Drawing.Point(0, 0);
            this.medalsPage.Name = "medalsPage";
            this.medalsPage.Size = new System.Drawing.Size(608, 181);
            this.medalsPage.TabIndex = 15;
            this.medalsPage.Tag = "Medals";
            this.medalsPage.Text = "medalsPage";
            // 
            // medalsList
            // 
            this.medalsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.medalsList.Location = new System.Drawing.Point(0, 0);
            this.medalsList.Name = "medalsList";
            this.medalsList.Size = new System.Drawing.Size(608, 181);
            this.medalsList.TabIndex = 0;
            // 
            // killLogPage
            // 
            this.killLogPage.Controls.Add(this.killLogList);
            this.killLogPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.killLogPage.Location = new System.Drawing.Point(0, 0);
            this.killLogPage.Name = "killLogPage";
            this.killLogPage.Size = new System.Drawing.Size(608, 181);
            this.killLogPage.TabIndex = 16;
            this.killLogPage.Tag = "KillLog";
            this.killLogPage.Text = "killLogPage";
            // 
            // killLogList
            // 
            this.killLogList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.killLogList.Location = new System.Drawing.Point(0, 0);
            this.killLogList.Name = "killLogList";
            this.killLogList.Size = new System.Drawing.Size(608, 181);
            this.killLogList.TabIndex = 0;
            this.killLogList.TextFilter = "";
            // 
            // planetaryPage
            // 
            this.planetaryPage.Controls.Add(this.planetaryList);
            this.planetaryPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planetaryPage.Location = new System.Drawing.Point(0, 0);
            this.planetaryPage.Name = "planetaryPage";
            this.planetaryPage.Size = new System.Drawing.Size(608, 181);
            this.planetaryPage.TabIndex = 17;
            this.planetaryPage.Tag = "PlanetaryColonies";
            this.planetaryPage.Text = "planetaryPage";
            // 
            // planetaryList
            // 
            this.planetaryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planetaryList.Grouping = PlanetaryGrouping.Colony;
            this.planetaryList.Location = new System.Drawing.Point(0, 0);
            this.planetaryList.Name = "planetaryList";
            this.planetaryList.Size = new System.Drawing.Size(608, 181);
            this.planetaryList.TabIndex = 0;
            this.planetaryList.TextFilter = "";
            // 
            // warningLabel
            // 
            this.warningLabel.AutoEllipsis = true;
            this.warningLabel.BackColor = System.Drawing.Color.Black;
            this.warningLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.warningLabel.ForeColor = System.Drawing.Color.White;
            this.warningLabel.Image = ((System.Drawing.Image)(resources.GetObject("warningLabel.Image")));
            this.warningLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningLabel.Location = new System.Drawing.Point(1, 1);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(608, 17);
            this.warningLabel.TabIndex = 1;
            this.warningLabel.Text = "This character has no associated API key, data won\'t be updated.";
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // notificationList
            // 
            this.notificationList.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.notificationList.Location = new System.Drawing.Point(2, 2);
            this.notificationList.Margin = new System.Windows.Forms.Padding(0);
            this.notificationList.Name = "notificationList";
            this.notificationList.Size = new System.Drawing.Size(611, 72);
            this.notificationList.TabIndex = 13;
            // 
            // CharacterMonitorBody
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.borderPanel);
            this.Controls.Add(this.toolstripPanel);
            this.Name = "CharacterMonitorBody";
            this.Size = new System.Drawing.Size(614, 331);
            this.toolstripPanel.ResumeLayout(false);
            this.toolstripPanel.PerformLayout();
            this.toolStripContextual.ResumeLayout(false);
            this.toolStripContextual.PerformLayout();
            this.toolStripFeatures.ResumeLayout(false);
            this.toolStripFeatures.PerformLayout();
            this.borderPanel.ResumeLayout(false);
            this.corePanel.ResumeLayout(false);
            this.multiPanel.ResumeLayout(false);
            this.loyaltyPage.ResumeLayout(false);
            this.standingsPage.ResumeLayout(false);
            this.skillsPage.ResumeLayout(false);
            this.ordersPage.ResumeLayout(false);
            this.skillQueuePage.ResumeLayout(false);
            this.jobsPage.ResumeLayout(false);
            this.researchPage.ResumeLayout(false);
            this.mailMessagesPage.ResumeLayout(false);
            this.eveNotificationsPage.ResumeLayout(false);
            this.employmentPage.ResumeLayout(false);
            this.contractsPage.ResumeLayout(false);
            this.assetsPage.ResumeLayout(false);
            this.walletJournalPage.ResumeLayout(false);
            this.walletTransactionsPage.ResumeLayout(false);
            this.factionalWarfareStatsPage.ResumeLayout(false);
            this.contactsPage.ResumeLayout(false);
            this.medalsPage.ResumeLayout(false);
            this.killLogPage.ResumeLayout(false);
            this.planetaryPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel toolstripPanel;
        private System.Windows.Forms.ToolStrip toolStripContextual;
        private System.Windows.Forms.ToolStripDropDownButton preferencesMenu;
        private System.Windows.Forms.ToolStripMenuItem columnSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSizeColumnMenuItem;
        private System.Windows.Forms.ToolStripSeparator tsColumnSettingsSeparator;
        private System.Windows.Forms.ToolStripMenuItem hideInactiveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberAbsFormatMenuItem;
        private System.Windows.Forms.ToolStripSeparator tsOptionsSeparator;
        private System.Windows.Forms.ToolStripMenuItem showOnlyCharMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyCorpMenuItem;
        private System.Windows.Forms.ToolStripSeparator tsReadingPaneSeparator;
        private System.Windows.Forms.ToolStripMenuItem readingPaneMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paneRightMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paneBottomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paneOffMenuItem;
        private System.Windows.Forms.ToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripDropDownButton groupMenu;
        private System.Windows.Forms.ToolStripButton walletJournalCharts;
        private System.Windows.Forms.ToolStrip toolStripFeatures;
        private System.Windows.Forms.ToolStripButton skillsIcon;
        private System.Windows.Forms.ToolStripButton skillQueueIcon;
        private System.Windows.Forms.ToolStripButton employmentIcon;
        private System.Windows.Forms.ToolStripButton standingsIcon;
        private System.Windows.Forms.ToolStripButton factionalWarfareStatsIcon;
        private System.Windows.Forms.ToolStripButton assetsIcon;
        private System.Windows.Forms.ToolStripButton ordersIcon;
        private System.Windows.Forms.ToolStripButton contractsIcon;
        private System.Windows.Forms.ToolStripButton walletJournalIcon;
        private System.Windows.Forms.ToolStripButton walletTransactionsIcon;
        private System.Windows.Forms.ToolStripButton jobsIcon;
        private System.Windows.Forms.ToolStripButton researchIcon;
        private System.Windows.Forms.ToolStripButton mailMessagesIcon;
        private System.Windows.Forms.ToolStripButton eveNotificationsIcon;
        private System.Windows.Forms.ToolStripButton toggleSkillsIcon;
        private System.Windows.Forms.ToolStripSeparator tsToggleSeparator;
        private System.Windows.Forms.ToolStripDropDownButton featuresMenu;
        private System.Windows.Forms.ToolStripMenuItem EnableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DisableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator SelectionToolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator tsPagesSeparator;
        private Common.Controls.BorderPanel borderPanel;
        private System.Windows.Forms.Panel corePanel;
        private Common.Controls.MultiPanel.MultiPanel multiPanel;
        private Common.Controls.MultiPanel.MultiPanelPage standingsPage;
        private CharacterStandingsList standingsList;
        private Common.Controls.MultiPanel.MultiPanelPage skillsPage;
        private CharacterSkillsList skillsList;
        private Common.Controls.MultiPanel.MultiPanelPage ordersPage;
        private CharacterMarketOrdersList ordersList;
        private Common.Controls.MultiPanel.MultiPanelPage skillQueuePage;
        private CharacterSkillsQueueList skillQueueList;
        private Common.Controls.MultiPanel.MultiPanelPage jobsPage;
        private CharacterIndustryJobsList jobsList;
        private Common.Controls.MultiPanel.MultiPanelPage researchPage;
        private CharacterResearchPointsList researchList;
        private Common.Controls.MultiPanel.MultiPanelPage mailMessagesPage;
        private CharacterEveMailMessagesList mailMessagesList;
        private Common.Controls.MultiPanel.MultiPanelPage eveNotificationsPage;
        private CharacterEveNotificationsList eveNotificationsList;
        private Common.Controls.MultiPanel.MultiPanelPage employmentPage;
        private CharacterEmploymentHistoryList employmentList;
        private Common.Controls.MultiPanel.MultiPanelPage contractsPage;
        private CharacterContractsList contractsList;
        private Common.Controls.MultiPanel.MultiPanelPage assetsPage;
        private CharacterAssetsList assetsList;
        private Common.Controls.MultiPanel.MultiPanelPage walletJournalPage;
        private CharacterWalletJournalList walletJournalList;
        private Common.Controls.MultiPanel.MultiPanelPage walletTransactionsPage;
        private CharacterWalletTransactionsList walletTransactionsList;
        private Common.Controls.MultiPanel.MultiPanelPage factionalWarfareStatsPage;
        private CharacterFactionalWarfareStatsList factionalWarfareStatsList;
        private System.Windows.Forms.Label warningLabel;
        private Controls.NotificationList notificationList;
        private System.Windows.Forms.Timer searchTextTimer;
        private System.Windows.Forms.ToolStripButton contactsIcon;
        private Common.Controls.MultiPanel.MultiPanelPage contactsPage;
        private CharacterContactList contactsList;
        private System.Windows.Forms.ToolStripButton allContacts;
        private System.Windows.Forms.ToolStripButton contactsExcellent;
        private System.Windows.Forms.ToolStripButton contactsGood;
        private System.Windows.Forms.ToolStripButton contactsNeutral;
        private System.Windows.Forms.ToolStripButton contactsBad;
        private System.Windows.Forms.ToolStripButton contactsTerrible;
        private System.Windows.Forms.ToolStripButton inWatchList;
        private System.Windows.Forms.ToolStripButton medalsIcon;
        private System.Windows.Forms.ToolStripButton killLogIcon;
        private System.Windows.Forms.ToolStripButton calendarEventsIcon;
        private Common.Controls.MultiPanel.MultiPanelPage medalsPage;
        private CharacterMedalsList medalsList;
        private Common.Controls.MultiPanel.MultiPanelPage killLogPage;
        private CharacterKillLogList killLogList;
        private System.Windows.Forms.ToolStripSeparator combatLogSeparator;
        private System.Windows.Forms.ToolStripMenuItem combatLogMenuItem;
        private System.Windows.Forms.ToolStripButton searchTextDel;
        private System.Windows.Forms.ToolStripButton planetaryIcon;
        private Common.Controls.MultiPanel.MultiPanelPage planetaryPage;
        private CharacterPlanetaryList planetaryList;
        private System.Windows.Forms.ToolStripSeparator tsPlanetarySeparator;
        private System.Windows.Forms.ToolStripMenuItem showOnlyExtractorMenuItem;
        private System.Windows.Forms.ToolStripButton loyaltyIcon;
        private Common.Controls.MultiPanel.MultiPanelPage loyaltyPage;
        private CharacterLoyaltyList loyaltyList;
    }
}
