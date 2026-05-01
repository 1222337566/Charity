using InfrastructureManagmentCore.Domains.Billing;
using InfrastructureManagmentCore.Domains.Products;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
//using MaintainanceSystem.Models;
//using MaintainanceSystemWebService.Models;

namespace InfrastructureManagmentCore.Domains.Products
{
    [Table("MissingItem")]
    public class missingitem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public missingitem()
        {
            miss_products = new HashSet<miss_products>();

        }

        [Key]
        [Column("miss id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int miss_id { get; set; }

        public int? Remove_id { get; set; }
        public int? RemovePro_id { get; set; }

        [Column("unit")]
        [StringLength(50)]
        public string unit { get; set; }
        [Column("nickname", TypeName = "text")]
        public string nickname { get; set; }

        [ForeignKey("Remove_id")]
        public virtual Remove_Receipt Remove_Receipt { get; set; }
        [ForeignKey("RemovePro_id")]
        public virtual Remove_Product Remove_Product { get; set; }
        [Column("description", TypeName = "text")]
        public string description { get; set; }
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
