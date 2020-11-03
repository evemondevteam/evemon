namespace EVEMon.XmlGenerator.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class agtAgents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int agentID { get; set; }

        public int? divisionID { get; set; }

        public int? corporationID { get; set; }

        public int? locationID { get; set; }

        public int? level { get; set; }

        public int? quality { get; set; }

        public int? agentTypeID { get; set; }

        public bool? isLocator { get; set; }
    }
}
