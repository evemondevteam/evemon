using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Net
{
    /// <summary>
    /// This class notifies subscribers about the change and the status of an network interface
    /// </summary>
    public static class NetworkMonitor
    {
        private static readonly object s_syncLock = new object();

        private static List<WeakReference<INetworkChangeSubscriber>> s_subscribers;
        private static bool s_networkAvailable;
        private static bool s_manualTestRequired;

        /// <summary>
        /// Initializer.
        /// </summary>
        public static void Initialize()
        {
            if (s_subscribers != null)
                return;

            // Subscribe to network changes
            s_subscribers = new List<WeakReference<INetworkChangeSubscriber>>();

            try
            {
                s_networkAvailable = NetworkInterface.GetIsNetworkAvailable();
                NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
            }
            catch (Exception ex)
            {
                // GetIsNetworkAvailable doesn't seem to work on every system (f.ex. Mac OSX/Darwine)
                ExceptionHandler.LogException(ex, true);

                // Check the network manually and set the manual flag to true
                s_networkAvailable = IsNetworkAvailableManual();
                s_manualTestRequired = true;
            }
        }

        /// <summary>
        /// Tests to see if the network is available 
        /// </summary>
        /// <returns>true if ping is sucessfull, otherwise false</returns>
        private static bool IsNetworkAvailableManual()
        {
            // Send a ping to www.google.com
            using (Ping pingSender = new Ping())
            {
                PingOptions options = new PingOptions(50, false);
                byte[] buffer = Encoding.ASCII.GetBytes("EVEMon Network Status Ping");
                const int Timeout = 120;
                const string Host = "www.google.com";
                PingReply reply = pingSender.Send(Host, Timeout, buffer, options);

                return reply != null && (reply.Status == IPStatus.Success);
            }
        }

        /// <summary>
        /// Gets true when a TCP/IP connection is available.
        /// </summary>
        public static bool IsNetworkAvailable
        {
            get
            {
                // Edge case: Wine/Darwine user with broken .NET Networking Stack
                if (Settings.Updates.IgnoreNetworkStatus)
                    return true;

                return !s_manualTestRequired ? s_networkAvailable : IsNetworkAvailableManual();
            }
        }

        /// <summary>
        /// Registers the given object for notifications about a network availability change.
        /// </summary>
        /// <param name="monitor"></param>
        public static void Register(INetworkChangeSubscriber monitor)
        {
            lock (s_syncLock)
                s_subscribers?.Add(new WeakReference<INetworkChangeSubscriber>(monitor, false));
        }

        /// <summary>
        /// When the networks connection is opened or closed, we notify the subscribers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            lock (s_syncLock)
            {
                s_networkAvailable = e.IsAvailable;

                if (s_subscribers == null)
                    return;

                // Scroll through the monitors and remove them when they're no longer available
                int index = 0;
                while (index < s_subscribers.Count)
                {
                    INetworkChangeSubscriber target;
                    if (s_subscribers[index].TryGetTarget(out target))
                    {
                        target.SetNetworkStatus = e.IsAvailable;
                        index++;
                    }
                    else
                        s_subscribers.RemoveAt(index);
                }
            }
        }
    }
}