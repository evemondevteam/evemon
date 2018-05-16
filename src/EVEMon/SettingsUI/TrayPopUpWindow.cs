using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.Models;
using EVEMon.Common.Models.Comparers;
using EVEMon.Controls;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// Popup form displayed when the user hovers over the tray icon.
    /// </summary>
    /// <remarks>
    /// Display contents are governed by Settings.TrayPopupConfig<br/>
    /// Popup location is determined using mouse location, screen and screen bounds (see SetPosition()).<br/>
    /// </remarks>
    public class TrayPopupWindow : TrayBaseWindow
    {
        private readonly Label m_eveTimeLabel = new Label();
        private readonly Label m_serverStatusLabel = new Label();


        #region Inherited Events

        /// <summary>
        /// Adds the character panes to the form, gets the TQ status message and sets the popup position.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            // Client events
            EveMonClient.QueuedSkillsCompleted += EveMonClient_QueuedSkillsCompleted;
            EveMonClient.ServerStatusUpdated += EveMonClient_ServerStatusUpdated;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Unregister events.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            EveMonClient.QueuedSkillsCompleted -= EveMonClient_QueuedSkillsCompleted;
            EveMonClient.ServerStatusUpdated -= EveMonClient_ServerStatusUpdated;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
        }

        #endregion


        #region Content management : add the controls to the panel, update them, etc

        /// <summary>
        /// Recreates the controls for character and warning.
        /// </summary>
        protected override void UpdateContent()
        {
            if (!Visible)
            {
                UpdatePending = true;
                return;
            }
            UpdatePending = false;

            // Controls layout
            PerformCustomLayout();

            // Supplemental controls layout
            PerformSupplementalControlsLayout();

            // Updates the tooltip width
            CompleteLayout();
        }

        /// <summary>
        /// Gets the characters list, sorted, grouped and filter according to the user settings.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Character> GetCharacters
        {
            get
            {
                List<Character> characters = EveMonClient.MonitoredCharacters.ToList();

                // Filter characters not in training ?
                if (!Settings.UI.SystemTrayPopup.ShowCharNotTraining)
                    characters = characters.Where(x => x.IsTraining).ToList();

                // Sort
                List<Character> charactersList = characters.ToList();
                charactersList.StableSort(new CharacterComparer(Settings.UI.SystemTrayPopup.SecondarySortOrder));
                charactersList.StableSort(new CharacterComparer(Settings.UI.SystemTrayPopup.PrimarySortOrder));

                // Grouping
                List<Character> newCharacters = new List<Character>();
                switch (Settings.UI.SystemTrayPopup.GroupBy)
                {
                    case TrayPopupGrouping.None:
                        return charactersList;
                    case TrayPopupGrouping.Account:
                        newCharacters.AddRange(charactersList.Where(x => x.Identity.ESIKeys.Any()));
                        return newCharacters.GroupBy(AccountAPIKeyOrDefault).SelectMany(y => y);
                    case TrayPopupGrouping.TrainingAtTop:
                        newCharacters.AddRange(charactersList.Where(x => x.IsTraining));
                        newCharacters.AddRange(charactersList.Where(x => !x.IsTraining));
                        return newCharacters;
                    case TrayPopupGrouping.TrainingAtBottom:
                        newCharacters.AddRange(charactersList.Where(x => !x.IsTraining));
                        newCharacters.AddRange(charactersList.Where(x => x.IsTraining));
                        return newCharacters;
                    default:
                        return characters;
                }
            }
        }

        /// <summary>
        /// Returns the API key for characters in the same account or the default one.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>The API key for characters in the same account; otherwise the default API key of the character</returns>
        private static ESIKey AccountAPIKeyOrDefault(Character character) => character.Identity.ESIKeys
            .First(apiKey => EveMonClient.MonitoredCharacters
                .Any(monitoredCharacter => monitoredCharacter.Identity.ESIKeys.Contains(apiKey)));

        /// <summary>
        /// Performs the custom layout.
        /// </summary>
        private void PerformCustomLayout()
        {
            // Remove controls and dispose them
            IEnumerable<Control> oldControls = MainFlowLayoutPanel.Controls.Cast<Control>().ToList();
            MainFlowLayoutPanel.Controls.Clear();
            foreach (Control ctl in oldControls)
            {
                ctl.Dispose();
            }

            IEnumerable<Character> characters = GetCharacters;

            // Add controls for characters
            if (Settings.UI.SystemTrayPopup.GroupBy == TrayPopupGrouping.Account &&
                Settings.UI.SystemTrayPopup.IndentGroupedAccounts)
            {
                List<ESIKey> prevAPIKeys = new List<ESIKey>();
                foreach (Character character in characters)
                {
                    OverviewItem charPanel = GetOverviewItem(character);
                    List<ESIKey> apiKeys = character.Identity.ESIKeys.ToList();

                    if (!apiKeys.Exists(apiKey => prevAPIKeys.Contains(apiKey)))
                    {
                        MainFlowLayoutPanel.Controls.Add(charPanel);
                        prevAPIKeys = apiKeys;
                    }
                    else
                    {
                        FlowLayoutPanel tempAccountGroupPanel = null;
                        try
                        {
                            tempAccountGroupPanel = new FlowLayoutPanel();
                            tempAccountGroupPanel.Controls.Add(charPanel);
                            tempAccountGroupPanel.AutoSize = true;
                            tempAccountGroupPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            tempAccountGroupPanel.FlowDirection = FlowDirection.TopDown;
                            tempAccountGroupPanel.Padding = new Padding(10, 0, 0, 0);

                            tempAccountGroupPanel.CreateControl();

                            FlowLayoutPanel accountGroupPanel = tempAccountGroupPanel;
                            tempAccountGroupPanel = null;

                            MainFlowLayoutPanel.Controls.Add(accountGroupPanel);
                        }
                        finally
                        {
                            tempAccountGroupPanel?.Dispose();
                        }

                        prevAPIKeys = apiKeys;
                    }
                }
            }
            else
                MainFlowLayoutPanel.Controls.AddRange(characters.Select(GetOverviewItem).ToArray<Control>());
        }

        /// <summary>
        /// Gets the overview item.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        private static OverviewItem GetOverviewItem(Character character)
        {
            OverviewItem overviewItem;
            OverviewItem tempOverviewItem = null;
            try
            {
                // Creates a new page
                tempOverviewItem = new OverviewItem(character, true);
                tempOverviewItem.Tag = character;

                tempOverviewItem.CreateControl();

                overviewItem = tempOverviewItem;
                tempOverviewItem = null;
            }
            finally
            {
                tempOverviewItem?.Dispose();
            }

            return overviewItem;
        }

        /// <summary>
        /// Performs the supplemental controls layout.
        /// </summary>
        private void PerformSupplementalControlsLayout()
        {
            // Skip if the user do not want to be warned about accounts not in training
            // No longer possible to determine since ESI keys are for only one character
#if false
            string warningMessage;
            if (Settings.UI.SystemTrayPopup.ShowWarning && ESIKey.HasCharactersNotTraining(out warningMessage))
            {
                FlowLayoutPanel warningPanel = CreateAccountsNotTrainingPanel(warningMessage);
                if (!MainFlowLayoutPanel.IsDisposed)
                    MainFlowLayoutPanel.Controls.Add(warningPanel);
            }
#endif

            // Server Status
            if (Settings.UI.SystemTrayPopup.ShowServerStatus)
            {
                m_serverStatusLabel.AutoSize = true;
                if (!MainFlowLayoutPanel.IsDisposed)
                {
                    MainFlowLayoutPanel.Controls.Add(m_serverStatusLabel);
                    UpdateServerStatusLabel();
                }
            }

            // EVE Time
            if (Settings.UI.SystemTrayPopup.ShowEveTime)
            {
                m_eveTimeLabel.AutoSize = true;
                if (!MainFlowLayoutPanel.IsDisposed)
                {
                    MainFlowLayoutPanel.Controls.Add(m_eveTimeLabel);
                    UpdateEveTimeLabel();
                }
            }
        }

#if false
        /// <summary>
        /// Creates a panel contains the warning message for accounts not in training.
        /// </summary>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        private static FlowLayoutPanel CreateAccountsNotTrainingPanel(string warningMessage)
        {
            // Create a flowlayout to hold the content
            FlowLayoutPanel warningPanel;
            FlowLayoutPanel tempWarningPanel = null;
            try
            {
                tempWarningPanel = new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Margin = new Padding(0, 0, 0, 2)
                };

                // Add a picture on the left with a warning icon
                if (!Settings.UI.SafeForWork)
                {
                    int portraitSize = Int32.Parse(Settings.UI.SystemTrayPopup.PortraitSize.ToString().Substring(1),
                                                   CultureConstants.InvariantCulture);

                    PictureBox tempPictureBoxWarning = null;
                    try
                    {
                        tempPictureBoxWarning = new PictureBox();
                        tempPictureBoxWarning.Image = SystemIcons.Warning.ToBitmap();
                        tempPictureBoxWarning.SizeMode = PictureBoxSizeMode.StretchImage;
                        tempPictureBoxWarning.Size = new Size(portraitSize, portraitSize);
                        tempPictureBoxWarning.Margin = new Padding(2);

                        PictureBox pbWarning = tempPictureBoxWarning;
                        tempPictureBoxWarning = null;

                        tempWarningPanel.Controls.Add(pbWarning);
                    }
                    finally
                    {
                        tempPictureBoxWarning?.Dispose();
                    }
                }

                // Adds a label to hold the message
                Label tempLabelMessage = null;
                try
                {
                    tempLabelMessage = new Label
                    {
                        AutoSize = true,
                        Text = warningMessage
                    };

                    Label lblMessage = tempLabelMessage;
                    tempLabelMessage = null;

                    tempWarningPanel.Controls.Add(lblMessage);
                }
                finally
                {
                    tempLabelMessage?.Dispose();
                }

                tempWarningPanel.CreateControl();

                warningPanel = tempWarningPanel;
                tempWarningPanel = null;
            }
            finally
            {
                tempWarningPanel?.Dispose();
            }

            return warningPanel;
        }
#endif

        /// <summary>
        /// Completes the layout after new controls have been added.
        /// </summary>
        private void CompleteLayout()
        {
            // Quit if the panel was disposed
            if (MainFlowLayoutPanel.IsDisposed)
                return;

            // Fix the panel widths to the largest
            // We let the framework determine the appropriate widths, then fix them so that
            // updates to training time remaining don't cause the form to resize
            int pnlWidth = MainFlowLayoutPanel.Controls.Cast<Control>().Select(control => control.Width).Concat(new[] { 0 }).Max();

            foreach (FlowLayoutPanel flowPanel in MainFlowLayoutPanel.Controls.OfType<FlowLayoutPanel>())
            {
                flowPanel.AutoSize = false;
                flowPanel.Width = pnlWidth;
            }

            // Position Popup
            TrayIcon.SetToolTipLocation(this);
        }

        /// <summary>
        /// Updates the EVE time label.
        /// </summary>
        private void UpdateEveTimeLabel()
        {
            if (!Visible || m_eveTimeLabel == null)
                return;

            if (Settings.UI.SystemTrayPopup.ShowEveTime)
                m_eveTimeLabel.Text = $"EVE Time: {EveMonClient.EVEServer.ServerDateTime:HH:mm}";
        }

        /// <summary>
        /// Updates the server status label.
        /// </summary>
        private void UpdateServerStatusLabel()
        {
            if (!Visible || m_serverStatusLabel == null)
                return;

            if (Settings.UI.SystemTrayPopup.ShowServerStatus)
                m_serverStatusLabel.Text = EveMonClient.EVEServer.StatusText;
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Updates the TQ status message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_ServerStatusUpdated(object sender, EveServerEventArgs e)
        {
            UpdateServerStatusLabel();
        }

        /// <summary>
        /// Once per second, we update the eve time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateEveTimeLabel();
        }

        /// <summary>
        /// Occur when characters completed skills. We refresh the controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_QueuedSkillsCompleted(object sender, QueuedSkillsEventArgs e)
        {
            UpdateContent();
        }

        #endregion
    }
}
