using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using EVEMon.Common.PluginSytem;

namespace EVEMon.PluginViewer
{
    public partial class PluginViewerForm : Form
    {
        #region Constructor

        /// <summary>
        /// Constructor for the Plugin Viewer form.
        /// </summary>
        public PluginViewerForm()
        {
            InitializeComponent();

            LoadPluginsInfo();
        }

        #endregion Constructor


        #region Event Handler

        /// <summary>
        /// Event handler for the close button.
        /// </summary>
        /// <param name="sender">control sender</param>
        /// <param name="e">event args</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Event Handler

        #region Methods

        /// <summary>
        /// Populates the Plugin info list and displays it.
        /// </summary>
        public void LoadPluginsInfo()
        {
            StringBuilder output = new StringBuilder();
            List<IEVEMonPluginContract> pluginList = PluginLibrary.Instance.GetAllPlugins();
            foreach (IEVEMonPluginContract pluginInfo in pluginList)
            {
                output.AppendLine($"{pluginInfo.PluginName}  v{pluginInfo.PluginVersion}");
            }
            txtPluginInfo.Text = output.ToString();
            lblPluginCount.Text = pluginList.Count.ToString();
        }

        #endregion Methods
    }
}
