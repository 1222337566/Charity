//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Documents;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Requests
{
    [Table("RequestResponse")]
    public class RequestResponse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RequestResponse()
        {
            Documents = new List<Document>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RequestRes ID")]
        public int RequestRes_ID { get; set; }

        public int? employerid { get; set; }

        public int? personalid { get; set; }

        public int? groupid { get; set; }
        [Column("RequestRes Name")]
        public string RequestRes_Name { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public DateTime Date { get; set; }
        public string status { get; set; }
        public string Type { get; set; }
        public int? Depid { get; set; }
        [ForeignKey("Depid")]
        public virtual Department Department { get; set; }
        public int? Reqid { get; set; }
        [ForeignKey("Reqid")]
        public virtual Request Request { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<Document> Documents { get; set; }
        [ForeignKey("employerid")]
        public virtual Employee employer { get; set; }
        [ForeignKey("groupid")]
        public virtual Group group { get; set; }
        [ForeignKey("personalid")]
        public virtual PersonalInformation Personals { get; set; }

    }
}