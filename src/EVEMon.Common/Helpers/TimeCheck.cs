using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Ensures synchronization of local time to a known time source.
    /// </summary>
    public static class TimeCheck
    {
        /// <summary>
        /// Occurs when time check completed.
        /// </summary>
        public static event EventHandler<TimeCheckSyncEventArgs> TimeCheckCompleted;

        /// <summary>
        /// Check for time synchronization,
        /// or reschedule it for later if no connection is available.
        /// </summary>
        public static void ScheduleCheck(TimeSpan time)
        {
            Dispatcher.Schedule(time, () => BeginCheckAsync().ConfigureAwait(false));
            EveMonClient.Trace($"in {time}");
        }

        /// <summary>
        /// Method to determine if the user's clock is syncrhonised to NTP time pool.
        /// Updated to move to NTP (global NTP pool) rather than NIST port 13 time check, which is being deprecated
        /// </summary>
        private static async Task BeginCheckAsync()
        {
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                // Reschedule later otherwise
                ScheduleCheck(TimeSpan.FromMinutes(1));
                return;
            }

            EveMonClient.Trace();

            string ntpServer = NetworkConstants.GlobalNTPPool;// "pool.ntp.org";
            DateTime serverTimeToLocalTime;
            bool isSynchronised;

            await Dns.GetHostAddressesAsync(ntpServer)
                .ContinueWith(task =>
                {
                    IPAddress[] ipAddresses = task.Result;

                    if (!ipAddresses.Any())
                        return;

                    try
                    {
                        DateTime dateTimeNowUtc;
                        DateTime localTime = DateTime.Now;
                        
                        var ntpData = new byte[48];
                        ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

                        //var addresses = task.Dns.GetHostEntry(ntpServer).AddressList;
                        var ipEndPoint = new IPEndPoint(task.Result.First(), 123);
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        socket.Connect(ipEndPoint);
                        socket.Send(ntpData);
                        socket.Receive(ntpData);
                        socket.Close();

                        ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
                        ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

                        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                        var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                        dateTimeNowUtc = networkDateTime;

                        serverTimeToLocalTime = dateTimeNowUtc.ToLocalTime();
                        TimeSpan timediff =
                            TimeSpan.FromSeconds(Math.Abs(serverTimeToLocalTime.Subtract(localTime).TotalSeconds));
                        isSynchronised = timediff < TimeSpan.FromSeconds(60);

                        OnCheckCompleted(isSynchronised, serverTimeToLocalTime, localTime);
                    }
                    catch (Exception exc)
                    {
                        CheckFailure(exc);
                    }
                }, EveMonClient.CurrentSynchronizationContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when the check fails.
        /// </summary>
        /// <param name="exc">The exc.</param>
        private static void CheckFailure(Exception exc)
        {
            EveMonClient.Trace(exc.Message);
            ScheduleCheck(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Called when time check completed.
        /// </summary>
        /// <param name="isSynchronised">if set to <c>true</c> [is synchronised].</param>
        /// <param name="serverTimeToLocalTime">The server time to local time.</param>
        /// <param name="localTime">The local time.</param>
        private static void OnCheckCompleted(bool isSynchronised, DateTime serverTimeToLocalTime, DateTime localTime)
        {
            EveMonClient.Trace(Settings.Updates.CheckTimeOnStartup ?  "Synchronised" : "Disabled");

            TimeCheckCompleted?.ThreadSafeInvoke(null, new TimeCheckSyncEventArgs(isSynchronised, serverTimeToLocalTime, localTime));

            // Reschedule
            ScheduleCheck(TimeSpan.FromDays(1));
        }
    }
}