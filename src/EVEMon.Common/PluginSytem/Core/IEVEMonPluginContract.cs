namespace EVEMon.Common.PluginSytem
{
    /// <summary>
    /// Base Plugin interface for the EVEMon plugins.
    /// </summary>
    public interface IEVEMonPluginContract
    {
        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Gets the plugin version information.
        /// </summary>
        string PluginVersion { get; }
    }
}
