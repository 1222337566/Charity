namespace InfrastructureManagmentCore.Domains.Projects
{
    using InfrastructureManagmentCore.Domains.Documents;
    using InfrastructureManagmentCore.Domains.Profiling;
    using InfrastructureManagmentCore.Domains.Reactions;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            Milestones = new HashSet<Milestone>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int projectid { get; set; }

        public string projectName { get; set; }

        public string Description { get; set; }

        public DateTime Planning_StartDate { get; set; }

        public DateTime Planning_StopDate { get; set; }

        public DateTime Actual_StartDate { get; set; }

        public DateTime Actual_StopDate { get; set; }

        public double Hourly_Rate { get; set; }

        public double budget { get; set; }

        public int Active { get; set; }

        public double TOtalDayes { get; set; }

        public double Labor_cost { get; set; }

        public double MaterialCosts { get; set; }

        public int? personalid { get; set; }
        public double TotalCost { get; set; }

        public int? companyid { get; set; }

        public int? statusid { get; set; }

        public int? teamid { get; set; }
        [ForeignKey("companyid")]
        public virtual Company Company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Milestone> Milestones { get; set; }
        [ForeignKey("statusid")]
        public virtual Status Status { get; set; }
        [ForeignKey("teamid")]
        public virtual Team Team { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IList<Document> Documents { get; set; }
        [ForeignKey("personalid")]
        public virtual PersonalInformation Personals { get; set; }
    }
}
