namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Sales Replace")]
    public partial class Sales_Replace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sales_Replace()
        {
            SalesReplace_Products = new HashSet<SalesReplace_Product>();
        }

        [Key]
        [Column("Replace id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Replace_id { get; set; }

        [Column("Customer id")]
        public int? Customer_id { get; set; }

        public DateTime date { get; set; }
        public double previousbalance { get; set; }
        public double finalbalance { get; set; }
        // public double total { get; set; }

        [Column(TypeName = "text")]
        public string Reason { get; set; }

        public double? discount { get; set; }
        public int? code { get; set; }
        public double? total { get; set; }
        public double? subtotal { get; set; }
        [Column("Descrip", TypeName = "text")]
        public string descrip { get; set; }
        [ForeignKey("Customer_id")]
        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesReplace_Product> SalesReplace_Products { get; set; }
        public int? comid { get; set; }
        [ForeignKey("comid")]
        public virtual Company Company { get; set; }

    }
}
