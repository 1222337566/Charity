using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace MaintainanceSystem.Models
{
    public class Instruction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Instruction ID")]
        public int Instruction_ID { get; set; }

        [Column("Instruction Name")]
        [Required]
        [StringLength(50)]
        public string Instruction_Name { get; set; }

        public int? TaskId { get; set; }


        [Column(TypeName = "text")]
        public string Notes { get; set; }



        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

    }
}