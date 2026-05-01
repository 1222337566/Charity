namespace InfrastructureManagmentCore.Domains.Complains
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using InfrastructureManagmentCore.Domains.Profiling;
    //using System.Data.Entity.Spatial;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("Transaction2")]
    public partial class Transaction2
    {

        [Key]
        [Column("Trans id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Trans_id { get; set; }
        [Column("stocktake id")]
        public int? stocktake_id { get; set; }

        [Column("name of operation", TypeName = "text")]
        [Required]
        public string description { get; set; }

        [Column(TypeName = "text")]
        public string affectedobject { get; set; }

        [Column(TypeName = "text")]
        public string Previous { get; set; }

        [Column(TypeName = "text")]
        public string Current { get; set; }

        [Column(TypeName = "text")]
        public string Change { get; set; }
        public int idOO { get; set; }
        public string NOO { get; set; }

        public DateTime date { get; set; }
        [ForeignKey("stocktake_id")]
        public virtual Stocktaking stocktake { get; set; }
        public int? Employerid { get; set; }

        public int? Personalid { get; set; }
        public int? Compid { get; set; }

        public int? Groupid { get; set; }
        [ForeignKey("Groupid")]
        public virtual Group Group { get; set; }
        [ForeignKey("Compid")]
        public virtual Company Company { get; set; }

        [ForeignKey("Employerid")]
        public virtual Employee Employer { get; set; }

        [ForeignKey("Personalid")]
        public virtual PersonalInformation personal { get; set; }
    }
}
