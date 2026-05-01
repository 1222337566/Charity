namespace InfrastructureManagmentCore.Domains.Complains
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    public partial class Hour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HourId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Period { get; set; }

        public int? taskid { get; set; }
        [ForeignKey("taskid")]
        public virtual Task Task { get; set; }
    }
}
