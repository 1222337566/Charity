

namespace InfrastructureManagmentCore.Domains.Reactions
{
    using InfrastructureManagmentCore.Domains.Documents;
    using InfrastructureManagmentCore.Domains.Profiling;
    //using ContosoUniversity.Models;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;
    [Table("Notification")]
    public partial class Notification
    {
        [Key]
        [Column("Notify id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Notify_id { get; set; }

        [Column(TypeName = "text")]
        public string title { get; set; }
        [Column("name of operation", TypeName = "text")]
        [Required]
        public string description { get; set; }

        [Column(TypeName = "text")]
        public string affectedobject { get; set; }

        public string Type { get; set; }
        public string Sender { get; set; }

        public string Receiver { get; set; }

        public Boolean Read { get; set; }
        public DateTime date { get; set; }

        public string ContentType { get; set; }

        public byte[] ContentData { get; set; }


        public FileType FileType { get; set; }
        public int? Extension { get; set; }
        public int? Employerid { get; set; }

        public int? Personalid { get; set; }
        
        [ForeignKey("Employerid")]
        public virtual Employee Employer { get; set; }

        [ForeignKey("Personalid")]
        public virtual PersonalInformation personal { get; set; }
    }

}