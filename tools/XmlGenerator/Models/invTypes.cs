namespace EVEMon.XmlGenerator.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class invTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int typeID { get; set; }

        public int? groupID { get; set; }

        [StringLength(100)]
        public string typeName { get; set; }

        [StringLength(3000)]
        public string description { get; set; }

        public double? mass { get; set; }

        public double? volume { get; set; }

        public double? capacity { get; set; }

        public int? portionSize { get; set; }

        public int? raceID { get; set; }

        [Column(TypeName = "DECIMAL")]
        public decimal? basePrice { get; set; }

        public bool? published { get; set; }

        public int? marketGroupID { get; set; }

        public int? graphicID { get; set; }

        public int? iconID { get; set; }

        public int? soundID { get; set; }
    }
}
