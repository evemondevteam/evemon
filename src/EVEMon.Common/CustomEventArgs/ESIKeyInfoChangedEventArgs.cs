using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class ESIKeyInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="esiKey"></param>
        public ESIKeyInfoChangedEventArgs(ESIKey esiKey)
        {
            ESIKey = esiKey;
        }

        /// <summary>
        /// Gets the ESI key related to this event.
        /// </summary>
        public ESIKey ESIKey { get; }
    }
}
