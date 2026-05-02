namespace InfrastructureManagmentCore.Domains.Reactions
{
    using InfrastructureManagmentCore.Domains.Complains;
    using InfrastructureManagmentCore.Domains.Profiling;
    using InfrastructureManagmentCore.Domains.Projects;
    using InfrastructureManagmentCore.Domains.supplies;
    //using MaintainanceSystem.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int commentid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
        public int? todoid { get; set; }
        public int? Complainid { get; set; }
        public int? userid { get; set; }
        public int? employerid { get; set; }
        public int? Miloid { get; set; }
        public int? postid { get; set; }
        public int? Tickid { get; set; }
        public int? Personalid { get; set; }
        [ForeignKey("Personalid")]
        public virtual PersonalInformation personal { get; set; }
        [ForeignKey("employerid")]
        public virtual Employee employer { get; set; }
        public int? Taskid { get; set; }
        [ForeignKey("todoid")]
        public virtual Todo todo { get; set;}
        [ForeignKey("Complainid")]
        public virtual Complain Complain { get; set; }
        [ForeignKey("Taskid")]
        public virtual Task Task { get; set; }
        [ForeignKey("userid")]
        public virtual DomainUser User  { get; set; }
        [ForeignKey("postid")]
        public virtual Post post { get; set; }
        [ForeignKey("Miloid")]
        public virtual Milestone Milestone { get; set; }

        [ForeignKey("Tickid")]
        public virtual TroubleTicket TroubleTicket { get; set; }
    }
}
