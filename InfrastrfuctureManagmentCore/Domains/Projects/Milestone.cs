namespace InfrastructureManagmentCore.Domains.Projects
{
    using InfrastructureManagmentCore.Domains.Documents;
    using InfrastructureManagmentCore.Domains.Reactions;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Milestone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Milestone()
        {
            Tasks = new HashSet<InfrastructureManagmentCore.Domains.Complains.Task>();
            Costs = new List<Cost>() { };
            Deliverables = new List<Deliverable>() { };

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Milestoneid { get; set; }

        public string MilestoneName { get; set; }

        public string Description { get; set; }

        public DateTime Planning_StartDate { get; set; }

        public DateTime Planning_StopDate { get; set; }

        public DateTime Actual_StartDate { get; set; }

        public DateTime Actual_StopDate { get; set; }

        public int? statusid { get; set; }

        public int? objectiveid { get; set; }
        


        public int? Project_projectid { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<Cost> Costs { get; set; }
        [ForeignKey("objectiveid")]
        public virtual Objective Objective { get; set; }
        [ForeignKey("Project_projectid")]
        public virtual Project Project { get; set; }
        [ForeignKey("statusid")]
        public virtual Status Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InfrastructureManagmentCore.Domains.Complains.Task> Tasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<Deliverable> Deliverables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<Document> Documents { get; set; }
    }
}
