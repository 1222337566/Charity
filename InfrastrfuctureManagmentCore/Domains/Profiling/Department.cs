namespace InfrastructureManagmentCore.Domains.Profiling
{
    using InfrastructureManagmentCore.Domains.Complains;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            TroubleTickets = new HashSet<TroubleTicket>();
        }

        [Key]
        [Column("Department ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Department_ID { get; set; }

        [Column("Department Name")]
        [Required]
        [StringLength(50)]
        public string Department_Name { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public string test { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TroubleTicket> TroubleTickets { get; set; }
    }
}
