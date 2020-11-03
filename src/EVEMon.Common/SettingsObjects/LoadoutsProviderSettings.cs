using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Loadouts;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class LoadoutsProviderSettings
    {
        private static readonly Dictionary<string, LoadoutsProvider> s_loadoutsProviders = new Dictionary<string, LoadoutsProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadoutsProviderSettings"/> class.
        /// </summary>
        public LoadoutsProviderSettings()
        {
            foreach (LoadoutsProvider provider in LoadoutsProvider.Providers)
            {
                s_loadoutsProviders[provider.Name] = provider;
            }

            ProviderName = s_loadoutsProviders.FirstOrDefault().Key ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        [XmlAttribute("provider")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        [XmlIgnore]
        public LoadoutsProvider Provider
        {
            get
            {
                if (s_loadoutsProviders.ContainsKey(ProviderName))
                    return s_loadoutsProviders[ProviderName];

                ProviderName = s_loadoutsProviders.FirstOrDefault().Key ?? string.Empty;

                return s_loadoutsProviders.FirstOrDefault().Value;
            }
        }
    }
}
