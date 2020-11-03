using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPISkillQueue : List<EsiSkillQueueListItem>, ISynchronizableWithLocalClock
    {
        public ICollection<SerializableQueuedSkill> CreateSkillQueue()
        {
            var queue = new List<SerializableQueuedSkill>(Count);
            foreach (var queueItem in this)
                queue.Add(queueItem.ToXMLItem());
            return queue;
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
