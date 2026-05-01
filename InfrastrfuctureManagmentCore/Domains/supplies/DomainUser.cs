namespace InfrastructureManagmentCore.Domains.supplies
{
    using InfrastructureManagmentCore.Domains.Complains;
    using InfrastructureManagmentCore.Domains.Reactions;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class DomainUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DomainUser()
        {
            SystemComponents = new HashSet<SystemComponent>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userid { get; set; }

        public string Username { get; set; }

        public string FirstOU { get; set; }

        public string SecondOU { get; set; }

        public string ThirdOU { get; set; }

        public string logonname { get; set; }

        public int? Enabled { get; set; }

        public string DomainName { get; set; }

        public string ComputerName { get; set; }

        public string printerconnected { get; set; }

        public string email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SystemComponent> SystemComponents { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Complain> Complains { get; set; }
        public string loginid  { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        public string group { get; set; }
    }
}
