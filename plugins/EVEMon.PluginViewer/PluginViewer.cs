using System;
using System.Collections.Generic;
using System.Windows.Forms;

using EVEMon.Common.PluginSytem;

namespace EVEMon.PluginViewer
{
    class PluginViewer : IPluginMenuItem
    {

        #region Fields

        private ToolStripMenuItem _mnuPluginViewer;

        private const string _pluginName = "Plugin Viewer";
        private const string _pluginVersion = "0.1";

        #endregion Fields


        #region Constructor

        public PluginViewer()
        {
            ToolStripMenuItem mnuPluginViewer = new ToolStripMenuItem();
            mnuPluginViewer.Name = "mnuPluginViewer";
            mnuPluginViewer.Size = new System.Drawing.Size(114, 20);
            mnuPluginViewer.Text = "Plugin Viewer...";
            mnuPluginViewer.Click += new System.EventHandler(mnuPluginViewer_Click);

            _mnuPluginViewer = mnuPluginViewer;
        }

        #endregion Constructor


        #region IPLuginMenuItem implementation

        public IEnumerable<ToolStripMenuItem> MenuItemList
        {
            get
            {
                var list = new List<ToolStripMenuItem>();
                list.Add(_mnuPluginViewer);
                return list;
            }
        }

        public string PluginName
        {
            get
            {
                return _pluginName;
            }

        }

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
