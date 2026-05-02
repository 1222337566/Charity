namespace InfrastructureManagmentCore.Domains.Billing
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

    [Table("Purchses Bill")]
    public partial class Purchses_Bill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Purchses_Bill()
        {
            Add_Receipt = new HashSet<Add_Receipt>();
            Bill_Products = new HashSet<Bill_Product>();
        }

        [Key]
        [Column("Bill id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Bill_id { get; set; }

        [Column("importer id")]
        public int importer_id { get; set; }
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
        public virtual ICollection<Add_Receipt> Add_Receipt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bill_Product> Bill_Products { get; set; }
        [ForeignKey("importer_id")]
        public virtual Importer importer { get; set; }
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
