namespace InfrastructureManagmentCore.Domains.Projects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Cost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int costid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double priceper { get; set; }

        public double quantity { get; set; }

        public double totalCost { get; set; }

        public int? Milestoneid { get; set; }
        [ForeignKey("Milestoneid")]
        public virtual Milestone Milestone { get; set; }
    }
}
