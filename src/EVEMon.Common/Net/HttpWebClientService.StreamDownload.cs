using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string StreamAccept = "application/json;q=0.9,text/plain;q=0.8,*/*;q=0.5";

        /// <summary>
        /// Passed to DownloadStreamAsync to process the stream into actual data.
        /// </summary>
        /// <typeparam name="T">The type to decode.</typeparam>
        /// <param name="stream">The input stream to read.</param>
        /// <param name="response">The HTTP response returned by the server.</param>
        /// <returns>The decoded value, or default(T) if none could be parsed.</returns>
        public delegate T ParseDataDelegate<T>(Stream stream, ResponseParams response);

        /// <summary>
        /// Asynchronously downloads an object (streaming) from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parser">The function which will parse the stream.</param>
        /// <param name="param">The request parameters. If null, defaults will be used.</param>
        public static async Task<DownloadResult<T>> DownloadStreamAsync<T>(Uri url,
            ParseDataDelegate<T> parser, RequestParams param)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);
            var request = new HttpClientServiceRequest();
            try
            {
                var response = await request.SendAsync(url, param, StreamAccept).
                    ConfigureAwait(false);
                using (response)
                {
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(
                        false);
                    return GetResult(url, stream, parser, response);
                }
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadResult<T>(default(T), ex);
            }
        }

        /// <summary>
        /// Helper method to return an object from the completed request stream.
        /// </summary>
        /// <param name="requestBaseUrl">The request base URL.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="parser">The function which will parse the stream.</param>
        /// <param name="response">The response from the server.</param>
        /// <returns>The parsed object.</returns>
        private static DownloadResult<T> GetResult<T>(Uri requestBaseUrl, Stream stream,
            ParseDataDelegate<T> parser, HttpResponseMessage response)
        {
            T result = default(T);
            HttpWebClientServiceException error = null;
            var param = new ResponseParams(response);
            if (stream == null)
                // No stream (can this happen)?
                error = HttpWebClientServiceException.Exception(requestBaseUrl,
                    new ArgumentNullException(nameof(stream)));
            else
                // Attempt to invoke parser
                result = parser.Invoke(Util.ZlibUncompress(stream), param);
            return new DownloadResult<T>(result, error, param);
        }
    }
}
