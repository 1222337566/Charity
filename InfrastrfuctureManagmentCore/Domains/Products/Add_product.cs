namespace InfrastructureManagmentCore.Domains.Products
{
    using System;
    using MaintainanceSystem.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Profiling;

    //using System.Data.Entity.Spatial;
    //using MaintainanceSystemWebService.Models;

    [Table("Add product")]
    public partial class Add_product
    {
        [Key]
        [Column("AddPro id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddPro_id { get; set; }
        
        [Column("add id", Order = 1)]
        
        public int? add_id { get; set; }

        
        [Column("Product id", Order = 2)]
        
        public int Product_id { get; set; }

        
        [Column(Order = 3)]
        public double amount { get; set; }
        
        [Column(Order = 4)]
        public string unit { get; set; }
        [Column(Order = 5)]
        public string descrip { get; set; }
        public string status { get; set; }
        [ForeignKey("add_id")]
        public virtual Add_Receipt Add_Receipt { get; set; }
        [ForeignKey("Product_id")]
        public virtual Product Product { get; set; }
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
