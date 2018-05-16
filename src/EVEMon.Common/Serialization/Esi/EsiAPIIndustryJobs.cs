using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIIndustryJobs : List<EsiJobListItem>
    {
        public SerializableAPIIndustryJobs ToXMLItem()
        {
            var ret = new SerializableAPIIndustryJobs();
            foreach (var job in this)
                ret.Jobs.Add(job.ToXMLItem());
            return ret;
        }
    }
}
