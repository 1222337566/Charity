using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Reactions
{
    public class Obstackle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObsId { get; set; }

        public string Obs_Name { get; set; }
        public string Obs_Descrip { get; set; }

        public DateTime ObsDate { get; set; }

        public string solu_Name { get; set; }
        public string solu_Descrip { get; set; }

        public DateTime soluDate { get; set; }
        public string status { get; set; }

        public int? Taskid { get; set; }

        [ForeignKey("Taskid")]
        public virtual InfrastructureManagmentCore.Domains.Complains.Task Task { get; set; }

    }
}