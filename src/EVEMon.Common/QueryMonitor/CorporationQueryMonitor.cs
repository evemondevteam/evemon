using System;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.QueryMonitor
{
    public sealed class CorporationQueryMonitor<T> : QueryMonitor<T> where T : class
    {
        private readonly Character m_character;
        private ESIKey m_esiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="method"></param>
        /// <param name="onUpdated"></param>
        internal CorporationQueryMonitor(CCPCharacter character, Enum method, Action<T>
            onSuccess, NotifyErrorCallback onFailure) : base(method, (result) =>
            {
                if (character.Monitored)
                {
                    // "No corp role(s)" = 403
                    if (result.HasError)
                    {
                        int rolesError = result.ErrorMessage?.IndexOf("role",
                            StringComparison.InvariantCultureIgnoreCase) ?? -1;
                        // Do not invoke onFailure on corp roles error since we cannot actually
                        // determine whether the key had the roles until we try
                        if ((result.ErrorCode != 403 || rolesError <= 0) && character.
                                ShouldNotifyError(result, method))
                            onFailure.Invoke(character, result);
                    }
                    else
                        onSuccess.Invoke(result.Result);
                }
            })
        {
            m_character = character;
        }

        /// <summary>
        /// Returns true if the required API key information is known.
        /// </summary>
        /// <returns>False if an API key was required and not found.</returns>
        protected override bool HasAPIKey => m_character.Identity.ESIKeys.Any();

        /// <summary>
        /// Gets a value indicating whether this monitor has access to data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this monitor has access; otherwise, <c>false</c>.
        /// </value>
        public override bool HasAccess
        {
            get
            {
                m_esiKey = m_character.Identity.FindAPIKeyWithAccess((ESIAPICorporationMethods)Method);
                return m_esiKey != null;
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        protected override void QueryAsyncCore(APIProvider provider, APIProvider.
            ESIRequestCallback<T> callback)
        {
            provider.ThrowIfNull(nameof(provider));

            provider.QueryEsiAsync(Method, m_esiKey.AccessToken, m_character.CorporationID,
                callback);
        }
    }
}
