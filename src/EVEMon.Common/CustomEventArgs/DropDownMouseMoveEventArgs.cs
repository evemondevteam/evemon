using System;
using System.Drawing;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class DropDownMouseMoveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownMouseMoveEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="location">The location.</param>
        public DropDownMouseMoveEventArgs(object item, Point location)
        {
            Item = item;
            Location = location;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public object Item { get; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; }
    }
}
