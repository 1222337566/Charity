namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Sales Bill")]
    public partial class Sales_Bill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sales_Bill()
        {
            Remove_Receipt = new HashSet<Remove_Receipt>();
            SalesBill_Products = new HashSet<SalesBill_Product>();
        }

        [Key]
        [Column("Bill id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Bill_id { get; set; }

        [Column("Customer id")]
        public int? Customer_id { get; set; }
        public double previousbalance { get; set; }
        public double finalbalance { get; set; }
        public DateTime date { get; set; }

        public int? code { get; set; }
        public double? discount { get; set; }
        public double total { get; set; }
        public double subtotal { get; set; }
        [Column("Descrip", TypeName = "text")]
        public string descrip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Receipt> Remove_Receipt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesBill_Product> SalesBill_Products { get; set; }
        [ForeignKey("Customer_id")]
        public virtual Customer Customer { get; set; }
        public int? comid { get; set; }
        [ForeignKey("comid")]
        public virtual Company Company { get; set; }

    }
}
