using System;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Extensions;

namespace EVEMon.DetailsWindow
{
    // TODO:
    // -Add a way to handle IEnumerables values to have the values from it instead of the type.

    public sealed partial class DetailInfoWindow : EVEMonForm
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailInfoWindow"/> class.
        /// </summary>
        private DetailInfoWindow()
        {
            InitializeComponent();

            RememberPositionKey = "DetailInfoWindow";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailInfoWindow"/> class.
        /// Constructor used in WindowsFactory.
        /// </summary>
        /// <param name="info">The information list.</param>
        /// <exception cref="System.ArgumentNullException">DetailInfo</exception>
        public DetailInfoWindow(string title, Object obj)
            : this()
        {
            obj.ThrowIfNull(nameof(obj));

            Tag = obj;
            Text = "Details: " + title;

            // fetch the properties of obj by reflection and use these as input for the window
            foreach (var prop in obj.GetType().GetProperties())
            {
                ListViewItem row = new ListViewItem();
                row.Text = prop.Name;
                row.SubItems.Add(prop.GetValue(obj, null).ToString());

                lvDetails.Items.Add(row);
            }

            lvDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #endregion Constructor


        #region Events Handlers

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DetailInfoWindow_ResizeEnd(object sender, EventArgs e)
        {
            lvDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void DetailInfoWindow_Resize(object sender, EventArgs e)
        {
            lvDetails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #endregion Events Handlers
    }
}