using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;

namespace EVEMon.PatchXmlCreator
{
    internal static class Helper
    {
        private static Action s_action;
        private static MessageBoxCustom s_msgBox;

        private static string s_dataFilesDir;
        private static string s_patchDir;
        private static string s_sourceFilesDir;
        private static string s_solutionDir;
        private static string s_projectDir;
        private static string s_outputPath;
        
        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        internal static string Caption => "Patch Xml File Creator";

        /// <summary>
        /// Gets EVEMon execute filename.
        /// </summary>
        /// <value>
        /// The EVEMon execute filename.
        /// </value>
        internal static string EVEMonExecFilename => "EVEMon.exe";

        /// <summary>
        /// Gets the solution directory.
        /// </summary>
        /// <returns></returns>
        private static string GetSolutionDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_solutionDir))
                {
                    s_solutionDir = Regex.Match(Directory.GetCurrentDirectory(), @"[a-zA-Z]+:.*\\(?=tools)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase).ToString();
                }
                return s_solutionDir;
            }
        }

        /// <summary>
        /// Gets the project directory.
        /// </summary>
        /// <returns></returns>
        private static string GetProjectDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_projectDir))
                {
                    s_projectDir = Regex.Match(Directory.GetCurrentDirectory(), @"[a-zA-Z]+:.*\\(?=bin)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase).ToString();
                }
                return s_projectDir;
            }
        }

        /// <summary>
        /// Gets the output path.
        /// </summary>
        /// <returns></returns>
        internal static string GetOutputPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_outputPath))
                {
                    s_outputPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Remove(0, GetProjectDirectory.Length);
                }
                return s_outputPath;
            }
        }

        /// <summary>
        /// Gets the source files directory.
        /// </summary>
        /// <returns></returns>
        internal static string GetSourceFilesDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_sourceFilesDir))
                {
                    s_sourceFilesDir = Path.GetFullPath(Path.Combine(GetSolutionDirectory, @"src\EVEMon\", GetOutputPath));
                }
                return s_sourceFilesDir;
            }
        }

        /// <summary>
        /// Gets the source files directory.
        /// </summary>
        /// <returns></returns>
        internal static string GetDataFilesDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_dataFilesDir))
                {
                    s_dataFilesDir = Path.GetFullPath(Path.Combine(GetSolutionDirectory, @"src\EVEMon.Common\Resources"));
                }
                return s_dataFilesDir;
            }
        }

        /// <summary>
        /// Gets the patch directory.
        /// </summary>
        /// <returns></returns>
        private static string GetPatchDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(s_patchDir))
                {
                    s_patchDir = Path.GetFullPath(Path.Combine(GetProjectDirectory, @"Output"));
                }
                return s_patchDir;
            }
        }

        /// <summary>
        /// Gets the patch file path.
        /// </summary>
        /// <returns></returns>
        internal static string GetPatchFilePath => Path.Combine(GetPatchDirectory, EveMonConstants.PatchXmlFilename);

        /// <summary>
        /// Gets or sets a value indicating whether an application exit is requested.
        /// </summary>
        /// <value><c>true</c> if application exit is requested; otherwise, <c>false</c>.</value>
        internal static bool ExitRequested { get; private set; }

        /// <summary>
        /// Ensures that the prerequisites to run the application are met.
        /// </summary>
        internal static void EnsurePrerequisites()
        {
            string text;
            string eveMonExecFilePath = Path.Combine(GetSourceFilesDirectory, EVEMonExecFilename);

            // Ensure that a release version of EVEMon has been created
            if (!File.Exists(eveMonExecFilePath))
            {
                text = $"An EVEMon release version has to be created first{Environment.NewLine}before you can use {Caption}.";

                ShowMessage(text);
                return;
            }

            // Ensure that the installer file has been created
            if (PatchXmlCreatorWindow.GetInstallerPath().Exists)
                return;

            text = $"An EVEMon installer file has to be created first{Environment.NewLine}before you can use {Caption}.";

            ShowMessage(text);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="text">The text.</param>
        private static void ShowMessage(string text)
        {
            MessageBox.Show(text, Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            ExitRequested = true;
        }

        /// <summary>
        /// Selects the action.
        /// </summary>
        /// <returns></returns>
        internal static Action SelectAction()
        {
            s_msgBox = new MessageBoxCustom();
            s_msgBox.Button1.Click += OnButtonClick;
            s_msgBox.Button2.Click += OnButtonClick;
            s_msgBox.Button3.Click += OnButtonClick;
            s_msgBox.Text = $"{Caption} - Action Selector";
            s_msgBox.Message.Text = @"Select an action for patch file creation.";
            s_msgBox.Button1.Text = @"Datafiles Only";
            s_msgBox.Button2.Text = @"Release Only";
            s_msgBox.Button3.Text = @"Release && Datafiles";
            s_msgBox.Button1.AutoSize = true;
            s_msgBox.Button2.AutoSize = true;
            s_msgBox.Button3.AutoSize = true;
            s_msgBox.PictureBox.Image = SystemIcons.Question.ToBitmap();
            s_msgBox.CheckBox.Visible = false;

            s_msgBox.ShowDialog();

            if (s_action == Action.None)
                ExitRequested = true;

            return s_action;
        }

        /// <summary>
        /// Called when a button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnButtonClick(object sender, EventArgs e)
        {
            ButtonBase button = sender as Button;

            if (button == null)
                return;

            s_action = GetAction(button.Text);
            s_msgBox.Close();
        }

        /// <summary>
        /// Returns action based on button text.
        /// </summary>
        /// <param name="buttonText">Text on selected button.</param>
        /// <returns>Corresponding <see cref="Action"/>.</returns>
        private static Action GetAction(string buttonText)
        {
            switch (buttonText)
            {
                case "Release && Datafiles":
                    return Action.ReleaseAndDatafiles;
                case "Release Only":
                    return Action.ReleaseOnly;
                case "Datafiles Only":
                    return Action.DatafilesOnly;
                default:
                    return Action.None;
            }
        }
    }
}
