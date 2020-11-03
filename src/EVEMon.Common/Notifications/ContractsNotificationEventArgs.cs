using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Notifications
{
    public sealed class ContractsNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="contracts">The contracts.</param>
        /// <exception cref="System.ArgumentNullException">contracts</exception>
        public ContractsNotificationEventArgs(object sender, IEnumerable<Contract> contracts)
            : base(sender, NotificationCategory.ContractsEnded)
        {
            contracts.ThrowIfNull(nameof(contracts));

            Contracts = new Collection<Contract>();
            foreach (Contract contract in contracts)
            {
                Contracts.Add(contract);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<Contract> Contracts { get; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;

        /// <summary>
        /// Enqueue the contracts from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            foreach (Contract contract in ((ContractsNotificationEventArgs)other).Contracts.Where(x => !x.NotificationSend))
            {
                Contracts.Add(contract);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = $"{Contracts.Count} contract{(Contracts.Count > 1 ? "s" : string.Empty)} finished or needs attention.";
        }
    }
}

