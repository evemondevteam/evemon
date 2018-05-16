using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPICharacterNames : List<EsiCharacterNameListItem>
    {
        public SerializableAPICharacterName ToXMLItem()
        {
            var ret = new SerializableAPICharacterName();
            foreach (var namePair in this)
                ret.Entities.Add(new SerializableCharacterNameListItem()
                {
                    Name = namePair.Name,
                    ID = namePair.ID
                });
            return ret;
        }
    }
}
