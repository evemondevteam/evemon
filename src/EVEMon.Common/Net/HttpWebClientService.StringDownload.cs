using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string StringAccept = "text/html;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        /// <returns></returns>
        public static DownloadResult<string> DownloadString(Uri url, HttpMethod method = null, bool acceptEncoded = false,
            string postdata = null, DataCompression dataCompression = DataCompression.None)
            => DownloadStringAsync(url, method, acceptEncoded, postdata, dataCompression).Result;
        
        /// <summary>
        /// Asynchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        /// <param name="token">The ESI token, or null if none is used.</param>
        public static async Task<DownloadResult<string>> DownloadStringAsync(Uri url,
            HttpMethod method = null, bool acceptEncoded = false, string postdata = null,
            DataCompression dataCompression = DataCompression.None, string token = null)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = string.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpClientServiceRequest request = new HttpClientServiceRequest();
            request.AuthToken = token;
            request.AcceptEncoded = acceptEncoded;
            request.DataCompression = dataCompression;
            try
            {
                HttpResponseMessage response = await request.SendAsync(url, method, postData,
                    StringAccept).ConfigureAwait(false);

                using (response)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    return GetString(request.BaseUrl, stream, response);
                }
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadResult<string>(string.Empty, ex);
            }
        }

        /// <summary>
        /// Helper method to return a string from the completed request.
        /// </summary>
        /// <param name="requestBaseUrl">The request base URL.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="response">The response from the server.</param>
        private static DownloadResult<string> GetString(Uri requestBaseUrl, Stream stream,
            HttpResponseMessage response)
        {
            string text = string.Empty;
            HttpWebClientServiceException error = null;
            int responseCode = (int)response.StatusCode;
            DateTime serverTime = response.Headers.ServerTimeUTC();

            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl, new ArgumentNullException(nameof(stream)));
                return new DownloadResult<string>(text, error, responseCode, serverTime);
            }

            try
            {
                using (StreamReader reader = new StreamReader(Util.ZlibUncompress(stream)))
                    text = reader.ReadToEnd();
            }
            catch (ArgumentException ex)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl, ex);
            }

            return new DownloadResult<string>(text, error, responseCode, serverTime);
        }
    }
}
