using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPISkillQueue : List<EsiSkillQueueListItem>, ISynchronizableWithLocalClock
    {
        public SerializableAPISkillQueue ToXMLItem()
        {
            var ret = new SerializableAPISkillQueue();
            foreach (var queueItem in this)
                ret.Queue.Add(queueItem.ToXMLItem());
            return ret;
        }

        #region ISynchronizableWithLocalClock Members

        /// <summary>
        /// Synchronizes the stored times with local clock
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            foreach (ISynchronizableWithLocalClock queueItem in this)
                queueItem.SynchronizeWithLocalClock(drift);
        }

        #endregion

    }
}
