namespace InfrastructureManagmentCore.Domains.Reactions
{
    using InfrastructureManagmentCore.Domains.Complains;
    using InfrastructureManagmentCore.Domains.Projects;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Objective
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Objective()
        {
            Milestones = new HashSet<Milestone>();
            //TroubleTickets = new HashSet<TroubleTicket>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int objectiveid { get; set; }
        public int? Tickid { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public bool? Resolved { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Milestone> Milestones { get; set; }

        [ForeignKey("Tickid")]
        public virtual TroubleTicket TroubleTicket { get; set; }
    }
}
