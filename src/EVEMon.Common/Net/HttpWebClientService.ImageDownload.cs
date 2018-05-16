using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Net
{
    static partial class HttpWebClientService
    {
        private const string ImageAccept = "image/*,*/*;q=0.5";

        /// <summary>
        /// Synchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static DownloadResult<Image> DownloadImage(Uri url, HttpMethod method = null, bool acceptEncoded = false, string postdata = null,
            DataCompression dataCompression = DataCompression.None)
            => DownloadImageAsync(url, method, acceptEncoded, postdata, dataCompression).Result;

        /// <summary>
        /// Asynchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="acceptEncoded">if set to <c>true</c> accept encoded response.</param>
        /// <param name="postdata">The post data.</param>
        /// <param name="dataCompression">The post data compression method.</param>
        public static async Task<DownloadResult<Image>> DownloadImageAsync(Uri url, HttpMethod method = null,
            bool acceptEncoded = false, string postdata = null, DataCompression dataCompression = DataCompression.None)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);

            HttpPostData postData = string.IsNullOrWhiteSpace(postdata) ? null : new HttpPostData(postdata, dataCompression);
            HttpClientServiceRequest request = new HttpClientServiceRequest();
            request.AuthToken = null;
            request.AcceptEncoded = acceptEncoded;
            request.DataCompression = dataCompression;
            try
            {
                HttpResponseMessage response = await request.SendAsync(url, method, postData,
                    ImageAccept).ConfigureAwait(false);

                using (response)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    return GetImage(request.BaseUrl, stream, response);
                }
            }
            catch (HttpWebClientServiceException ex)
            {
                return new DownloadResult<Image>(null, ex);
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="requestBaseUrl">The request base URL.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="response">The response from the server.</param>
        /// <returns></returns>
        private static DownloadResult<Image> GetImage(Uri requestBaseUrl, Stream stream,
            HttpResponseMessage response)
        {
            Image image = null;
            HttpWebClientServiceException error = null;
            int responseCode = (int)response.StatusCode;
            DateTime serverTime = response.Headers.ServerTimeUTC();

            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl, new ArgumentNullException(nameof(stream)));
                return new DownloadResult<Image>(null, error, responseCode, serverTime);
            }

            try
            {
                image = Image.FromStream(Util.ZlibUncompress(stream), true);
            }
            catch (ArgumentException ex)
            {
                error = HttpWebClientServiceException.ImageException(requestBaseUrl, ex);
            }

            return new DownloadResult<Image>(image, error, responseCode, serverTime);
        }
    }
}
