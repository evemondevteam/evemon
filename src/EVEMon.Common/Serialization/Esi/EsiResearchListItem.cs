using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiResearchListItem
    {
        private DateTime researchStart;

        public EsiResearchListItem()
        {
            researchStart = DateTime.MinValue;
        }

        [DataMember(Name = "agent_id")]
        public int AgentID { get; set; }

        [DataMember(Name = "skill_type_id")]
        public int SkillID { get; set; }

        [DataMember(Name = "started_at")]
        public string ResearchStartDateJson
        {
            get { return researchStart.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    researchStart = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "points_per_day")]
        public double PointsPerDay { get; set; }

        [DataMember(Name = "remainder_points")]
        public float RemainderPoints { get; set; }

        /// <summary>
        /// The time this research was started.
        /// </summary>
        [IgnoreDataMember]
        public DateTime ResearchStartDate
        {
            get
            {
                return researchStart;
            }
        }

        public SerializableResearchListItem ToXMLItem()
        {
            return new SerializableResearchListItem()
            {
                AgentID = AgentID,
                PointsPerDay = PointsPerDay,
                RemainderPoints = RemainderPoints,
                ResearchStartDate = ResearchStartDate
            };
        }
    }
}
