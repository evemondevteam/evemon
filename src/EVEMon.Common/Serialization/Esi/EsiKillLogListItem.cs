using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiKillLogListItem
    {
        [DataMember(Name = "killmail_id")]
        public int KillID { get; set; }

        [DataMember(Name = "killmail_hash")]
        public string Hash { get; set; }
    }
}
