using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    /// <summary>
    /// A wrapper around <see cref="ToolTip"/> to use for infinite display.
    /// </summary>
    public class InfiniteDisplayToolTip : ToolTip
    {
        private readonly Control m_owner;
        private readonly Timer m_initialDelayTimer;
        private Point m_point;
        private string m_text;

        /// <summary>
        /// Initializes <see cref="InfiniteDisplayToolTip"/> instance.
        /// </summary>
        /// <param name="owner">Owner of this tooltip</param>
        public InfiniteDisplayToolTip(Control owner)
        {
            m_owner = owner;
            m_initialDelayTimer = new Timer();
            m_initialDelayTimer.Tick += m_initialDelayTimer_Tick;
            m_initialDelayTimer.Interval = InitialDelay;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Called when disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            m_initialDelayTimer.Tick -= m_initialDelayTimer_Tick;
            m_initialDelayTimer.Dispose();
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Diplays the tooltip on timer tick.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void m_initialDelayTimer_Tick(object sender, EventArgs e)
        {
            m_initialDelayTimer.Stop();
            Show(m_text, m_owner, m_point);
        }

        /// <summary>
        /// Popups a tool tip with provided text, at the given point.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="pt">The pt.</param>
        public void Show(string text, Point pt)
        {
            if (text == m_text)
                return;

            pt.Offset(0, 24);
            m_point = pt;

            Hide(m_owner);
            m_text = text;
            m_initialDelayTimer.Start();
        }

        /// <summary>
        /// Hides a tooltip.
        /// </summary>
        public void Hide()
        {
            Hide(m_owner);
            m_text = string.Empty;
            m_initialDelayTimer.Stop();
        }
    }
}
