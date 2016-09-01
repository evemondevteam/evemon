using System.Collections.Generic;
using System.Windows.Forms;

namespace EVEMon.Common.PluginSytem
{
    /// <summary>
    /// Plugin contract to add a MenuItem in the plugins list.
    /// </summary>
    public interface IPluginMenuItem : IEVEMonPluginContract
    {
        /// <summary>
        /// Fill with the list of MenuItem that will be appended to the Main menu list.
        /// </summary>
        IEnumerable<ToolStripMenuItem> MenuItemList { get; }
    }
}
