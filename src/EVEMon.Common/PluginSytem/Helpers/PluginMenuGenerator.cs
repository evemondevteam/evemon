using System.Collections.Generic;
using System.Windows.Forms;

namespace EVEMon.Common.PluginSytem.Helpers
{
    /// <summary>
    /// Generator to fetch the different Plugins menu.
    /// </summary>
    public class PluginMenuGenerator
    {
        /// <summary>
        /// The enumerable of the loaded plugins.
        /// </summary>
        public IEnumerable<ToolStripMenuItem> PluginData;

        /// <summary>
        /// The amount of plugins loaded.
        /// </summary>
        public int PluginCount = 0;

        /// <summary>
        /// Constructor for the PluginMenuGenerator.
        /// </summary>
        public PluginMenuGenerator()
        {
            LoadPluginData();
        }

        /// <summary>
        /// Method to launch the loading of plugins.
        /// </summary>
        public void LoadPluginData()
        {
            List<ToolStripMenuItem> output = new List<ToolStripMenuItem>();
            PluginCount = 0;

            List<IEVEMonPluginContract> pluginList = PluginLibrary.Instance.GetPluginByType(typeof(IPluginMenuItem));
            foreach (IPluginMenuItem pluginInfo in pluginList)
            {
                output.AddRange(pluginInfo.MenuItemList);
                PluginCount++;
            }

            PluginData = output.ToArray();
        }

        /// <summary>
        /// Method to know if there is any plugin loaded with menu items.
        /// </summary>
        /// <returns>A value representing the number of loaded plugins with menu items.</returns>
        public bool AnyPluginsAvailable()
        {
            return PluginCount > 0;
        }

    }
}
