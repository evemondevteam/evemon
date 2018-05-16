using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public class EsiAPISkills
    {
        public EsiAPISkills()
        {
            UnallocatedSP = 0;
        }

        [DataMember(Name = "total_sp")]
        public int TotalSP { get; set; }

        [DataMember(Name = "unallocated_sp")]
        public int UnallocatedSP { get; set; }

        [DataMember(Name = "skills")]
        public List<EsiSkillListItem> Skills { get; set; }
    }
}
