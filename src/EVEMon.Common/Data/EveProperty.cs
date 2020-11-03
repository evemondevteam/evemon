using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Datafiles;
using System;

namespace EVEMon.Common.Data
{
    public sealed class EveProperty
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="serial"></param>
        internal EveProperty(EvePropertyCategory category, SerializableProperty serial)
        {
            ID = serial.ID;
            Name = serial.Name;
            Description = serial.Description;
            DefaultValue = serial.DefaultValue;
            Icon = serial.Icon;
            Unit = serial.Unit;
            UnitID = serial.UnitID;
            HigherIsBetter = serial.HigherIsBetter;
            Category = category;

            switch (serial.ID)
            {
                case DBConstants.CPUNeedPropertyID:
                    CPU = this;
                    break;
                case DBConstants.PGNeedPropertyID:
                    Powergrid = this;
                    break;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the property's ID.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the property name (the displayName in the CCP tables).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the property's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the string representation of the default value.
        /// </summary>
        public string DefaultValue { get; }

        /// <summary>
        /// Gets the icon for this property.
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Gets the unit for this property.
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// Gets the unitID for this property.
        /// </summary>
        public long UnitID { get; }

        /// <summary>
        /// When true, the higher the value, the better it is.
        /// </summary>
        public bool HigherIsBetter { get; }

        /// <summary>
        /// Gets the property's category.
        /// </summary>
        public EvePropertyCategory Category { get; }

        /// <summary>
        /// When true, the property is always visible in the ships browsers, even when an object has no value for this property.
        /// </summary>
        public bool AlwaysVisibleForShips { get; internal set; }

        /// <summary>
        /// When true, the property is hidden in the ships/items browsers if the value is the same then the default value.
        /// </summary>
        public bool HideIfDefault { get; internal set; }

        /// <summary>
        /// Gets the CPU Output property.
        /// </summary>
        public static EveProperty CPU { get; private set; }

        /// <summary>
        /// Gets the Powergrid Output property.
        /// </summary>
        public static EveProperty Powergrid { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the label of this property's value for the given item or, when the property is absent, the default value.
        /// The returned string will include the unit.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public string GetLabelOrDefault(Item obj)
        {
            obj.ThrowIfNull(nameof(obj));

            EvePropertyValue? value = obj.Properties[ID];
            return Format(value?.Value ?? DefaultValue);
        }

        /// <summary>
        /// Gets the numeric interpretation of the object's property value, or NaN if the property value is not a numeric. 
        /// When the given object has no value for this property, we use the default one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public double GetNumericValue(Item obj)
        {
            obj.ThrowIfNull(nameof(obj));

            // Retrieve the string for the number
            EvePropertyValue? value = obj.Properties[ID];
            string number = value?.Value ?? DefaultValue;

            // Try to parse it as a real
            float result;
            number.TryParseInv(out result);
            // result is set to default(float) if it fails anyways
            return result;
        }

        /// <summary>
        /// Format a property value as shown in EVE.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Format(string value)
        {
            float numericValue;
            if (!value.TryParseInv(out numericValue))
                return value;

            try
            {
                // Format a value of Packaged Volume
                if (Name == DBConstants.PackagedVolumePropertyName)
                    return $"{numericValue:N1} {Unit}";

                // Format a value of Structure Volume
                if (ID == DBConstants.VolumePropertyID)
                {
                    return numericValue <= 1000
                        ? $"{numericValue:N2} {Unit}"
                        : $"{numericValue:#,##0.0##} {Unit}";
                }

                // Format a value of Capacitor Capacity
                if (ID == DBConstants.CapacitorCapacityPropertyID)
                    return $"{Math.Floor(numericValue):N0} {Unit}";

                // Format a value of Ships Warp Speed
                if (ID == DBConstants.ShipWarpSpeedPropertyID)
                    return $"{numericValue:N2} {Unit}";

                switch (UnitID)
                {
                        // Format a value of Mass
                    case DBConstants.MassUnitID:
                        return numericValue <= 1000
                            ? $"{numericValue:#,##0.0#} {Unit}"
                            : $"{numericValue:N0} {Unit}";

                        // Format a value of Millseconds
                    case DBConstants.MillsecondsUnitID:
                        return $"{numericValue / 1000:N2} {Unit}";

                        // Format a value of Absolute Percentage
                    case DBConstants.AbsolutePercentUnitID:
                        return $"{numericValue * 100:0.###} {Unit}";

                        // Format a value of Inverse Absolute Percentage
                    case DBConstants.InverseAbsolutePercentUnitID:
                        return $"{(1 - numericValue) * 100:0.###} {Unit}";

                        // Format a value of Modifier Percentage
                    case DBConstants.ModifierPercentUnitID:
                        return $"{(numericValue - 1) * 100:0.###} {Unit}";

                        // Format a value of Inverse Modifier Percentage
                    case DBConstants.InversedModifierPercentUnitID:
                        return $"{(1 - numericValue) * 100:0.###} {Unit}";

                        // A reference to a group (groupID), it has been pre-transformed on XmlGenerator
                    case DBConstants.GroupIDUnitID:
                        return value;

                        // A reference to an item or a skill (typeID)
                    case DBConstants.TypeUnitID:
                        int id;
                        return (!value.TryParseInv(out id) || id == 0) ? string.Empty :
                            StaticItems.GetItemName(id);

                        // Format a Sizeclass ("1=small 2=medium 3=l")
                    case DBConstants.SizeclassUnitID:
                        int size;
                        value.TryParseInv(out size);
                        switch (size)
                        {
                            case 1:
                                return "Small";
                            case 2:
                                return "Medium";
                            case 3:
                                return "Large";
                            case 4:
                                return "Extra Large";
                            default:
                                return EveMonConstants.UnknownText;
                        }

                        // Format all other values (use of thousand and decimal separator)
                    default:
                        return $"{numericValue:#,##0.###} {Unit}";
                }
            }
            catch (FormatException)
            {
                return "N/A";
            }
        }

        #endregion
    }
}
