using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Models;

namespace EVEMon.Common.Collections.Global
{
    /// <summary>
    /// Represents the API providers defined by the user.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalAPIProviderCollection : ReadonlyVirtualCollection<APIProvider>
    {
        private readonly List<APIProvider> m_customProviders = new List<APIProvider>();
        private APIProvider m_currentProvider;

        /// <summary>
        /// Private constructor, only the mother class can instantiate it.
        /// </summary>
        internal GlobalAPIProviderCollection()
        {
            CurrentProvider = DefaultProvider;
        }


        #region Public properties

        /// <summary>
        /// Gets the default provider.
        /// </summary>
        public static APIProvider DefaultProvider => APIProvider.DefaultProvider;

        /// <summary>
        /// Gets the default provider.
        /// </summary>
        public static APIProvider TestProvider => APIProvider.TestProvider;
        
        /// <summary>
        /// Gets the used provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">The given provider is not in the list</exception>
        public APIProvider CurrentProvider
        {
            get { return m_currentProvider; }
            set
            {
                // Is it the default provider ?
                if (APIProvider.DefaultProvider == value)
                    m_currentProvider = value;
                // is it the test provider
                else if (APIProvider.TestProvider == value)
                    m_currentProvider = value;
                // Then it's a non-register provider, we messed up since it should be in this global collection
                else
                    throw new InvalidOperationException("The given provider is not in the list");
            }
        }

        #endregion


        #region Indexer

        /// <summary>
        /// Gets an API provider by its name, returning null when not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The wanted API provider when found; null otherwise.</returns>
        // Is it the default provider ? If not look among custom providers
        private APIProvider this[string name]
            => DefaultProvider.Name == name
                ? DefaultProvider
                : m_customProviders.FirstOrDefault(provider => provider.Name == name);

        #endregion
        

        /// <summary>
        /// Core method to implement for collection services.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<APIProvider> Enumerate()
        {
            yield return APIProvider.DefaultProvider;

            foreach (APIProvider provider in m_customProviders)
            {
                yield return provider;
            }
        }
    }
}
