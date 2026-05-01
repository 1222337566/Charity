namespace InfrastructureManagmentCore.Domains.Complains
{
    using InfrastructureManagmentCore.Domains.Messages;
    using InfrastructureManagmentCore.Domains.Profiling;
    using InfrastructureManagmentCore.Domains.Reactions;
    using InfrastructureManagmentCore.Domains.Requests;
    using InfrastructureManagmentCore.Domains.supplies;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class TroubleTicket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TroubleTicket()
        {
            Comments = new HashSet<Comment>();
            Objectives = new HashSet<Objective>();
            Tasks = new HashSet<Task>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Ticketid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime StopTime { get; set; }

        public int? issuerUserID { get; set; }

        public TimeSpan Period { get; set; }

        public int? statusid { get; set; }

        public int? depid { get; set; }

        public int? personalid { get; set; }

        public int? groupid { get; set; }
        public int? typid { get; set; }

        public int? syscomid { get; set; }

        public long? catid { get; set; }
        public int? Reqid { get; set; }

        [ForeignKey("Reqid")]
        public virtual Request Request { get; set; }
        [ForeignKey("catid")]
        public virtual Category Category { get; set; }
        [ForeignKey("depid")]
        public virtual Department Department { get; set; }
        [ForeignKey("issuerUserID")]
        public virtual Employee Employer { get; set; }
        [ForeignKey("statusid")]
        public virtual Status Status { get; set; }
        [ForeignKey("syscomid")]
        public virtual SystemComponent syscom { get; set; }
        [ForeignKey("typid")]
        public virtual InfrastructureManagmentCore.Domains.Requests.Type type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Objective> Objectives { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Tasks { get; set; }
        [ForeignKey("groupid")]
        public virtual Group group { get; set; }
        [ForeignKey("personalid")]
        public virtual PersonalInformation Personals { get; set; }
    }
}
