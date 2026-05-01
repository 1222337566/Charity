namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("SalesBill Products")]
    public partial class SalesBill_Product
    {
        [Key]
        [Column("BillPro id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalesBillPro_id { get; set; }

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
        public virtual Sales_Bill Saleses_Bill { get; set; }
    }
}
