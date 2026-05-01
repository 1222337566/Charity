using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Projects
{
    public class Deliverable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Deliverable ID")]
        public int Deliverable_ID { get; set; }

        [Column("Deliverable Name")]
        [Required]
        [StringLength(50)]
        public string Deliverable_Name { get; set; }

        public int? MilestoneId { get; set; }


        [Column(TypeName = "text")]
        public string Notes { get; set; }
        


        [ForeignKey("MilestoneId")]
        public virtual Milestone milestone { get; set; }

    }
}