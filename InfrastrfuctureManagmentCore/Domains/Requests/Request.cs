//using MaintainanceSystemWebService.Models;
using InfrastructureManagmentCore.Domains.Complains;
using InfrastructureManagmentCore.Domains.Documents;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Requests
{
    [Table("Request")]
    public class Request
    {
       public Request()
        {
            Documents = new List<Document>();
            
        }
   

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Request ID")]
        public int Request_ID { get; set; }

        [Column("Request Name")]
        public string Request_Name { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string status { get; set; }
        public int? employerid { get; set; }

        public int? personalid { get; set; }

        public int? groupid { get; set; }

        public int? Depid { get; set; }

        [ForeignKey("Depid")]
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TroubleTicket> TroubleTickets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Complains.Task> Tasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestResponse> RequestResponse { get; set; }
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