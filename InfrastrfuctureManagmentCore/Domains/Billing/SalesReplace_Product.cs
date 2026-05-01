namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("SalesReplace Products")]
    public partial class SalesReplace_Product
    {
        [Key]
        [Column("ReplacePro id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReplacePro_id { get; set; }

        [Column("replace id", Order = 1)]

        public int replace_id { get; set; }


        [Column("Product id", Order = 2)]

        public int Product_id { get; set; }


        [Column(Order = 3)]

        public double amount { get; set; }
        [Column("invent id")]
        public int? invent_id { get; set; }
        [Column("exhib id")]
        public int? exhib_id { get; set; }
        [Column(TypeName = "text")]
        public string reason { get; set; }
        public string unit { get; set; }
        public double? unitprice { get; set; }
        public double? total { get; set; }
        public string status { get; set; }

        [ForeignKey("Product_id")]
        public virtual Product Product { get; set; }
        [ForeignKey("replace_id")]
        public virtual Sales_Replace Sales_Replace { get; set; }
        [ForeignKey("invent_id")]
        public virtual invent invent { get; set; }
        [ForeignKey("exhib_id")]
        public virtual exhib exhib { get; set; }
        
    }
}
