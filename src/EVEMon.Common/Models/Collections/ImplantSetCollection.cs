using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Data;
using EVEMon.Common.Service;
using System.Globalization;

namespace EVEMon.Common.Models.Collections
{
    /// <summary>
    /// Represents a collection of implants sets.
    /// </summary>
    public sealed class ImplantSetCollection : ReadonlyVirtualCollection<ImplantSet>
    {
        private readonly Character m_character;
        private readonly List<ImplantSet> m_cloneSets;
        private readonly List<ImplantSet> m_customSets;
        private ImplantSet m_current;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="character"></param>
        internal ImplantSetCollection(Character character)
        {
            m_character = character;
            m_customSets = new List<ImplantSet>();
            m_cloneSets = new List<ImplantSet>();
            ActiveClone = new ImplantSet(character, "Active Clone");
            None = new ImplantSet(character, "None");
            m_current = ActiveClone;
        }

        /// <summary>
        /// Gets the implant set by its index.
        /// First items are <see cref="ActiveClone"/> then the jump clones then the custom sets.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ImplantSet this[int index] => Enumerate().ElementAt(index);

        /// <summary>
        /// Gets the none implant.
        /// </summary>
        public ImplantSet None { get; }

        /// <summary>
        /// Gets the implants retrieved from the API.
        /// </summary>
        public ImplantSet ActiveClone { get; }

        /// <summary>
        /// Gets or sets the current implant set.
        /// </summary>
        public ImplantSet Current
        {
            get { return m_current; }
            set
            {
                if (m_current == value)
                    return;

                m_current = value;
                EveMonClient.OnCharacterUpdated(m_character);
            }
        }

        /// <summary>
        /// Adds a new implant set.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ImplantSet Add(string name)
        {
            ImplantSet set = new ImplantSet(m_character, name);
            m_customSets.Add(set);
            EveMonClient.OnCharacterUpdated(m_character);
            return set;
        }

        /// <summary>
        /// Removes the given set.
        /// </summary>
        /// <param name="set"></param>
        public void Remove(ImplantSet set)
        {
            m_customSets.Remove(set);
            EveMonClient.OnCharacterUpdated(m_character);
        }

        /// <summary>
        /// Core method on which everything else relies.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ImplantSet> Enumerate()
        {
            yield return ActiveClone;

            foreach (ImplantSet set in m_cloneSets)
            {
                yield return set;
            }

            foreach (ImplantSet set in m_customSets)
            {
                yield return set;
            }
        }

        /// <summary>
        /// Imports data from an ESI deserialization object.
        /// </summary>
        public void Import(EsiAPIClones serial)
        {
            if (serial == null)
                return;

            m_cloneSets.Clear();
            // Jump clones
            foreach (var clone in serial.JumpClones)
            {
                int cloneID = clone.JumpCloneID;
                string name = clone.Name;
                // Try to pick a sane name if it is null
                if (string.IsNullOrEmpty(name))
                {
                    var location = EveIDToStation.GetIDToStation(clone.LocationID);
                    if (location == null)
                        name = "Clone at location #" + clone.LocationID.ToString(CultureInfo.
                            InvariantCulture);
                    else
                        name = "Clone in " + location.Name;
                }
                ImplantSet set = new ImplantSet(m_character, name);
                // Jump clone implants
                var jcImplants = new LinkedList<SerializableNewImplant>();
                foreach (int implant in clone.Implants)
                    jcImplants.AddLast(new SerializableNewImplant()
                    {
                        ID = implant,
                        Name = StaticItems.GetItemName(implant)
                    });
                set.Import(jcImplants);
                m_cloneSets.Add(set);
            }

            EveMonClient.OnCharacterImplantSetCollectionChanged(m_character);
        }

        /// <summary>
        /// Imports data from a deserialization object.
        /// </summary>
        public void Import(SerializableImplantSetCollection serial)
        {
            if (serial == null)
                return;

            ActiveClone.Import(serial.ActiveClone);

            m_cloneSets.Clear();
            foreach (SerializableSettingsImplantSet serialSet in serial.JumpClones)
            {
                ImplantSet set = new ImplantSet(m_character, serialSet.Name);
                set.Import(serialSet);
                m_cloneSets.Add(set);
            }

            m_customSets.Clear();
            foreach (SerializableSettingsImplantSet serialSet in serial.CustomSets)
            {
                ImplantSet set = new ImplantSet(m_character, serialSet.Name);
                set.Import(serialSet);
                m_customSets.Add(set);
            }

            // Imports selection
            m_current = this[serial.SelectedIndex];

            EveMonClient.OnCharacterImplantSetCollectionChanged(m_character);
        }

        /// <summary>
        /// Imports data from an API serialization object provided by CCP.
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(SerializableAPICharacterSheet serial)
        {
            if (serial == null)
                return;

            // Import the active clone implants
            ActiveClone.Import(serial.Implants);

            m_cloneSets.Clear();
            foreach (SerializableCharacterJumpClone jumpClone in serial.JumpClones)
            {
                var cloneImplants = serial.JumpCloneImplants.Where(x => x.JumpCloneID ==
                    jumpClone.JumpCloneID).Select(cloneImplant => new SerializableNewImplant
                    {
                        ID = cloneImplant.TypeID,
                        Name = cloneImplant.TypeName
                    });

                ImplantSet set = new ImplantSet(m_character, jumpClone.CloneName);
                set.Import(cloneImplants);
                m_cloneSets.Add(set);
            }

            EveMonClient.OnCharacterImplantSetCollectionChanged(m_character);
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        public SerializableImplantSetCollection Export()
        {
            SerializableImplantSetCollection serial = new SerializableImplantSetCollection { ActiveClone = ActiveClone.Export() };
            serial.JumpClones.AddRange(m_cloneSets.Select(x => x.Export()));
            serial.CustomSets.AddRange(m_customSets.Select(x => x.Export()));
            serial.SelectedIndex = Enumerate().IndexOf(m_current);
            return serial;
        }
    }
}
