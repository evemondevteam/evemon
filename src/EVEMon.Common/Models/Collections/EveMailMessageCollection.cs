using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Models.Collections
{
    public sealed class EveMailMessageCollection : ReadonlyCollection<EveMailMessage>
    {
        private readonly CCPCharacter m_ccpCharacter;
        private long m_highestID;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal EveMailMessageCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the number of new messages.
        /// </summary>
        /// <value>The new messages.</value>
        internal int NewMessages { get; private set; }

        #endregion


        #region Importation & Exportation

        /// <summary>
        /// Imports the eve mail messages IDs from a serializable object.
        /// </summary>
        /// <param name="eveMailMessagesIDs">The eve mail messages IDs.</param>
        internal void Import(string eveMailMessagesIDs)
        {
            if (string.IsNullOrEmpty(eveMailMessagesIDs))
                return;

            Items.Clear();
            List<string> ids = eveMailMessagesIDs.Split(',').ToList();
            foreach (long id in ids.Select(long.Parse))
            {
                Items.Add(new EveMailMessage(m_ccpCharacter,
                    new SerializableMailMessagesListItem
                    {
                        MessageID = id
                    }));
            }

            // Set the last received ID 
            m_highestID = Items.Any() ? Items.Max(item => item.MessageID) : 0;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable mail messages from the API.</param>
        internal void Import(IEnumerable<SerializableMailMessagesListItem> src)
        {
            NewMessages = 0;
            List<EveMailMessage> newMessages = new List<EveMailMessage>();

            // Import the mail messages from the API
            // To distinguish new EVE mails from old EVE mails that have been added to the API list
            // due to deletion of some EVE mails in-game, we need to sort the received data 
            foreach (SerializableMailMessagesListItem srcEVEMailMessage in src.OrderBy(x => x.MessageID))
            {
                // Is it an Inbox message ?
                if (m_ccpCharacter.CharacterID != srcEVEMailMessage.SenderID)
                {
                    // If it's a newly mail message and not an old mail message added to the API list, increase the counter
                    EveMailMessage message = Items.FirstOrDefault(x => x.MessageID == srcEVEMailMessage.MessageID);
                    if (message == null && srcEVEMailMessage.MessageID > m_highestID)
                    {
                        NewMessages++;
                        m_highestID = srcEVEMailMessage.MessageID;
                    }
                }

                newMessages.Add(new EveMailMessage(m_ccpCharacter, srcEVEMailMessage));
            }

            Items.Clear();
            Items.AddRange(newMessages);

            // Set the last received ID 
            m_highestID = Items.Any() ? Items.Max(item => item.MessageID) : 0;

            // Fires the event regarding EVE mail messages update
            EveMonClient.OnCharacterEVEMailMessagesUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Exports the eve mail messages IDs to a serializable object.
        /// </summary>
        /// <returns></returns>
        // Store only the mail messages IDs from the inbox in a descending order
        internal string Export() => string.Join(",", Items.Where(x => x.SenderName != m_ccpCharacter.Name).OrderByDescending(
            x => x.MessageID).Select(message => message.MessageID.ToString(CultureConstants.InvariantCulture)));

        #endregion
    }
}