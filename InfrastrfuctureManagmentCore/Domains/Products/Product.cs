namespace InfrastructureManagmentCore.Domains.Products
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Profiling;
    //using System.Data.Entity.Spatial;
    //using System.Web.UI.WebControls;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Add_product = new HashSet<Add_product>();
           Bill_Products = new HashSet<Bill_Product>();
           Remove_Product = new HashSet<Remove_Product>();
            Replace_Products = new HashSet<Replace_Product>();
            identity_product = new HashSet<identity_products>();
        }

        [Key]
        [Column("Product ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Product_ID { get; set; }

        [Column("product Name", TypeName = "text")]
        
        public string product_Name { get; set; }

        [Column(TypeName = "text")]
        
        public string Descrition { get; set; }
        [Column(TypeName = "text")]
        
        public string Brand { get; set; }

        [Column(TypeName = "text")]
        
        public string Model { get; set; }

        public double? length { get; set; }

        public double? width { get; set; }

        public double? Height { get; set; }
        public double? amount { get; set; }
        [Column(TypeName = "text")]
        
        public string Type { get; set; }

        [Column(TypeName = "text")]
        
        public string Category { get; set; }


        [Column("buy price")]
        public double? buy_price { get; set; }
        public double? total { get; set; }
        public double? cartoon { get; set; }

        public double? Roll { get; set; }
        public double? unit { get; set; }
        [Column("product code")]
        public int? product_code { get; set; }

        [Column("importer id")]
        public int? importer_id { get; set; }
        [Column("unity id")]
        public int? unity_id { get; set; }
        public double? goodunit { get; set; }
        public double? badunit { get; set; }
        public double?  usedunit { get; set; }
        public double? goodroll { get; set; }
        public double? badroll { get; set; }
        public double? usedroll { get; set; }
        public double? Saleprice { get; set;}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Add_product> Add_product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bill_Product> Bill_Products { get; set; }
        [ForeignKey("importer_id")]
        public virtual Importer importer { get; set; }
        [ForeignKey("unity_id")]
        public virtual  Unity unity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Product> Remove_Product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Replace_Product> Replace_Products { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invent_product> invent_product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<exhib_product> exhib_product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<identity_products> identity_product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<miss_products> miss_products { get; set; }
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
