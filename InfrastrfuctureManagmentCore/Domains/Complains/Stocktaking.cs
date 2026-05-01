

namespace InfrastructureManagmentCore.Domains.Complains
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;

    [Table("Stock Taking")]
    public partial class Stocktaking
    {
        public Stocktaking()
        {

            Transactions = new HashSet<Transaction2>();

        }
        [Key]
        [Column("Stocktake id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Stocktake_id { get; set; }
        public DateTime date { get; set; }
        public string code { get; set; }
        public string status { get; set; }
        public string descrip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction2> Transactions  { get; set; }
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
