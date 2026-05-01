//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentCore.Domains.supplies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Complains
{
    [Table("Complain")]
    public class Complain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Complain ID")]
        public int Complain_id { get; set; }

        public string title { get; set; }
        public int importance { get; set; }
        public int status { get; set; }

        public string type { get; set; }

        public string description { get; set; }

        public string time { get; set; }

        public int lasttransaction { get; set; }
        public string endtime { get; set; }

        public string details { get; set; }

        public int? senderid { get; set; }
        public int? solverid { get; set; }
        [ForeignKey("solverid")]
        public virtual Employee employer { get; set; }
        [ForeignKey("senderid")]
        public virtual DomainUser User { get; set; }
        public int? personald { get; set; }
        [ForeignKey("personald")]
        public virtual PersonalInformation personal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
