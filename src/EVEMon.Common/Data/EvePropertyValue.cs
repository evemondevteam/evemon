using EVEMon.Common.Constants;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Describes a property of a ship/item (e.g. CPU size)
    /// </summary>
    public struct EvePropertyValue
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal EvePropertyValue(SerializablePropertyValue src)
            : this()
        {
            Property = StaticProperties.GetPropertyByID(src.ID);
            Value = src.Value;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the property.
        /// </summary>
        public EveProperty Property { get; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        public long Int64Value => long.Parse(Value, CultureConstants.InvariantCulture);

        /// <summary>
        /// Gets the floating point value.
        /// </summary>
        public double DoubleValue => double.Parse(Value, CultureConstants.InvariantCulture);

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this prerequisite.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Property.Name;

        #endregion
    }
}
