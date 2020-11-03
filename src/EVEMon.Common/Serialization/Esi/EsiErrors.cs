using System;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Avoid saturating ESI with failed requests by refusing new requests if the error count
    /// approaches the limit.
    /// </summary>
    public static class EsiErrors
    {
        // How many errors remaining to allow as a "buffer" before refusing requests
        private const int ERROR_THRESHOLD = 10;

        // The number of errors remaining until throttling kicks in
        private static int s_errorCount;
        // Locks synchronous access to the error count / time
        private static object s_errorLock = new object();
        // When the error count resets
        private static DateTime s_errorReset = DateTime.MinValue;

        /// <summary>
        /// Returns the time when the error count will reset; delayed requests can be
        /// rescheduled on or after this time.
        /// </summary>
        public static DateTime ErrorCountResetTime
        {
            get
            {
                // No need to lock as it reads once and does not write
                DateTime when = DateTime.UtcNow, reset = s_errorReset;
                if (reset > when)
                    when = reset;
                return when;
            }
        }

        /// <summary>
        /// Returns true if no ESI requests should be issued due to error count problems.
        /// </summary>
        public static bool IsErrorCountExceeded
        {
            get
            {
                bool error;
                lock (s_errorLock)
                {
                    error = s_errorCount <= ERROR_THRESHOLD && DateTime.UtcNow < s_errorReset;
                }
                return error;
            }
        }

        /// <summary>
        /// Updates the error count when it is reported by ESI.
        /// </summary>
        /// <param name="errorCount">The number of errors remaining until throttling.</param>
        /// <param name="errorReset">The time when the error count resets.</param>
        /// <returns>true if throttling is in effect, or false otherwise</returns>
        public static bool UpdateErrors(int errorCount, DateTime errorReset)
        {
            if (errorCount <= ERROR_THRESHOLD)
            {
                var maxErrorReset = DateTime.UtcNow.AddMinutes(5.0);
                // Error reset will occur after at most 5 minutes
                if (errorReset > maxErrorReset)
                    errorReset = maxErrorReset;
                lock (s_errorLock)
                {
                    s_errorCount = Math.Min(errorCount, s_errorCount);
                    if (s_errorReset < errorReset)
                        s_errorReset = errorReset;
                }
            }
            return errorCount > ERROR_THRESHOLD;
        }
    }
}
