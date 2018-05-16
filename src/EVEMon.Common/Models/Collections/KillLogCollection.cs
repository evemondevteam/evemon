using System.Collections.Generic;
using System.IO;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Constants;
using System;

namespace EVEMon.Common.Models.Collections
{
    public sealed class KillLogCollection : ReadonlyCollection<KillLog>
    {
        private readonly CCPCharacter m_ccpCharacter;
        private readonly object m_counterLock;
        private int m_killMailCounter;

        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal KillLogCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_killMailCounter = 0;
            m_counterLock = new object();
        }

        #endregion


        #region Importation

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable kill log from the API.</param>
        internal void Import(IEnumerable<SerializableKillLogListItem> src)
        {
            Items.Clear();
            foreach (SerializableKillLogListItem srcKillLog in src)
                Items.Add(new KillLog(m_ccpCharacter, srcKillLog));
        }

        /// <summary>
        /// Imports an enumeration of ESI objects.
        /// </summary>
        /// <param name="kills">The enumeration of serializable kill data from ESI.</param>
        internal void Import(EsiAPIKillLog kills)
        {
            if (m_killMailCounter == 0)
            {
                Items.Clear();
                EveMonClient.Notifications.InvalidateAPIError();
                lock (m_counterLock)
                {
                    m_killMailCounter = kills.Count;
                }
                foreach (EsiKillLogListItem srcKillLog in kills)
                {
                    // Query each individual mail
                    string hash = srcKillLog.Hash;
                    EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIKillMail>(
                        ESIAPIGenericMethods.KillMail, srcKillLog.KillID, hash,
                        OnKillMailDownloaded, hash);
                }
            }
        }

        private void OnKillMailDownloaded(EsiResult<EsiAPIKillMail> result, object hashValue)
        {
            var target = m_ccpCharacter;
            string hash = hashValue?.ToString() ?? EveMonConstants.UnknownText;

            // If character is still around and monitored
            if (target != null && target.Monitored)
            {
                if (result.HasError)
                    EveMonClient.Notifications.NotifyKillMailError(result, hash);
                else
                    Items.Add(new KillLog(target, result.Result.ToXMLItem()));
            }
            // Synchronization is required here since multiple requests can finish at once
            lock (m_counterLock)
            {
                m_killMailCounter = Math.Max(0, m_killMailCounter - 1);
            }
        }

        /// <summary>
        /// Imports the kill logs from a cached file.
        /// </summary>
        public void ImportFromCacheFile()
        {
            string filename = LocalXmlCache.GetFileInfo(
                $"{m_ccpCharacter.Name}-{ESIAPICharacterMethods.KillLog}").FullName;
            // Abort if the file hasn't been obtained for any reason
            if (!File.Exists(filename))
                return;
            var result = Util.DeserializeAPIResultFromFile<SerializableAPIKillLog>(
                filename, APIProvider.RowsetsTransform);
            if (!result.HasError)
                Import(result.Result.Kills);
        }

        #endregion

    }
}
