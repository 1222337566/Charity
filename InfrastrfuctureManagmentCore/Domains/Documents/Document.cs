//using ContosoUniversity.Models;
using InfrastructureManagmentCore.Domains.Projects;
using InfrastructureManagmentCore.Domains.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using System.Runtime.InteropServices;
//using WebGrease.Activities;

namespace InfrastructureManagmentCore.Domains.Documents
{
    [Table("Document")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Docid { get; set; }

        public string FileName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }
        public string ContentType { get; set; }

        public byte[] ContentData { get; set; }

        public FileType FileType { get; set; }
        public string  Extension { get; set; }
        public int? Miloid { get; set; }
        [ForeignKey("Miloid")]
        public virtual Milestone Milestone { get; set; }
        public int? proid { get; set; }
        [ForeignKey("proid")]
        public virtual Project Project { get; set; }
        public int? reqid { get; set; }
        [ForeignKey("reqid")]
        public virtual Request Request { get; set; }
        public int? ReqResid { get; set; }
        [ForeignKey("ReqResid")]
        public virtual RequestResponse RequestResponse { get; set; }
    }
}