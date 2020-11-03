using System;
using System.ComponentModel;
using System.Windows.Forms;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Controls
{
    public class PersistentSplitContainer : SplitContainer
    {
        private string m_rememberDistanceKey;

        /// <summary>
        /// A key used to store position for this control.
        /// Do not set up directly in the designer, call it from the control's constructor, after InitializeComponent().
        /// </summary>
        [Browsable(false)]
        public string RememberDistanceKey
        {
            get { return m_rememberDistanceKey; }
            set
            {
                m_rememberDistanceKey = value;

                // Set the splitter width here rather than in an override of CreateControl()
                // because CreateControl is only called when we make the container visible
                // so if the container is created, but never shown, the persistant splitter 
                // width will be reset to the default for the base SplitContainer
                try
                {
                    if (string.IsNullOrEmpty(m_rememberDistanceKey))
                        return;

                    if (Settings.UI.Splitters.ContainsKey(m_rememberDistanceKey))
                    {
                        int d = Settings.UI.Splitters[m_rememberDistanceKey];
                        d = VerifyValidSplitterDistance(d);
                        SplitterDistance = d;
                    }
                    else
                        Settings.UI.Splitters.Add(m_rememberDistanceKey, Math.Min(Width / 4, 100));
                }
                catch (InvalidOperationException err)
                {
                    // This occurs when we're in the designer. DesignMode doesn't get set
                    // when the control is a subcontrol of a user control, so we should handle
                    // this here :(
                    ExceptionHandler.LogException(err, false);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogRethrowException(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> 
        /// and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(m_rememberDistanceKey))
            {
                int d = SplitterDistance;
                if (VerifyValidSplitterDistance(d) == d)
                    Settings.UI.Splitters[m_rememberDistanceKey] = d;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Verifies the valid splitter distance.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        private int VerifyValidSplitterDistance(int d)
        {
            int defaultDistance = SplitterDistance;

            if ((d < Panel1MinSize) || (d + Panel2MinSize > Width))
                return defaultDistance;
            return d;
        }
    }
}