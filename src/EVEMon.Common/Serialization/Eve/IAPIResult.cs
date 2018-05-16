using System;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents the result of an API operation.
    /// </summary>
    public interface IAPIResult
    {
        /// <summary>
        /// Gets true if there is an error.
        /// </summary>
        /// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
        bool HasError { get; }
        
        /// <summary>
        /// Gets the type of the error or <see cref="APIErrorType.None"/> when there was no error.
        /// </summary>
        /// <value>The type of the error.</value>
        APIErrorType ErrorType { get; }

        /// <summary>
        /// Gets the error message without bothering about its nature.
        /// </summary>
        /// <value>The error message.</value>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the error code. The meaning of this code varies depending on the source, and
        /// is invalid if there was no error.
        /// </summary>
        int ErrorCode { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        Exception Exception { get; }
    }
}
