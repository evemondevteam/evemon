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
    public partial class BlueprintSelectControl : EveObjectSelectControl
    {
        private Func<Item, bool> m_metaGroupPredicate = x => true;

        private bool m_init;


        #region Initialization

        public BlueprintSelectControl()
        {
            InitializeComponent();
            UsabilityPredicate = SelectAll;
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
            cbUsabilityFilter.Items.Add("All Blueprints");

            // On Data browser exit here
            if (Character == null)
                return;

            cbUsabilityFilter.Items.Add("Blueprints I can use");
            cbUsabilityFilter.Items.Add("Blueprints I cannot use");
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
                BlueprintBrowserSettings settings;

                // Skill Planner
                if (Plan != null)
                    settings = Settings.UI.BlueprintBrowser;
                // Character associated Data Browser
                else if (Character != null)
                    settings = Settings.UI.BlueprintCharacterDataBrowser;
                // Data Browser
                else
                    settings = Settings.UI.BlueprintDataBrowser;

                cbUsabilityFilter.SelectedIndex = (int)settings.UsabilityFilter;
                cbActivityFilter.SelectedIndex = (int)settings.ActivityFilter;

                cbTech1.Checked = (settings.MetagroupFilter & ItemMetaGroup.T1) != ItemMetaGroup.None;
                cbTech2.Checked = (settings.MetagroupFilter & ItemMetaGroup.T2) != ItemMetaGroup.None;
                cbTech3.Checked = (settings.MetagroupFilter & ItemMetaGroup.T3) != ItemMetaGroup.None;
                cbFaction.Checked = (settings.MetagroupFilter & ItemMetaGroup.Faction) != ItemMetaGroup.None;
                cbStoryline.Checked = (settings.MetagroupFilter & ItemMetaGroup.Storyline) != ItemMetaGroup.None;
                cbOfficer.Checked = (settings.MetagroupFilter & ItemMetaGroup.Officer) != ItemMetaGroup.None;

                m_metaGroupPredicate = x => (x.MetaGroup & settings.MetagroupFilter) != ItemMetaGroup.None;

                tbSearchText.Text = settings.TextSearch;
                lbSearchTextHint.Visible = string.IsNullOrEmpty(tbSearchText.Text);

                return;
            }

            cbUsabilityFilter.SelectedIndex = 0;
            cbActivityFilter.SelectedIndex = 0;
            cbTech1.Checked = true;
            cbTech2.Checked = true;
            cbTech3.Checked = true;
            cbFaction.Checked = true;
            cbStoryline.Checked = true;
            cbOfficer.Checked = true;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the combo for filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged();
        }

        /// <summary>
        /// When the combo for activity filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbActivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivityFilter = (ObjectActivityFilter)cbActivityFilter.SelectedIndex;

            switch (ActivityFilter)
            {
                case ObjectActivityFilter.Manufacturing:
                    Activity = BlueprintActivity.Manufacturing;
                    break;

                case ObjectActivityFilter.Copying:
                    Activity = BlueprintActivity.Copying;
                    break;

                case ObjectActivityFilter.ResearchingMaterialEfficiency:
                    Activity = BlueprintActivity.ResearchingMaterialEfficiency;
                    break;

                case ObjectActivityFilter.ResearchingTimeEfficiency:
                    Activity = BlueprintActivity.ResearchingTimeEfficiency;
                    break;

                case ObjectActivityFilter.Invention:
                    Activity = BlueprintActivity.Invention;
                    break;

                default:
                    Activity = BlueprintActivity.None;
                    break;
            }

            OnSelectedIndexChanged();

            BlueprintBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.BlueprintBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.BlueprintCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.BlueprintDataBrowser;

            // Update settings
            settings.ActivityFilter = ActivityFilter;
        }

        /// <summary>
        /// When one of the metagroups combo is checked/unchecked, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMetagroup_CheckedChanged(object sender, EventArgs e)
        {
            // Update the control's content
            if (!m_init)
                return;

            // Retrieve the metagroup
            ItemMetaGroup metagroup = ItemMetaGroup.None;
            if (cbTech1.Checked)
                metagroup |= ItemMetaGroup.T1;
            if (cbTech2.Checked)
                metagroup |= ItemMetaGroup.T2;
            if (cbTech3.Checked)
                metagroup |= ItemMetaGroup.T3;
            if (cbFaction.Checked)
                metagroup |= ItemMetaGroup.Faction;
            if (cbStoryline.Checked)
                metagroup |= ItemMetaGroup.Storyline;
            if (cbOfficer.Checked)
                metagroup |= ItemMetaGroup.Officer;

            // Update the predicate
            m_metaGroupPredicate = x => (x.MetaGroup & metagroup) != ItemMetaGroup.None;

            // Update content
            UpdateContent();

            BlueprintBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.BlueprintBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.BlueprintCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.BlueprintDataBrowser;

            // Update the settings
            settings.MetagroupFilter |= metagroup;
        }

        /// <summary>
        /// When the search text changed, we store the next settings
        /// and update the list view and the list/tree visibilities.
        /// </summary>
        protected override void OnSearchTextChanged()
        {
            base.OnSearchTextChanged();

            BlueprintBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.BlueprintBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.BlueprintCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.BlueprintDataBrowser;

            settings.TextSearch = tbSearchText.Text;
        }

        /// <summary>
        /// Called when the selected index changed.
        /// </summary>
        private void OnSelectedIndexChanged()
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

            // Enable/Disable the activity filter
            cbActivityFilter.Enabled = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex != ObjectUsabilityFilter.All;

            // Update the control's content
            if (!m_init)
                return;

            // Update content
            UpdateContent();

            BlueprintBrowserSettings settings;

            // Skill Planner
            if (Plan != null)
                settings = Settings.UI.BlueprintBrowser;
            // Character associated Data Browser
            else if (Character != null)
                settings = Settings.UI.BlueprintCharacterDataBrowser;
            // Data Browser
            else
                settings = Settings.UI.BlueprintDataBrowser;

            // Update settings
            settings.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;
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
                foreach (BlueprintMarketGroup group in StaticBlueprints.BlueprintMarketGroups)
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
        /// Create the tree nodes for the given category and add them to the given nodes collection
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodeCollection"></param>
        /// <returns></returns>
        private int BuildSubtree(BlueprintMarketGroup group, TreeNodeCollection nodeCollection)
        {
            // Total blueprints count in this category and its subcategories
            int result = 0;

            // Add all subcategories
            foreach (BlueprintMarketGroup childGroup in group.SubGroups)
            {
                TreeNode node = new TreeNode
                {
                    Text = childGroup.Name,
                    Tag = childGroup
                };

                // Add this subcategory's blueprints count
                result += BuildSubtree(childGroup, node.Nodes);

                // Only add if this subcategory has children
                if (node.GetNodeCount(true) > 0)
                    nodeCollection.Add(node);
            }

            // Add all blueprints
            foreach (TreeNode node in group.Blueprints.Where(
                x => UsabilityPredicate(x) && m_metaGroupPredicate(x)).Select(
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
