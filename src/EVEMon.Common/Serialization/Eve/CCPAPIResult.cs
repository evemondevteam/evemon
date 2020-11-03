using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using System.Xml.Xsl;

namespace EVEMon.Common.Serialization.Eve
{
    [XmlRoot("eveapi")]
    public sealed class CCPAPIResult<T> : IAPIResult
    {
        private readonly APIErrorType m_error;
        private readonly string m_errorMessage;
        private readonly Exception m_exception;


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CCPAPIResult()
        {
            m_error = APIErrorType.None;
            m_errorMessage = string.Empty;
            m_exception = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCPAPIResult{T}" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        private CCPAPIResult(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));

            m_errorMessage = exception.Message;
            m_exception = exception;
        }

        /// <summary>
        /// Constructor from an http exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public CCPAPIResult(HttpWebClientServiceException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Http;
        }

        /// <summary>
        /// Constructor from an XML exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public CCPAPIResult(XmlException exception)
            : this((Exception)exception)
        {
            m_error = APIErrorType.Xml;
        }

        /// <summary>
        /// Constructor from an XSLT exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public CCPAPIResult(XsltException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Xml;
        }

        /// <summary>
        /// Constructor from an XML serialization exception wrapped into an InvalidOperationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public CCPAPIResult(InvalidOperationException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Xml;
        }

        /// <summary>
        /// Constructor from a custom exception.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="message">The message.</param>
        public CCPAPIResult(APIErrorType error, string message)
        {
            m_error = error;
            m_errorMessage = message;
            m_exception = null;
        }

        #endregion


        #region Errors handling

        /// <summary>
        /// Gets true if the information is outdated.
        /// </summary>
        public bool IsOutdated => DateTime.UtcNow > CachedUntil;

        /// <summary>
        /// Gets true if there is an error.
        /// </summary>
        public bool HasError => CCPError != null || m_error != APIErrorType.None || Result == null;
        
        /// <summary>
        /// Gets the type of the error or <see cref="APIErrorType.None"/> when there was no error.
        /// </summary>
        public APIErrorType ErrorType => CCPError != null ? APIErrorType.CCP : m_error;

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception => m_exception;

        /// <summary>
        /// Gets the error message without bothering about its nature.
        /// </summary>
        public string ErrorMessage => CCPError?.ErrorMessage ?? m_errorMessage;

        /// <summary>
        /// Gets / sets the XML document when there's no HTTP error.
        /// </summary>
        [XmlIgnore]
        public IXPathNavigable XmlDocument { get; set; }

        #endregion


        #region CCP Mapping

        [XmlAttribute("version")]
        public int APIVersion { get; set; }

        [XmlElement("currentTime")]
        public string CurrentTimeXml
        {
            get { return CurrentTime.DateTimeToTimeString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                CurrentTime = value.TimeStringToDateTime();
            }
        }

        [XmlElement("cachedUntil")]
        public string CachedUntilXml
        {
            get { return CachedUntil.DateTimeToTimeString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                CachedUntil = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime CurrentTime { get; set; }

        [XmlIgnore]
        public DateTime CachedUntil { get; set; }

        [XmlElement("error")]
        public CCPAPIError CCPError { get; set; }

        [XmlElement("result")]
        public T Result { get; set; }

        [XmlIgnore]
        public int ErrorCode => CCPError?.ErrorCode ?? 0;

        #endregion


        #region Time fixing

        /// <summary>
        /// Fixup the result time to match the user's clock.
        /// This should ONLY be called when the xml is first received from CCP.
        /// </summary>
        /// <param name="millisecondsDrift"></param>
        public void SynchronizeWithLocalClock(double millisecondsDrift)
        {
            // Convert the drift between webserver time and local time
            // to a timespan. It is possible for millisecondsDrift to
            // be erroniously outside of the range of an int thus we 
            // need to catch an overflow exception and reset to 0.
            TimeSpan drift;
            try
            {
                drift = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(millisecondsDrift));
            }
            catch (OverflowException)
            {
                drift = new TimeSpan(0, 0, 0, 0);
            }

            // Fix the start/end times for the results implementing synchronization
            ISynchronizableWithLocalClock synchronizable = Result as ISynchronizableWithLocalClock;

            synchronizable?.SynchronizeWithLocalClock(drift);
        }

        #endregion
    }
}
