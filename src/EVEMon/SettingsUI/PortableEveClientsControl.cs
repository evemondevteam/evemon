using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.SettingsUI
{
    public partial class PortableEveClientsControl : UserControl
    {
        private const int RowHeight = 29;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PortableEveClientsControl"/> class.
        /// </summary>
        public PortableEveClientsControl()
        {
            InitializeComponent();
            LayoutControls();
        }

        #endregion


        #region Controls Layout

        /// <summary>
        /// Layouts the controls.
        /// </summary>
        private void LayoutControls()
        {
            SuspendLayout();
            try
            {
                Controls.Clear();

                int height = 0;

                // Add the controls for every member of the enumeration
                foreach (SerializablePortableEveInstallation installation in Settings.PortableEveInstallations.EVEClients)
                {
                    // Add the "Delete" icon
                    AddIcon(height);

                    // Add the installation path text box
                    AddTextBox(installation, height);

                    // Add the "Browse" button
                    AddButton(height);

                    // Updates the row ordinate
                    height += RowHeight;
                }

                // Add the initial controls at the last row
                AddInitControls(height);

                height += RowHeight;

                Height = height;
            }
            finally
            {
                ResumeLayout(false);
                PerformLayout();
            }
        }

        /// <summary>
        /// Adds the initial controls.
        /// </summary>
        /// <param name="height">The height.</param>
        private void AddInitControls(int height)
        {
            Controls.Add(DeletePictureBox);
            Controls.Add(EveInstallationPathTextBox);
            Controls.Add(BrowseButton);

            DeletePictureBox.Enabled = false;
            DeletePictureBox.Location = new Point(DeletePictureBox.Location.X,
                                                  height + (RowHeight - DeletePictureBox.Size.Height) / 2);
            EveInstallationPathTextBox.Text = string.Empty;
            int offset = (int)Math.Ceiling((double)(RowHeight - EveInstallationPathTextBox.Size.Height) / 2);
            EveInstallationPathTextBox.Location = new Point(EveInstallationPathTextBox.Location.X, height + offset);
            BrowseButton.Location = new Point(BrowseButton.Location.X, height + (RowHeight - BrowseButton.Size.Height) / 2);
        }

        /// <summary>
        /// Adds the button.
        /// </summary>
        /// <param name="height">The height.</param>
        private void AddButton(int height)
        {
            Button tempButton = null;
            try
            {
                tempButton = new Button();
                tempButton.Location = new Point(BrowseButton.Location.X, height + (RowHeight - BrowseButton.Size.Height) / 2);
                tempButton.Text = BrowseButton.Text;
                tempButton.Anchor = BrowseButton.Anchor;
                tempButton.UseVisualStyleBackColor = BrowseButton.UseVisualStyleBackColor;
                tempButton.Size = BrowseButton.Size;
                tempButton.Click += BrowseButton_Click;

                Button button = tempButton;
                tempButton = null;

                Controls.Add(button);
            }
            finally
            {
                tempButton?.Dispose();
            }
        }

        /// <summary>
        /// Adds the text box.
        /// </summary>
        /// <param name="installation">The installation.</param>
        /// <param name="height">The height.</param>
        private void AddTextBox(SerializablePortableEveInstallation installation, int height)
        {
            TextBox tempTextBox = null;
            try
            {
                tempTextBox = new TextBox();
                int offset = (int)Math.Ceiling((double)(RowHeight - EveInstallationPathTextBox.Size.Height) / 2);
                tempTextBox.Location = new Point(EveInstallationPathTextBox.Location.X, height + offset);
                tempTextBox.Text = installation.Path;
                tempTextBox.Anchor = EveInstallationPathTextBox.Anchor;
                tempTextBox.ReadOnly = EveInstallationPathTextBox.ReadOnly;
                tempTextBox.Size = EveInstallationPathTextBox.Size;

                TextBox textbox = tempTextBox;
                tempTextBox = null;

                Controls.Add(textbox);
            }
            finally
            {
                tempTextBox?.Dispose();
            }
        }

        /// <summary>
        /// Adds the icon.
        /// </summary>
        /// <param name="height">The height.</param>
        private void AddIcon(int height)
        {
            PictureBox tempPicture = null;
            try
            {
                tempPicture = new PictureBox();
                tempPicture.Location = new Point(DeletePictureBox.Location.X,
                                                 height + (RowHeight - DeletePictureBox.Size.Height) / 2);
                tempPicture.Image = DeletePictureBox.Image;
                tempPicture.Size = DeletePictureBox.Size;
                tempPicture.Click += DeletePictureBox_Click;

                PictureBox picture = tempPicture;
                tempPicture = null;

                Controls.Add(picture);
            }
            finally
            {
                tempPicture?.Dispose();
            }
        }

        #endregion


        #region Exporatation to Settings

        /// <summary>
        /// Exports to settings.
        /// </summary>
        private void ExportToSettings()
        {
            Settings.PortableEveInstallations.EVEClients.Clear();
            Settings.PortableEveInstallations.EVEClients.AddRange(
                Controls.OfType<TextBox>().Where(textBox => !string.IsNullOrEmpty(textBox.Text)).Select(
                    textBox => new SerializablePortableEveInstallation
                                   {
                                       Path = textBox.Text
                                   }));
        }

        #endregion


        #region Local Event Handlers

        /// <summary>
        /// Handles the Click event of the BrowseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                    return;

                // Quit iff the selected path already exist in another control 
                if (
                    Settings.PortableEveInstallations.EVEClients.Any(
                        eveClient => eveClient.Path == folderBrowserDialog.SelectedPath))
                    return;

                TextBox textbox = Controls.OfType<TextBox>().ElementAt(Controls.OfType<Button>().IndexOf((Button)sender));
                textbox.Text = folderBrowserDialog.SelectedPath;

                ExportToSettings();
                LayoutControls();
            }
        }

        /// <summary>
        /// Handles the Click event of the DeletePictureBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeletePictureBox_Click(object sender, EventArgs e)
        {
            PictureBox icon = (PictureBox)sender;
            int index = Controls.OfType<PictureBox>().IndexOf(icon);

            // Remove and dispose the controls
            TextBox textbox = Controls.OfType<TextBox>().ElementAt(index);
            Button button = Controls.OfType<Button>().ElementAt(index);
            Controls.Remove(icon);
            Controls.Remove(textbox);
            Controls.Remove(button);
            icon.Dispose();
            textbox.Dispose();
            button.Click -= BrowseButton_Click;
            button.Dispose();

            ExportToSettings();
            LayoutControls();
        }

        #endregion
    }
}
