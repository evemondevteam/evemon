using System;

namespace EVEMon.WindowsApi
{
    /// <summary>
    /// Windows 7 Specific API Calls
    /// </summary>
    public static class Windows7
    {
        /// <summary>
        /// Calls SetCurrentProcessExplicitAppUserModelID() to set the current process AppID.
        /// </summary>
        /// <param name="appId">128 character or smaller Application ID.</param>
        public static void SetProcessAppId(string appId)
        {
            if (!OSFeatureCheck.TaskbarSupported)
                return;

            if (string.IsNullOrWhiteSpace(appId) || appId.Length > 128)
                throw new ArgumentException("AppID must be 128 characters or less", nameof(appId));

            SafeNativeMethods.SetCurrentProcessExplicitAppUserModelID(appId);
        }
    }
}