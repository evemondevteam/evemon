using System;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization
{
    /// <summary>
    /// Wraps the results returned from a JSON request.
    /// </summary>
    public class JsonResult<T>
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonResult(ResponseParams response, T result = default(T))
        {
            ErrorType = APIErrorType.None;
            Exception = null;
            ErrorMessage = string.Empty;
            Response = response;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCPAPIResult{T}" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        protected JsonResult(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            Exception = exception;
            ErrorMessage = exception?.Message ?? string.Empty;
            Response = new ResponseParams(0);
            Result = default(T);
        }

        /// <summary>
        /// Creates a JSON result wrapped from another JSON result.
        /// </summary>
        /// <param name="wrapped">The JSON result to wrap.</param>
        protected JsonResult(JsonResult<T> wrapped)
        {
            Exception = wrapped.Exception;
            ErrorMessage = wrapped.ErrorMessage;
            ErrorType = wrapped.ErrorType;
            Response = wrapped.Response;
            Result = wrapped.Result;
        }

        /// <summary>
        /// Constructor from an http exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(HttpWebClientServiceException exception)
            : this(exception as Exception)
        {
            ErrorType = APIErrorType.Http;
            Response = new ResponseParams((int)exception.StatusCode);
        }

        /// <summary>
        /// Constructor from a JSON exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(InvalidDataContractException exception)
            : this(exception as Exception)
        {
            ErrorType = APIErrorType.Json;
        }
        
        /// <summary>
        /// Constructor from a JSON serialization exception wrapped into an InvalidOperationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(InvalidOperationException exception)
            : this(exception as Exception)
        {
            ErrorType = APIErrorType.Json;
        }

        /// <summary>
        /// Constructor from a JSON serialization exception wrapped into a SerializationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(SerializationException exception)
            : this(exception as Exception)
        {
            ErrorType = APIErrorType.Json;
        }

        /// <summary>
        /// Constructor from a CCP API internal error
        /// </summary>
        /// <param name="response">The response parameters including the error code.</param>
        /// <param name="message">The CCP error message.</param>
        public JsonResult(ResponseParams response, string message)
        {
            ErrorType = APIErrorType.CCP;
            Exception = null;
            ErrorMessage = message ?? string.Empty;
            Response = response;
            Result = default(T);
        }

        #endregion


        #region Errors handling

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; }

        public bool HasError => Exception != null || ErrorType != APIErrorType.None;

        /// <summary>
        /// Gets the response code from the server.
        /// </summary>
        public int ResponseCode => Response.ResponseCode;

        #endregion


        #region Properties

        public string ErrorMessage { get; }

        public APIErrorType ErrorType { get; }

        public T Result { get; set; }

        public DateTime? CurrentTime => Response.Time;

        public ResponseParams Response { get; }

        #endregion

    }
}
