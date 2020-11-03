using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents planet information.
    /// </summary>
    [DataContract]
    public class EsiAPIPlanet
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "planet_id")]
        public int PlanetID { get; set; }

        [DataMember(Name = "position")]
        public EsiPosition Position { get; set; }

        [DataMember(Name = "system_id")]
        public int SystemID { get; set; }

        [DataMember(Name = "type_id")]
        public int TypeID { get; set; }
    }
}
