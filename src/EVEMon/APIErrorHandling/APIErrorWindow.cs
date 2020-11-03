using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Helpers;
using EVEMon.Common.Net;
using EVEMon.Common.Notifications;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EVEMon.ApiErrorHandling
{
    /// <summary>
    /// Displays an error window if appropriate a troubleshooter is displayed to help the user resolve the issue.
    /// </summary>
    public partial class ApiErrorWindow : EVEMonForm
    {
        private readonly HttpTimeoutTroubleshooter m_httpTimeoutTroubleshooter = new HttpTimeoutTroubleshooter();

        private APIErrorNotificationEventArgs m_notification;
        private ApiErrorTroubleshooter m_troubleshooter;
        private bool m_troubleshooterUsed;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiErrorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the notification for this error.
        /// </summary>
        internal APIErrorNotificationEventArgs Notification
        {
            get { return m_notification; }
            set
            {
                if (value != null)
                {
                    var exception = value.Result?.Exception;
                    string errorText = GetErrorLabelText(value);
                    m_notification = value;
                    ErrorLabel.Text = errorText;
                    // Several clients are getting TrustFailure for badly configured SSL
                    // certificates, provide better debugging info
                    if (errorText.ToLower().Contains("trustfailure"))
                        DetailsTextBox.Text = Properties.Resources.ErrorTrustFailure;
                    else if (exception != null)
                        DetailsTextBox.Text = exception.ToString();
                    DisplayTroubleshooter(exception);
                }
            }
        }

        /// <summary>
        /// Displays the troubleshooter.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private void DisplayTroubleshooter(Exception exception)
        {
            TroubleshooterPanel.Visible = false;

            m_troubleshooter = GetTroubleshooter(exception);

            if (m_troubleshooter == null)
                return;

            TroubleshooterPanel.Visible = true;
            TroubleshooterPanel.Controls.Add(m_troubleshooter);

            m_troubleshooter.ErrorResolved += troubleshooter_ErrorResolved;
            m_troubleshooter.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Gets the troubleshooter.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A troubleshooter for the error message.</returns>
        private ApiErrorTroubleshooter GetTroubleshooter(Exception exception)
        {
            var httpException = exception as HttpWebClientServiceException;
            return httpException?.Status == HttpWebClientServiceExceptionStatus.Timeout ?
                m_httpTimeoutTroubleshooter : null;
        }

        /// <summary>
        /// Handles the ErrorResolved event when a http timeout is displayed of the troubleshooter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ApiErrorTroubleshooterEventArgs"/> instance containing the event data.</param>
        private void troubleshooter_ErrorResolved(object sender, ApiErrorTroubleshooterEventArgs e)
        {
            m_troubleshooterUsed = true;

            if (e == null)
                return;

            if (!e.Resolved)
            {
                TroubleshooterPanel.BackColor = Color.DarkSalmon;
                return;
            }

            EveMonClient.Notifications.Invalidate(new NotificationInvalidationEventArgs(
                m_notification));
            PerformAction(e.Action);
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="action">The action.</param>
        private void PerformAction(ResolutionAction action)
        {
            switch (action)
            {
                case ResolutionAction.Close:
                    Close();
                    break;
                case ResolutionAction.HideTroubleshooter:
                    TroubleshooterPanel.Hide();
                    break;
                case ResolutionAction.None:
                    TroubleshooterPanel.BackColor = Color.PaleGreen;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the error label text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>String representing the error.</returns>
        private static string GetErrorLabelText(APIErrorNotificationEventArgs value)
        {
            if (value == null)
                return "No error selected.";
            return value.ToString() + Environment.NewLine + (value.Result == null ?
                "No details were provided." : GetErrorLabelTextDetail(value.Result));
        }

        /// <summary>
        /// Gets the error label text detail.
        /// </summary>
        /// <param name="result">The value.</param>
        private static string GetErrorLabelTextDetail(IAPIResult result)
        {
            switch (result.ErrorType)
            {
                case APIErrorType.None:
                    return "No error specified";
                case APIErrorType.CCP:
                    return $"CCP Error: {result.ErrorMessage}";
                case APIErrorType.Http:
                    return $"HTTP error: {result.ErrorMessage}";
                case APIErrorType.Xml:
                    return $"XML error: {result.ErrorMessage}";
                case APIErrorType.Json:
                    return $"XSLT error: {result.ErrorMessage}";
                default:
                    throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// On closing, disposes of the troubleshooter.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (m_troubleshooter != null)
            {
                m_troubleshooter.Dispose();
                m_troubleshooter = null;
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the CopyToClipboardLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CopyToClipboardLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("EVEMon ");
            builder.AppendLine(EveMonClient.FileVersionInfo.FileVersion);
            builder.AppendLine().AppendLine("API Error:");
            builder.AppendLine(GetErrorLabelText(Notification));

            if (m_troubleshooter != null)
            {
                builder.AppendLine();
                builder.Append("A troubleshooter was displayed " + (m_troubleshooterUsed ?
                    "and used." : "but not used."));
            }
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(builder.ToString(), TextDataFormat.Text);
            }
            catch (ExternalException ex)
            {
                // Occurs when another process is using the clipboard
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show(Properties.Resources.ErrorClipboardFailure, "Error copying",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
