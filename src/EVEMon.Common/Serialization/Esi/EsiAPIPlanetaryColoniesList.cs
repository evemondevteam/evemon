using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIPlanetaryColoniesList : List<EsiPlanetaryColonyListItem>
    {
        public SerializableAPIPlanetaryColonies ToXMLItem()
        {
            var ret = new SerializableAPIPlanetaryColonies();
            foreach (var colony in this)
                ret.Colonies.Add(colony.ToXMLItem());
            return ret;
        }
    }
}
