using EVEMon.Common.Extensions;
using System;
using System.Net;
using System.Net.Http;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Groups together response parameters from HTTP requests. Included are the status code,
    /// ETag (if present), expiration date (if present), and encoding.
    /// </summary>
    public sealed class ResponseParams
    {
        /// <summary>
        /// The error count reported from ESI to avoid running into backoff. This represents
        /// the number of errors remaining in the current period.
        /// </summary>
        public int? ErrorCount { get; set; }

        /// <summary>
        /// The E-Tag received from the server. Null if no e-tag was sent.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// The time when this data expires. Null if no expiry was sent.
        /// </summary>
        public DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// Reports true if the response indicates that data was not modified. Reports false
        /// for all other status codes.
        /// </summary>
        public bool IsNotModifiedResponse
        {
            get
            {
                return ResponseCode == (int)HttpStatusCode.NotModified;
            }
        }

        /// <summary>
        /// Reports true if the response indicates that it was successful (HTTP response code).
        /// Reports false for all other status codes.
        /// </summary>
        public bool IsOKResponse
        {
            get
            {
                return ResponseCode == (int)HttpStatusCode.OK;
            }
        }

        /// <summary>
        /// The maximum number of pages required. Will be 0 if no paging is required, 1 if
        /// there is only one page.
        /// </summary>
        public int Pages { get; }

        /// <summary>
        /// The response code from the server.
        /// </summary>
        public int ResponseCode { get; }

        /// <summary>
        /// The date and time reported by the server in UTC.
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// Creates a new set of response parameters.
        /// </summary>
        /// <param name="code">The response returned by the server</param>
        public ResponseParams(HttpResponseMessage response) : this((int)response.StatusCode)
        {
            // Fill in header data
            var headers = response.Headers;
            ErrorCount = headers.ErrorCount();
            Pages = headers.PageCount();
            // ETag has quotes on it, keep them to reuse on output tag
            ETag = headers.ETag?.Tag;
            Expires = response.Content?.Headers?.Expires;
            Time = headers.Date?.UtcDateTime ?? DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new set of response parameters.
        /// </summary>
        /// <param name="code">The status code returned by the server</param>
        public ResponseParams(int responseCode)
        {
            ErrorCount = null;
            ETag = null;
            Expires = null;
            ResponseCode = responseCode;
            Time = null;
        }

        public override string ToString()
        {
            return string.Format("ResponseParams[code={0:D},expires={1}]", ResponseCode,
                Expires);
        }
    }
}
