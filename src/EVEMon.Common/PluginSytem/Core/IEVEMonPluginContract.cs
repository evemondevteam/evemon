namespace EVEMon.Common.PluginSytem
{
    /// <summary>
    /// Base Plugin interface for the EVEMon plugins.
    /// </summary>
    public interface IEVEMonPluginContract
    {
        /// <summary>
        /// Define the plugin name.
        /// </summary>
        /// <returns>Returns a value identifying the plugin by name.</returns>
        string PluginName();
    }
}
