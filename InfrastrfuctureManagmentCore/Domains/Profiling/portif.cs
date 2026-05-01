
namespace InfrastructureManagmentCore.Domains.Profiling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public  class portif
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public portif()
        {
           // this.Groups = new HashSet<Group>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int portfilioId { get; set; }
        public int? companyid { get; set; }
        public int? Employer_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Trial { get; set; }

        public int? groupid { get; set; }
        public int? Accounting { get; set; }
        public int? Finincial { get; set; }
        public int? System { get; set; }
        public int? Chat { get; set; }
        public int? Event { get; set; }
        public int? Graphs { get; set; }
        public int? Social { get; set; }
        public int? Project { get; set; }
        public int? CProject { get; set; }
        public int? Notification { get; set; }
        public int? Todo { get; set; }
        public int? Complain { get; set; }
        public int? TT { get; set; }

        [ForeignKey("companyid")]

        public virtual Company company { get; set; }
        [ForeignKey("Employer_ID")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("groupid")]
        public virtual Group group { get; set; }
    }
}