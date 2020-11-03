using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Service;

namespace EVEMon.Common.Controls
{
    public partial class CharacterPortrait : UserControl
    {
        private Character m_character;
        private bool m_updatingPortrait;
        private bool m_pendingUpdate;


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacterPortrait()
        {
            InitializeComponent();
            pictureBox.Image = pictureBox.InitialImage;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// When the control is made visible, we check for a pending update.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // If we previously delayed an update, we do it now.
            if (Visible && m_pendingUpdate)
                UpdateContent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        [Browsable(false)]
        public Character Character
        {
            get { return m_character; }
            set
            {
                if (value == null || m_character == value)
                    return;

                m_character = value;
                UpdateContent();
            }
        }

        #endregion


        #region Default mechanism on character change (portraits cache, then ImageService for CCP url)

        /// <summary>
        /// Update the portrait.
        /// <list type="bullet">
        /// <item>We check for a cached portrait in %APPDATA%\cache\portraits.</item>
        /// <item>It if failed, we assemble the url for a CCP download and give it to ImageService.</item>
        /// <item>ImageService will first check its cache (%APPDATA%\cache),
        /// then download the url if no image was found in cache.</item>
        /// </list>
        /// </summary>
        /// <remarks>Note this method will first check the ImageService cache before to resort to download.</remarks>
        private void UpdateContent()
        {
            if (!Visible)
            {
                m_pendingUpdate = true;
                return;
            }
            m_pendingUpdate = false;

            // In safe mode, doesn't bother with the character portrait
            if (Settings.UI.SafeForWork)
                return;

            if (m_character == null)
                return;

            // Try to retrieve the portrait from our portrait cache (%APPDATA%\cache\portraits)
            var image = ImageService.GetImageFromCache($"{m_character.Guid}.png",
                EveMonClient.EVEMonPortraitCacheDir);
            if (image != null)
            {
                pictureBox.Image = image;
            }
            else
            {
                // The image does not exist in cache, we try to retrieve it from CCP
                pictureBox.Image = pictureBox.InitialImage;
                UpdateCharacterImageFromCCPAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Download the image from CCP...
        /// <list type="bullet">
        /// <item>We assemble the url for a CCP download and give it to ImageService.</item>
        /// <item>ImageService will first check its cache (%APPDATA%\cache),
        /// then download the url if no image was found in cache.</item>
        /// </list>
        /// </summary>
        private async Task UpdateCharacterImageFromCCPAsync()
        {
            if (m_updatingPortrait)
                return;

            // Skip if it's a blank character
            if (m_character.CharacterID == UriCharacter.BlankCharacterID)
                return;

            pictureBox.Cursor = Cursors.WaitCursor;

            m_updatingPortrait = true;

            Image image = await ImageService.GetCharacterImageAsync(m_character.CharacterID);

            pictureBox.Cursor = CustomCursors.ContextMenu;

            // Release the updating flag
            m_updatingPortrait = false;

            if (image == null)
                return;

            // Update the portrait
            pictureBox.Image = image;

            // The image was retrieved, we save it to the cache
            await SaveCharacterImageToCacheAsync(image);
        }

        /// <summary>
        /// Save the specified image to the EVEMon cache as this character's portrait.
        /// </summary>
        /// <param name="image">The portrait image.</param>
        private Task SaveCharacterImageToCacheAsync(Image image)
        {
            if (m_character == null)
                return Task.CompletedTask;

            // Save to the portraits cache
            return ImageService
                .AddImageToCacheAsync(image, $"{m_character.Guid}.png", EveMonClient.EVEMonPortraitCacheDir);
        }

        #endregion


        #region Mechanisms related to the game folder

        /// <summary>
        /// Download the image from the EVE cache (in EVE Online client installation folder).
        /// </summary>
        private Task UpdateCharacterFromEVECacheAsync()
        {
            m_updatingPortrait = true;

            // If we don't have the game's portraits cache already, prompt the user
            // Return if the user canceled
            if (!EveMonClient.DefaultEvePortraitCacheFolders.Any() ||
                EveMonClient.EvePortraitCacheFolders == null ||
                !EveMonClient.EvePortraitCacheFolders.Any())
            {
                if (!ChangeEVEPortraitCache() || EveMonClient.EvePortraitCacheFolders == null)
                {
                    m_updatingPortrait = false;
                    return Task.CompletedTask;
                }
            }

            // Now, search in the game folder all matching files 
            // (different resolutions are available for every character)
            // Retrieve all files in the EVE cache directory which matches "<characterId>*"
            List<FileInfo> filesInEveCache = new List<FileInfo>();
            List<FileInfo> imageFilesInEveCache = new List<FileInfo>();
            foreach (DirectoryInfo di in EveMonClient.EvePortraitCacheFolders
                .Select(evePortraitCacheFolder => new DirectoryInfo(evePortraitCacheFolder))
                .Where(directory => directory.Exists))
            {
                filesInEveCache.AddRange(di.GetFiles($"{m_character.CharacterID}*"));

                // Look up for an image file and add it to the list
                // Note by Jimi : CCP changed image format in Incursion 1.1.0
                // as part of new character portraits creator,
                // so I added an image file check method to provide compatibility
                // with all image formats
                imageFilesInEveCache.AddRange(filesInEveCache.Where(IsImageFile));
            }

            // Displays an error message if none found
            if (!imageFilesInEveCache.Any())
            {
                StringBuilder message = new StringBuilder();

                message.AppendLine("No portraits for your character were found in the folder you selected.");
                message.AppendLine().AppendLine("Ensure that you have checked the following:");
                message.AppendLine(" - You have logged into EVE with that characters' account.");
                message.AppendLine(" - You have selected a folder that contains EVE Portraits.");

                if (EveMonClient.DefaultEvePortraitCacheFolders.Any())
                {
                    message.AppendLine("Your default EVE Portrait directory is:");
                    message.Append(EveMonClient.DefaultEvePortraitCacheFolders.First());
                }

                MessageBox.Show(message.ToString(), @"Portrait Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                m_updatingPortrait = false;
                return Task.CompletedTask;
            }

            // Search for the largest portrait
            int bestSize = 0;
            string bestFile = string.Empty;
            int charIDLength = m_character.CharacterID.ToString(CultureConstants.
                InvariantCulture).Length;
            foreach (FileInfo file in imageFilesInEveCache)
            {
                int sizeLength = file.Name.Length - (file.Extension.Length + 1) - charIDLength,
                    imageSize;
                if (file.Name.Substring(charIDLength + 1, sizeLength).TryParseInv(out
                    imageSize) && imageSize > bestSize)
                {
                    bestFile = file.FullName;
                    bestSize = imageSize;
                }
            }

            // Open the largest image and save it
            Image image = Image.FromFile(bestFile);

            // Release the updating flag
            m_updatingPortrait = false;

            // Update the portrait
            pictureBox.Image = image;

            return SaveCharacterImageToCacheAsync(image);
        }

        /// <summary>
        /// Determines whether the specified file is an image file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the specified file is an image file; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsImageFile(FileInfo file)
        {
            try
            {
                Image.FromFile(file.FullName);
                return true;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Pops up a window for the user to select their EVE portrait cache folder.
        /// </summary>
        private static bool ChangeEVEPortraitCache()
        {
            using (EveFolderWindow folderWindow = new EveFolderWindow())
            {
                if (folderWindow.ShowDialog() != DialogResult.OK)
                    return false;

                EveMonClient.EvePortraitCacheFolders = folderWindow.SpecifiedEVEPortraitCacheFolder;
                return true;
            }
        }

        #endregion


        #region Controls and global events handler

        /// <summary>
        /// Handles the Click event of the miUpdatePicture control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void miUpdatePicture_Click(object sender, EventArgs e)
        {
            await UpdateCharacterImageFromCCPAsync();
        }

        /// <summary>
        /// Handles the Click event of the miUpdatePictureFromEVECache control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void miUpdatePictureFromEVECache_Click(object sender, EventArgs e)
        {
            await UpdateCharacterFromEVECacheAsync();
        }

        /// <summary>
        /// Handles the Click event of the miSetEVEFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void miSetEVEFolder_Click(object sender, EventArgs e)
        {
            ChangeEVEPortraitCache();
        }

        /// <summary>
        /// Handles the MouseDown event of the pictureBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_updatingPortrait)
                return;

            pictureBox.Cursor = Cursors.Default;

            if (e.Button == MouseButtons.Right)
                return;

            cmsPictureOptions.Show(MousePosition);
        }

        /// <summary>
        /// Handles the MouseMove event of the pictureBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            pictureBox.Cursor = CustomCursors.ContextMenu;
        }

        #endregion
    }
}
