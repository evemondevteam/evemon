namespace EVEMon.Common.Net
{
    /// <summary>
    /// Status types for an HttpWebServiceException
    /// </summary>
    public enum HttpWebClientServiceExceptionStatus
    {
        Exception,
        WebException,
        RedirectsExceeded,
        RequestsDisabled,
        ServerError,
        ProxyError,
        NameResolutionFailure,
        ConnectFailure,
        Timeout,
        XmlException,
        ImageException,
        FileError,
        Forbidden
    }
}
