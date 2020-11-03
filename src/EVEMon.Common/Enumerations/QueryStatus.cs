using System.ComponentModel;

namespace EVEMon.Common.Enumerations
{
    public enum QueryStatus
    {
        /// <summary>
        /// The query will be updated on due time.
        /// </summary>
        [Description("Pending...")]
        Pending,

        /// <summary>
        /// The query is being updated.
        /// </summary>
        [Description("Updating...")]
        Updating,

        /// <summary>
        /// The query is disabled.
        /// </summary>
        [Description("Disabled.")]
        Disabled,

        /// <summary>
        /// There is no network connection.
        /// </summary>
        [Description("No TCP/IP connection.")]
        NoNetwork,

        /// <summary>
        /// The character is not on any API key.
        /// </summary>
        [Description("No associated API key.")]
        NoESIKey,

        /// <summary>
        /// The API key has no access to query the call.
        /// </summary>
        [Description("No access via the API key.")]
        NoAccess
    }
}