using System;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.CustomEventArgs
{
    /// <summary>
    /// Represents the argument for the callback for uri characters addition
    /// </summary>
    public sealed class UriCharacterEventArgs : EventArgs
    {
        private readonly CCPAPIResult<SerializableAPICharacterSheet> m_apiResult;
        private readonly SerializableCharacterSheetBase m_result;

        /// <summary>
        /// Constructor for API Characters.
        /// </summary>
        /// <param name="uri">URI of the character</param>
        /// <param name="result">API Result</param>
        /// <exception cref="System.ArgumentNullException">result</exception>
        public UriCharacterEventArgs(Uri uri, CCPAPIResult<SerializableAPICharacterSheet> result)
        {
            result.ThrowIfNull(nameof(result));

            Uri = uri;
            m_apiResult = result;
            m_result = m_apiResult.Result;
            HasError = m_apiResult.HasError;
            Error = m_apiResult.ErrorMessage;
        }

        /// <summary>
        /// Constructor for CCP Characters.
        /// </summary>
        /// <param name="uri">URI of the character</param>
        /// <param name="result">Serialized Result</param>
        public UriCharacterEventArgs(Uri uri, SerializableCharacterSheetBase result)
        {
            Uri = uri;
            m_result = result;
            HasError = false;
            Error = string.Empty;
        }

        /// <summary>
        /// Constructor for characters that throw errors.
        /// </summary>
        /// <param name="uri">URI of the chracter</param>
        /// <param name="error"></param>
        public UriCharacterEventArgs(Uri uri, string error)
        {
            Uri = uri;
            HasError = true;
            Error = error;
        }

        /// <summary>
        /// Gets the created Uri character.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets a value indicating if there was an error.
        /// </summary>
        public bool HasError { get; }

        /// <summary>
        /// Gets or sets the error which occurred.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string CharacterName => m_result.Name;

        /// <summary>
        /// Creates the character.
        /// </summary>
        public UriCharacter CreateCharacter()
        {
            CharacterIdentity identity = GetIdentity(m_result);

            // Instantiates characters, adds, notify
            var uriCharacter = m_apiResult != null ? new UriCharacter(identity, Uri, m_apiResult) :
                new UriCharacter(identity, Uri, m_result as SerializableSettingsCharacter);

            EveMonClient.Characters.Add(uriCharacter);

            return uriCharacter;
        }

        /// <summary>
        /// Updates the given character.
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public void UpdateCharacter(UriCharacter character)
        {
            character.ThrowIfNull(nameof(character));

            CharacterIdentity identity = GetIdentity(m_result);

            // Updates
            if (m_apiResult != null)
                character.Update(identity, Uri, m_apiResult);
            else
            {
                SerializableCCPCharacter ccpCharacter = m_result as SerializableCCPCharacter;
                character.Update(identity, Uri, ccpCharacter);
            }
        }

        /// <summary>
        /// Gets the character identity.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        // Retrieve the identity and create one if needed
        private static CharacterIdentity GetIdentity(ISerializableCharacterIdentity character)
            => EveMonClient.CharacterIdentities[character.ID] ??
               EveMonClient.CharacterIdentities.Add(character.ID, character.Name);
    }
}
