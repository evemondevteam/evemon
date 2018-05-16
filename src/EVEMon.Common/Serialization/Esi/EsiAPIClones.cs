using EVEMon.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{     
    /// <summary>
    /// Represents a serializable version of a character's sheet. Used for querying CCP.
    /// </summary>
    [DataContract]
    public sealed class EsiAPIClones
    {
        private DateTime lastCloneJump;
        private DateTime lastStationChange;

        public EsiAPIClones()
        {
            lastCloneJump = DateTime.MinValue;
            lastStationChange = DateTime.MinValue;
        }

        [DataMember(Name = "home_location")]
        public EsiLocationBase HomeLocation { get; set; }

        // Returns the last date the character used a jump clone, in UTC
        [IgnoreDataMember]
        public DateTime LastCloneJump
        {
            get
            {
                return lastCloneJump;
            }
        }

        [DataMember(Name = "last_clone_jump_date", EmitDefaultValue = false, IsRequired = false)]
        private string LastCloneJumpJson
        {
            get
            {
                return lastCloneJump.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    lastCloneJump = value.TimeStringToDateTime();
            }
        }

        // Returns the last date the home station was changed, in UTC
        [IgnoreDataMember]
        public DateTime LastStationChange
        {
            get
            {
                return lastStationChange;
            }
        }

        [DataMember(Name = "last_station_change_date", EmitDefaultValue = false, IsRequired = false)]
        private string LastStationChangeJson
        {
            get
            {
                return lastStationChange.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    lastStationChange = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "jump_clones")]
        public List<EsiJumpCloneListItem> JumpClones { get; set; }
    }
}
