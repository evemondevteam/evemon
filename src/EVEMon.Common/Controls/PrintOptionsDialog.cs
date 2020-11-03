using System;
using System.Drawing.Printing;
using EVEMon.Common.Extensions;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Controls
{
    public partial class PrintOptionsDialog : EVEMonForm
    {
        private readonly PlanExportSettings m_pto;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintOptionsDialog"/> class.
        /// </summary>
        /// <param name="pto">The pto.</param>
        /// <param name="doc">The document.</param>
        /// <exception cref="System.ArgumentNullException">pto or doc</exception>
        public PrintOptionsDialog(PlanExportSettings pto, PrintDocument doc)
        {
            pto.ThrowIfNull(nameof(pto));

            doc.ThrowIfNull(nameof(doc));

            InitializeComponent();

            string curPrinter = doc.PrinterSettings.PrinterName;

            m_pto = pto;

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                int index = comboPrinters.Items.Add(printer);

                doc.PrinterSettings.PrinterName = printer;
                if (doc.PrinterSettings.IsDefaultPrinter)
                    comboPrinters.SelectedIndex = index;
            }

            // If this dialog is cancelled, we dont want the name of the printer to have changed
            doc.PrinterSettings.PrinterName = curPrinter;

            EntryFinishDate = pto.EntryFinishDate;
            EntryNumber = pto.EntryNumber;
            EntryStartDate = pto.EntryStartDate;
            EntryTrainingTimes = pto.EntryTrainingTimes;
            EntryNotes = pto.EntryNotes;
            FooterCount = pto.FooterCount;
            FooterDate = pto.FooterDate;
            FooterTotalTime = pto.FooterTotalTime;
            IncludeHeader = pto.IncludeHeader;
        }

        /// <summary>
        /// Gets the name of the printer.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName => comboPrinters.Items[comboPrinters.SelectedIndex].ToString();

        /// <summary>
        /// Gets or sets a value indicating whether [entry finish date].
        /// </summary>
        /// <value><c>true</c> if [entry finish date]; otherwise, <c>false</c>.</value>
        public bool EntryFinishDate
        {
            get { return checkFinishDate.Checked; }
            set { checkFinishDate.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [entry number].
        /// </summary>
        /// <value><c>true</c> if [entry number]; otherwise, <c>false</c>.</value>
        public bool EntryNumber
        {
            get { return checkEntryNumber.Checked; }
            set { checkEntryNumber.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [entry start date].
        /// </summary>
        /// <value><c>true</c> if [entry start date]; otherwise, <c>false</c>.</value>
        public bool EntryStartDate
        {
            get { return checkStartDate.Checked; }
            set { checkStartDate.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [entry training times].
        /// </summary>
        /// <value><c>true</c> if [entry training times]; otherwise, <c>false</c>.</value>
        public bool EntryTrainingTimes
        {
            get { return checkTrainingTimes.Checked; }
            set { checkTrainingTimes.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [entry notes].
        /// </summary>
        /// <value><c>true</c> if [entry notes]; otherwise, <c>false</c>.</value>
        public bool EntryNotes
        {
            get { return checkNotes.Checked; }
            set { checkNotes.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [footer count].
        /// </summary>
        /// <value><c>true</c> if [footer count]; otherwise, <c>false</c>.</value>
        public bool FooterCount
        {
            get { return checkPageNumbers.Checked; }
            set { checkPageNumbers.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [footer date].
        /// </summary>
        /// <value><c>true</c> if [footer date]; otherwise, <c>false</c>.</value>
        public bool FooterDate
        {
            get { return checkDateInformation.Checked; }
            set { checkDateInformation.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [footer total time].
        /// </summary>
        /// <value><c>true</c> if [footer total time]; otherwise, <c>false</c>.</value>
        public bool FooterTotalTime
        {
            get { return checkTotalTimes.Checked; }
            set { checkTotalTimes.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include header].
        /// </summary>
        /// <value><c>true</c> if [include header]; otherwise, <c>false</c>.</value>
        public bool IncludeHeader
        {
            get { return checkPageHeaders.Checked; }
            set { checkPageHeaders.Checked = value; }
        }

        /// <summary>
        /// Called when [set as defaults].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSetAsDefaults(object sender, EventArgs e)
        {
            OnAccept(sender, e);
            Settings.Save();
        }

        /// <summary>
        /// Called when [accept].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAccept(object sender, EventArgs e)
        {
            m_pto.EntryFinishDate = EntryFinishDate;
            m_pto.EntryNumber = EntryNumber;
            m_pto.EntryStartDate = EntryStartDate;
            m_pto.EntryTrainingTimes = EntryTrainingTimes;
            m_pto.EntryNotes = EntryNotes;
            m_pto.FooterCount = FooterCount;
            m_pto.FooterDate = FooterDate;
            m_pto.FooterTotalTime = FooterTotalTime;
            m_pto.IncludeHeader = IncludeHeader;
        }
    }
}