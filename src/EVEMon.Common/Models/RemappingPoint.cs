using System;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a remapping operation attached to a plan entry
    /// </summary>
    public sealed class RemappingPoint
    {
        private readonly long[] m_attributes = new long[5];
        private string m_description = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RemappingPoint()
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <exception cref="System.ArgumentNullException">serial</exception>
        public RemappingPoint(SerializableRemappingPoint serial)
        {
            serial.ThrowIfNull(nameof(serial));

            Guid = Guid.NewGuid();
            m_attributes[(int)EveAttribute.Intelligence] = serial.Intelligence;
            m_attributes[(int)EveAttribute.Perception] = serial.Perception;
            m_attributes[(int)EveAttribute.Willpower] = serial.Willpower;
            m_attributes[(int)EveAttribute.Charisma] = serial.Charisma;
            m_attributes[(int)EveAttribute.Memory] = serial.Memory;
            m_description = serial.Description;
            Status = serial.Status;
        }

        /// <summary>
        /// Gets a global identified of this remapping point.
        /// </summary>
        [XmlIgnore]
        public Guid Guid { get; private set; }

        /// <summary>
        /// Gets the point's status (whether is has been initialized/computed or not).
        /// </summary>
        [XmlIgnore]
        public RemappingPointStatus Status { get; private set; }

        /// <summary>
        /// Gets the new base value for the given attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public long this[EveAttribute attrib] => m_attributes[(int)attrib];

        /// <summary>
        /// Gets a short string representation of the point ("i5 p7 c8 w9 m5").
        /// </summary>
        /// <returns></returns>
        private string ToShortString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("i").Append(m_attributes[(int)EveAttribute.Intelligence].ToString(CultureConstants.DefaultCulture)).
                Append(" p").Append(m_attributes[(int)EveAttribute.Perception].ToString(CultureConstants.DefaultCulture)).
                Append(" c").Append(m_attributes[(int)EveAttribute.Charisma].ToString(CultureConstants.DefaultCulture)).
                Append(" w").Append(m_attributes[(int)EveAttribute.Willpower].ToString(CultureConstants.DefaultCulture)).
                Append(" m").Append(m_attributes[(int)EveAttribute.Memory].ToString(CultureConstants.DefaultCulture));

            return builder.ToString();
        }

        /// <summary>
        /// Gets a long string representation of this point. Two possible formats : 
        /// <list type="">
        /// <item>"Remapping (not computed, use the attributes optimizer)"</item>
        /// <item>"Remapping (active) : &lt;description&gt;</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public string ToLongString()
        {
            switch (Status)
            {
                case RemappingPointStatus.NotComputed:
                    return "Remapping (not computed, use the attributes optimizer)";
                case RemappingPointStatus.UpToDate:
                    return $"Remapping : {m_description}";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets a string representation of this point. Two possible formats : 
        /// <list type="">
        /// <item>"Remapping (not computed, use the attributes optimizer)"</item>
        /// <item>"Remapping (active) : i5 p7 c8 w9 m5"</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Status)
            {
                case RemappingPointStatus.NotComputed:
                    return "Remapping (not computed, use the attributes optimizer)";
                case RemappingPointStatus.UpToDate:
                    return $"Remapping (active) : {ToShortString()}";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Will set the provided character scratchpad's base attributes as the target values to remap.
        /// </summary>
        /// <param name="newScratchpad">The scratchpad with the target base values to assign to this point</param>
        /// <param name="oldScratchpad">The scratchpad before we remapped</param>
        internal void SetBaseAttributes(CharacterScratchpad newScratchpad, CharacterScratchpad oldScratchpad)
        {
            // Update the status
            Status = RemappingPointStatus.UpToDate;

            // Initialize the string
            StringBuilder builder = new StringBuilder();

            // Scroll through attributes
            for (int i = 0; i < 5; i++)
            {
                // Compute the new base attribute
                EveAttribute attrib = (EveAttribute)i;
                m_attributes[i] = newScratchpad[attrib].Base;

                // Update description
                builder.AppendLine().Append(GetStringForAttribute(attrib, oldScratchpad, newScratchpad));
            }

            // Return the final string
            m_description = builder.ToString();
        }

        /// <summary>
        /// Gets a string representation of the attribute.
        /// </summary>
        /// <param name="attrib">The attribute.</param>
        /// <param name="oldScratchpad">The old scratchpad.</param>
        /// <param name="newScratchpad">The new scratchpad.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// oldScratchpad
        /// or
        /// newScratchpad
        /// </exception>
        public static string GetStringForAttribute(EveAttribute attrib, CharacterScratchpad oldScratchpad,
            CharacterScratchpad newScratchpad)
        {
            oldScratchpad.ThrowIfNull(nameof(oldScratchpad));

            newScratchpad.ThrowIfNull(nameof(newScratchpad));

            long bonusDifference = newScratchpad[attrib].Base - oldScratchpad[attrib].Base;

            if (bonusDifference == 0)
                return newScratchpad[attrib].ToString("%N (0) = %e = (%B + %r + %i)");

            return newScratchpad[attrib].ToString(bonusDifference > 0
                ? $"%N (+{bonusDifference}) = %e = (%B + %r + %i)"
                : $"%N ({bonusDifference}) = %e = (%B + %r + %i)");
        }

        /// <summary>
        /// Gets a hash code from the GUID.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Guid.GetHashCode();

        /// <summary>
        /// Clones the remapping point.
        /// </summary>
        /// <returns></returns>
        public RemappingPoint Clone()
        {
            RemappingPoint clone = new RemappingPoint();
            Array.Copy(m_attributes, clone.m_attributes, 5);
            clone.Status = Status;
            clone.Guid = Guid;
            return clone;
        }

        /// <summary>
        /// Creates a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableRemappingPoint Export() => new SerializableRemappingPoint
        {
            Intelligence = m_attributes[(int)EveAttribute.Intelligence],
            Perception = m_attributes[(int)EveAttribute.Perception],
            Willpower = m_attributes[(int)EveAttribute.Willpower],
            Charisma = m_attributes[(int)EveAttribute.Charisma],
            Memory = m_attributes[(int)EveAttribute.Memory],
            Description = m_description,
            Status = Status
        };
    }
}