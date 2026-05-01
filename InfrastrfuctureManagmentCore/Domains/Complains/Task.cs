namespace InfrastructureManagmentCore.Domains.Complains
{
    using InfrastructureManagmentCore.Domains.Profiling;
    using InfrastructureManagmentCore.Domains.Projects;
    using InfrastructureManagmentCore.Domains.Reactions;
    using InfrastructureManagmentCore.Domains.Requests;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            Hours = new HashSet<Hour>();
            TeamMembers = new HashSet<TeamMember>();
            //TroubleTickets = new HashSet<TroubleTicket>();
            Obstackles = new HashSet<Obstackle>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Taskid { get; set; }
        public int? Tickid { get; set; }
        public string Name { get; set; }

        public string description { get; set; }

        public bool resolved { get; set; }
        public DateTime Planning_StartDate { get; set; }

        public DateTime Planning_StopDate { get; set; }

        public DateTime Actual_StartDate { get; set; }

        public DateTime Actual_StopDate { get; set; }

        public string status { get; set; }

        public int? employeeid { get; set; }

        public int? milestoneid { get; set; }

        public virtual Employee Employee { get; set; }
        public int? Reqid { get; set; }

        [ForeignKey("Reqid")]
        public virtual Request Request { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hour> Hours { get; set; }

        public virtual Milestone Milestone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeamMember> TeamMembers { get; set; }

        [ForeignKey("Tickid")]
        public virtual TroubleTicket TroubleTickets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Instruction> Instructions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Obstackle> Obstackles { get; set; }
    }
}
