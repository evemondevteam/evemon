using System.Collections.Generic;
using System.Net.Http.Headers;

namespace EVEMon.Common.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Retrieves an integer header value.
        /// </summary>
        /// <param name="headers">The headers to use.</param>
        /// <param name="name">The header name to query.</param>
        /// <returns>The value of that header as an integer, or null if the header is missing
        /// or in a non-integer format.</returns>
        private static int? GetIntParam(HttpResponseHeaders headers, string name)
        {
            IEnumerable<string> values;
            int? ret = null;
            // If values are available, try to parse as integer, use the last one
            if (headers.TryGetValues(name, out values))
                foreach (string value in values)
                {
                    int intVal;
                    if (value.Trim().TryParseInv(out intVal) && intVal >= 0)
                        ret = intVal;
                }
            return ret;
        }

        /// <summary>
        /// Retrieves the number of ESI errors remaining, or null if this header is not
        /// included.
        /// </summary>
        /// <param name="headers">The response headers.</param>
        public static int? ErrorCount(this HttpResponseHeaders headers)
        {
            return GetIntParam(headers, "X-Esi-Error-Limit-Remain");
        }

        /// <summary>
        /// Retrieves the number of ESI pages in the response, or 0 if this header is not
        /// included.
        /// </summary>
        /// <param name="headers">The response headers.</param>
        public static int PageCount(this HttpResponseHeaders headers)
        {
            return GetIntParam(headers, "X-Pages") ?? 0;
        }
    }
}
