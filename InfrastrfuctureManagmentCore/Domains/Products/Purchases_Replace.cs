namespace InfrastructureManagmentCore.Domains.Products
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Profiling;
    //using System.Data.Entity.Spatial;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("Purchases Replace")]
    public partial class Purchases_Replace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Purchases_Replace()
        {
            Replace_Products = new HashSet<Replace_Product>();
        }

        [Key]
        [Column("Replace id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Replace_id { get; set; }

        [Column("importer id")]
        public int importer_id { get; set; }

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
        [ForeignKey("importer_id")]
        public virtual Importer importer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Replace_Product> Replace_Products { get; set; }
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
