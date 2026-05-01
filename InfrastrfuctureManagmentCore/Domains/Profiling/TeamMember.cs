namespace InfrastructureManagmentCore.Domains.Profiling
{
    using InfrastructureManagmentCore.Domains.Reactions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   // using System.Data.Entity.Spatial;

    public partial class TeamMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeamMemberid { get; set; }

        public string TeamMemberName { get; set; }

        public int? teamid { get; set; }

        public int? employee_id { get; set; }

        public int? role_id { get; set; }

        public int? Tasks_Taskid { get; set; }
        [ForeignKey("employee_id")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("role_id")]
        public virtual Role Role { get; set; }
        [ForeignKey("Tasks_Taskid")]
        public virtual InfrastructureManagmentCore.Domains.Complains.Task Task { get; set; }
        [ForeignKey("teamid")]
        public virtual Team Team { get; set; }
    }
}
