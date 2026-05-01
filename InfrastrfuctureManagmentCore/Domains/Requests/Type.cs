namespace InfrastructureManagmentCore.Domains.Requests
{
    using InfrastructureManagmentCore.Domains.Complains;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Type")]
    public partial class Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Type()
        {
            TroubleTickets = new HashSet<TroubleTicket>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Column("Type ID")]
        public int Type_ID { get; set; }

        [Column("Type Name")]
        public string Type_Name { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public string hhdsd { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TroubleTicket> TroubleTickets { get; set; }
    }
}
