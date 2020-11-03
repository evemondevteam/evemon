using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        /// <param name="param">The request parameters. If null, defaults will be used.</param>
        /// <returns></returns>
        public static DownloadResult<string> DownloadString(Uri url, RequestParams param = null)
            => DownloadStringAsync(url, param).Result;

        /// <summary>
        /// Asynchronously downloads a string from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="param">The request parameters. If null, defaults will be used.</param>
        public static async Task<DownloadResult<string>> DownloadStringAsync(Uri url,
            RequestParams param = null)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);
            var request = new HttpClientServiceRequest();
            try
            {
                var response = await request.SendAsync(url, param, StringAccept).
                    ConfigureAwait(false);
                using (response)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(
                        false);
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
            var param = new ResponseParams(response);
            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl, new
                    ArgumentNullException(nameof(stream)));
                return new DownloadResult<string>(text, error, param);
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
            return new DownloadResult<string>(text, error, param);
        }
    }
}
