using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Helpers;

namespace EVEMon.Updater
{
    public partial class UpdateNotifyForm : EVEMonForm
    {
        private readonly UpdateAvailableEventArgs m_args;
        private bool m_formClosing;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private UpdateNotifyForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateNotifyForm(UpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
        }

        /// <summary>
        /// On form shown we subcribe the event handler.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Set the basic update information
            labelInfo.Text = string.Format(labelInfo.Text, m_args.CurrentVersion, m_args.NewestVersion);
            // Set the detailed update information (from the XML)
            string updMessage = m_args.UpdateMessage;
            updMessage = updMessage.Replace("\r", string.Empty);
            updateNotesTextBox.Lines = updMessage.Split('\n');

            cbAutoInstall.Enabled = m_args.CanAutoInstall;
            cbAutoInstall.Checked = m_args.CanAutoInstall;
        }

        /// <summary>
        /// Handles the FormClosing event of the UpdateNotifyForm control.
        /// </summary>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (Visible && (e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason ==
                CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.WindowsShutDown))
                m_formClosing = true;
        }
        /// <summary>
        /// Occurs on "ignore" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(Properties.Resources.PromptIgnoreUpdate,
                @"Ignore Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.No)
            {
                Settings.Updates.MostRecentDeniedUpgrade = m_args.NewestVersion.ToString();
                Settings.Save();
                DialogResult = DialogResult.Ignore;
            }
        }

        /// <summary>
        /// Occurs on "update" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbAutoInstall.Enabled && cbAutoInstall.Checked)
            {
                DialogResult result = DialogResult.Yes;
                while (result == DialogResult.Yes && !DownloadUpdate())
                {
                    // File download failed
                    result = MessageBox.Show(Properties.Resources.ErrorDownloadFailure,
                        @"Failed Download", MessageBoxButtons.YesNo);
                }
            }
            else
            {
                Util.OpenURL(m_args.ForumUrl);
                DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Downloads the update.
        /// </summary>
        private bool DownloadUpdate()
        {
            string filename = Path.GetFileName(m_args.InstallerUrl.AbsoluteUri);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            string localFilename = Path.Combine(EveMonClient.EVEMonDataDir, filename);

            // If the file already exists delete it
            if (File.Exists(localFilename))
                UpdateManager.DeleteInstallationFiles();

            using (UpdateDownloadForm form = new UpdateDownloadForm(m_args.InstallerUrl, localFilename))
            {
                if (m_formClosing || form.ShowDialog() != DialogResult.OK)
                    return false;

                string downloadedFileMD5Sum = Util.CreateMD5From(localFilename);
                if (m_args.MD5Sum != null && m_args.MD5Sum != downloadedFileMD5Sum)
                    return false;

                UpdateManager.DeleteDataFiles();
                ExecutePatcher(localFilename, m_args.AutoInstallArguments);
                DialogResult = DialogResult.OK;
            }
            return true;
        }

        /// <summary>
        /// Initiates the auto installer.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="args"></param>
        private void ExecutePatcher(string filename, string args)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show(this, Properties.Resources.ErrorInstallerNotFound,
                    @"File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(filename, args);
            }
            catch (InvalidOperationException e)
            {
                ExceptionHandler.LogException(e, true);
                if (File.Exists(filename))
                    FileHelper.DeleteFile(filename);
            }
        }

        /// <summary>
        /// Occurs on "remind me later" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLater_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
        }
    }
}
