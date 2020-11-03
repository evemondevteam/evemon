using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        /// <param name="param">The request parameters. If null, defaults will be used.</param>
        public static DownloadResult<Image> DownloadImage(Uri url, RequestParams param = null)
            => DownloadImageAsync(url, param).Result;

        /// <summary>
        /// Asynchronously downloads an image from the specified url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="param">The request parameters. If null, defaults will be used.</param>
        public static async Task<DownloadResult<Image>> DownloadImageAsync(Uri url,
            RequestParams param = null)
        {
            string urlValidationError;
            if (!IsValidURL(url, out urlValidationError))
                throw new ArgumentException(urlValidationError);
            var request = new HttpClientServiceRequest();
            try
            {
                var response = await request.SendAsync(url, param, ImageAccept).
                    ConfigureAwait(false);
                using (response)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync().
                        ConfigureAwait(false);
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
            var param = new ResponseParams(response);
            if (stream == null)
            {
                error = HttpWebClientServiceException.Exception(requestBaseUrl,
                    new ArgumentNullException(nameof(stream)));
                return new DownloadResult<Image>(null, error, param);
            }
            try
            {
                image = Image.FromStream(Util.ZlibUncompress(stream), true);
            }
            catch (ArgumentException ex)
            {
                error = HttpWebClientServiceException.ImageException(requestBaseUrl, ex);
            }
            return new DownloadResult<Image>(image, error, param);
        }
    }
}
