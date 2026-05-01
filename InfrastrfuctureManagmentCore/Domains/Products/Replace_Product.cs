namespace InfrastructureManagmentCore.Domains.Products
{
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Profiling;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;
    //using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("Replace Products")]
    public partial class Replace_Product
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
        public virtual Purchases_Replace Purchases_Replace { get; set; }
        [ForeignKey("invent_id")]
        public virtual invent invent { get; set; }
        [ForeignKey("exhib_id")]
        public virtual exhib exhib { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Identity> identites { get; set; }
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
