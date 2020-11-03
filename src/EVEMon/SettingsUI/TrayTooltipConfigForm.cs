using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Models;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// Configuration editor form for the ToolTip style tray icon popup
    /// </summary>
    public partial class TrayTooltipConfigForm : EVEMonForm
    {
        private readonly TrayTooltipSettings m_settings;

        // Array containing the example tooltip formats that are populated into the dropdown box.
        private readonly string[] m_tooltipCodes = {
                                                       "%n - %s %tr - %r",
                                                       "%n - %s [%cr->%tr]: %r",
                                                       "%n : %s - %d : %b ISK",
                                                       "%s %ci to %ti, %r left"
                                                   };

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayTooltipConfigForm"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public TrayTooltipConfigForm(TrayTooltipSettings settings)
        {
            InitializeComponent();
            m_settings = settings;
        }

        /// <summary>
        /// On load, restores the window rectangle from the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (string tooltip in m_tooltipCodes)
            {
                cbTooltipDisplay.Items.Add(FormatExampleTooltipText(tooltip));
            }
            cbTooltipDisplay.Items.Add(" -- Custom -- ");

            tbTooltipString.Text = m_settings.Format;
            tbTooltipString_TextChanged(null, null);
            cbTooltipOrder.Checked = m_settings.DisplayOrder;
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            m_settings.Format = tbTooltipString.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the TextChanged event of the tbTooltipString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbTooltipString_TextChanged(object sender, EventArgs e)
        {
            tbTooltipTestDisplay.Text = FormatExampleTooltipText(tbTooltipString.Text);

            if (cbTooltipDisplay.SelectedIndex != -1)
                return;

            int index = m_tooltipCodes.Length;

            for (int i = 0; i < m_tooltipCodes.Length; i++)
            {
                if (m_tooltipCodes[i].Equals(tbTooltipString.Text))
                    index = i;
            }

            cbTooltipDisplay.SelectedIndex = index;
            DisplayCustomControls(index == m_tooltipCodes.Length);
        }

        // Formats the argument format string with hardcoded exampe values. Works basically the
        // same as TrayTooltipWindow.FormatTooltipText(...), with the exception of the exampe values.
        private static string FormatExampleTooltipText(string fmt) => Regex.Replace(fmt, "%([nbsdr]|[ct][ir])", TransformTooltipText, RegexOptions.Compiled);

        /// <summary>
        /// Transforms the tooltip text.
        /// </summary>
        /// <param name="m">The regex match.</param>
        /// <returns></returns>
        private static string TransformTooltipText(Match m)
        {
            string value = string.Empty;
            char capture = m.Groups[1].Value[0];

            switch (capture)
            {
                case 'n':
                    value = "John Doe";
                    break;
                case 'b':
                    value = "183,415,254.05";
                    break;
                case 's':
                    value = "Gunnery";
                    break;
                case 'd':
                    value = "9/15/2006 6:36 PM";
                    break;
                case 'r':
                    value = "2h, 53m, 28s";
                    break;
                default:
                    int level = -1;
                    switch (capture)
                    {
                        case 'c':
                            level = 3;
                            break;
                        case 't':
                            level = 4;
                            break;
                    }

                    if (m.Groups[1].Value.Length > 1 && level >= 0)
                    {
                        capture = m.Groups[1].Value[1];

                        switch (capture)
                        {
                            case 'i':
                                value = level.ToString(CultureConstants.DefaultCulture);
                                break;
                            case 'r':
                                value = Skill.GetRomanFromInt(level);
                                break;
                        }
                    }
                    break;
            }

            return value;
        }

        /// <summary>
        /// Handles the SelectionChangeCommitted event of the cbTooltipDisplay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbTooltipDisplay_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbTooltipDisplay.SelectedIndex;

            if (index == m_tooltipCodes.Length)
            {
                tbTooltipString.Text = m_settings.Format;
                DisplayCustomControls(true);
                return;
            }

            tbTooltipString.Text = m_tooltipCodes[index];
            DisplayCustomControls(false);
        }

        /// <summary>
        /// Toggles the visibility of the tooltip example display and code label, as well as the readonly status of the tooltip string itself.
        /// </summary>
        /// <param name="custom">Show tbTooltipTestDisplay?</param>
        private void DisplayCustomControls(bool custom)
        {
            SuspendLayout();
            tbTooltipString.ReadOnly = !custom;
            ResumeLayout(false);
        }

        /// <summary>
        /// Sets the same display order as the one in popup
        /// </summary>
        private void cbTooltipOrder_CheckedChanged(object sender, EventArgs e)
        {
            m_settings.DisplayOrder = cbTooltipOrder.Checked;
        }
    }
}