namespace InfrastructureManagmentCore.Domains.Billing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using InfrastructureManagmentCore.Domains.Products;
    using InfrastructureManagmentCore.Domains.Profiling;
    //using System.Data.Entity.Spatial;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("invent")]
    public partial class invent
    {
        [Key]
        [Column("invent id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int invent_id { get; set; }
        public string invent_name { get; set; }
        public int? address_id { get; set; }
        [Column("Descrip", TypeName = "text")]
        [Required]
        public string description { get; set; }
        [Column("total")]
        public double total { get; set; }
        [ForeignKey("address_id")]
        public virtual address address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Product> Remove_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invent_product> invent_product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Add_Receipt> Adds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Receipt> Removes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Identity> identities { get; set; }
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

