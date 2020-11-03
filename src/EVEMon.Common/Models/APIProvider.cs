using EVEMon.Common.Attributes;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Xsl;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Serializable class abstracting an API queries provider and its configuration.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class APIProvider
    {
        // Called when an ESI request completes
        public delegate void ESIRequestCallback<T>(EsiResult<T> result, object state);

        private static APIProvider s_ccpProvider;
        private static APIProvider s_ccpTestProvider;
        private static XslCompiledTransform s_rowsetsTransform;

        private readonly List<ESIMethod> m_methods;
        private bool m_supportsCompressedResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal APIProvider()
        {
            m_methods = new List<ESIMethod>(ESIMethod.CreateDefaultSet());
            Url = new Uri("http://your-custom-API-provider.com");
            Name = "your provider's name";
        }


        #region Properties

        /// <summary>
        /// Returns the name of this APIConfiguration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the server host for this APIConfiguration.
        /// </summary>
        public Uri Url { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the provider supports compressed responses.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the provider supports compressed responses; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsCompressedResponse
        {
            get { return IsDefault || this == TestProvider || m_supportsCompressedResponse; }
            set { m_supportsCompressedResponse = value; }
        }

        /// <summary>
        /// Returns true if this APIConfiguration represents the default API service.
        /// </summary>
        public bool IsDefault => this == DefaultProvider;

        /// <summary>
        /// Gets the default API provider
        /// </summary>
        public static APIProvider DefaultProvider
            => s_ccpProvider ??
               (s_ccpProvider = new APIProvider
               {
                   Url = new Uri(NetworkConstants.ESIBase),
                   Name = "CCP"
               });

        /// <summary>
        /// Gets the test API provider (RIP SiSi ESI)
        /// </summary>
        public static APIProvider TestProvider
            => s_ccpTestProvider ??
               (s_ccpTestProvider = new APIProvider
               {
                   Url = new Uri("https://www.google.com/"),
                   Name = "CCP Test API"
               });

        #endregion


        #region Helpers
        
        /// <summary>
        /// Returns the request method.
        /// </summary>
        /// <param name="requestMethod">An ESIMethodsEnum enumeration member specifying the
        /// method for which the URL is required.</param>
        private ESIMethod GetESIMethod(Enum requestMethod)
        {
            var esiMethod = m_methods.FirstOrDefault(method => method.Method.Equals(
                requestMethod));
            if (esiMethod == null)
                throw new InvalidOperationException("No ESI method found for " + requestMethod);
            return esiMethod;
        }

        /// <summary>
        /// Creates an ESI result to wrap a result from DownloadJsonAsync, and synchronizes
        /// its times if necessary.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The downloaded data.</param>
        /// <returns>An ESI result wrapping the data, with errors set as necessary.</returns>
        private EsiResult<T> GetESIResult<T>(JsonResult<T> result)
        {
            result.ThrowIfNull(nameof(result));

            // Update ESI error count; since ESI currently throttles by minute add 90 seconds
            var response = result.Response;
            if (response?.ErrorCount != null && !response.IsNotModifiedResponse && !response.
                    IsOKResponse)
                EsiErrors.UpdateErrors((int)response.ErrorCount, DateTime.UtcNow.AddSeconds(90.0));

            var esiResult = new EsiResult<T>(result);
            // Sync clock on the answer if necessary and provided
            var sync = esiResult.Result as ISynchronizableWithLocalClock;
            DateTime? when = esiResult.CurrentTime;
            if (sync != null && when != null)
                sync.SynchronizeWithLocalClock(DateTime.UtcNow - (DateTime)when);
            return esiResult;
        }

        /// <summary>
        /// Returns the full canonical ESI URL for the specified APIMethod as constructed from
        /// the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethodsEnum enumeration member specifying the
        /// method for which the URL is required.</param>
        /// <param name="data">The ESI parameters for this URL.</param>
        /// <param name="page">The page to fetch; 0 or 1 will fetch without requesting a page
        /// </param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        private Uri GetESIUrl(Enum requestMethod, ESIParams data, int page = 1)
        {
            long id = data.ParamOne;
            string paramStr = string.IsNullOrEmpty(data.GetData) ? data.ParamTwo.ToString(
                CultureConstants.InvariantCulture) : data.GetData;
            string path = string.Format(GetESIMethod(requestMethod).Path, id, paramStr);
            
            // Build the URI
            var builder = new UriBuilder(new Uri(NetworkConstants.ESIBase));
            builder.Path = Path.Combine(builder.Path, path);
            if (page > 1)
                builder.Query = "page=" + page.ToString(CultureConstants.InvariantCulture);
            return builder.Uri;
        }

        /// <summary>
        /// Creates the request parameters for ESI.
        /// </summary>
        /// <param name="data">The ESI parameters.</param>
        /// <returns>The required request parameters, including the ETag/Expiry (if supplied)
        /// and POST data/token.</returns>
        private RequestParams GetRequestParams(ESIParams data)
        {
            return new RequestParams(data.LastResponse, data.PostData)
            {
                Authentication = data.Token,
                AcceptEncoded = SupportsCompressedResponse
            };
        }

        #endregion


        #region Querying

        /// <summary>
        /// Helper method for fetching paginated items.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being
        /// <see cref="CCPAPIResult{T}" />). It must be a collection type of U!</typeparam>
        /// <typeparam name="U">The item type to deserialize.</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.
        /// </param>
        /// <param name="data">The parameters to use for the request, including the token,
        /// arguments, and POST data.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        private void QueryEsiPageHelper<T, U>(Enum method, ESIRequestCallback<T> callback,
            ESIParams data, PageInfo<T, U> state) where T : List<U> where U : class
        {
            int page = state.CurrentPage;
            var first = state.FirstResult;
            Uri pageUrl = GetESIUrl(method, data, page);
            // Create RequestParams manually to zero out the ETag/Expiry since it was already
            // checked
            Util.DownloadJsonAsync<T>(pageUrl, new RequestParams(null, data.PostData)
            {
                Authentication = data.Token,
                AcceptEncoded = SupportsCompressedResponse
            }).ContinueWith(task =>
            {
                var esiResult = GetESIResult(task.Result);
                object callbackState = state.State;
                if (esiResult.HasError)
                    // Invoke the callback if an error occurred
                    Dispatcher.Invoke(() => callback.Invoke(esiResult, callbackState));
                else if (!esiResult.HasData)
                    // This should not occur
                    Dispatcher.Invoke(() => callback.Invoke(first, callbackState));
                else
                {
                    first.Result.AddRange(esiResult.Result);
                    if (page >= state.LastPage)
                        // All pages fetched
                        Dispatcher.Invoke(() => callback.Invoke(first, callbackState));
                    else
                        // Go to the next page
                        QueryEsiPageHelper(method, callback, data, state.NextPage());
                }
            });
        }

        /// <summary>
        /// Asynchronously queries this method, fetching all pages if necessary, with the
        /// provided request data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being
        /// <see cref="CCPAPIResult{T}" />). It must be a collection type of U!</typeparam>
        /// <typeparam name="U">The item type to deserialize.</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.
        /// </param>
        /// <param name="data">The parameters to use for the request, including the token,
        /// arguments, and POST data.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be
        /// null.</exception>
        public void QueryPagedEsi<T, U>(Enum method, ESIRequestCallback<T> callback,
            ESIParams data, object state = null) where T : List<U> where U : class
        {
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");

            Uri url = GetESIUrl(method, data);
            Util.DownloadJsonAsync<T>(url, GetRequestParams(data)).ContinueWith(task =>
            {
                var esiResult = GetESIResult(task.Result);
                // Check page count
                int pages = esiResult.Response.Pages;
                if (pages > 1 && esiResult.HasData && !esiResult.HasError)
                    // Fetch the other pages
                    QueryEsiPageHelper(method, callback, data, new PageInfo<T, U>(esiResult,
                        pages, state));
                else
                    // Invokes the callback
                    Dispatcher.Invoke(() => callback.Invoke(esiResult, state));
            });
        }

        /// <summary>
        /// Asynchronously queries this method with the provided request data.
        /// </summary>
        /// <typeparam name="T">The subtype to deserialize (the deserialized type being
        /// <see cref="CCPAPIResult{T}" />).</typeparam>
        /// <param name="method">The method to query</param>
        /// <param name="callback">The callback to invoke once the query has been completed.
        /// </param>
        /// <param name="data">The parameters to use for the request, including the token,
        /// arguments, and POST data.</param>
        /// <param name="state">State to be passed to the callback when it is used.</param>
        /// <exception cref="System.ArgumentNullException">callback; The callback cannot be
        /// null.</exception>
        public void QueryEsi<T>(Enum method, ESIRequestCallback<T> callback, ESIParams
            data, object state = null) where T : class
        {
            callback.ThrowIfNull(nameof(callback), "The callback cannot be null.");

            Uri url = GetESIUrl(method, data);
            Util.DownloadJsonAsync<T>(url, GetRequestParams(data)).ContinueWith(task =>
            {
                // Invokes the callback
                Dispatcher.Invoke(() => callback.Invoke(GetESIResult(task.Result), state));
            });
        }
        
        /// <summary>
        /// Gets the XSLT used for transforming rowsets into something deserializable by
        /// <see cref="System.Xml.Serialization.XmlSerializer"/>
        /// </summary>
        internal static XslCompiledTransform RowsetsTransform => s_rowsetsTransform ??
            (s_rowsetsTransform = Util.LoadXslt(Properties.Resources.RowsetsXSLT));

        #endregion


        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;


        #region Helper classes

        /// <summary>
        /// Tracks the state of a multi-paged request.
        /// </summary>
        private sealed class PageInfo<T, U> where T : List<U> where U : class
        {
            /// <summary>
            /// The current page which was just fetched.
            /// </summary>
            public int CurrentPage { get; }

            /// <summary>
            /// The result from fetching the first page.
            /// </summary>
            public EsiResult<T> FirstResult { get; }

            /// <summary>
            /// The last page to fetch.
            /// </summary>
            public int LastPage { get; }

            /// <summary>
            /// The state object to be passed to the callback.
            /// </summary>
            public object State { get; }

            /// <summary>
            /// Creates the information for fetching the second page.
            /// </summary>
            /// <param name="first">The result from the first page.</param>
            /// <param name="count">The number of total pages.</param>
            /// <param name="state">The state to be passed to the callback.</param>
            public PageInfo(EsiResult<T> first, int count, object state)
            {
                first.ThrowIfNull(nameof(first));
                if (count < 2)
                    throw new ArgumentOutOfRangeException("count");
                CurrentPage = 2;
                FirstResult = first;
                LastPage = count;
                State = state;
            }

            /// <summary>
            /// Creates a page info from the previous page.
            /// </summary>
            /// <param name="previous">The information for the previous page fetched.</param>
            private PageInfo(PageInfo<T, U> previous)
            {
                previous.ThrowIfNull(nameof(previous));

                CurrentPage = previous.CurrentPage + 1;
                FirstResult = previous.FirstResult;
                LastPage = previous.LastPage;
                State = previous.State;
            }

            /// <summary>
            /// Creates a page info from the previous page.
            /// </summary>
            /// <returns>The state for the next page to fetch.</returns>
            public PageInfo<T, U> NextPage()
            {
                return new PageInfo<T, U>(this);
            }

            public override string ToString()
            {
                return string.Format("Page {0:D}/{1:D}", CurrentPage, LastPage);
            }
        }

        #endregion

    }

    #region Helper classes

    /// <summary>
    /// Simplifies ESI request building by allowing parameters to be flexibly included.
    /// </summary>
    public struct ESIParams
    {
        /// <summary>
        /// The first parameter, usually used for the ID of the target object in public
        /// requests and for the character/corporation ID in private requests.
        /// </summary>
        public long ParamOne;
        /// <summary>
        /// The second parameter, usually used for the ID of the target object in private
        /// requests.
        /// </summary>
        public long ParamTwo;
        /// <summary>
        /// The GET data to be passed in as a string.
        /// </summary>
        public string GetData;
        /// <summary>
        /// The POST data to be passed in as a string.
        /// </summary>
        public string PostData;
        /// <summary>
        /// The last response from the server.
        /// </summary>
        public ResponseParams LastResponse;
        /// <summary>
        /// The token to use for authentication.
        /// </summary>
        public string Token;

        public ESIParams(ResponseParams lastResponse, string token = null)
        {
            ParamOne = 0L;
            ParamTwo = 0L;
            GetData = null;
            LastResponse = lastResponse ?? new ResponseParams(0);
            PostData = null;
            Token = token;
        }
    }

    #endregion

}
