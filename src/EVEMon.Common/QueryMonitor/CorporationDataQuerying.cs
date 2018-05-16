using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.QueryMonitor
{
    internal sealed class CorporationDataQuerying
    {
        #region Fields

        private readonly CorporationQueryMonitor<EsiAPIMedals> m_corpMedalsMonitor;
        private readonly CorporationQueryMonitor<EsiAPIMarketOrders> m_corpMarketOrdersMonitor;
        private readonly CorporationQueryMonitor<EsiAPIContracts> m_corpContractsMonitor;
        private readonly CorporationQueryMonitor<EsiAPIIndustryJobs> m_corpIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_corporationQueryMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        #endregion


        #region Constructor

        internal CorporationDataQuerying(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_corporationQueryMonitors = new List<IQueryMonitorEx>(4);

            // Initializes the query monitors 
            m_corpMedalsMonitor = new CorporationQueryMonitor<EsiAPIMedals>(ccpCharacter,
                ESIAPICorporationMethods.CorporationMedals, OnMedalsUpdated,
                EveMonClient.Notifications.NotifyCorporationMedalsError)
            { QueryOnStartup = true };
            // Add the monitors in an order as they will appear in the throbber menu
            m_corporationQueryMonitors.Add(m_corpMedalsMonitor);
            m_corpMarketOrdersMonitor = new CorporationQueryMonitor<EsiAPIMarketOrders>(
                ccpCharacter, ESIAPICorporationMethods.CorporationMarketOrders,
                OnMarketOrdersUpdated, EveMonClient.Notifications.
                NotifyCorporationMarketOrdersError) { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpMarketOrdersMonitor);
            m_corpContractsMonitor = new CorporationQueryMonitor<EsiAPIContracts>(ccpCharacter,
                ESIAPICorporationMethods.CorporationContracts, OnContractsUpdated,
                EveMonClient.Notifications.NotifyCorporationContractsError)
            { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpContractsMonitor);
            m_corpIndustryJobsMonitor = new CorporationQueryMonitor<EsiAPIIndustryJobs>(
                ccpCharacter, ESIAPICorporationMethods.CorporationIndustryJobs,
                OnIndustryJobsUpdated, EveMonClient.Notifications.
                NotifyCorporationIndustryJobsError) { QueryOnStartup = true };
            m_corporationQueryMonitors.Add(m_corpIndustryJobsMonitor);

            foreach (var monitor in m_corporationQueryMonitors)
                ccpCharacter.QueryMonitors.Add(monitor);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the corporation market orders have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation market orders have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationMarketOrdersQueried => !m_corpMarketOrdersMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the corporation contracts have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation contracts have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationContractsQueried => !m_corpContractsMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the corporation industry jobs have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the corporation industry jobs have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CorporationIndustryJobsQueried => !m_corpIndustryJobsMonitor.IsUpdating;

        #endregion


        #region Dispose

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal void Dispose()
        {
            // Unsubscribe events in monitors
            foreach (IQueryMonitorEx monitor in m_corporationQueryMonitors)
            {
                monitor.Dispose();
            }
        }

        #endregion


        #region Querying
        
        /// <summary>
        /// Processes the queried character's corporation medals.
        /// </summary>
        /// <param name="result"></param>
        private void OnMedalsUpdated(EsiAPIMedals result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.CorporationMedals.Import(result.ToXMLItem().CorporationMedals);
                EveMonClient.OnCorporationMedalsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnMarketOrdersUpdated(EsiAPIMarketOrders result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var orders = result.ToXMLItem(target.CorporationID).Orders;
                // Mark all orders as corporation issued
                foreach (var order in orders)
                    order.IssuedFor = IssuedFor.Corporation;
                var endedOrders = new LinkedList<MarketOrder>();
                target.CorporationMarketOrders.Import(orders, endedOrders);
                EveMonClient.OnCorporationMarketOrdersUpdated(target, endedOrders);
            }
        }

        /// <summary>
        /// Processes the queried character's corporation contracts.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which contracts gets queried first</remarks>
        private void OnContractsUpdated(EsiAPIContracts result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var contracts = result.ToXMLItem().Contracts;
                // Mark all contracts as corporation issued
                foreach (var contract in contracts)
                {
                    contract.APIMethod = ESIAPICorporationMethods.CorporationContracts;
                    contract.IssuedFor = IssuedFor.Corporation;
                }
                var endedContracts = new List<Contract>();
                target.CorporationContracts.Import(contracts, endedContracts);
                EveMonClient.OnCorporationContractsUpdated(target, endedContracts);
            }
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnIndustryJobsUpdated(EsiAPIIndustryJobs result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // Mark all jobs as corporation issued
                var jobs = result.ToXMLItem().Jobs;
                foreach (var job in jobs)
                    job.IssuedFor = IssuedFor.Corporation;
                target.CorporationIndustryJobs.Import(jobs);
                EveMonClient.OnCorporationIndustryJobsUpdated(target);
            }
        }

        #endregion
    }
}
