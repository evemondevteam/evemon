using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a character's identity, defined by its sole ID. 
    /// Character identities to ensure 
    /// </summary>
    public sealed class CharacterIdentity
    {
        private readonly Collection<ESIKey> m_apiKeys;

        /// <summary>
        /// Constructor from an id and a name.
        /// </summary>
        /// <param name="id">The id for this identity</param>
        /// <param name="name">The name.</param>
        internal CharacterIdentity(long id, string name)
        {
            CharacterID = id;
            CharacterName = name;

            m_apiKeys = new Collection<ESIKey>();
        }

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        /// <value>The character ID.</value>
        public long CharacterID { get; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        /// <value>The name of the character.</value>
        public string CharacterName { get; internal set; }
        
        /// <summary>
        /// Gets the API keys this identity is associated with.
        /// </summary>
        public Collection<ESIKey> ESIKeys => m_apiKeys;

        /// <summary>
        /// Gets the character type API keys.
        /// </summary>
        /// <value>The character type API keys.</value>
        public IEnumerable<ESIKey> CharacterTypeAPIKeys => ESIKeys;
        
        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter => EveMonClient.Characters.OfType<CCPCharacter>()
                .FirstOrDefault(character => character.CharacterID == CharacterID);

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public ESIKey FindAPIKeyWithAccess(ESIAPICharacterMethods method)
            => ESIKeys.FirstOrDefault(apiKey => apiKey.Monitored && (ulong)method == (apiKey.AccessMask & (ulong)method));

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public ESIKey FindAPIKeyWithAccess(ESIAPICorporationMethods method)
            => ESIKeys.FirstOrDefault(apiKey => apiKey.Monitored && (ulong)method == (apiKey.AccessMask & (ulong)method));
    }
}
