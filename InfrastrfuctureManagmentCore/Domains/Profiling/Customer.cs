namespace InfrastructureManagmentCore.Domains.Profiling
{
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Products;
    //using FastOrderFinalVersion.Entities_Layers;
    using MaintainanceSystem.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            Remove_Receipt = new HashSet<Remove_Receipt>();
            Products = new HashSet<Product>();
            Sales_Replace = new HashSet<Sales_Replace>();
            Sales_Bill = new HashSet<Sales_Bill>();
            Add_Receipt = new HashSet<Add_Receipt>();
        }

        [Key]
        [Column("Customer id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Customter_id { get; set; }

        
        [Required]
        //[StringLength(50)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }

        [Column("address id")]
        public int address_id { get; set; }

        public double Debit { get; set; }

        public double payed { get; set; }

        public string Tel { get; set; }
        public double remined { get; set; }
        [Column("Descrip", TypeName = "text")]
        public string descrip { get; set; }
        public int? Account_id { get; set; }
        [ForeignKey("Account_id")]
        public virtual Account Account { get; set; }
        public int? comid { get; set; }
        [ForeignKey("comid")]
        public virtual Company Company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Add_Receipt> Add_Receipt { get; set; }
        [ForeignKey("address_id")]
        public virtual address address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sales_Replace> Sales_Replace { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sales_Bill> Sales_Bill { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remove_Receipt> Remove_Receipt { get; set; }
    }
}
