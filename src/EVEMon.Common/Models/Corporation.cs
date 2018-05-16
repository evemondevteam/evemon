namespace EVEMon.Common.Models
{
    public sealed class Corporation
    {
        private readonly Character m_character;

        /// <summary>
        /// Initializes a new instance of the <see cref="Corporation"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Corporation(Character character)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID => m_character.CorporationID;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => m_character.CorporationName;

        /// <summary>
        /// Gets the name of the corporation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
    }
}
