using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfrastructureManagmentCore.Domains.Profiling;

//using System.Data.Entity.Spatial;
using MaintainanceSystem.Models;
//using MaintainanceSystemWebService.Models;

namespace InfrastructureManagmentCore.Domains.Reactions
{
    [Table("Event")]
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public bool AllDays { get; set; }
        public string className { get; set; }

        public string icon { get; set; }

        public string loginid { get; set; }

        //public string group { get; set; }

        public int? Employerid { get; set; }

        public int? Personalid { get; set; }
        public int? Compid { get; set; }

        public int? Groupid { get; set; }
        [ForeignKey("Groupid")]
        public virtual Group Group { get; set; }
        [ForeignKey("Compid")]
        public virtual Company Company { get; set; }

        [ForeignKey("Employerid")]
        public virtual Employee Employer { get; set; }

        [ForeignKey("Personalid")]
        public virtual PersonalInformation personal { get; set; }
    }
}