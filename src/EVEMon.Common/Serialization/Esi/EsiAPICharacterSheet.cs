using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Base class for the ESI character sheet.
    /// </summary>
    [DataContract]
    public sealed class EsiAPICharacterSheet
    {
        private DateTime birthday;

        public EsiAPICharacterSheet()
        {
            birthday = DateTime.MinValue;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Name = "birthday")]
        private string BirthdayJson
        {
            get { return birthday.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    birthday = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "race_id")]
        private int RaceID { get; set; }

        public Race Race
        {
            get
            {
                return (Race)RaceID;
            }
        }

        [DataMember(Name = "bloodline_id")]
        private int BloodLineID { get; set; }

        public Bloodline BloodLine
        {
            get
            {
                return (Bloodline)BloodLineID;
            }
        }

        [DataMember(Name = "ancestry_id", EmitDefaultValue = false, IsRequired = false)]
        private int AncestryID { get; set; }

        public Ancestry Ancestry
        {
            get
            {
                return (Ancestry)AncestryID;
            }
        }

        // One of: female, male
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "corporation_id")]
        public long CorporationID { get; set; }

        [DataMember(Name = "alliance_id", EmitDefaultValue = false, IsRequired = false)]
        public long AllianceID { get; set; }

        [DataMember(Name = "faction_id", EmitDefaultValue = false, IsRequired = false)]
        public int FactionID { get; set; }

        [DataMember(Name = "security_status")]
        public double SecurityStatus { get; set; }

        /// <summary>
        /// The date and time the character was created.
        /// </summary>
        [IgnoreDataMember]
        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                if (value > DateTime.MinValue)
                    birthday = value;
            }
        }
    }
}
