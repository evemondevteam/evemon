using System;
using EVEMon.Common.Attributes;
using EVEMon.Common.Models;

namespace EVEMon.Common.Collections.Global
{
    /// <summary>
    /// Represents the characters list
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalCharacterIdentityCollection : ReadonlyKeyedCollection<long, CharacterIdentity>
    {
        /// <summary>
        /// 
        /// </summary>
        internal GlobalCharacterIdentityCollection()
        {
        }

        /// <summary>
        /// Gets the character identity with the given id, or null if none was created so far.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CharacterIdentity this[long id] => GetByKey(id);

        /// <summary>
        /// Creates and stores a new character identity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <returns>The identity created.</returns>
        internal CharacterIdentity Add(long id, string name)
        {
            if (Items.ContainsKey(id))
                throw new ArgumentException("An identity with the same ID already exists.");

            CharacterIdentity identity = new CharacterIdentity(id, name);
            Items[id] = identity;
            return identity;
        }
    }
}
