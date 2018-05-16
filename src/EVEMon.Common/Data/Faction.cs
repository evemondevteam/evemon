using EVEMon.Common.Extensions;
using System;

namespace EVEMon.Common.Data
{
    public sealed class Faction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Faction"/> class.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <exception cref="System.ArgumentNullException">station</exception>
        public Faction(int id, NPCCorporation baseCorp, NPCCorporation militiaCorp, string name)
        {
            baseCorp.ThrowIfNull(nameof(baseCorp));
            // Militia corp can be null
            if (name.IsEmptyOrUnknown())
                throw new ArgumentException("name");

            Corporation = baseCorp;
            ID = id;
            MilitiaCorporation = militiaCorp;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the faction's executor corporation.
        /// </summary>
        /// <value>The executor NPC corporation.</value>
        public NPCCorporation Corporation { get; }

        /// <summary>
        /// Gets or sets the faction's ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; }

        /// <summary>
        /// Gets or sets the faction's militia corporation.
        /// </summary>
        /// <value>The militia NPC corporation.</value>
        public NPCCorporation MilitiaCorporation { get; }

        /// <summary>
        /// Gets or sets the faction's name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
    }
}
