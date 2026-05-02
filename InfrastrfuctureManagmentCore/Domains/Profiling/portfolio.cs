using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;


namespace InfrastructureManagmentCore.Domains.Profiling
{
    public class portfolio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string username { get; set; }
        public string Firstname { get; set; }
        public string Domainuser { get; set; }

    }
}