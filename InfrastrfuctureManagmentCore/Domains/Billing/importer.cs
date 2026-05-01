namespace InfrastructureManagmentCore.Domains.Billing
{
    using InfrastructureManagmentCore.Domains.Products;
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Importer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Importer()
        {
            Add_Receipt = new HashSet<Add_Receipt>();
            Products = new HashSet<Product>();
            Purchases_Replace = new HashSet<Purchases_Replace>();
            Purchses_Bill = new HashSet<Purchses_Bill>();
            Remove_Receipt = new HashSet<Remove_Receipt>();
        }

        [Key]
        [Column("importer id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int importer_id { get; set; }

        [Required]
        //[StringLength(50)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }

        [Column("address id")]
        public int address_id { get; set; }

        public string Tel { get; set; }

        public double Debit { get; set; }

        public double payed { get; set; }

        public double remined { get; set; }
        [Column("Descrip", TypeName = "text")]
        public string descrip { get; set; }
        public int? Account_id { get; set; }
        [ForeignKey("Account_id")]
        public virtual Account Account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Add_Receipt> Add_Receipt { get; set; }
        [ForeignKey("address_id")]
        public virtual address address { get; set; }
        public int? comid { get; set; }
        [ForeignKey("comid")]
        public virtual Company Company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchases_Replace> Purchases_Replace { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchses_Bill> Purchses_Bill { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Receipt> Remove_Receipt { get; set; }
    }
}
