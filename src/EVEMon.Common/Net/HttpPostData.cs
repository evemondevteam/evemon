using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// Container class for data to be submitted to a url as a POST request.
    /// </summary>
    public sealed class HttpPostData
    {
        /// <summary>
        /// The content type used if none is specified. Defaults to
        /// "application/x-www-form-urlencoded".
        /// </summary>
        public const string DEFAULT_CONTENT_TYPE = "application/x-www-form-urlencoded";

        private readonly string m_data;
        private readonly DataCompression m_compression;
        private readonly IEnumerable<byte> m_content;
        private readonly string m_contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostData"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dataCompression">The compression.</param>
        public HttpPostData(string data, DataCompression dataCompression = DataCompression.None,
            string contentType = DEFAULT_CONTENT_TYPE)
        {
            m_data = data;

            byte[] encoded = Encoding.UTF8.GetBytes(data);
            switch (dataCompression)
            {
                case DataCompression.Gzip:
                    m_content = Util.GZipCompress(encoded);
                    break;
                case DataCompression.Deflate:
                    m_content = Util.DeflateCompress(encoded);
                    break;
                case DataCompression.None:
                    m_content = encoded;
                    break;
                default:
                    throw new NotImplementedException();
            }

            m_compression = dataCompression;
            m_contentType = contentType ?? DEFAULT_CONTENT_TYPE;
        }

        /// <summary>
        /// Gets the content's bytes.
        /// </summary>
        public IEnumerable<byte> Content => m_content;

        /// <summary>
        /// Gets the MIME content type.
        /// </summary>
        public string ContentType => m_contentType;

        /// <summary>
        /// Gets the compression tpye used.
        /// </summary>
        public DataCompression Compression => m_compression;

        /// <summary>
        /// Gets the number of bytes of the content.
        /// </summary>
        public int Length => Content.Count();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the post data.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the post data.
        /// </returns>
        public override string ToString() => m_data;
    }
}
