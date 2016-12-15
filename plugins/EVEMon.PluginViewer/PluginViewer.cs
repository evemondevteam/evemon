using System;
using System.Collections.Generic;
using System.Windows.Forms;

using EVEMon.Common.PluginSytem;

namespace EVEMon.PluginViewer
{
    /// <summary>
    /// PluginViewer class used to see what plugins are loaded.
    /// </summary>
    class PluginViewer : IPluginMenuItem
    {

        #region Fields
        
        /// <summary>
        /// The MenuItem to be sent to the plugin interface.
        /// </summary>
        private ToolStripMenuItem _mnuPluginViewer;

        /// <summary>
        /// The Plugin name constant.
        /// </summary>
        private const string _pluginName = "Plugin Viewer";

        /// <summary>
        /// The plugin version constant.
        /// </summary>
        private const string _pluginVersion = "0.1";

        #endregion Fields


        #region Constructor

        /// <summary>
        /// Constructor for the PluginViewer class.
        /// </summary>
        public PluginViewer()
        {
            // Initialize the menuitem.
            ToolStripMenuItem mnuPluginViewer = new ToolStripMenuItem();
            mnuPluginViewer.Name = "mnuPluginViewer";
            mnuPluginViewer.Size = new System.Drawing.Size(114, 20);
            mnuPluginViewer.Text = "Plugin Viewer...";
            mnuPluginViewer.Click += new System.EventHandler(mnuPluginViewer_Click);
            _mnuPluginViewer = mnuPluginViewer;
        }

        #endregion Constructor


        #region IPLuginMenuItem implementation

        /// <summary>
        /// Gets the MenuItemList information for the plugin system.
        /// </summary>
        public IEnumerable<ToolStripMenuItem> MenuItemList
        {
            get
            {
                var list = new List<ToolStripMenuItem>();
                list.Add(_mnuPluginViewer);
                return list;
            }
        }

        /// <summary>
        /// Gets the plugin name for the plugin system.
        /// </summary>
        public string PluginName
        {
            get
            {
                return _pluginName;
            }

        }

        /// <summary>
        /// Gets the plugin version for the plugin system.
        /// </summary>
        public string PluginVersion
        {
            get
            {
                return _pluginVersion;
            }
        }

        #endregion IPLuginMenuItem implementation


        #region Event Handler

        /// <summary>
        /// Event handling for when the Menu item is clicked.
        /// </summary>
        /// <param name="sender">the control sending.</param>
        /// <param name="e">event args</param>
        private void mnuPluginViewer_Click(object sender, EventArgs e)
        {
            PluginViewerForm viewer = new PluginViewerForm();
            viewer.ShowDialog();
        }

        #endregion Event Handler

    }
}
