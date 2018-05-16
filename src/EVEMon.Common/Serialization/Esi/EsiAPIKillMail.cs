using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIKillMail
    {
        private DateTime killTime;

        public EsiAPIKillMail()
        {
            killTime = DateTime.MinValue;
        }

        [DataMember(Name = "killmail_id")]
        public int KillID { get; set; }

        [DataMember(Name = "killmail_time")]
        private string KillTimeJson
        {
            get
            {
                return killTime.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    killTime = value.TimeStringToDateTime();
            }
        }

        [DataMember(Name = "solar_system_id")]
        public int SolarSystemID { get; set; }

        [DataMember(Name = "moon_id", EmitDefaultValue = false, IsRequired = false)]
        public int MoonID { get; set; }

        [DataMember(Name = "war_id", EmitDefaultValue = false, IsRequired = false)]
        public long WarID { get; set; }

        [DataMember(Name = "victim")]
        public EsiKillLogVictim Victim { get; set; }

        [DataMember(Name = "attackers")]
        public Collection<EsiKillLogAttackersListItem> Attackers { get; set; }

        [IgnoreDataMember]
        public DateTime KillTime
        {
            get
            {
                return killTime;
            }
        }

        public SerializableKillLogListItem ToXMLItem()
        {
            var ret = new SerializableKillLogListItem()
            {
                KillID = KillID,
                KillTime = KillTime,
                MoonID = MoonID,
                SolarSystemID = SolarSystemID,
                Victim = Victim?.ToXMLItem()
            };

            // Attackers
            if (Attackers != null)
                foreach (var attacker in Attackers)
                    ret.Attackers.Add(attacker.ToXMLItem());

            // Items
            if (Victim != null)
            {
                var items = Victim.Items;
                if (items != null)
                {
                    foreach (var item in items)
                        ret.Items.Add(item.ToXMLItem());
                }
            }

            return ret;
        }
    }
}
