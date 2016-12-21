using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;


namespace EVEMon.DetailsWindow
{
    public sealed partial class DetailInfoWindow : EVEMonForm
    {

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

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}