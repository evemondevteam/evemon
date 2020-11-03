using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public sealed class EveMailingList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailingList"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal EveMailingList(EsiMailingListsListItem src)
        {
            ID = src.ID;
            Name = src.DisplayName;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        internal long ID { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        internal string Name { get; }
    }
}
