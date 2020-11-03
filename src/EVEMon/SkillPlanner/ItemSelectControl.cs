using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SkillPlanner
{
    public partial class ItemSelectControl : EveObjectSelectControl
    {
        private readonly List<MarketGroup> m_presetGroups = new List<MarketGroup>();
        private readonly List<ItemMetaGroup> m_metaGroups = new List<ItemMetaGroup>();

        private Func<Item, bool> m_slotPredicate = x => true;
        private Func<Item, bool> m_metaGroupPredicate = x => true;
        private Func<Item, bool> m_fittingPredicate = x => true;

        private bool m_init;


        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemSelectControl()
        {
            InitializeComponent();

            // Initialize the search text timer
            SearchTextTimer = new Timer { Interval = 300 };
        }

        /// <summary>
        /// On load, we read the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Call the base method
            base.OnLoad(e);

            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Subscribe the 'ItemCheck' event
            ccbGroupFilter.ItemCheck += ccbGroupFilter_ItemCheck;

            // Initialize the filters controls
            InitializeFiltersControls();

            // Update the control's content
            UpdateContent();
        }

        /// <summary>
        /// Initializes the filters controls.
        /// </summary>
        protected override void InitializeFiltersControls()
        {
            m_init = false;

            // Set the preset groups
            m_presetGroups.Clear();
            if (StaticItems.MarketGroups.Any())
            {
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.AmmosAndChargesMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ImplantsAndBoostersMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.StarbaseStructuresMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ShipModificationsMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ShipEquipmentsMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.DronesMarketGroupID));
            }

            InitializeFilterControl();
            InitializeGroupControl();

            InitiliazeSelectedIndexes();

            m_init = true;
        }

        /// <summary>
        /// Initializes the filter control.
        /// </summary>
        private void InitializeFilterControl()
        {
            // Initialize the usability filter combo box
            cbUsabilityFilter.Items.Clear();
            cbUsabilityFilter.Items.Add("All Items");

            // On Data browser exit here
            if (Character == null)
                return;

            cbUsabilityFilter.Items.Add("Items I can use");
            cbUsabilityFilter.Items.Add("Items I cannot use");
        }

        /// <summary>
        /// Initializes the group control.
        /// </summary>
        private void InitializeGroupControl()
        {
            // Metagroups combo
            m_metaGroups.Clear();
            m_metaGroups.AddRange(EnumExtensions.GetBitValues<ItemMetaGroup>());

            // Initialize the metagroup combo
            ccbGroupFilter.Items.Clear();
            ccbGroupFilter.Items.AddRange(m_metaGroups.Cast<object>().ToArray());
            ccbGroupFilter.ToolTip = toolTip;
        }

        /// <summary>
        /// Initiliazes the selected indexes.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void InitiliazeSelectedIndexes()
        {
            // Read the settings
            if (Settings.UI.UseStoredSearchFilters)
            {
                ItemBrowserSettings settings;

                // Skill Planner
                if (Plan != null)
                    settings = Settings.UI.ItemBrowser;
                // Character associated Data Browser
                else if (Character != null)
                    settings = Settings.UI.ItemCharacterDataBrowser;
                // Data Browser
                else
                    settings = Settings.UI.ItemDataBrowser;

                // Usability combo
                cbUsabilityFilter.SelectedIndex = (int)settings.UsabilityFilter;

                // Slots combo
                switch (settings.SlotFilter)
                {
                    case ItemSlot.All:
                        cbSlotFilter.SelectedIndex = 0;
                        break;
                    case ItemSlot.High:
                        cbSlotFilter.SelectedIndex = 1;
                        break;
                    case ItemSlot.Medium:
                        cbSlotFilter.SelectedIndex = 2;
                        break;
                    case ItemSlot.Low:
                        cbSlotFilter.SelectedIndex = 3;
                        break;
                    case ItemSlot.NoSlot:
                        cbSlotFilter.SelectedIndex = 4;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                m_slotPredicate = x => settings.SlotFilter == ItemSlot.All || (x.FittingSlot & settings.SlotFilter) != ItemSlot.None;

                for (int i = 0; i < m_metaGroups.Count; i++)
                {
                    ccbGroupFilter.SetItemChecked(i, isChecked: false);
                    ccbGroupFilter.SetItemChecked(i, (settings.MetagroupFilter & m_metaGroups[i]) != ItemMetaGroup.None);
                }
                m_metaGroupPredicate = x => (x.MetaGroup & settings.MetagroupFilter) != ItemMetaGroup.None;

                tbSearchText.Text = settings.TextSearch;
                lbSearchTextHint.Visible = string.IsNullOrEmpty(tbSearchText.Text);

                return;
            }

            cbUsabilityFilter.SelectedIndex = 0;
            cbSlotFilter.SelectedIndex = 0;
            for (int i = 0; i < m_metaGroups.Count; i++)
            {
                ccbGroupFilter.SetItemChecked(i, true);
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the search text changed, we store the next settings
        /// and update the list view and the list/tree visibilities.
        /// </summary>
        protected override void OnSearchTextChanged()
        {
            base.OnSearchTextChanged();

            ItemBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ItemBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ItemCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ItemDataBrowser;

            settings.TextSearch = tbSearchText.Text;
        }

        /// <summary>
        /// When the skill filter combo is changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the predicate
            switch ((ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex)
            {
                case ObjectUsabilityFilter.All:
                    UsabilityPredicate = SelectAll;
                    break;
                case ObjectUsabilityFilter.Usable:
                    UsabilityPredicate = CanUse;
                    break;
                case ObjectUsabilityFilter.Unusable:
                    UsabilityPredicate = CannotUse;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Update the control's content
            if (!m_init)
                return;

            // Update content
            UpdateContent();

            ItemBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ItemBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ItemCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ItemDataBrowser;

            // Update the settings
            settings.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;
        }

        /// <summary>
        /// When the slot filter combo changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSlotFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the control's content
            if (!m_init)
                return;

            // Update the predicate
            ItemSlot slot;

            switch (cbSlotFilter.SelectedIndex)
            {
                case 0:
                    slot = ItemSlot.All;
                    break;
                case 1:
                    slot = ItemSlot.High;
                    break;
                case 2:
                    slot = ItemSlot.Medium;
                    break;
                case 3:
                    slot = ItemSlot.Low;
                    break;
                case 4:
                    slot = ItemSlot.NoSlot;
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            m_slotPredicate = x => slot == ItemSlot.All || (x.FittingSlot & slot) != ItemSlot.None;

            // Update content
            UpdateContent();

            ItemBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ItemBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ItemCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ItemDataBrowser;

            // Update the settings
            settings.SlotFilter = slot;
        }

        /// <summary>
        /// When the meta group combo changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ccbGroupFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Update the control's content
            if (!m_init)
                return;

            // Update the predicate
            ItemMetaGroup filter = m_metaGroups
                .Where((t, i) => ccbGroupFilter.GetItemChecked(i))
                .Aggregate(ItemMetaGroup.None, (current, t) => current | t);

            m_metaGroupPredicate = x => (x.MetaGroup & filter) != ItemMetaGroup.None;

            // Update content
            UpdateContent();

            ItemBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ItemBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ItemCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ItemDataBrowser;

            settings.MetagroupFilter = filter;
        }

        /// <summary>
        /// When the CPU's numeric box changed, we update the predicate, the content (no settings)
        /// and the numeric box's availability.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCPU_CheckedChanged(object sender, EventArgs e)
        {
            numCPU.Enabled = cbCPU.Checked;
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the powergrid's numeric box changed, we update the predicate, the content (no settings)
        /// and the numeric box's availability.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPowergrid_CheckedChanged(object sender, EventArgs e)
        {
            numPowergrid.Enabled = cbPowergrid.Checked;
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the CPU's numeric box changed, we update the predicate and the content (no settings).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numCPU_ValueChanged(object sender, EventArgs e)
        {
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the powergrid's numeric box changed, we update the predicate and the content (no settings).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numPowergrid_ValueChanged(object sender, EventArgs e)
        {
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the "show all items" checkbox changed, we update the settings and trigger a content update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAllGroupsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateContent();

            ItemBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ItemBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ItemCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ItemDataBrowser;

            settings.ShowAllGroups = showAllGroupsCheckbox.Checked;
        }

        /// <summary>
        /// Updates the fitting predicate.
        /// </summary>
        private void UpdateFittingPredicate()
        {
            if (!numCPU.Enabled && !numPowergrid.Enabled)
                m_fittingPredicate = x => true;
            else
            {
                double? gridAvailable = null;
                if (numPowergrid.Enabled)
                    gridAvailable = (double)numPowergrid.Value;

                double? cpuAvailable = null;
                if (numCPU.Enabled)
                    cpuAvailable = (double)numCPU.Value;

                m_fittingPredicate = item => item.CanActivate(cpuAvailable, gridAvailable);
            }
        }

        #endregion


        #region Content creation

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        protected override void BuildTreeView()
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = tvItems.SelectedNode?.Tag?.GetHashCode() ?? 0;

            int numberOfItems = 0;
            tvItems.BeginUpdate();
            try
            {
                tvItems.Nodes.Clear();

                // Create the nodes
                foreach (MarketGroup group in StaticItems.MarketGroups)
                {
                    // Skip some groups
                    if (!showAllGroupsCheckbox.Checked && !m_presetGroups.Contains(group))
                        continue;

                    TreeNode node = new TreeNode
                    {
                        Text = group.Name,
                        Tag = group
                    };

                    int result = BuildSubtree(group, node.Nodes);

                    if (result == 0)
                        continue;

                    numberOfItems += result;
                    tvItems.Nodes.Add(node);
                }

                TreeNode selectedNode = null;

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (TreeNode node in tvItems.GetAllNodes()
                        .Where(node => node.Tag.GetHashCode() == selectedItemHash))
                    {
                        tvItems.SelectNodeWithTag(node.Tag);
                        selectedNode = node;
                    }
                }

                if (selectedNode != null)
                    return;

                // Reset if the node doesn't exist anymore
                tvItems.SelectNodeWithTag(null);
                SelectedObject = null;
            }
            finally
            {
                tvItems.EndUpdate();
                AllExpanded = false;

                // If the filtered set is small enough to fit all nodes on screen, call expandAll()
                if (numberOfItems < tvItems.DisplayRectangle.Height / tvItems.ItemHeight)
                {
                    tvItems.ExpandAll();
                    AllExpanded = true;
                }
            }
        }

        /// <summary>
        /// Create the tree nodes for the given category and add them to the given nodes collection.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodeCollection"></param>
        /// <returns></returns>
        private int BuildSubtree(MarketGroup group, TreeNodeCollection nodeCollection)
        {
            // Total items count in this category and its subcategories
            int result = 0;

            // Add all subcategories
            foreach (MarketGroup childGroup in group.SubGroups)
            {
                TreeNode node = new TreeNode
                {
                    Text = childGroup.Name,
                    Tag = childGroup
                };

                // Add this subcategory's items count
                result += BuildSubtree(childGroup, node.Nodes);

                // Only add if this subcategory has children
                if (node.GetNodeCount(true) > 0)
                    nodeCollection.Add(node);
            }

            // Add all items
            foreach (TreeNode node in group.Items
                .Where(x => UsabilityPredicate(x)
                            && m_slotPredicate(x)
                            && m_metaGroupPredicate(x)
                            && m_fittingPredicate(x))
                .Select(childItem =>
                    new TreeNode
                    {
                        Text = childItem.Name,
                        Tag = childItem
                    }))
            {
                nodeCollection.Add(node);
                result++;
            }

            return result;
        }

        #endregion
    }
}
