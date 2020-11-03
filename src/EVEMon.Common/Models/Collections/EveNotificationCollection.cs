using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models.Collections
{
    public sealed class EveNotificationCollection : ReadonlyCollection<EveNotification>
    {
        private readonly CCPCharacter m_ccpCharacter;
        private long m_highestID;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal EveNotificationCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the number of new notifications.
        /// </summary>
        /// <value>The new notifications.</value>
        public int NewNotifications { get; private set; }

        #endregion


        #region Importation & Exportation

        /// <summary>
        /// Imports the eve notifications IDs from a serializable object.
        /// </summary>
        /// <param name="eveNotificationsIDs">The eve notifications IDs.</param>
        internal void Import(string eveNotificationsIDs)
        {
            if (string.IsNullOrEmpty(eveNotificationsIDs))
                return;

            Items.Clear();
            List<string> ids = eveNotificationsIDs.Split(',').ToList();
            foreach (long id in ids.Select(long.Parse))
            {
                Items.Add(new EveNotification(m_ccpCharacter, new EsiNotificationsListItem
                {
                    NotificationID = id
                }));
            }

            // Set the last received ID 
            m_highestID = Items.Any() ? Items.Max(item => item.NotificationID) : 0;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable notifications from the API.</param>
        internal void Import(IEnumerable<EsiNotificationsListItem> src)
        {
            NewNotifications = 0;

            List<EveNotification> newNotifications = new List<EveNotification>();

            // Import the notifications from the API
            foreach (EsiNotificationsListItem srcEVENotification in src.OrderBy(x => x.NotificationID))
            {
                // If it's a new notification and not an old notification added to the API list, increase the counter
                EveNotification notification = Items.FirstOrDefault(x => x.NotificationID == srcEVENotification.NotificationID);
                if (notification == null && !srcEVENotification.Read && srcEVENotification.NotificationID > m_highestID)
                {
                    NewNotifications++;
                    m_highestID = srcEVENotification.NotificationID;
                }

                newNotifications.Add(new EveNotification(m_ccpCharacter, srcEVENotification));
            }

            Items.Clear();
            Items.AddRange(newNotifications);

            // Set the last received ID 
            m_highestID = Items.Any() ? Items.Max(item => item.NotificationID) : 0;

            // Fires the event regarding EVE mail messages update
            EveMonClient.OnCharacterEVENotificationsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Exports the eve notifications IDs to a serializable object.
        /// </summary>
        /// <returns></returns>
        internal string Export() => string.Join(",", Items.Select(notification =>
            notification.NotificationID.ToString(CultureConstants.InvariantCulture)));

        #endregion

    }
}
