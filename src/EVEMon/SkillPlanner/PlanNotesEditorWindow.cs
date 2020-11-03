using System;
using System.Windows.Forms;
using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    public sealed partial class PlanNotesEditorWindow : EVEMonForm
    {
        private PlanNotesEditorWindow()
        {
            InitializeComponent();
        }

        public PlanNotesEditorWindow(string skillName)
            : this()
        {
            Text = @"Notes for " + skillName;
        }

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        /// <value>The note text.</value>
        public string NoteText
        {
            get { return NoteTextBox.Text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;

                NoteTextBox.Lines = value.Split(new[] { "\r\n", "\n\r", "\r", "\n" }, StringSplitOptions.None);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
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
        /// Handles the KeyDown event of the NoteTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
                btnOk_Click(sender, e);
        }
    }
}