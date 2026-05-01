//using ContosoUniversity.Models;
//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Documents;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;


namespace InfrastructureManagmentCore.Domains.Reactions
{
    [Table("Post")]
    public class Post
    {
        [Key]
        [Column("Post ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Post_ID { get; set; }

        public int? senderid { get; set; }

        public int? personalid { get; set; }

        public int? groupid { get; set; }
        public string text { get; set;}

        public DateTime time { get; set; }

        public string ContentType { get; set; }

        public byte[] ContentData { get; set; }

        public FileType FileType { get; set; }
        public string Extension { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [ForeignKey("senderid")]
        public virtual Employee employer { get; set; }
        [ForeignKey("groupid")]
        public virtual Group group { get; set; }
        [ForeignKey("personalid")]
        public virtual PersonalInformation Personals { get; set; }
    }
}