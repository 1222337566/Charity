namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using InfrastructureManagmentCore.Domains.Profiling;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;
    //using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("Remove Receipt")]
    public partial class Remove_Receipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Remove_Receipt()
        {
            Remove_Product = new HashSet<Remove_Product>();
        }

        [Key]
        [Column("Remove id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Remove_id { get; set; }

        [Column("importer id")]
        public int? importer_id { get; set; }

        [Column("bill number")]
        public int? bill_number { get; set; }
        
        public int? code { get; set; }
        [Column(TypeName = "text")]
        public string Deliveredto { get; set; }
        [Column(TypeName = "text")]
        public string status { get; set; }
        [Column("invent id")]
        public int? invent_id { get; set; }
        [Column("exhib id")]
        public int? exhib_id { get; set; }
        public DateTime date { get; set; }

        [Column(TypeName = "text")]
        public string description { get; set; }

        public double? total { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Product> Remove_Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<missingitem> missitems { get; set; }
        [ForeignKey("importer_id")]
        public virtual Importer importer { get; set; }
        [ForeignKey("invent_id")]
        public virtual invent invent { get; set; }
        [ForeignKey("exhib_id")]
        public virtual exhib exhib { get; set; }
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
