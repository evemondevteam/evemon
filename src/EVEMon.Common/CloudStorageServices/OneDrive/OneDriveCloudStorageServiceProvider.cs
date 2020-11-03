using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Serialization;
using Microsoft.OneDrive.Sdk;
using Microsoft.OneDrive.Sdk.WindowsForms;
using Image = System.Drawing.Image;

namespace EVEMon.Common.CloudStorageServices.OneDrive
{
    public sealed class OneDriveCloudStorageServiceProvider : CloudStorageServiceProvider
    {
        private SerializableAPIResult<SerializableAPICredentials> m_result =
                new SerializableAPIResult<SerializableAPICredentials>();

        private const string RedirectUri = "https://login.live.com/oauth20_desktop.srf";

        private static readonly string[] s_scopes = { "wl.offline_access", "onedrive.appfolder" };

        private string m_fileId;


        #region Properties

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "OneDrive";

        /// <summary>
        /// Gets the authentication steps.
        /// </summary>
        /// <value>
        /// The authentication steps.
        /// </value>
        public override AuthenticationSteps AuthSteps => AuthenticationSteps.One;

        /// <summary>
        /// Gets a value indicating whether this <see cref="CloudStorageServiceProvider" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => true;

        /// <summary>
        /// Gets a value indicating whether the provider API credentials are stored.
        /// </summary>
        /// <value>
        /// <c>true</c> if the provider API credentials are stored; otherwise, <c>false</c>.
        /// </value>
        public override bool HasCredentialsStored
            => !string.IsNullOrEmpty(OneDriveCloudStorageServiceSettings.Default.RefreshToken);

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected override ApplicationSettingsBase Settings => OneDriveCloudStorageServiceSettings.Default;

        /// <summary>
        /// Gets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public override Image Logo => CloudStorageServiceResources.OneDriveLogo;

        #endregion


        #region Implementation Methods

        /// <summary>
        /// Asynchronously checks that the provider authentication with credentials is valid.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override Task<SerializableAPIResult<SerializableAPICredentials>>
            CheckProviderAuthWithCredentialsIsValidAsync(uint userId, string apiKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously requests the provider an authentication code.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<SerializableAPICredentials>> RequestProviderAuthCodeAsync()
        {
            m_result = new SerializableAPIResult<SerializableAPICredentials>();

            await CheckAuthCodeAsync(string.Empty).ConfigureAwait(false);

            return m_result;
        }

        /// <summary>
        /// Asynchronously checks the provider authentication code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<SerializableAPICredentials>> CheckProviderAuthCodeAsync(
            string code)
        {
            if (m_result == null)
                m_result = new SerializableAPIResult<SerializableAPICredentials>();

            try
            {
                // Checks that the client is authenticated
                // Settings save is done on caller method
                return await CheckAuthAsync().ConfigureAwait(false);
            }
            catch (OneDriveException exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Message };
            }

            return m_result;
        }

        /// <summary>
        /// Asynchronously checks the authentication.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<SerializableAPICredentials>> CheckAuthenticationAsync()
        {
            if (m_result == null)
                m_result = new SerializableAPIResult<SerializableAPICredentials>();

            try
            {
                if (!HasCredentialsStored)
                    return m_result;

                // Checks that the client is authenticated
                // Settings save is done on called method
                return await CheckAuthAsync(saveOnChangeCheck: true).ConfigureAwait(false);
            }
            catch (OneDriveException exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Error.Message };

                if (exc.Error.Code == OneDriveErrorCode.AuthenticationFailure.ToString() && HasCredentialsStored)
                    await ResetSettingsAsync().ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Message };
            }

            return m_result;
        }

        /// <summary>
        /// Asynchronously revokes the authorization.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<SerializableAPICredentials>> RevokeAuthorizationAsync()
        {
            m_result = new SerializableAPIResult<SerializableAPICredentials>();

            try
            {
                using (OneDriveClient client = (OneDriveClient)await GetClient().ConfigureAwait(false))
                {
                    bool cansignout = client.AuthenticationProvider.CurrentAccountSession.CanSignOut;

                    if (cansignout)
                        await client.AuthenticationProvider.SignOutAsync().ConfigureAwait(false);
                    else
                        m_result.Error = new SerializableAPIError { ErrorMessage = "Unable to revoke authorization" };
                }
            }
            catch (OneDriveException exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                m_result.Error = new SerializableAPIError { ErrorMessage = exc.Message };
            }

            return m_result;
        }

        /// <summary>
        /// Asynchronously uploads the file.
        /// </summary>
        /// <returns></returns>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> UploadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                m_fileId = m_fileId ?? await GetFileIdAsync().ConfigureAwait(false);

                byte[] content = Util.GZipCompress(SettingsFileContentByteArray).ToArray();

                using (OneDriveClient client = (OneDriveClient)await GetClient().ConfigureAwait(false))
                using (Stream stream = Util.GetMemoryStream(content))
                {
                    Item response = await (string.IsNullOrWhiteSpace(m_fileId)
                        ? client.Drive.Special.AppRoot
                            .ItemWithPath(Uri.EscapeUriString(SettingsFileNameWithoutExtension))
                        : client.Drive.Items[m_fileId])
                        .Content.Request().PutAsync<Item>(stream).ConfigureAwait(false);

                    m_fileId = response?.Id;
                }
            }
            catch (OneDriveException exc)
            {
                if (exc.Error.Code == OneDriveErrorCode.AuthenticationFailure.ToString())
                    IsAuthenticated = false;

                result.Error = new SerializableAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new SerializableAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        /// <summary>
        /// Asynchronously downloads the file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        protected override async Task<SerializableAPIResult<CloudStorageServiceAPIFile>> DownloadFileAsync()
        {
            SerializableAPIResult<CloudStorageServiceAPIFile> result = new SerializableAPIResult<CloudStorageServiceAPIFile>();

            try
            {
                m_fileId = m_fileId ?? await GetFileIdAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(m_fileId))
                    throw new FileNotFoundException();

                using (OneDriveClient client = (OneDriveClient)await GetClient().ConfigureAwait(false))
                {
                    Stream stream = await client.Drive.Items[m_fileId].Content.Request().GetAsync().ConfigureAwait(false);
                    return await GetMappedAPIFileAsync(result, stream);
                }
            }
            catch (OneDriveException exc)
            {
                if (exc.Error.Code == OneDriveErrorCode.AuthenticationFailure.ToString())
                    IsAuthenticated = false;

                result.Error = new SerializableAPIError { ErrorMessage = exc.Error.Message };
            }
            catch (Exception exc)
            {
                result.Error = new SerializableAPIError { ErrorMessage = exc.Message };
            }

            return result;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Asynchronously checks the authentication.
        /// </summary>
        /// <param name="saveOnChangeCheck">if set to <c>true</c> save the settings on change check.</param>
        /// <returns></returns>
        private async Task<SerializableAPIResult<SerializableAPICredentials>> CheckAuthAsync(bool saveOnChangeCheck = false)
        {
            SerializableAPIResult<SerializableAPICredentials> result =
                new SerializableAPIResult<SerializableAPICredentials>();

            using (OneDriveClient client = (OneDriveClient)await GetClient())
            {
                if (!client.IsAuthenticated)
                {
                    result.Error = new SerializableAPIError { ErrorMessage = "The client could not be authenticated" };
                    return result;
                }

                AuthenticationProvider authenticationProvider =
                    (MicrosoftAccountAuthenticationProvider)client.AuthenticationProvider;

                if (authenticationProvider.CurrentAccountSession.RefreshToken ==
                    OneDriveCloudStorageServiceSettings.Default.RefreshToken)
                {
                    return result;
                }

                OneDriveCloudStorageServiceSettings.Default.Credentials =
                    Encoding.Default.GetString(authenticationProvider.ServiceInfo.CredentialCache.GetCacheBlob());
                OneDriveCloudStorageServiceSettings.Default.UserId = authenticationProvider.CurrentAccountSession.UserId;
                OneDriveCloudStorageServiceSettings.Default.RefreshToken =
                    authenticationProvider.CurrentAccountSession.RefreshToken;

                if (saveOnChangeCheck)
                    Settings.Save();
            }

            return result;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private static async Task<IOneDriveClient> GetClient()
        {
            CredentialCache credentialCache = new CredentialCache();
            string credentials = OneDriveCloudStorageServiceSettings.Default.Credentials;

            if (!string.IsNullOrWhiteSpace(credentials))
                credentialCache.InitializeCacheFromBlob(Encoding.Default.GetBytes(credentials));

            IServiceInfoProvider serviceInfoProvider = new ServiceInfoProvider(new FormsWebAuthenticationUi())
            {
                UserSignInName = OneDriveCloudStorageServiceSettings.Default.UserId
            };

            return await OneDriveClient.GetAuthenticatedMicrosoftAccountClient(
                Util.Decrypt(OneDriveCloudStorageServiceSettings.Default.AppKey, CultureConstants.InvariantCulture.NativeName),
                RedirectUri, s_scopes, serviceInfoProvider, credentialCache).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the file identifier.
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetFileIdAsync()
        {
            try
            {
                using (OneDriveClient client = (OneDriveClient)await GetClient().ConfigureAwait(false))
                {
                    IItemRequest request = client.Drive.Special.AppRoot
                        .ItemWithPath($"/{Uri.EscapeUriString(SettingsFileNameWithoutExtension)}")
                        .Request();
                    Item response = await request.GetAsync().ConfigureAwait(false);
                    return response.Id;
                }
            }
            catch (OneDriveException exc)
            {
                if (exc.IsMatch(OneDriveErrorCode.ItemNotFound.ToString()))
                    return string.Empty;

                throw;
            }
        }

        #endregion

    }
}
