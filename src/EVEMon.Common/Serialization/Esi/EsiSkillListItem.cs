using EVEMon.Common.Serialization.Eve;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a skill
    /// </summary>
    [DataContract]
    public sealed class EsiSkillListItem
    {
        [DataMember(Name = "skill_id")]
        public int ID { get; set; }

        [DataMember(Name = "trained_skill_level")]
        public int Level { get; set; }

        [DataMember(Name = "active_skill_level")]
        public int ActiveLevel { get; set; }

        [DataMember(Name = "skillpoints_in_skill")]
        public long Skillpoints { get; set; }

        public SerializableCharacterSkill ToXMLItem()
        {
            return new SerializableCharacterSkill()
            {
                ID = ID,
                Level = Level,
                ActiveLevel = ActiveLevel,
                Skillpoints = Skillpoints,
                OwnsBook = true,
                IsKnown = true
            };
        }
    }
}
