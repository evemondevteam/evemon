using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Presents the user with an interface in which a fitting tool
    /// exported ship loadout is parsed it into a TreeView
    /// which can be used to browse the items in the loadout.
    /// </summary>
    public partial class LoadoutImportationWindow : EVEMonForm
    {
        #region Fields

        private readonly List<Item> m_objects = new List<Item>();
        private readonly List<StaticSkillLevel> m_skillsToAdd = new List<StaticSkillLevel>();


        private Plan m_plan;
        private Character m_character;
        private LoadoutFormat m_loadoutFormat;
        private ILoadoutInfo m_loadoutInfo;
        private string m_clipboardText;
        private bool m_allExpanded;

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        private LoadoutImportationWindow()
        {
            InitializeComponent();
            RememberPositionKey = "LoadoutImportationForm";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <exception cref="System.ArgumentNullException">plan</exception>
        public LoadoutImportationWindow(Plan plan)
            : this()
        {
            plan.ThrowIfNull(nameof(plan));

            Plan = plan;

            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.PlanNameChanged += EveMonClient_PlanNameChanged;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Checks and pastes loadout from clipboard.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!Visible || ResultsTreeView.Nodes.Count > 0 || !Clipboard.ContainsText())
                return;

            m_clipboardText = Clipboard.GetText();

            if (!LoadoutHelper.IsLoadout(m_clipboardText, out m_loadoutFormat))
                return;

            UpdateExplanationLabel();

            BuildTreeView();
        }

        /// <summary>
        /// Unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.PlanNameChanged -= EveMonClient_PlanNameChanged;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the plan to which the extracted skills of the loadout should be added.
        /// </summary>
        internal Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;

                UpdateExplanationLabel();

                UpdatePlanStatus();
            }
        }

        #endregion


        #region Global Event Handlers

        /// <summary>
        /// When the plan changed, we need to update the training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan == m_plan)
                UpdatePlanStatus();
        }

        /// <summary>
        /// When the character changed, we need to update training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character == m_character)
                UpdatePlanStatus();
        }

        /// <summary>
        /// Occurs when a plan name changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlanChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_PlanNameChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan == m_plan)
                UpdateExplanationLabel();
        }

        #endregion


        #region Local Event Handlers

        /// <summary>
        /// Sets the DialogResult to Cancel and closes the form.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Adds the required skills to the Plan specified by the Plan property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlan_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryAddSet(m_skillsToAdd, m_loadoutInfo.Loadouts.First().Name);
            if (operation == null)
                return;

            PlanWindow planWindow = PlanWindow.ShowPlanWindow(plan: operation.Plan);
            if (planWindow == null)
                return;

            PlanHelper.Perform(new PlanToOperationWindow(operation), planWindow);
            planWindow.ShowPlanEditor();
            UpdatePlanStatus();
        }

        /// <summary>
        /// Browses the form that opened this instance of EFTLoadout to
        /// the item that was double clicked in the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_DoubleClick(object sender, EventArgs e)
        {
            Item item = ResultsTreeView.SelectedNode?.Tag as Item;

            PlanWindow.ShowPlanWindow(m_character, m_plan).ShowItemInBrowser(item);
        }

        /// <summary>
        /// Pops up the context menu for the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked
            if (e.Button != MouseButtons.Right)
                return;

            ResultsTreeView.Cursor = Cursors.Default;

            // Get the node that the user has clicked
            ResultsTreeView.SelectedNode = ResultsTreeView.GetNodeAt(e.Location);

            // Select the node the user has clicked
            contextMenu.Show(ResultsTreeView, e.Location);
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tvLoadout_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            ResultsTreeView.Cursor = ResultsTreeView.GetNodeAt(e.Location)?.Tag != null
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// When the context menu opens, we update the menus status.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = ResultsTreeView.Nodes.Count == 0;

            if (e.Cancel)
                return;

            TreeNode node = ResultsTreeView.SelectedNode;
            Item selectedItem = node?.Tag as Item;

            ShowInBrowserMenuItem.Visible = showInMenuSeparator.Visible = selectedItem != null;

            selectedSeparator.Visible = selectedItem == null && node != null;

            // "Collapse" and "Expand" menus
            cmiCollapseSelected.Visible = selectedItem == null && node != null && node.IsExpanded;
            cmiExpandSelected.Visible = selectedItem == null && node != null && !node.IsExpanded;

            cmiExpandSelected.Text = selectedItem == null && node != null && !node.IsExpanded
                ? $"Expand \"{node.Text}\""
                : string.Empty;
            cmiCollapseSelected.Text = selectedItem == null && node != null && node.IsExpanded
                ? $"Collapse \"{node.Text}\""
                : string.Empty;
            
            // "Expand All" and "Collapse All" menus
            cmiCollapseAll.Enabled = cmiCollapseAll.Visible = m_allExpanded;
            cmiExpandAll.Enabled = cmiExpandAll.Visible = !cmiCollapseAll.Enabled;
        }

        /// <summary>
        /// Treeview's context menu > Collapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            ResultsTreeView.SelectedNode.Collapse();
        }

        /// <summary>
        /// Treeview's context menu > Expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            ResultsTreeView.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Expand all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            ResultsTreeView.ExpandAll();
            m_allExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            ResultsTreeView.CollapseAll();
            m_allExpanded = false;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the explanation label.
        /// </summary>
        private void UpdateExplanationLabel()
        {
            if (m_loadoutFormat == LoadoutFormat.None)
                return;

            ExplanationLabel.Text = $"The parsed {m_loadoutFormat} formated loadout is shown below{Environment.NewLine}" +
                                    $"for \" {m_character.Name} [{m_plan.Name}] \" plan.";
        }

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        private void BuildTreeView()
        {
            m_objects.Clear();
            ResultsTreeView.Nodes.Clear();

            // Decode EFT format
            if (m_loadoutFormat == LoadoutFormat.EFT)
                m_loadoutInfo = LoadoutHelper.DeserializeEftFormat(m_clipboardText);

            // Decode XML format
            if (m_loadoutFormat == LoadoutFormat.XML)
                m_loadoutInfo = LoadoutHelper.DeserializeXmlFormat(m_clipboardText);

            // Decode DNA format
            if (m_loadoutFormat == LoadoutFormat.DNA)
                m_loadoutInfo = LoadoutHelper.DeserializeDnaFormat(m_clipboardText);

            // Decode CLF format
            if (m_loadoutFormat == LoadoutFormat.CLF)
                m_loadoutInfo = LoadoutHelper.DeserializeClfFormat(m_clipboardText);

            if (m_loadoutInfo == null || !m_loadoutInfo.Loadouts.Any())
                return;

            LoadoutNameLabel.Text = $"Name: {m_loadoutInfo.Loadouts.First().Name}{(m_loadoutFormat == LoadoutFormat.DNA ? " - DNA loadout" : string.Empty)}"
                .WordWrap(55);

            ShipTypeNameLabel.Text = $"Ship: {(m_loadoutInfo.Ship?.Name ?? string.Empty)}"
                .WordWrap(55);

            DescriptionLabel.Text = $"Description: {m_loadoutInfo.Loadouts.First().Description}"
                .WordWrap(55);

            m_objects.Add(m_loadoutInfo.Ship);

            BuildTreeNodes(m_loadoutInfo.Loadouts.First().Items);

            // Order the nodes
            TreeNode[] orderNodes = ResultsTreeView.Nodes.Cast<TreeNode>().OrderBy(
                node => LoadoutHelper.OrderedSlotNames.IndexOf(string.Intern(node.Text))).ToArray();
            ResultsTreeView.Nodes.Clear();
            ResultsTreeView.Nodes.AddRange(orderNodes);

            // Update the controls
            UpdatePlanStatus();
            ResultsTreeView.ExpandAll();
            ResultsTreeView.Enabled = true;
            Cursor = Cursors.Default;
            m_allExpanded = true;
        }

        /// <summary>
        /// Builds the tree nodes.
        /// </summary>
        /// <param name="items">The items.</param>
        private void BuildTreeNodes(IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                TreeNode slotNode;
                string nodeName = LoadoutHelper.OrderedSlotNames[7];

                // Regular item ?
                if (!item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID))
                {
                    // Retrieve the tree node name for the slot
                    switch (item.FittingSlot)
                    {
                        case ItemSlot.High:
                            nodeName = LoadoutHelper.OrderedSlotNames[0];
                            break;
                        case ItemSlot.Medium:
                            nodeName = LoadoutHelper.OrderedSlotNames[1];
                            break;
                        case ItemSlot.Low:
                            nodeName = LoadoutHelper.OrderedSlotNames[2];
                            break;
                    }

                    // Is it a rig?
                    if (item.MarketGroup.BelongsIn(DBConstants.ShipModificationsMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[3];
                    // Is it a subsystem?
                    else if (item.MarketGroup.BelongsIn(DBConstants.SubsystemsMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[4];
                    // Is it a drone?
                    else if (item.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[6];

                    // Gets or create the node for the slot
                    slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                        ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                        : ResultsTreeView.Nodes[nodeName];

                    // Add a new node
                    TreeNode itemNode = new TreeNode { Text = item.Name, Tag = item };
                    slotNode.Nodes.Add(itemNode);

                    m_objects.Add(item);

                    continue;
                }

                // Item is a charge ? 
                nodeName = LoadoutHelper.OrderedSlotNames[5];
                slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                    ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                    : ResultsTreeView.Nodes[nodeName];

                TreeNode ammoNode = new TreeNode { Text = item.Name, Tag = item };
                slotNode.Nodes.Add(ammoNode);

                m_objects.Add(item);
            }
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void UpdatePlanStatus()
        {
            if (m_loadoutFormat == LoadoutFormat.None)
                return;

            // Compute the skills to add
            m_skillsToAdd.Clear();
            CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);

            // Compute the training time for the prerequisites
            foreach (Item obj in m_objects)
            {
                scratchpad.Train(obj.Prerequisites.Where(x => m_character.Skills[x.Skill.ID].Level < x.Level));
            }
            m_skillsToAdd.AddRange(scratchpad.TrainedSkills);

            // All skills already trained ?
            if (m_skillsToAdd.Count == 0)
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = @"All skills already trained.";
                TrainTimeLabel.Visible = false;
            }
            // Are skills already planned ?
            else if (m_plan.AreSkillsPlanned(m_skillsToAdd))
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = @"All skills already trained or planned.";
                TrainTimeLabel.Visible = false;
            }
            // There is at least one untrained or non-planned skill
            else
            {
                AddToPlanButton.Enabled = true;
                PlanedLabel.Text = string.Empty;
                PlanedLabel.Visible = false;
                TrainTimeLabel.Visible = true;

                // Compute training time
                TimeSpan trainingTime = m_character.GetTrainingTimeToMultipleSkills(m_skillsToAdd);
                TrainTimeLabel.Text = trainingTime.ToDescriptiveText(
                    DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.SpaceText);
            }
        }

        #endregion
    }
}