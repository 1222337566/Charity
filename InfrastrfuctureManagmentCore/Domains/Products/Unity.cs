
using MaintainanceSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using MaintainanceSystem.Models;
using InfrastructureManagmentCore.Domains.Profiling;
//using MaintainanceSystemWebService.Models;

namespace InfrastructureManagmentCore.Domains.Products
{
    [Table("Unity")]
    public partial class Unity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Unity()
        {
             Products= new HashSet<Product>();
        }
        [Key]
        [Column("unity id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int unity_id { get; set; }

        [Column("cartoon", TypeName = "text")]
        
        public string Cartoon { get; set; }
        [Column("Roll", TypeName = "text")]

        public string Roll { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
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
