using EVEMon.Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents the character's attributes
    /// </summary>
    [DataContract]
    public sealed class EsiAPIAttributes
    {
        private DateTime lastRemap;
        private DateTime remapCooldownDate;

        public EsiAPIAttributes()
        {
            Intelligence = Memory = Perception = Charisma = Willpower = 1;
            lastRemap = DateTime.MinValue;
            remapCooldownDate = DateTime.MinValue;
        }

        [DataMember(Name = "intelligence")]
        public int Intelligence { get; set; }

        [DataMember(Name = "memory")]
        public int Memory { get; set; }

        [DataMember(Name = "perception")]
        public int Perception { get; set; }

        [DataMember(Name = "willpower")]
        public int Willpower { get; set; }

        [DataMember(Name = "charisma")]
        public int Charisma { get; set; }

        [DataMember(Name = "bonus_remaps", IsRequired = false)]
        public short BonusRemaps { get; set; }

        [DataMember(Name = "last_remap_date", EmitDefaultValue = false, IsRequired = false)]
        private string LastRemapJson
        {
            get
            {
                return lastRemap.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    lastRemap = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "accrued_remap_cooldown_date", EmitDefaultValue = false, IsRequired = false)]
        private string RemapCooldownDateJson
        {
            get
            {
                return remapCooldownDate.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    remapCooldownDate = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime LastRemap
        {
            get
            {
                return lastRemap;
            }
        }

        [IgnoreDataMember]
        public DateTime RemapCooldownDate
        {
            get
            {
                return remapCooldownDate;
            }
        }
    }
}
