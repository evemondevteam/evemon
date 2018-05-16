using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIResearchPoints : List<EsiResearchListItem>
    {
        public SerializableAPIResearch ToXMLItem()
        {
            var ret = new SerializableAPIResearch();
            foreach (var agent in this)
                ret.ResearchPoints.Add(agent.ToXMLItem());
            return ret;
        }
    }
}
