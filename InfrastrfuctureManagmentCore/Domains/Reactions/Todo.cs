//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
namespace InfrastructureManagmentCore.Domains.Reactions
{
    [Table("ToDo")]
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ToDo ID")]
        public int todo_id { get; set; }

        public string title  { get; set; }
        public  int importance { get; set; } 
        public int status { get; set; }

        public string type { get; set; }

        public string description { get; set; }
        public int? personalid { get; set; }

        public int? groupid { get; set; }
        public string time  { get; set; }

        public int lasttransaction { get; set; }
        public string endtime { get; set; }

        public string details { get; set; }

        public int? senderid { get; set; }
        public int? solverid { get; set; }
        [ForeignKey("senderid")]
        public virtual Employee employer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [ForeignKey("groupid")]
        public virtual Group group { get; set; }
        [ForeignKey("personalid")]
        public virtual PersonalInformation Personals { get; set; }
    }
}