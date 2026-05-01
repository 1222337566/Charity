using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace  InfrastructureManagmentCore.Domains.supplies
{
    [Table("IPAddress")]

    public class IPAdress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IPid { get; set; }
        public string address { get; set; }
        public string Description { get; set; }
        public string openTCPInboundports { get; set; }
        public string openUDPInboundports { get; set; }
        public string openTCPOutboundports { get; set; }
        public string openUDPOutboundports { get; set; }
        public int? ILineid { get; set; }
        public int?  VLineid { get; set;}

        [ForeignKey("ILineid")]
        public virtual InternetLeasedLine Iline { get; set; }
        [ForeignKey("VLineid")]
        public virtual VPNLeasedLine Vline { get; set; }
    }
}