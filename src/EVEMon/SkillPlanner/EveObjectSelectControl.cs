using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// The standard control to search for an item
    /// </summary>
    public partial class EveObjectSelectControl : UserControl
    {
        public event EventHandler SelectionChanged;

        private Character m_character;
        private Plan m_plan;
        private Timer m_searchTextTimer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EveObjectSelectControl"/> class.
        /// </summary>
        protected EveObjectSelectControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        /// <value>
        /// The character.
        /// </value>
        internal Character Character
        {
            get { return m_character; }
            set
            {
                if (value == null || m_character == value)
                    return;

                m_character = value;
            }
        }

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        internal Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                // Should we be transforming a Data Browser to a Skill Planner?
                bool transformToPlanner = (value != null) && (m_plan == null) && (m_character != null);

                if (value == null)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;

                // Transform a Data Browser to a Skill Planner
                if (!transformToPlanner)
                    return;

                InitializeFiltersControls();
                UpdateContent();
            }
        }

        /// <summary>
        /// Gets or sets the usability predicate.
        /// </summary>
        /// <value>The usability predicate.</value>
        protected Func<Item, bool> UsabilityPredicate { get; set; }

        /// <summary>
        /// Gets or sets the activity filter.
        /// </summary>
        /// <value>The activity filter.</value>
        protected ObjectActivityFilter ActivityFilter { get; set; }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        protected BlueprintActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all expanded].
        /// </summary>
        /// <value><c>true</c> if [all expanded]; otherwise, <c>false</c>.</value>
        protected bool AllExpanded { get; set; }

        /// <summary>
        /// Gets or sets the search text timer.
        /// </summary>
        /// <value>
        /// The search text timer.
        /// </value>
        protected Timer SearchTextTimer
        {
            get { return m_searchTextTimer; }
            set
            {
                m_searchTextTimer = value;

                if (m_searchTextTimer != null)
                    m_searchTextTimer.Tick += searchTextTimer_Tick;
            }
        }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        internal string SearchText
        {
            get { return tbSearchText.Text; }
            set
            {
                tbSearchText.Text = value ?? string.Empty;
                OnSearchTextChanged();
            }
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            UsabilityPredicate = SelectAll;

            // Subscribe the events
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;

            // Update the controls
            UpdateControlVisibility();
        }

        /// <summary>
        /// Occurs when the control visibility changed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!Visible)
                return;

            UpdateSearchTextHintVisibility();
        }

        /// <summary>
        /// Called when [disposed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Handles the SettingsChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            pbSearchImage.Visible = !Settings.UI.SafeForWork;
        }

        /// <summary>
        /// Updates the search text hint visibility.
        /// </summary>
        private void UpdateSearchTextHintVisibility()
        {
            lbSearchTextHint.Visible = !tbSearchText.Focused && string.IsNullOrEmpty(tbSearchText.Text);
        }

        #endregion


        #region Search

        /// <summary>
        /// Occurs when clicking on the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        /// <summary>
        /// Occurs upon entering the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        /// <summary>
        /// Occurs upon leaving the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Leave(object sender, EventArgs e)
        {
            UpdateSearchTextHintVisibility();
        }

        /// <summary>
        /// Handles the MouseUp event of the pbSearchTextDel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void pbSearchTextDel_MouseUp(object sender, MouseEventArgs e)
        {
            tbSearchText.Clear();
            UpdateSearchTextHintVisibility();
        }

        /// <summary>
        /// Occurs when the search text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            if (m_searchTextTimer == null)
            {
                OnSearchTextChanged();
                return;
            }

            if (m_searchTextTimer.Enabled)
                m_searchTextTimer.Stop();

            m_searchTextTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the searchTextTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void searchTextTimer_Tick(object sender, EventArgs e)
        {
            m_searchTextTimer.Stop();
            OnSearchTextChanged();
        }

        /// <summary>
        /// Occurs when pressing a key while inside the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            // (Ctrl + A) has KeyChar value 1
            if (e.KeyChar != (char)Keys.LButton)
                return;

            tbSearchText.SelectAll();
            e.Handled = true;
        }

        /// <summary>
        /// Updates the control when the search text changes.
        /// </summary>
        protected virtual void OnSearchTextChanged()
        {
            UpdateContent();
        }

        /// <summary>
        /// Initializes the filters controls.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual void InitializeFiltersControls()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Update Content

        /// <summary>
        /// Refresh the controls.
        /// </summary>
        protected void UpdateContent()
        {
            BuildTreeView();
            BuildListBox();
        }

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        protected virtual void BuildTreeView()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the tree node and extracts all the items to build the content of the list box. 
        /// It also deals with text filtering and the treeview/listbox visibility.
        /// </summary>
        protected virtual void BuildListBox()
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = tvItems.SelectedNode?.Tag?.GetHashCode() ?? 0;

            lbSearchList.Items.Clear();

            if (string.IsNullOrEmpty(tbSearchText.Text))
            {
                tvItems.Show();
                lbSearchList.Hide();
                lbNoMatches.Hide();
                return;
            }

            // Find everything in the current tree that matches the search string
            List<Item> filteredItems = new List<Item>();
            foreach (TreeNode n in tvItems.Nodes)
            {
                SearchNode(n, tbSearchText.Text, filteredItems);
            }

            filteredItems.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));

            lbSearchList.BeginUpdate();
            try
            {
                foreach (Item item in filteredItems)
                {
                    lbSearchList.Items.Add(item);

                    // Restore the selected node (if any)
                    if (selectedItemHash > 0 && item.GetHashCode() == selectedItemHash)
                        lbSearchList.SelectedItem = item;
                }
            }
            finally
            {
                lbSearchList.EndUpdate();
            }

            lbSearchList.Show();
            tvItems.Hide();
            lbNoMatches.Visible = !filteredItems.Any();
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="searchText"></param>
        /// <param name="filteredItems"></param>
        private static void SearchNode(TreeNode tn, string searchText, List<Item> filteredItems)
        {
            Item item = tn.Tag as Item;
            if (item == null)
            {
                foreach (TreeNode subNode in tn.Nodes)
                {
                    SearchNode(subNode, searchText, filteredItems);
                }
                return;
            }

            if (item.Name.Contains(searchText, ignoreCase: true)
                || item.Description.Contains(searchText, ignoreCase: true))
            {
                filteredItems.Add(item);
            }
        }
        
        #endregion

        
        #region Selected Objects

        /// <summary>
        /// All the selected objects (through multi-select).
        /// </summary>
        internal IEnumerable<Item> SelectedObjects { get; private set; }

        /// <summary>
        /// The primary selected object.
        /// </summary>
        internal Item SelectedObject
        {
            get
            {
                if (SelectedObjects == null || !SelectedObjects.Any())
                    return null;

                return SelectedObjects.First();
            }
            set
            {
                List<Item> selectedObjects = new List<Item>();
                if (value != null)
                    selectedObjects.Add(value);

                SetSelectedObjects(selectedObjects);
            }
        }

        /// <summary>
        /// Selects the given nodes.
        /// </summary>
        /// <param name="items"></param>
        private void SetSelectedObjects(IEnumerable<Item> items)
        {
            // Updates selection
            SelectedObjects = items == null ? new List<Item>() : new List<Item>(items);

            // Selects the proper nodes
            if (SelectedObjects.Count() == 1)
            {
                // If the object is not already selected
                Item obj = SelectedObjects.First();
                tvItems.SelectNodeWithTag(obj);
            }

            // Notify subscribers
            SelectionChanged?.ThreadSafeInvoke(this, new EventArgs());
        }

        #endregion


        #region Events

        /// <summary>
        /// Occurs when selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_SelectionsChanged(object sender, EventArgs e)
        {
            if (tvItems.SelectedNodes.Count != 0)
            {
                List<Item> selectedObjects = tvItems.SelectedNodes.Select(node => node.Tag).OfType<Item>().ToList();
                SetSelectedObjects(selectedObjects);
                return;
            }

            SetSelectedObjects(null);
        }

        /// <summary>
        /// When the mouse gets pressed, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void tvItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            tvItems.Cursor = Cursors.Default;
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tvItems_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            tvItems.Cursor = CustomCursors.ContextMenu;
        }

        /// <summary>
        /// Occurs when the search list selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSearchList.SelectedItems.Count != 0)
            {
                List<Item> selectedObjects = lbSearchList.SelectedItems.OfType<Item>().ToList();
                SetSelectedObjects(selectedObjects);
                return;
            }

            SetSelectedObjects(null);
        }

        /// <summary>
        /// Changes the selection when you right click on a search.
        /// </summary>
        /// <param name="sender">is lbSearchList</param>
        /// <param name="e"></param>
        private void lbSearchList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            lbSearchList.SelectedItems.Clear();
            lbSearchList.SelectedIndex = lbSearchList.IndexFromPoint(e.Location);
            lbSearchList.Cursor = Cursors.Default;
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbSearchList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            // If it's not the Ship Select Control we don't display a context menu,
            // hence there is no need to change the cursor
            if (!(this is ShipSelectControl))
                return;

            lbSearchList.Cursor = m_plan != null && lbSearchList.IndexFromPoint(e.Location) > -1
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// Treeview's context menu > Expand all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
            AllExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
            AllExpanded = false;
        }

        /// <summary>
        /// Treeview's context menu > Expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Collapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        /// <summary>
        /// Occurs upon opening the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStripControl = sender as ContextMenuStrip;

            e.Cancel = contextMenuStripControl?.SourceControl == null ||
                       (!contextMenuStripControl.SourceControl.Visible && SelectedObject == null) ||
                       (!tvItems.Visible && m_plan == null);

            if (e.Cancel || contextMenuStripControl?.SourceControl == null)
                return;
            
            contextMenuStripControl.SourceControl.Cursor = Cursors.Default;

            UpdateContextMenu();
        }

        /// <summary>
        /// Updates the context menu.
        /// </summary>
        private void UpdateContextMenu()
        {
            TreeNode node = tvItems.SelectedNode;

            // Special case for mastery ship levels
            PlanToMasteryLevel(node);

            // "Expand" and "Collapse" selected menu
            cmiExpandSelected.Visible = node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded;
            cmiCollapseSelected.Visible = node != null && node.GetNodeCount(true) > 0 && node.IsExpanded;

            cmiExpandSelected.Text = node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded
                ? $"Expand \"{node.Text.Replace("&", "&&")}\""
                : string.Empty;
            cmiCollapseSelected.Text = node != null && node.GetNodeCount(true) > 0 && node.IsExpanded
                ? $"Collapse \"{node.Text.Replace("&", "&&")}\""
                : string.Empty;

            tsSeparatorExpandCollapse.Visible = tvItems.Visible && node != null && node.GetNodeCount(true) > 0;

            // "Expand All" and "Collapse All" menu
            cmiCollapseAll.Enabled = cmiCollapseAll.Visible = AllExpanded && tvItems.Visible;
            cmiExpandAll.Enabled = cmiExpandAll.Visible = !cmiCollapseAll.Enabled && tvItems.Visible;
        }

        /// <summary>
        /// Plans to mastery level.
        /// </summary>
        /// <param name="node">The node.</param>
        private void PlanToMasteryLevel(TreeNode node)
        {
            cmiLvPlanTo.Visible = tsSeparatorPlanTo.Visible = Plan != null;

            if (Plan == null)
                return;

            ShipSelectControl shipSelectorControl = this as ShipSelectControl;

            cmiLvPlanTo.Visible = shipSelectorControl != null;
            tsSeparatorPlanTo.Visible = shipSelectorControl != null && lbSearchList.Items.Count == 0;

            if (shipSelectorControl == null || ((node != null) && node.GetNodeCount(true) > 0) || SelectedObject == null)
            {
                cmiLvPlanTo.Visible = false;
                tsSeparatorPlanTo.Visible = false;
                return;
            }

            MasteryShip masteryShip = ((Character)Plan.Character).MasteryShips.GetMasteryShipByID(SelectedObject.ID);

            if (masteryShip == null)
            {
                cmiLvPlanTo.Enabled = false;
                cmiLvPlanTo.Text = @"Plan Mastery to...";
                return;
            }

            cmiLvPlanTo.Enabled = !Plan.WillGrantEligibilityFor(masteryShip.GetLevel(5));
            cmiLvPlanTo.Text = $"Plan \"{masteryShip.Ship.Name}\" Mastery to...";

            // "Plan to N" menus
            for (int i = 1; i <= 5; i++)
            {
                SetAdditionMenuStatus(cmiLvPlanTo.DropDownItems[i - 1], masteryShip.GetLevel(i));
            }
        }

        /// <summary>
        /// Sets the visible status of the context menu submenu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="masteryLevel">The mastery level.</param>
        private void SetAdditionMenuStatus(ToolStripItem menu, Mastery masteryLevel)
        {
            menu.Visible = masteryLevel != null;

            if (masteryLevel == null)
                return;

            menu.Enabled = !Plan.WillGrantEligibilityFor(masteryLevel);

            if (menu.Enabled)
                menu.Tag = Plan.TryPlanTo(masteryLevel);
        }

        /// <summary>
        /// Context > Plan To > Level N
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void planToLevelMenuItem_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = ((ToolStripMenuItem)sender).Tag as IPlanOperation;
            if (operation == null)
                return;

            PlanWindow planWindow = ParentForm as PlanWindow;
            if (planWindow == null)
                return;

            PlanHelper.SelectPerform(new PlanToOperationWindow(operation), planWindow, operation);
        }

        #endregion


        #region Predicates

        /// <summary>
        /// Filter for all items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool SelectAll(Item item) => true;

        /// <summary>
        /// Filter for items which can be used (prereqs met).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">item</exception>
        protected bool CanUse(Item item)
        {
            item.ThrowIfNull(nameof(item));

            IList<StaticSkillLevel> prerequisites =
                item.Prerequisites.Where(x => x.Activity != BlueprintActivity.ReverseEngineering).ToList();

            BlueprintSelectControl blueprintSelectControl = this as BlueprintSelectControl;

            // Is item a blueprint and supports the selected activity ?  
            if (blueprintSelectControl != null)
            {
                bool hasSelectedActivity = prerequisites.Any(x => x.Activity == Activity)
                                           || ((Blueprint)item).MaterialRequirements.Any(x => x.Activity == Activity);

                // Can not be used when item doesn't support the selected activity
                if ((ActivityFilter == ObjectActivityFilter.Manufacturing || ActivityFilter == ObjectActivityFilter.Invention)
                    && !hasSelectedActivity)
                    return false;

                // Enumerates the prerequisites skills to the selected activity 
                if (ActivityFilter != ObjectActivityFilter.All && ActivityFilter != ObjectActivityFilter.Any)
                    prerequisites = prerequisites.Where(x => x.Activity == Activity).ToList();
            }

            // Item doesn't have prerequisites
            if (!prerequisites.Any())
                return true;

            // Is this the "Blueprint Browser" and the activity filter is set to "Any" ?
            List<bool> prereqTrained = new List<bool>();
            if (blueprintSelectControl != null && ActivityFilter == ObjectActivityFilter.Any)
            {
                List<BlueprintActivity> prereqActivity = new List<BlueprintActivity>();

                // Create a list with the activities this item supports
                foreach (StaticSkillLevel prereq in prerequisites.Where(x => !prereqActivity.Contains(x.Activity)))
                {
                    prereqActivity.Add(prereq.Activity);
                }

                // Create a list with each prereq skill trained status for the questioned activity
                foreach (BlueprintActivity activity in prereqActivity)
                {
                    prereqTrained.Clear();

                    prereqTrained.AddRange(prerequisites
                        .Where(prereq => prereq.Skill != null && prereq.Activity == activity)
                        .Select(prereq => new
                        {
                            prereq,
                            level = m_character.GetSkillLevel(prereq.Skill)
                        })
                        .Select(y => y.level >= y.prereq.Level));

                    // Has the character trained all prereq skills for this activity ?
                    if (prereqTrained.All(x => x))
                        return true;
                }
                return false;
            }

            // Do a simple predication and create a list with each prereq skill trained status
            prereqTrained.AddRange(prerequisites
                .Where(prereq => prereq.Skill != null)
                .Select(prereq => new
                {
                    prereq,
                    level = m_character.GetSkillLevel(prereq.Skill)
                })
                .Select(y => y.level >= y.prereq.Level));

            // Has the character trained all prereq skills ?
            return prereqTrained.All(x => x);
        }

        /// <summary>
        /// Filter for items which can not be used (prereqs not met).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">item</exception>
        protected bool CannotUse(Item item)
        {
            item.ThrowIfNull(nameof(item));

            Blueprint blueprint = item as Blueprint;

            bool hasActivity = blueprint == null || ActivityFilter == ObjectActivityFilter.All
                               || blueprint.Prerequisites.Any(x => x.Activity == Activity)
                               || blueprint.MaterialRequirements.Any(x => x.Activity == Activity);

            // Special condition check for activity 'Any' 
            // as negative logic returns incorrect results
            if (!(this is BlueprintSelectControl) || ActivityFilter != ObjectActivityFilter.Any)
                return !CanUse(item) && hasActivity;

            List<StaticSkillLevel> prerequisites =
                item.Prerequisites.Where(x => x.Activity != BlueprintActivity.ReverseEngineering).ToList();

            IEnumerable<bool> prereqTrained = prerequisites
                .Where(prereq => prereq.Skill != null)
                .Select(prereq => new
                {
                    prereq,
                    level = m_character.GetSkillLevel(prereq.Skill)
                })
                .Select(y => y.level >= y.prereq.Level);

            // Has the character trained all prereq skills for this activity ?
            return prerequisites.Any() && !prereqTrained.All(x => x);
        }

        #endregion
    }
}
