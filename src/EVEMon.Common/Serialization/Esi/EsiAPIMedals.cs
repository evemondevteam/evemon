using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIMedals : List<EsiMedalsListItem>
    {
        public SerializableAPIMedals ToXMLItem()
        {
            var ret = new SerializableAPIMedals();
            foreach (var medal in this)
                ret.CorporationMedals.Add(medal.ToXMLItem());
            return ret;
        }
    }
}
