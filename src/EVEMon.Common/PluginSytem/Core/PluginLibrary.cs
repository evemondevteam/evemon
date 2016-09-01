using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

// TODO: 
// - use 1 plugin loader for each interface type request in a dictionary

namespace EVEMon.Common.PluginSytem
{
    /// <summary>
    /// Library of plugins. Will allow to load matching assemblies to a specified interface.
    /// </summary>
    public class PluginLibrary
    {
        /// <summary>Allows to override the plugin path.</summary>
        /// <remarks>If changed after the plugins are used, make sure to FlushAndLoadPlugins() again.</remarks>
        public static string pluginPathOverride = null;

        /// <summary>Allows to override the plugin recursive folder check.</summary>
        /// <remarks>If changed after the plugins are used, make sure to FlushAndLoadPlugins() again.</remarks>
        public static bool pluginRecursiveOverride = true;

        // singleton
        private static Lazy<PluginLibrary> instance;

        /// <summary>Gets the instance of the library</summary>
        /// <value>The instance.</value>
        public static PluginLibrary Instance 
        {
            get 
            { 
                if (instance == null)
                {
                    instance = new Lazy<PluginLibrary>(() => new PluginLibrary(), LazyThreadSafetyMode.ExecutionAndPublication);       
                }
                return instance.Value; 
            }
        }

        private PluginLibrary()
        {
            FlushAndLoadPlugins();
        }

        private PluginLoader<IEVEMonPluginContract> pluginList;

        /// <summary>Flushs and load the plugins. Will Obliterate the old plugins loaded. Use only in rare cases.</summary>
        public void FlushAndLoadPlugins()
        {
            string pluginPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(pluginPathOverride))
            {
                pluginPath = pluginPathOverride;
            }
            pluginList = new PluginLoader<IEVEMonPluginContract>(pluginPath, pluginRecursiveOverride);
        }

        /// <summary>
        /// Gets all plugins.
        /// </summary>
        /// <returns>The all plugins.</returns>
        public List<IEVEMonPluginContract> GetAllPlugins()
        {
            return pluginList.GetPlugins();
        }

        /// <summary>
        /// Gets the plugins that matches an interface
        /// </summary>
        /// <returns>List of baseplugin (to be casted).</returns>
        /// <param name="contractType">Contract type.</param>
        public List<IEVEMonPluginContract> GetPluginByType (Type contractType)
        {
            return GetPluginByType(contractType, -1);
        }

        /// <summary>
        /// Gets the plugins that matches an interface
        /// </summary>
        /// <returns>List of baseplugin (to be casted).</returns>
        /// <param name="contractType">Contract type.</param>
        /// <param name="maxCount">The maximum number of hits. Default is -1 (no maximum)</param> 
        public List<IEVEMonPluginContract> GetPluginByType (Type contractType, int maxCount)
        {
            List<IEVEMonPluginContract> outputList = new List<IEVEMonPluginContract>();
            foreach (IEVEMonPluginContract plugin in pluginList.GetPlugins())
            {
                if (plugin.GetType().GetInterface(contractType.FullName) != null)
                {
                    outputList.Add(plugin);
                }

                if ((maxCount != -1) && (outputList.Count() >= maxCount))
                {
                    // we have enough, return the list now
                    return outputList;
                }
            }
            return outputList;
        }
    }
}