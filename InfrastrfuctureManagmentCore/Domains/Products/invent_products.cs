namespace InfrastructureManagmentCore.Domains.Products
{
    using InfrastructureManagmentCore.Domains.Billing;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("invent products")]
    public partial class invent_product
    {
        [Key]
        [Column("invent id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int invent_id { get; set; }

        
        [Column("Product id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Product_id { get; set; }
        public double previousbalance { get; set; }
        public double finalbalance { get; set; }
        
        [Column(Order = 2)]
        public double amount { get; set; }
        
        [Column(Order = 3)]
        public DateTime Date { get; set; }
        
        [Column(Order = 4)]
        public string Unit { get; set; }
        public double? previousunit { get; set; }
        public double? finalunit { get; set; }
        public double? previouscartoon { get; set; }
        public double? finalcartoon { get; set; }
        public double? previousroll { get; set; }
        public double? finalroll { get; set; }
        public double? goodunit { get; set; }
        public double? goodcartoon { get; set; }
        public double? goodroll { get; set; }

        public double? badunit { get; set; }
        public double? badcartoon { get; set; }
        public double? badroll { get; set; }
        public double? usedunit { get; set; }
        public double? usedcartoon { get; set; }
        public double? usedroll { get; set; }
        [ForeignKey("Product_id")]
        public virtual Product Product { get; set; }
        [ForeignKey("invent_id")]
        public virtual invent invent { get; set; }
    }
}
