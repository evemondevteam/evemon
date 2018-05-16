using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiCharacterNameListItem
    {
        [DataMember(Name = "id")]
        public long ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
