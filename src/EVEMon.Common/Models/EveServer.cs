using System;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.QueryMonitor;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents the status of a server
    /// </summary>
    public sealed class EveServer
    {
        private int m_users;
        private ServerStatus m_status;
        private readonly QueryMonitor<EsiAPIServerStatus> m_serverStatusMonitor;
        private DateTime m_serverDateTime = DateTime.UtcNow;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal EveServer()
        {
            m_status = ServerStatus.Online;

            m_serverStatusMonitor = new QueryMonitor<EsiAPIServerStatus>(ESIAPIGenericMethods.ServerStatus,
                 OnServerStatusMonitorUpdated) { Enabled = true };

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Gets the server's name.
        /// </summary>
        private static string Name => EveMonClient.APIProviders.CurrentProvider.Url.Host !=
            APIProvider.TestProvider.Url.Host ? "Tranquility" : "Singularity";

        /// <summary>
        /// Gets the server status message.
        /// </summary>
        public string StatusText
        {
            get
            {
                switch (m_status)
                {
                    case ServerStatus.Online:
                        return $"{Name} Server Online ({m_users:N0} Pilots)";
                    case ServerStatus.Offline:
                        return $"{Name} Server Offline";
                    case ServerStatus.CheckDisabled:
                        return $"{Name} Server Status Check Disabled";
                    default:
                        return $"{Name} Server Status Unknown";
                }
            }
        }

        /// <summary>
        /// Gets the server date time.
        /// </summary>
        public DateTime ServerDateTime => m_serverDateTime;

        /// <summary>
        /// Forces an update of the server status.
        /// </summary>
        public void ForceUpdate()
        {
            ((IQueryMonitorEx)m_serverStatusMonitor).ForceUpdate();
        }

        /// <summary>
        /// Occurs when CCP returns new data.
        /// </summary>
        /// <param name="result"></param>
        private void OnServerStatusMonitorUpdated(EsiResult<EsiAPIServerStatus> result)
        {
            ServerStatus lastStatus = m_status;

            // Update the server date and time (in case of API server total failure use UTC time)
            m_serverDateTime = result.CurrentTime != DateTime.MinValue ? result.CurrentTime : DateTime.UtcNow;
            
            // Was there an error ?
            if (result.HasError)
            {
                m_status = ServerStatus.Unknown;
                EveMonClient.Notifications.NotifyServerStatusError(result);

                // Notify subscribers about update
                EveMonClient.OnServerStatusUpdated(this, lastStatus, m_status);
                return;
            }

            // Update status and users
            m_users = result.Result.Players;
            m_status = (m_users < 1 || result.Result.VIP) ? ServerStatus.Offline : ServerStatus.Online;

            // Invalidate any error notifications
            EveMonClient.Notifications.InvalidateAPIError();

            // Notify subscribers about update
            EveMonClient.OnServerStatusUpdated(this, lastStatus, m_status);

            // Send a notification
            if (lastStatus != m_status)
            {
                EveMonClient.Notifications.NotifyServerStatusChanged(Name, m_status);
                return;
            }

            EveMonClient.Notifications.InvalidateServerStatusChange();
        }

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            m_serverDateTime = m_serverDateTime.AddSeconds(1);
        }
    }
}
