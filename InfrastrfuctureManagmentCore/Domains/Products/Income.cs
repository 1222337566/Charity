using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfrastructureManagmentCore.Domains.Profiling;

//using System.Data.Entity.Spatial;
using MaintainanceSystem.Models;

namespace InfrastructureManagmentCore.Domains.Products
{
    [Table("Income")]
    public partial class Income
    {
        [Key]
        [Column("Expense ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Incomee_ID { get; set; }
        public string title { get; set; }
        public string descrip { get; set; }
        public double? Amount { get; set; }
        public int? Type { get; set; }

        public DateTime? Time { get; set; }
        public string Description { get; set; }
        public int? Employeeid { get; set; }
        [ForeignKey("Employeeid")]
        public virtual Employee Employee { get; set; }
        public string status { get; set; }

        public int? TreasuryId { get; set; }
       // public int? EmployerId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury Treasury { get; set; }
        public int? AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        //[ForeignKey("EmployerId")]
        //public virtual Employee Employer { get; set; }
       
    }
}
