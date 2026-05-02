namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Add Receipt")]
    public partial class Add_Receipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Add_Receipt()
        {
            Add_product = new HashSet<Add_product>();
        }

        [Key]
        [Column("add id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int add_id { get; set; }

        [Column("importer id")]
        public int? importer_id { get; set; }

        [Column("bill id")]
        public int? bill_id { get; set; }
        [Column(TypeName = "text")]
        public string Deliveredto { get; set; }
       
        public int? code { get; set; }
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
        public virtual ICollection<Add_product> Add_product { get; set; }
        [ForeignKey("bill_id")]
        public virtual Purchses_Bill Purchses_Bill { get; set; }
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
