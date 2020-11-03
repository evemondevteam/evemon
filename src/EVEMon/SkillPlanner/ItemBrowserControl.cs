using System;
using System.ComponentModel;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    internal partial class ItemBrowserControl : EveObjectBrowserControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemBrowserControl()
        {
            InitializeComponent();
            scObjectBrowser.RememberDistanceKey = "ItemBrowser_Left";
            SelectControl = itemSelectControl;
            PropertiesList = lvItemProperties;

            PropertiesList.MouseDown += PropertiesList_MouseDown;
            PropertiesList.MouseMove += PropertiesList_MouseMove;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList, true);
        }

        /// <summary>
        /// When the mouse gets pressed, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PropertiesList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            PropertiesList.Cursor = Cursors.Default;
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void PropertiesList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            PropertiesList.Cursor = CustomCursors.ContextMenu;
        }

        /// <summary>
        /// Occurs when double clicking on a list view item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesList_DoubleClick(object sender, EventArgs e)
        {
            // Is it a skill?
            Skill skill = PropertiesList.FocusedItem?.Tag as Skill;

            if (skill != null)
            {
                PlanWindow.ShowPlanWindow(SelectControl.Character, Plan)?.ShowSkillInBrowser(skill);
                return;
            }

            // Is it an item?
            Item item = PropertiesList.FocusedItem?.Tag as Item;

            if (item != null)
                PlanWindow.ShowPlanWindow(SelectControl.Character, Plan)?.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Handles the Opening event of the ShipAttributeContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ShipAttributeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Is it a skill?
            Skill skill = PropertiesList.FocusedItem?.Tag as Skill;
            showInSkillBrowser.Visible = skill != null;

            // Is it an item?
            Item item = PropertiesList.FocusedItem?.Tag as Item;
            showInItemBrowser.Visible = item != null;

            showInMenuSeparator.Visible = skill != null || item != null;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Replaces the user set search string with another.
        /// </summary>
        /// <param name="text">The text to replace it with.</param>
        internal void SetSearchText(string text)
        {
            itemSelectControl.SearchText = text;
        }

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();

            if (SelectedObject == null)
                return;

            // Description
            tbDescription.Text = SelectedObject.Description;

            // Required Skills
            requiredSkillsControl.Object = SelectedObject;
        }

        /// <summary>
        /// Updates whenever the selected plan changed.
        /// </summary>
        protected override void OnSelectedPlanChanged()
        {
            base.OnSelectedPlanChanged();

            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = reqSkillPanelMinWidth > reqSkillControlMinWidth
                ? reqSkillPanelMinWidth
                : reqSkillControlMinWidth;
        }

        #endregion

    }
}
