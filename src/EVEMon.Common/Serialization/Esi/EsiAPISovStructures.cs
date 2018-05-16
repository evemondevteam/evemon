using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPISovStructures : List<EsiSovStructureListItem>
    {
        // No equivalent in XML API, this is used to determine conquerable station info
    }
}
