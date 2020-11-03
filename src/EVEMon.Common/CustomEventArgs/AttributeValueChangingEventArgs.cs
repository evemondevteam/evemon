using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class AttributeValueChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueChangingEventArgs"/> class.
        /// </summary>
        /// <param name="deltaValue">The delta value.</param>
        public AttributeValueChangingEventArgs(long deltaValue)
        {
            Value = deltaValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public long Value { get; }
    }
}
