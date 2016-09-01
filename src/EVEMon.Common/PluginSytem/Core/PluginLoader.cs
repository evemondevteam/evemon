using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace EVEMon.Common.PluginSytem
{
    /// <summary>
    /// Generic Plugin loader.
    /// </summary>
    public class PluginLoader<T> where T : IEVEMonPluginContract
    {
        private List<T> pluginList = new List<T>();

        /// <summary>
        /// Constructor to fill the plugin Loader with compliant plugins
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="recursive">If set to <c>true</c> recursive.</param>
        public PluginLoader(string path, bool recursive)
        {
            string[] dllFileNames = null;

            if (Directory.Exists(path))
            {
                if (recursive)
                {
                    dllFileNames = RecursiveFindDLLs(path).ToArray();
                } else
                {
                    dllFileNames = Directory.GetFiles(path, "*.dll");
                }

                List<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                Type pluginType = typeof(T);
                List<Type> pluginTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            } else
                            {
                                if (type.GetInterface(pluginType.FullName) != null)
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                List<T> plugins = new List<T>(pluginTypes.Count);
                foreach (Type type in pluginTypes)
                {
                    T plugin = (T)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }

                pluginList = plugins;
            }
        }

        /// <summary>
        /// Get the list of compliant plugins
        /// </summary>
        /// <returns>The plugins.</returns>
        public List<T> GetPlugins()
        {
            return pluginList;
        }

        private List<string> RecursiveFindDLLs (string path)
        {
            List<string> output = new List<string>();

            foreach (string d in Directory.GetDirectories(path))
            {
                output.AddRange(RecursiveFindDLLs(d));
            }

            output.AddRange(Directory.GetFiles(path, "*.dll"));
            return output;
        }
    }
}