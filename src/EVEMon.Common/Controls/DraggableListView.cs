using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Controls
{
    public class DraggableListView : ListView
    {
        public event EventHandler<ListViewDragEventArgs> ListViewItemsDragging;
        public event EventHandler<EventArgs> ListViewItemsDragged;

        private const string Reorder = "Reorder";

        private bool m_dragging;
        private bool m_allowRowReorder = true;
        private int m_dropMarkerOn = -1;
        private bool m_dropMarkerBelow;

        public DraggableListView()
        {
            Initialize();
        }


        #region Draggable stuff

        /// <summary>
        /// Gets or sets a value indicating whether [allow row reorder].
        /// </summary>
        /// <value><c>true</c> if [allow row reorder]; otherwise, <c>false</c>.</value>
        public bool AllowRowReorder
        {
            get { return m_allowRowReorder; }
            set
            {
                m_allowRowReorder = value;
                base.AllowDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort order for items in the control.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Windows.Forms.SortOrder"/> values.
        /// The default is <see cref="F:System.Windows.Forms.SortOrder.None"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the <see cref="T:System.Windows.Forms.SortOrder"/> values. </exception>
        public new System.Windows.Forms.SortOrder Sorting
        {
            get { return System.Windows.Forms.SortOrder.None; }
            set { base.Sorting = System.Windows.Forms.SortOrder.None; }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            AllowRowReorder = true;
        }

        /// <summary>
        /// Gets the dragging skill.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private static Skill GetDraggingSkill(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
                return (Skill)((TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode")).Tag;

            return null;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragDrop"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
                return;

            m_dragging = false;
            ClearDropMarker();
            if (!AllowRowReorder)
                return;

            if (SelectedItems.Count == 0)
                return;

            Point cp = PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
                return;

            int dropIndex = dragToItem.Index;
            if (dropIndex > SelectedItems[0].Index)
                dropIndex++;

            if (ListViewItemsDragging != null)
            {
                ListViewDragEventArgs args = new ListViewDragEventArgs(SelectedItems[0].Index,
                                                                       SelectedItems.Count, dropIndex);
                ListViewItemsDragging(this, args);
                if (args.Cancel)
                    return;
            }

            // Make a copy of all the selected items
            ArrayList insertItems = new ArrayList(SelectedItems.Count);
            foreach (ListViewItem item in SelectedItems)
            {
                insertItems.Add(item.Clone());
            }

            // insert the copied items in reverse order at the drop index so 
            // they appear in the right order after they've all been inserted
            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                Items.Insert(dropIndex, (ListViewItem)insertItems[i]);
            }

            // remove the selected items
            foreach (ListViewItem item in SelectedItems)
            {
                // must clear the items icon index or an exception is thrown when it is removed
                item.StateImageIndex = -1;
                Items.Remove(item);
            }

            ListViewItemsDragged?.ThreadSafeInvoke(this, new EventArgs());

            // if the item was dragged to the end of the plan.
            if (dropIndex >= Items.Count)
                EnsureVisible(Items.Count - 1);
            else
                EnsureVisible(dropIndex);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragOver"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
                return;

            if (!AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
                return;
            }
            Point cp = PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = GetItemAt(cp.X, cp.Y);
            if (hoverItem == null)
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
                return;
            }

            if (SelectedItems.Cast<ListViewItem>().Any(moveItem => moveItem.Index == hoverItem.Index))
            {
                e.Effect = DragDropEffects.None;
                hoverItem.EnsureVisible();
                ClearDropMarker();
                return;
            }

            base.OnDragOver(e);
            string text = (string)e.Data.GetData(Reorder.GetType());
            if (string.Compare(text, Reorder, StringComparison.CurrentCulture) == 0)
            {
                e.Effect = DragDropEffects.Move;
                hoverItem.EnsureVisible();

                Rectangle hoverBounds = hoverItem.GetBounds(ItemBoundsPortion.ItemOnly);
                DrawDropMarker(hoverItem.Index, cp.Y > hoverBounds.Top + hoverBounds.Height / 2);
            }
            else
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragEnter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            Skill dragSkill = GetDraggingSkill(e);
            if (dragSkill != null)
                return;

            if (!AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
                return;
            }

            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
                return;
            }

            base.OnDragEnter(e);
            string text = (string)e.Data.GetData(Reorder.GetType());
            if (string.Compare(text, Reorder, StringComparison.CurrentCulture) == 0)
                e.Effect = DragDropEffects.Move;
            else
            {
                e.Effect = DragDropEffects.None;
                ClearDropMarker();
            }
        }

        public void ClearDropMarker()
        {
            if (m_dropMarkerOn != -1)
                RestrictedPaint();
            m_dropMarkerOn = -1;
        }

        public void DrawDropMarker(int index, bool below)
        {
            if (m_dropMarkerOn != -1 && (m_dropMarkerOn != index || m_dropMarkerBelow != below))
                ClearDropMarker();

            if (m_dropMarkerOn == index)
                return;

            m_dropMarkerOn = index;
            m_dropMarkerBelow = below;
            RestrictedPaint();
        }

        private void RestrictedPaint()
        {
            Rectangle itemRect = GetItemRect(m_dropMarkerOn, ItemBoundsPortion.ItemOnly);
            Point start = new Point(itemRect.Left, m_dropMarkerBelow ? itemRect.Bottom : itemRect.Top);
            Point end = new Point(Width < itemRect.Right ? Width : itemRect.Right,
                                  m_dropMarkerBelow ? itemRect.Bottom : itemRect.Top);
            start = PointToScreen(start);
            end = PointToScreen(end);
            ControlPaint.DrawReversibleLine(start, end, SystemColors.Window);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_dragging)
                RestrictedPaint();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.ItemDrag"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.ItemDragEventArgs"/> that contains the event data. </param>
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            if (!AllowRowReorder)
                return;

            DoDragDrop(Reorder, DragDropEffects.Move);
            m_dragging = true;
        }

        #endregion
    }
}