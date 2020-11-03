using System.Globalization;
using System.Windows.Forms;
using EVEMon.Common.Constants;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Derived from TreeView class.
    /// <para>Overrides standard node double click behaviour to prevent node expand / collapse actions</para>
    /// </summary>
    internal class OverridenTreeView : TreeView
    {
        private const int WmLButtonDblClk = 0x203;

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)"/>.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmLButtonDblClk)
                HandleDoubleClick(ref m);
            else
                base.WndProc(ref m);
        }

        /// <summary>
        /// Handles the double click.
        /// </summary>
        /// <param name="m">The message.</param>
        private void HandleDoubleClick(ref Message m)
        {
            // Get mouse location from message.lparam
            // x is low order word, y is high order word
            string lparam = m.LParam.ToString("X08");
            int x = int.Parse(lparam.Substring(4, 4), NumberStyles.HexNumber, CultureConstants.
                InvariantCulture);
            int y = int.Parse(lparam.Substring(0, 4), NumberStyles.HexNumber, CultureConstants.
                InvariantCulture);

            // Test for a treenode at this location
            TreeViewHitTestInfo info = HitTest(x, y);

            if (info.Node == null)
                return;

            // Raise NodeMouseDoubleClick event
            OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(info.Node, MouseButtons.Left, 2, x, y));
        }

        /// <summary>
        /// Creates the handle.
        /// </summary>
        protected override void CreateHandle()
        {
            if (!IsDisposed)
                base.CreateHandle();
        }
    }
}
