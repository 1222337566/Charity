namespace InfrastructureManagmentCore.Domains.supplies
{
    using InfrastructureManagmentCore.Domains.Complains;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SystemComponent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SystemComponent()
        {
            TroubleTickets = new HashSet<TroubleTicket>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SyscomId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? GPSid { get; set; }

        public int? userid { get; set; }
        public int? ADSlid { get; set; }
        public int? Intenetlineid { get; set; }
        public int? VPNlineid { get; set; }

        public int? USBMid { get; set; }
        [ForeignKey("USBMid")]
        public virtual USB_Modem USB { get; set; }
        [ForeignKey("userid")]
        public virtual DomainUser DomainUser { get; set; }
        [ForeignKey("GPSid")]
        public virtual GPS_Tracking_Car Car { get; set; }
        [ForeignKey("ADSlid")]
        public virtual ADSL_Line adsllines { get; set; }
        [ForeignKey("Intenetlineid")]
        public virtual InternetLeasedLine InternetLines { get; set; }
        [ForeignKey("VPNlineid")]
        public virtual VPNLeasedLine VPNLines { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TroubleTicket> TroubleTickets { get; set; }
    }
}
