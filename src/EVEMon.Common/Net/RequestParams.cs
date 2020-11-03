using EVEMon.Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Represents the parameters used for a HTTP request.
    /// </summary>
    public sealed class RequestParams
    {
        /// <summary>
        /// The content type used if none is specified. Defaults to
        /// "application/x-www-form-urlencoded".
        /// </summary>
        public const string DEFAULT_CONTENT_TYPE = "application/x-www-form-urlencoded";

        /// <summary>
        /// Whether encoded responses are accepted. If true, the response may need to be
        /// unzipped/inflated.
        /// </summary>
        public bool AcceptEncoded { get; set; }

        /// <summary>
        /// The authentication information to send. If it does not have a type, it will be
        /// sent as Bearer authentication. If null, no header will be sent.
        /// </summary>
        public string Authentication { get; set; }

        /// <summary>
        /// Retrieves the header to be sent for the Authorization header based on the token,
        /// or null if none should be sent.
        /// </summary>
        internal AuthenticationHeaderValue AuthHeader
        {
            get
            {
                string token = Authentication;
                AuthenticationHeaderValue header = null;
                if (token != null)
                {
                    // If the token has a space, use that type of auth header
                    string type = "Bearer";
                    int index = token.IndexOf(' ');
                    if (index > 0)
                    {
                        type = token.Substring(0, index);
                        token = token.Substring(index + 1).TrimStart();
                    }
                    header = new AuthenticationHeaderValue(type, token);
                }
                return header;
            }
        }

        /// <summary>
        /// The data compression type to permit, if any.
        /// </summary>
        public DataCompression Compression { get; set; }

        /// <summary>
        /// The GET/POST data to send. This needs to be URL encoded.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The content type of the POST data. If unset, defaults to DEFAULT_CONTENT_TYPE.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The E-Tag to send to the server. If it matches the content on the server, the
        /// response will be "No Content", saving bandwidth. If it is null, no e-tag will be
        /// sent.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// The date/time to report to the server to potentially save on returning new data if
        /// the data has not been modified since then.
        /// </summary>
        public DateTimeOffset? IfModifiedSince { get; set; }

        /// <summary>
        /// The HTTP method, defaulting to GET.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// The HTTP method to be used based on the requested method and content. If there is
        /// no content, GET will be used rather than sending a pointless empty POST request.
        /// </summary>
        internal HttpMethod MethodChecked
        {
            get
            {
                return (Content == null || Method == null) ? HttpMethod.Get : Method;
            }
        }

        /// <summary>
        /// Creates a default set of request parameters.
        /// </summary>
        public RequestParams()
        {
            AcceptEncoded = false;
            Authentication = null;
            Compression = DataCompression.None;
            Content = null;
            ContentType = DEFAULT_CONTENT_TYPE;
            ETag = null;
            IfModifiedSince = null;
            Method = HttpMethod.Get;
        }

        /// <summary>
        /// Creates a request parameter with POST data.
        /// </summary>
        /// <param name="content">The POST data to include, URL encoded.</param>
        public RequestParams(string content) : this()
        {
            if (!string.IsNullOrEmpty(content))
            {
                Content = content;
                Method = HttpMethod.Post;
            }
        }

        /// <summary>
        /// Creates a request parameter based on the data from the last request.
        /// </summary>
        /// <param name="response">The previous response.</param>
        /// <param name="content">The optional POST data to include, URL encoded.</param>
        public RequestParams(ResponseParams response, string content = null) : this(content)
        {
            if (response != null)
            {
                ETag = response.ETag;
                IfModifiedSince = response.Expires;
            }
        }

        /// <summary>
        /// Retrieves the encoded content of this request.
        /// </summary>
        /// <returns>The GET/POST data encoded and compressed as necessary.</returns>
        internal IEnumerable<byte> GetEncodedContent()
        {
            IEnumerable<byte> content = null;
            if (Content != null)
            {
                // Only if there is content to encode
                byte[] encoded = Encoding.UTF8.GetBytes(Content);
                switch (Compression)
                {
                case DataCompression.Gzip:
                    content = Util.GZipCompress(encoded);
                    break;
                case DataCompression.Deflate:
                    content = Util.DeflateCompress(encoded);
                    break;
                case DataCompression.None:
                    content = encoded;
                    break;
                default:
                    throw new NotImplementedException("Encoding type " + Compression);
                }
            }
            return content;
        }

        public override string ToString()
        {
            return string.Format("RequestParams[encoded={0},content-type={1},method={2}]",
                AcceptEncoded, ContentType, Method);
        }
    }
}
