using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIType
    {
        [DataMember(Name = "type_id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "published")]
        public bool Published { get; set; }

        [DataMember(Name = "group_id")]
        public int GroupID { get; set; }

        [DataMember(Name = "market_group_id", EmitDefaultValue = false, IsRequired = false)]
        public int MarketGroupID { get; set; }

        [DataMember(Name = "radius", EmitDefaultValue = false, IsRequired = false)]
        public double Radius { get; set; }

        [DataMember(Name = "volume", EmitDefaultValue = false, IsRequired = false)]
        public double Volume { get; set; }

        [DataMember(Name = "packaged_volume", EmitDefaultValue = false, IsRequired = false)]
        public double PackagedVolume { get; set; }

        [DataMember(Name = "icon_id", EmitDefaultValue = false, IsRequired = false)]
        public int IconID { get; set; }

        [DataMember(Name = "capacity", EmitDefaultValue = false, IsRequired = false)]
        public double Capacity { get; set; }

        [DataMember(Name = "portion_size", EmitDefaultValue = false, IsRequired = false)]
        public int PortionSize { get; set; }

        [DataMember(Name = "mass", EmitDefaultValue = false, IsRequired = false)]
        public double Mass { get; set; }

        [DataMember(Name = "graphic_id", EmitDefaultValue = false, IsRequired = false)]
        public int GraphicID { get; set; }

        // DOGMA information is also returned here, but there is no need for it in EVEMon
    }
}
