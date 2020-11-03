using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiSkillQueueListItem : ISynchronizableWithLocalClock
    {
        private DateTime endTime;
        private DateTime startTime;

        public EsiSkillQueueListItem()
        {
            endTime = DateTime.MaxValue;
            startTime = DateTime.MinValue;
        }

        [DataMember(Name = "skill_id")]
        public int ID { get; set; }

        [DataMember(Name = "finished_level")]
        public int Level { get; set; }

        [DataMember(Name = "training_start_sp")]
        public int StartSP { get; set; }

        [DataMember(Name = "level_start_sp")]
        public int LevelStartSP { get; set; }

        [DataMember(Name = "level_end_sp")]
        public int EndSP { get; set; }

        [DataMember(Name = "queue_position")]
        public int QueuePosition { get; set; }

        [DataMember(Name = "start_date")]
        private string CCPStartTime
        {
            get
            {
                return startTime.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    startTime = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "finish_date")]
        private string CCPEndTime
        {
            get
            {
                return endTime.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    endTime = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if (value > DateTime.MinValue)
                    startTime = value;
            }
        }

        [IgnoreDataMember]
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if (value > DateTime.MinValue)
                    endTime = value;
            }
        }

        public SerializableQueuedSkill ToXMLItem()
        {
            return new SerializableQueuedSkill()
            {
                EndTime = EndTime,
                StartTime = StartTime,
                EndSP = EndSP,
                StartSP = StartSP,
                ID = ID,
                Level = Level
            };
        }

        #region ISynchronizableWithLocalClock Members

        /// <summary>
        /// Synchronizes the stored times with local clock
        /// </summary>
        /// <param name="drift"></param>
        void ISynchronizableWithLocalClock.SynchronizeWithLocalClock(TimeSpan drift)
        {
            if (startTime > DateTime.MinValue)
                startTime -= drift;

            if (endTime > DateTime.MinValue)
                endTime -= drift;
        }

        #endregion

    }
}
