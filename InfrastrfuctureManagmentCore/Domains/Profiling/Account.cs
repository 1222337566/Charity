//using FastOrderFinalVersion.Entities_Layers;
using InfrastructureManagmentCore.Domains.Billing;
using InfrastructureManagmentCore.Domains.Products;
using MaintainanceSystem.Models;
//using Retail_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Profiling
{
    [Table("Account")]
    public partial class Account
    {
        [Key]
        [Column("Account ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Account_ID { get; set; }
        public string title { get; set; }
        public string descrip { get; set; }
        public double? Balance { get; set; }
        public double? TotalExpenses { get; set; }
        public double? TotalIncome { get; set; }
        public double? PreviousBalalnce { get; set; }
        //public virtual Customer Customer { get; set; }
        public double? FirstBalance { get; set; }
        public double? FinalBalance { get; set; }
        //public virtual Importer importer { get; set; }
        public string status { get; set; }

        public int? TreasuryId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury  Treasury { get; set; }
        public int? Employeeid { get; set; }
        [ForeignKey("Employeeid")]
        public virtual Employee Employee { get; set; }
        //  public virtual Employee Employer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expense> Expenses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Income> Incomes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DebitTrans> DebitTrans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayedTrans> PayedTrans { get; set; }
    }
}
