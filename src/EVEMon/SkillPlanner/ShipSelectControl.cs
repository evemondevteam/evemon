using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SkillPlanner
{
    public sealed partial class ShipSelectControl : EveObjectSelectControl
    {
        private Func<Item, bool> m_racePredicate = x => true;

        private bool m_init;


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShipSelectControl()
        {
            InitializeComponent();

            // Bind the contextmenu for masteries
            lbSearchList.ContextMenuStrip = contextMenu;
            UsabilityPredicate = SelectAll;
        }

        /// <summary>
        /// On load, we read the settings and fill the tree.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Call the base method
            base.OnLoad(e);

            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

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

            InitializeFilterControl();

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
            cbUsabilityFilter.Items.Add("All Ships");

            // On Data browser exit here
            if (Character == null)
                return;

            cbUsabilityFilter.Items.Add("Ships I can fly");
            cbUsabilityFilter.Items.Add("Ships I cannot fly");
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
                ShipBrowserSettings settings;

                // Skill Planner
                if (Plan != null)
                    settings = Settings.UI.ShipBrowser;
                // Character associated Data Browser
                else if (Character != null)
                    settings = Settings.UI.ShipCharacterDataBrowser;
                // Data Browser
                else
                    settings = Settings.UI.ShipDataBrowser;

                cbUsabilityFilter.SelectedIndex = (int)settings.UsabilityFilter;

                cbAmarr.Checked = (settings.RacesFilter & Race.Amarr) != Race.None;
                cbCaldari.Checked = (settings.RacesFilter & Race.Caldari) != Race.None;
                cbGallente.Checked = (settings.RacesFilter & Race.Gallente) != Race.None;
                cbMinmatar.Checked = (settings.RacesFilter & Race.Minmatar) != Race.None;
                cbFaction.Checked = (settings.RacesFilter & Race.Faction) != Race.None;
                cbORE.Checked = (settings.RacesFilter & Race.Ore) != Race.None;

                // See comment in cbRace_SelectedChanged for rationale behind this workaround
                m_racePredicate = x => ((x.Race == Race.None ? Race.Faction : x.Race) &
                    settings.RacesFilter) != Race.None;

                tbSearchText.Text = settings.TextSearch;
                lbSearchTextHint.Visible = string.IsNullOrEmpty(tbSearchText.Text);

                return;
            }

            cbUsabilityFilter.SelectedIndex = 0;
            cbAmarr.Checked = true;
            cbCaldari.Checked = true;
            cbGallente.Checked = true;
            cbMinmatar.Checked = true;
            cbFaction.Checked = true;
            cbORE.Checked = true;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the combo for filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the filter delegate
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

            ShipBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ShipBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ShipCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ShipDataBrowser;

            // Update settings
            settings.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;
        }

        /// <summary>
        /// When one of the races combo is checked/unchecked, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRace_SelectedChanged(object sender, EventArgs e)
        {
            // Update the control's content
            if (!m_init)
                return;

            // Retrieve the race
            Race race = Race.None;
            if (cbAmarr.Checked)
                race |= Race.Amarr;
            if (cbCaldari.Checked)
                race |= Race.Caldari;
            if (cbGallente.Checked)
                race |= Race.Gallente;
            if (cbMinmatar.Checked)
                race |= Race.Minmatar;
            if (cbFaction.Checked)
                race |= Race.Faction;
            if (cbORE.Checked)
                race |= Race.Ore;

            // Update the predicate
            // Substitute Faction for "no race" since the CCP data dump has the CONCORD faction
            // ships with a NULL race (please fix CCP!)
            m_racePredicate = x => ((x.Race == Race.None ? Race.Faction : x.Race) & race) !=
                Race.None;

            // Update content
            UpdateContent();

            ShipBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ShipBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ShipCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ShipDataBrowser;

            // Update the settings
            settings.RacesFilter |= race;
        }

        /// <summary>
        /// When the search text changed, we store the next settings
        /// and update the list view and the list/tree visibilities.
        /// </summary>
        protected override void OnSearchTextChanged()
        {
            base.OnSearchTextChanged();

            ShipBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.ShipBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.ShipCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.ShipDataBrowser;

            settings.TextSearch = tbSearchText.Text;
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

            if (StaticItems.ShipsMarketGroup == null)
                return;

            int numberOfItems = 0;
            tvItems.BeginUpdate();
            try
            {
                tvItems.Nodes.Clear();

                // Create the nodes
                foreach (MarketGroup group in StaticItems.ShipsMarketGroup.SubGroups)
                {
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
                    foreach (TreeNode node in tvItems.GetAllNodes().Where(node => node.Tag.GetHashCode() == selectedItemHash))
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
        /// Create the tree nodes for the given group and add them to the given nodes collection
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
            foreach (TreeNode node in group.Items.Where(
                x => UsabilityPredicate(x) && m_racePredicate(x)).Select(
                    childItem => new TreeNode { Text = childItem.Name, Tag = childItem }))
            {
                nodeCollection.Add(node);
                result++;
            }
            return result;
        }

        #endregion
    }
}
