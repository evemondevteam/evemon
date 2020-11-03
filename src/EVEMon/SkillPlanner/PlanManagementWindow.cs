using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;
using EVEMon.Common.Models.Comparers;
using EVEMon.Common.Serialization.Settings;
using SortOrder = EVEMon.Common.Enumerations.SortOrder;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// This window allows the user to manage all the plans : renaming, reordering, etc.
    /// </summary>
    public partial class PlanManagementWindow : EVEMonForm
    {
        private readonly PlanComparer m_columnSorter;
        private readonly Character m_character;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
        private PlanManagementWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Regular constructor for use in code.
        /// </summary>
        /// <param name="character"></param>
        public PlanManagementWindow(Character character)
            : this()
        {
            m_character = character;
            m_columnSorter = new PlanComparer(PlanSort.Name);
        }

        /// <summary>
        /// On loading, populate the plan
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            MinimumSize = Size;

            EveMonClient.CharacterPlanCollectionChanged += EveMonClient_CharacterPlanCollectionChanged;

            UpdateContent(true);
            lbPlanList.ListViewItemSorter = null;
        }

        /// <summary>
        /// Unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            EveMonClient.CharacterPlanCollectionChanged -= EveMonClient_CharacterPlanCollectionChanged;
        }

        /// <summary>
        /// The button "open" is the same than "merge". When the button is in "open" state, we close the window and returns OK as a result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // Are we performing a merge ?
            if (lbPlanList.SelectedItems.Count > 1)
            {
                MergePlans();
                return;
            }

            // Or are we just opening a plan ?
            Plan plan = (Plan)lbPlanList.SelectedItems[0].Tag;
            PlanWindow.ShowPlanWindow(plan: plan);
            Close();
        }

        /// <summary>
        /// On close, nothing special.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }


        #region List management and creation

        /// <summary>
        /// Occurs when new plans are added or removed to the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterPlanCollectionChanged(object sender, CharacterChangedEventArgs e)
        {
            UpdateContent(true);
        }

        /// <summary>
        /// Rebuild the plans list.
        /// </summary>
        /// <param name="restoreSelectionAndFocus"></param>
        private void UpdateContent(bool restoreSelectionAndFocus)
        {
            // Store selection and focus
            Plan selection = lbPlanList.Items.Cast<ListViewItem>().Where(x => x.Selected)
                .Select(x => x.Tag).OfType<Plan>().FirstOrDefault();
            Plan focused = lbPlanList.FocusedItem?.Tag as Plan;

            lbPlanList.BeginUpdate();
            try
            {
                // Recreate the list from scratch
                lbPlanList.Items.Clear();
                foreach (Plan plan in m_character.Plans)
                {
                    // Create the item and add it
                    ListViewItem lvi = new ListViewItem(plan.Name) { Tag = plan };
                    lvi.SubItems.Add(plan.TotalTrainingTime
                        .ToDescriptiveText(DescriptiveTextOptions.FullText |
                                           DescriptiveTextOptions.IncludeCommas |
                                           DescriptiveTextOptions.SpaceText));
                    lvi.SubItems.Add(plan.UniqueSkillsCount.ToString(CultureConstants.DefaultCulture));
                    lvi.SubItems.Add(string.IsNullOrWhiteSpace(plan.Description)
                                         ? "(None)"
                                         : plan.Description.Replace(Environment.NewLine, " "));
                    lbPlanList.Items.Add(lvi);

                    // Restore selection and focus
                    if (!restoreSelectionAndFocus)
                        continue;

                    lvi.Selected = selection == plan;
                    lvi.Focused = focused == plan;
                }

                // Adjust the size of the columns
                AdjustColumns();

                // Enable/disable the button
                btnOpen.Enabled = lbPlanList.SelectedItems.Count > 0;
            }
            finally
            {
                lbPlanList.EndUpdate();
            }
        }

        /// <summary>
        /// Adjusts the columns.
        /// </summary>
        private void AdjustColumns()
        {
            foreach (ColumnHeader column in lbPlanList.Columns)
            {
                column.Width = -2;

                // Due to .NET design we need to prevent the last colummn to resize to the right end

                // Return if it's not the last column
                if (column.Index != lbPlanList.Columns.Count - 1)
                    continue;

                const int Pad = 4;

                // Calculate column header text width with padding
                int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                // If there is an image assigned to the header, add its width with padding
                if (lbPlanList.SmallImageList != null && column.ImageIndex > -1)
                    columnHeaderWidth += lbPlanList.SmallImageList.ImageSize.Width + Pad;

                // Calculate the width of the header and the items of the column
                int columnMaxWidth = column.ListView.Items.Cast<ListViewItem>().Select(
                    item => TextRenderer.MeasureText(item.SubItems[column.Index].Text, Font).Width).Concat(
                        new[] { columnHeaderWidth }).Max() + Pad + 1;

                // Assign the width found
                column.Width = columnMaxWidth;
            }
        }

        /// <summary>
        /// Merge the selected plans.
        /// </summary>
        private void MergePlans()
        {
            // Build the merge plan
            Plan result = new Plan(m_character);
            using (result.SuspendingEvents())
            {
                // Merge the plans
                foreach (ListViewItem item in lbPlanList.SelectedItems)
                {
                    Plan plan = (Plan)item.Tag;
                    foreach (PlanEntry entry in plan)
                    {
                        // If not planned yet, we add the new entry
                        if (!result.IsPlanned(entry.Skill, entry.Level))
                            result.PlanTo(entry.Skill, entry.Level, entry.Priority, entry.Notes);

                        // Then we update the entry's groups
                        PlanEntry newEntry = result.GetEntry(entry.Skill, entry.Level);

                        // The entry may be null if the character already knows it
                        newEntry?.PlanGroups.Add(plan.Name);
                    }
                }
            }

            // Request a new name for this plan
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Change the plan's name and add it
                result.Name = npw.PlanName;
                result.Description = npw.PlanDescription;
                m_character.Plans.Add(result);
            }
        }

        #endregion


        #region List's events

        /// <summary>
        /// When the user select another item, we update the control's status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbPlanList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // One one plan selected means we can move it
            tsbMoveUp.Enabled = lbPlanList.SelectedItems.Count == 1;
            tsbMoveDown.Enabled = lbPlanList.SelectedItems.Count == 1;

            // No items -> Disabled "open"
            // One item -> Enabled "open"
            // More items -> Enabled "merge"
            btnOpen.Enabled = lbPlanList.SelectedItems.Count > 0;
            btnOpen.Text = lbPlanList.SelectedItems.Count > 1 ? "Merge" : "Open";
        }

        /// <summary>
        /// When the user double-click an item, we open this plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbPlanList_DoubleClick(object sender, EventArgs e)
        {
            if (lbPlanList.SelectedItems.Count == 1)
                btnOpen_Click(this, new EventArgs());
        }

        /// <summary>
        /// When the mouse gets pressed, we change the cursor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbPlanList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            lbPlanList.Cursor = Cursors.Default;
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbPlanList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            lbPlanList.Cursor = lbPlanList.GetItemAt(e.X, e.Y) != null
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// On a column click, we update the sort.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbPlanList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Click on the same column than the one already sorted ?
            if (e.Column == (int)m_columnSorter.Sort)
            {
                // Swap sort order
                m_columnSorter.Order = m_columnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
                // Or a new column
            else
            {
                m_columnSorter.Sort = (PlanSort)e.Column;
                m_columnSorter.Order = SortOrder.Ascending;
            }

            // Update sort
            lbPlanList.ListViewItemSorter = new ListViewItemComparerByTag<Plan>(m_columnSorter);
            lbPlanList.Sort();

            // Rebuild the plans list from the listview items
            m_character.Plans.RebuildFrom(lbPlanList.Items.Cast<ListViewItem>().Select(x => x.Tag).OfType<Plan>());
        }

        /// <summary>
        /// On reordering through drag'n drop, we fetch the new data to the character's plans collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbPlanList_ListViewItemsDragged(object sender, EventArgs e)
        {
            // Rebuild the plans list from the listview items
            m_character.Plans.RebuildFrom(lbPlanList.Items.Cast<ListViewItem>().Select(x => x.Tag).OfType<Plan>());
        }

        #endregion


        #region Menus and buttons handlers

        /// <summary>
        /// File > New plan...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miNewPlan_Click(object sender, EventArgs e)
        {
            // Request a new name for this plan
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Create the plan and add it
                Plan plan = new Plan(m_character) { Name = npw.PlanName, Description = npw.PlanDescription };
                m_character.Plans.Add(plan);

                // Open a window for this plan
                PlanWindow.ShowPlanWindow(plan: plan);
            }

            Close();
        }

        /// <summary>
        /// File > Import Plan from File...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miImportPlanFromFile_Click(object sender, EventArgs e)
        {
            // Prompt the user to select a file
            DialogResult dr = ofdOpenDialog.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            // Load from file and returns if an error occurred (user has already been warned)
            SerializablePlan serial = PlanIOHelper.ImportFromXML(ofdOpenDialog.FileName);
            if (serial == null)
                return;

            // Imports the plan
            Plan loadedPlan = new Plan(m_character, serial);

            // Prompt the user for the plan name
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                npw.PlanName = Path.GetFileNameWithoutExtension(ofdOpenDialog.FileName);
                DialogResult xdr = npw.ShowDialog();
                if (xdr == DialogResult.Cancel)
                    return;

                loadedPlan.Name = npw.PlanName;
                loadedPlan.Description = npw.PlanDescription;
                m_character.Plans.Add(loadedPlan);
            }
        }

        /// <summary>
        /// File > Import Plan from Character....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miImportPlanFromCharacter_Click(object sender, EventArgs e)
        {
            // Prompt the user to choose the source character and plan.
            using (PlanImportationFromCharacterWindow cps = new PlanImportationFromCharacterWindow(m_character))
            {
                DialogResult dr = cps.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Retrieves the cloned plan
                Plan plan = cps.TargetPlan;

                // Adds and fixes the prerequisites order
                plan.FixPrerequisites();

                // Prompt the user for the new plan's name
                using (NewPlanWindow f = new NewPlanWindow())
                {
                    f.PlanName = $"{m_character.Name}-{plan.Name}";
                    f.Text = @"Save Plan As";

                    dr = f.ShowDialog();
                    if (dr == DialogResult.Cancel)
                        return;

                    plan.Name = f.PlanName;
                    plan.Description = f.PlanDescription;
                }

                // Add the plan to the character's list
                m_character.Plans.Add(plan);
            }
        }

        /// <summary>
        /// File > Export Plan...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void miExportPlan_Click(object sender, EventArgs e)
        {
            if (lbPlanList.SelectedItems.Count != 1)
                return;

            Plan plan = (Plan)lbPlanList.SelectedItems[0].Tag;
            await UIHelper.ExportPlanAsync(plan);
        }

        /// <summary>
        /// File > Export Character Skills as Plan...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void miExportCharacterSkillsAsPlan_Click(object sender, EventArgs e)
        {
            await UIHelper.ExportCharacterSkillsAsPlanAsync(m_character);
        }

        /// <summary>
        /// File > Restore Plans...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miRestorePlans_Click(object sender, EventArgs e)
        {
            // Prompt the user to select a file
            using (OpenFileDialog restorePlansDialog = new OpenFileDialog())
            {
                restorePlansDialog.Title = @"Restore from File";
                restorePlansDialog.Filter = @"EVEMon Plans Backup Format (*.epb)|*.epb";
                DialogResult dr = restorePlansDialog.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Load from file and returns if an error occurred (user has already been warned)
                IEnumerable<SerializablePlan> serial = PlanIOHelper.ImportPlansFromXML(restorePlansDialog.FileName);
                if (serial == null)
                    return;

                // Imports the plans
                IEnumerable<Plan> loadedPlans = serial.Select(plan => new Plan(m_character, plan));
                m_character.Plans.AddRange(loadedPlans);
            }
        }

        /// <summary>
        /// File > Save Plans...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void miSavePlans_Click(object sender, EventArgs e)
        {
            IList<Plan> plans = lbPlanList.Items.Cast<ListViewItem>().Select(item => item.Tag as Plan).ToList();
            await UIHelper.SavePlansAsync(plans);
        }

        /// <summary>
        /// Edit > Rename...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miRenameEdit_Click(object sender, EventArgs e)
        {
            // Quit if none selected
            if (lbPlanList.SelectedItems.Count == 0)
                return;

            // Prompts the user for a new name
            Plan plan = (Plan)lbPlanList.SelectedItems[0].Tag;
            using (NewPlanWindow f = new NewPlanWindow())
            {
                f.Text = @"Rename Plan or Edit Description";
                f.PlanName = plan.Name;
                f.PlanDescription = plan.Description;
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                // Change the name
                plan.Name = f.PlanName;
                plan.Description = f.PlanDescription;
                UpdateContent(true);
            }
        }

        /// <summary>
        /// File > Delete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDelete_Click(object sender, EventArgs e)
        {
            // Quit if none selected
            if (lbPlanList.SelectedItems.Count == 0)
                return;

            // Prepare the title and retrieve the plan's name for the incoming message box
            string planName;
            string title = "Delete Plan";
            if (lbPlanList.SelectedItems.Count > 1)
            {
                planName = "the selected plans";
                title += "s";
            }
            else
            {
                Plan plan = (Plan)lbPlanList.SelectedItems[0].Tag;
                planName = $"\"{plan.Name}\"";
            }

            // Prompt the user for confirmation with a message box
            DialogResult dr = MessageBox.Show($"Are you sure you want to delete {planName}?", title,
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                return;

            // Remove the items
            foreach (ListViewItem lvi in lbPlanList.SelectedItems)
            {
                m_character.Plans.Remove(lvi.Tag as Plan);
            }
        }

        /// <summary>
        /// Right toolbar > Move up.
        /// Move the plan and reinsert it at the proper position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            int idx = lbPlanList.SelectedIndices[0];
            if (idx == 0)
                return;

            // Rebuild a plans array
            Plan[] plans = lbPlanList.Items.Cast<ListViewItem>().Select(x => x.Tag).OfType<Plan>().ToArray();
            Plan temp = plans[idx - 1];
            plans[idx - 1] = plans[idx];
            plans[idx] = temp;

            lbPlanList.ListViewItemSorter = null;
            m_character.Plans.RebuildFrom(plans);
        }

        /// <summary>
        /// Right toolbar > Move down.
        /// Move the plan and reinsert it at the proper position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            int idx = lbPlanList.SelectedIndices[0];
            if (idx == lbPlanList.Items.Count - 1)
                return;

            // Rebuild a plans array
            Plan[] plans = lbPlanList.Items.Cast<ListViewItem>().Select(x => x.Tag).OfType<Plan>().ToArray();
            Plan temp = plans[idx + 1];
            plans[idx + 1] = plans[idx];
            plans[idx] = temp;

            lbPlanList.ListViewItemSorter = null;
            m_character.Plans.RebuildFrom(plans);
        }

        #endregion


        #region Context Menu

        /// <summary>
        /// When the context menu opens, we change the items states.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lbPlanList.SelectedItems.Count > 1)
            {
                cmiDelete.Enabled = true;
                cmiRenameEdit.Enabled = false;
                cmiExport.Enabled = false;
                cmiOpen.Text = @"Merge";
                return;
            }

            if (lbPlanList.SelectedItems.Count == 1)
            {
                cmiDelete.Enabled = true;
                cmiRenameEdit.Enabled = true;
                cmiExport.Enabled = true;
                cmiOpen.Enabled = true;
                cmiOpen.Text = @"Open";
                return;
            }

            e.Cancel = true;
        }

        /// <summary>
        /// When the "file" menu is opening, we enable or disable "load plans".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mFile_DropDownOpening(object sender, EventArgs e)
        {
            // See if we have multiple characters to determine if load from character is enabled
            miImportPlanFromCharacter.Enabled = EveMonClient.Characters.Count > 1;
            miExportPlan.Enabled = lbPlanList.SelectedItems.Count == 1;
            miSavePlans.Enabled = lbPlanList.Items.Count > 0;
        }

        /// <summary>
        /// When the "edit" menu is opening, we enable or disable the options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mEdit_DropDownOpening(object sender, EventArgs e)
        {
            miRenameEdit.Enabled = lbPlanList.SelectedItems.Count == 1;
            miDelete.Enabled = lbPlanList.SelectedItems.Count > 0;
        }

        #endregion
    }
}