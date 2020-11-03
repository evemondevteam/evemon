using System;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents an implant.
    /// </summary>
    public sealed class Implant : Item
    {

        #region Constructors

        /// <summary>
        /// Internal constructor for the default.
        /// </summary>
        internal Implant(ImplantSlots slot)
            : base(-1, ImplantSlots.None.ToString())
        {
            Slot = slot;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="src"></param>
        internal Implant(MarketGroup group, SerializableItem src)
            : base(group, src)
        {
            // Gets the slot
            EvePropertyValue? slotProperty = Properties[DBConstants.ImplantSlotPropertyID];

            if (slotProperty == null)
                return;

            Slot = (ImplantSlots)(slotProperty.Value.Int64Value - 1);

            // Sets the implant bonus
            SetImplantBonus();

            // Adds itself to the implants slot
            StaticItems.GetImplants(Slot).Add(this);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the slot.
        /// </summary>
        public ImplantSlots Slot { get; }

        /// <summary>
        /// For attributes implants, gets the amount of bonus points it grants.
        /// </summary>
        public long Bonus { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Sets the implant bonus.
        /// </summary>
        private void SetImplantBonus()
        {
            EvePropertyValue? eveProperty;

            // Get the bonus
            switch (Slot)
            {
                case ImplantSlots.Charisma:
                    eveProperty = Properties[DBConstants.CharismaModifierPropertyID];
                    if (eveProperty != null)
                        Bonus = eveProperty.Value.Int64Value;
                    break;
                case ImplantSlots.Intelligence:
                    eveProperty = Properties[DBConstants.IntelligenceModifierPropertyID];
                    if (eveProperty != null)
                        Bonus = eveProperty.Value.Int64Value;
                    break;
                case ImplantSlots.Memory:
                    eveProperty = Properties[DBConstants.MemoryModifierPropertyID];
                    if (eveProperty != null)
                        Bonus = eveProperty.Value.Int64Value;
                    break;
                case ImplantSlots.Perception:
                    eveProperty = Properties[DBConstants.PerceptionModifierPropertyID];
                    if (eveProperty != null)
                        Bonus = eveProperty.Value.Int64Value;
                    break;
                case ImplantSlots.Willpower:
                    eveProperty = Properties[DBConstants.WillpowerModifierPropertyID];
                    if (eveProperty != null)
                        Bonus = eveProperty.Value.Int64Value;
                    break;
                default:
                    Bonus = 0;
                    break;
            }
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Converts the provided slot to an attribute. Returns <see cref="EveAttribute.None"/> when the provided slot does not match any attribute.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static EveAttribute SlotToAttrib(ImplantSlots slot)
        {
            switch (slot)
            {
                case ImplantSlots.Perception:
                    return EveAttribute.Perception;
                case ImplantSlots.Memory:
                    return EveAttribute.Memory;
                case ImplantSlots.Willpower:
                    return EveAttribute.Willpower;
                case ImplantSlots.Intelligence:
                    return EveAttribute.Intelligence;
                case ImplantSlots.Charisma:
                    return EveAttribute.Charisma;
                default:
                    return EveAttribute.None;
            }
        }

        /// <summary>
        /// Converts the provided slot to an attribute. Returns <see cref="ImplantSlots.None"/> when the provided slot does not match any attribute.
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static ImplantSlots AttribToSlot(EveAttribute attr)
        {
            switch (attr)
            {
                case EveAttribute.Perception:
                    return ImplantSlots.Perception;
                case EveAttribute.Memory:
                    return ImplantSlots.Memory;
                case EveAttribute.Willpower:
                    return ImplantSlots.Willpower;
                case EveAttribute.Intelligence:
                    return ImplantSlots.Intelligence;
                case EveAttribute.Charisma:
                    return ImplantSlots.Charisma;
                default:
                    return ImplantSlots.None;
            }
        }

        #endregion
    }
}