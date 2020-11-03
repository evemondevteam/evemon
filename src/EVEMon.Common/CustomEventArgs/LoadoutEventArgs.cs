using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class LoadoutEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadoutEventArgs"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public LoadoutEventArgs(object loadout, string errorMessage)
        {
            HasError = !string.IsNullOrEmpty(errorMessage);
            Error = !string.IsNullOrEmpty(errorMessage) ? new Exception(errorMessage) : null;
            Loadout = loadout;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; }

        /// <summary>
        /// Gets the loadout.
        /// </summary>
        /// <value>
        /// The feed.
        /// </value>
        public object Loadout { get; }
    }
}
