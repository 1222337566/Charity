//using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using System.Runtime.InteropServices;
//using WebGrease.Activities;


namespace InfrastructureManagmentCore.Domains.supplies
{
    [Table("InternetLeasedLine")]
    public class InternetLeasedLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lineid { get; set; }
        public string Name { get; set; }

        public string description { get; set; }

        public string phonenumber { get; set; }

        public string Orderid { get; set; }

        public string speed { get; set; }

        public string package { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IPAdress> IPs { get; set; }

    }
}