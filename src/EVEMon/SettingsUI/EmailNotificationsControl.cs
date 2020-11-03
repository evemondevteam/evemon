using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.EmailProvider;
using EVEMon.Common.Extensions;
using EVEMon.Common.Service;
using EVEMon.Common.SettingsObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EVEMon.SettingsUI
{
    public partial class EmailNotificationsControl : UserControl
    {
        private NotificationSettings m_settings;
        private IEmailProvider m_defaultProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotificationsControl"/> class.
        /// </summary>
        public EmailNotificationsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [Browsable(false)]
        public NotificationSettings Settings
        {
            get { return m_settings; }
            set
            {
                if (value == null || m_settings == value)
                    return;

                m_settings = value;

                InitializeControls();
            }
        }

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        private void InitializeControls()
        {
            EmailProviders.Initialize();

            // Place default provider at the end
            List<IEmailProvider> providers = EmailProviders.Providers.ToList();
            m_defaultProvider = EmailProviders.Providers.First(provider => provider is DefaultProvider);
            int index = providers.IndexOf(m_defaultProvider);
            providers.RemoveAt(index);
            providers.Insert(providers.Count, m_defaultProvider);

            cbbEMailServerProvider.Items.AddRange(providers.Select(provider => provider.Name).ToArray<object>());
            tlpEmailAuthTable.Enabled = false;

            IEmailProvider emailProvider;
            // Backwards compatibility condition
            if (string.IsNullOrEmpty(m_settings.EmailSmtpServerProvider) && EmailProviders.Providers.Any(
                provider => provider.ServerAddress == m_settings.EmailSmtpServerAddress))
            {
                emailProvider = EmailProviders.Providers.First(
                    provider => provider.ServerAddress == m_settings.EmailSmtpServerAddress);
                cbbEMailServerProvider.SelectedIndex = cbbEMailServerProvider.Items.IndexOf(emailProvider.Name);
            }
                // Backwards compatibility condition
            else if (string.IsNullOrEmpty(m_settings.EmailSmtpServerProvider))
                cbbEMailServerProvider.SelectedIndex = cbbEMailServerProvider.Items.IndexOf(m_defaultProvider.Name);
                // Regular condition
            else
            {
                emailProvider = EmailProviders.GetByKey(m_settings.EmailSmtpServerProvider);
                cbbEMailServerProvider.SelectedIndex = cbbEMailServerProvider.Items.IndexOf(emailProvider.Name);
            }
        }

        /// <summary>
        /// Populates the settings from controls.
        /// </summary>
        /// <returns></returns>
        internal void PopulateSettingsFromControls()
        {
            m_settings.UseEmailShortFormat = cbEmailUseShortFormat.Checked;
            m_settings.EmailSmtpServerProvider = (string)cbbEMailServerProvider.SelectedItem;
            m_settings.EmailSmtpServerAddress = tbEmailServerAddress.Text.Trim();

            // Try and get a usable number out of the text box
            int emailPortNumber;
            m_settings.EmailPortNumber = (tbEmailPort.Text.Trim().TryParseInv(out
                emailPortNumber) && emailPortNumber > 0) ? emailPortNumber : 25;

            m_settings.EmailServerRequiresSsl = cbEmailServerRequireSsl.Checked;
            m_settings.EmailAuthenticationRequired = cbEmailAuthRequired.Checked;
            m_settings.EmailAuthenticationUserName = tbEmailUsername.Text.Trim();
            m_settings.EmailAuthenticationPassword = Util.Encrypt(tbEmailPassword.Text.Trim(), tbEmailUsername.Text.Trim());
            m_settings.EmailToAddress = tbToAddress.Text.Trim();
            m_settings.EmailFromAddress = tbFromAddress.Text.Trim();
        }

        /// <summary>
        /// Sets the controls.
        /// </summary>
        private void SetControls()
        {
            IEmailProvider provider = EmailProviders.GetByKey((string)cbbEMailServerProvider.SelectedItem);

            if (provider != null && provider != m_defaultProvider)
            {
                tbEmailServerAddress.Text = provider.ServerAddress;
                tbEmailPort.Text = provider.ServerPort.ToString(CultureConstants.DefaultCulture);
                cbEmailServerRequireSsl.Checked = provider.RequiresSsl;
                cbEmailAuthRequired.Checked = provider.RequiresAuthentication;
                tlpEmailServerSettings.Enabled = false;
            }
            else
            {
                tlpEmailServerSettings.Enabled = true;
                tbEmailServerAddress.Text = m_settings.EmailSmtpServerAddress;
                tbEmailPort.Text = m_settings.EmailPortNumber.ToString(CultureConstants.DefaultCulture);
                cbEmailServerRequireSsl.Checked = m_settings.EmailServerRequiresSsl;
                cbEmailAuthRequired.Checked = m_settings.EmailAuthenticationRequired;
            }

            tbEmailUsername.Text = m_settings.EmailAuthenticationUserName;
            tbEmailPassword.Text = Util.Decrypt(m_settings.EmailAuthenticationPassword, m_settings.EmailAuthenticationUserName);
            tbFromAddress.Text = m_settings.EmailFromAddress;
            tbToAddress.Text = m_settings.EmailToAddress;
        }

        /// <summary>
        /// Resets the controls.
        /// </summary>
        private void ResetControls()
        {
            cbEmailUseShortFormat.Checked = false;
            cbbEMailServerProvider.SelectedIndex = cbbEMailServerProvider.Items.IndexOf(m_defaultProvider.Name);
            tbEmailServerAddress.Text = m_defaultProvider.ServerAddress;
            tbEmailPort.Text = m_defaultProvider.ServerAddress;
            cbEmailServerRequireSsl.Checked = m_defaultProvider.RequiresSsl;
            cbEmailAuthRequired.Checked = m_defaultProvider.RequiresAuthentication;
            tbEmailUsername.ResetText();
            tbEmailPassword.ResetText();
            tbToAddress.ResetText();
            tbFromAddress.ResetText();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbbEMailServerProvider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbbEMailServerProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cbEmailAuthRequired control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cbEmailAuthRequired_CheckedChanged(object sender, EventArgs e)
        {
            tlpEmailAuthTable.Enabled = cbEmailAuthRequired.Checked;
        }

        /// <summary>
        /// Handles the Click event of the btnReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetControls();
        }

        /// <summary>
        /// Handles the Click event of the btnTestEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            PopulateSettingsFromControls();
            Emailer.SendTestMail(m_settings);
        }

        /// <summary>
        /// Handles the Validating event of the tbEmailServerAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbEmailServerAddress_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = string.IsNullOrWhiteSpace(tbEmailServerAddress.Text.Trim());

            if (e.Cancel)
                errorProvider.SetError(tbEmailServerAddress, "Server Address can not be blank");
        }

        /// <summary>
        /// Handles the Validating event of the tbEmailPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbEmailPort_Validating(object sender, CancelEventArgs e)
        {
            int ignore;
            e.Cancel = !SettingsForm.IsValidPort(tbEmailPort.Text, "Email Server port", out ignore);
        }

        private void tbEmailUsername_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = cbEmailAuthRequired.Checked && string.IsNullOrWhiteSpace(tbEmailUsername.Text.Trim());

            if (e.Cancel)
                errorProvider.SetError(tbEmailUsername, "Username can not be blank");
        }

        /// <summary>
        /// Handles the Validating event of the tbEmailPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbEmailPassword_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = cbEmailAuthRequired.Checked && string.IsNullOrWhiteSpace(tbEmailPassword.Text.Trim());

            if (e.Cancel)
                errorProvider.SetError(tbEmailPassword, "Password can not be blank");
        }

        /// <summary>
        /// Handles the Validating event of the tbFromAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbFromAddress_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !string.IsNullOrEmpty(tbFromAddress.Text.Trim()) && !tbFromAddress.Text.Trim().IsValidEmail();

            if (!e.Cancel)
                return;

            string text = $"{tbFromAddress.Text.Trim()} is not of a valid email format";

            errorProvider.SetError(tbFromAddress, text);
        }

        /// <summary>
        /// Handles the Validating event of the tbToAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void tbToAddress_Validating(object sender, CancelEventArgs e)
        {
            IEnumerable<string> toAddresses = tbToAddress.Text.Trim().Split(
                new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<string> invalidToAddresses = toAddresses.Where(address => !address.IsValidEmail());

            e.Cancel = string.IsNullOrEmpty(tbToAddress.Text.Trim()) || !toAddresses.Any() || invalidToAddresses.Any();

            // Receivers are not of valid email format
            if (!e.Cancel)
                return;

            string text = !invalidToAddresses.Any()
                ? "\'To address\' can not be blank"
                : $"{string.Join(", ", invalidToAddresses)} {(invalidToAddresses.Count() == 1 ? "is" : "are")}" +
                  @" not of a valid email format";

            errorProvider.SetError(tbToAddress, text);
        }

        /// <summary>
        /// Handles the Validated event of the tbEmailServerAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbEmailServerAddress_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbEmailServerAddress, string.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the tbEmailUsername control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbEmailUsername_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbEmailUsername, string.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the tbEmailPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbEmailPassword_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbEmailPassword, string.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the tbFromAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbFromAddress_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbFromAddress, string.Empty);
        }

        /// <summary>
        /// Handles the Validated event of the tbToAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tbToAddress_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbToAddress, string.Empty);
        }
    }
}
