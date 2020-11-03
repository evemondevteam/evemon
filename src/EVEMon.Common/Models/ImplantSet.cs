using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents one of the implant sets this character have (one per clone).
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class ImplantSet : ReadonlyVirtualCollection<Implant>
    {
        private string m_name;
        private readonly Character m_owner;
        private readonly Implant[] m_values;
        private const int SlotNumbers = 10;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        internal ImplantSet(Character owner, string name)
        {
            SetName(name);
            m_owner = owner;

            m_values = new Implant[SlotNumbers];
            for (int i = 0; i < SlotNumbers; i++)
            {
                m_values[i] = new Implant((ImplantSlots)i);
            }
        }

        /// <summary>
        /// Gets the set's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name == value)
                    return;

                m_name = value;

                EveMonClient.OnCharacterUpdated(m_owner);
            }
        }

        /// <summary>
        /// Gets or sets the implant for the given slot.
        /// </summary>
        /// <param name="slot">The slot for the implant to retrieve</param>
        /// <returns>The requested implant when found; null otherwise.</returns>
        private Implant this[ImplantSlots slot]
        {
            get { return slot == ImplantSlots.None ? new Implant(slot) : m_values[(int)slot]; }
            set
            {
                if (slot == ImplantSlots.None)
                    throw new InvalidOperationException("Cannot assign 'none' slot");

                if (value != null && value.Slot != slot)
                    throw new InvalidOperationException("Slot mismatch");

                m_values[(int)slot] = value ?? new Implant(slot);

                EveMonClient.OnCharacterUpdated(m_owner);
            }
        }

        /// <summary>
        /// Gets / sets the implant for the given slot.
        /// </summary>
        /// <param name="attrib">The attribute for the implant to retrieve</param>
        /// <returns>The requested implant when found; null otherwise.</returns>
        public Implant this[EveAttribute attrib]
        {
            get { return this[Implant.AttribToSlot(attrib)]; }
            set { this[Implant.AttribToSlot(attrib)] = value; }
        }

        /// <summary>
        /// Core method on which everything else relies.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Implant> Enumerate() => m_values;

        /// <summary>
        /// Generates a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableSettingsImplantSet Export() => new SerializableSettingsImplantSet
        {
            Intelligence = Export(ImplantSlots.Intelligence),
            Perception = Export(ImplantSlots.Perception),
            Willpower = Export(ImplantSlots.Willpower),
            Charisma = Export(ImplantSlots.Charisma),
            Memory = Export(ImplantSlots.Memory),
            Slot6 = Export(ImplantSlots.Slot6),
            Slot7 = Export(ImplantSlots.Slot7),
            Slot8 = Export(ImplantSlots.Slot8),
            Slot9 = Export(ImplantSlots.Slot9),
            Slot10 = Export(ImplantSlots.Slot10),
            Name = m_name
        };

        /// <summary>
        /// Exports an implant as a serialization object.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        private string Export(ImplantSlots slot) => m_values[(int)slot].Name;

        /// <summary>
        /// Imports data from a settings serialization object.
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(SerializableSettingsImplantSet serial)
        {
            SetName(serial.Name);
            Import(ImplantSlots.Intelligence, serial.Intelligence);
            Import(ImplantSlots.Perception, serial.Perception);
            Import(ImplantSlots.Willpower, serial.Willpower);
            Import(ImplantSlots.Charisma, serial.Charisma);
            Import(ImplantSlots.Memory, serial.Memory);
            Import(ImplantSlots.Slot6, serial.Slot6);
            Import(ImplantSlots.Slot7, serial.Slot7);
            Import(ImplantSlots.Slot8, serial.Slot8);
            Import(ImplantSlots.Slot9, serial.Slot9);
            Import(ImplantSlots.Slot10, serial.Slot10);
        }

        /// <summary>
        /// Updates the given slot with the provided serialization object.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="name"></param>
        private void Import(ImplantSlots slot, string name)
        {
            // Backwards compatibility for older versions
            name = name.Replace("<", string.Empty).Replace(">", string.Empty);

            m_values[(int)slot] = StaticItems.GetImplants(slot)[name] ?? new Implant(slot);
        }

        /// <summary>
        /// Imports data from an API serialization object.
        /// </summary>
        /// <param name="src">The source.</param>
        internal void Import(IEnumerable<SerializableNewImplant> src)
        {
            for (int i = 0; i < SlotNumbers; i++)
            {
                m_values[i] = StaticItems.GetImplants((ImplantSlots)i).FirstOrDefault(x => src.Any(y => y.ID == x.ID)) ??
                              new Implant((ImplantSlots)i);
            }
        }

        /// <summary>
        /// Sets the clone name. If it is invalid, it will be replaced with "Unknown".
        /// </summary>
        /// <param name="name">The clone name.</param>
        private void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                m_name = EveMonConstants.UnknownText;
            else
                m_name = name;
        }
    }
}
