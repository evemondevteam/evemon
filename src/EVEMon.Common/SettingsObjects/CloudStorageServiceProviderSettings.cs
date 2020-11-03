using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.CloudStorageServices;

namespace EVEMon.Common.SettingsObjects
{

    public sealed class CloudStorageServiceProviderSettings
    {
        private static readonly Dictionary<string, CloudStorageServiceProvider> s_cloudStorageServiceProviders = new Dictionary<string, CloudStorageServiceProvider>();

        public CloudStorageServiceProviderSettings()
        {
            foreach (CloudStorageServiceProvider provider in CloudStorageServiceProvider.Providers)
            {
                s_cloudStorageServiceProviders[provider.Name] = provider;
            }

            ProviderName = s_cloudStorageServiceProviders.FirstOrDefault().Key ?? string.Empty;
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
        public CloudStorageServiceProvider Provider
        {
            get
            {
                if (s_cloudStorageServiceProviders.ContainsKey(ProviderName))
                    return s_cloudStorageServiceProviders[ProviderName];

                ProviderName = s_cloudStorageServiceProviders.FirstOrDefault().Key ?? string.Empty;

                return s_cloudStorageServiceProviders.FirstOrDefault().Value;
            }
        }
    }
}
