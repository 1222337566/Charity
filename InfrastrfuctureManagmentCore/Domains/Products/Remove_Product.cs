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

    [Table("Remove Product")]
    public partial class Remove_Product
    {
        [Key]
        [Column(" RemovePro id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RemovePro_id { get; set; }

        [Column("remove id", Order = 1)]
       
        public int remove_id { get; set; }

        
        [Column("product id", Order = 2)]
       
        public int product_id { get; set; }

        
        [Column(Order = 3)]
        public double amount { get; set; }
        
        [Column(Order = 4)]
        public string Delivery_status { get; set; }
       
        [Column(Order = 5)]
        public DateTime Delivery_Date { get; set; }

        
        
        
        [Column(Order = 6)]
        public string unit { get; set; }
        [Column(Order = 7)]
        public int? invent_id  { get; set; }
        [Column(Order = 8)]
        public int? exhib_id { get; set; }
        [Column(Order = 9)]
        public string descrip { get; set; }

        public string status { get; set; }

        [ForeignKey("invent_id")]

        public virtual invent invernt { get; set; }
        [ForeignKey("exhib_id")]
        public virtual exhib exhib { get; set; }
        [ForeignKey("product_id")]
        
        public virtual Product Product { get; set; }
        [ForeignKey("remove_id")]
        public virtual Remove_Receipt Remove_Receipt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Identity> identites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<missingitem> missitems { get; set; }
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
