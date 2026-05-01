namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using InfrastructureManagmentCore.Domains.Profiling;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;
    //using MaintainanceSystem.Models;
   // using MaintainanceSystemWebService.Models;

    [Table("Bill Products")]
    public partial class Bill_Product
    {
        [Key]
        [Column("BillPro id",Order =0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillPro_id { get; set; }

        [Column("Bill id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Bill_id { get; set; }

        
        [Column("Product id", Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Product_id { get; set; }

        
        [Column(Order = 3)]
        public double amount { get; set; }
        
        [Column(Order = 4)]
        public string unit { get; set; }
        public int? addproid { get; set; }
        public string status { get; set; }
        public double? unitprice { get; set; }
        public double? total { get; set; }
        public string descrip { get; set; }
        [ForeignKey("Product_id")]
        public virtual Product Product { get; set; }
        [ForeignKey("Bill_id")]
        public virtual Purchses_Bill Purchses_Bill { get; set; }
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
