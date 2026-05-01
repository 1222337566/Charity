namespace InfrastructureManagmentCore.Domains.Products
{
    using InfrastructureManagmentCore.Domains.Billing;
    using InfrastructureManagmentCore.Domains.Profiling;
    using MaintainanceSystem.Models;
    //using MaintainanceSystemWebService.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    //using System.Data.Entity.Spatial;

    [Table("Identity")]
    public partial class Identity
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Identity()
        {
            identity_products = new HashSet<identity_products>();
        }

        [Key]
        [Column("identity id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int identity_id { get; set; }

        

        [Column("unit")]
        [StringLength(50)]
        public string unit { get; set; }
        [Column("nickname", TypeName = "text")]
        public string nickname { get; set; }


        [Column("description", TypeName = "text")]
        public string description { get; set; }
        public int? invent_id { get; set; }
        public int? exhib_id { get; set; }
        public int? RemovePro_id { get; set; }
        public int? Addpro_id { get; set; }
        public int? Replacepro_id { get; set; }
        [Column("serial number", TypeName = "text")]
        public string serial_number { get; set; }
        [Column("status", TypeName = "text")]
        public string status { get; set; }
        public string removestatus { get; set; }
        public string deliverystatus { get; set; }
        public double? unitprice { get; set; }
        public double? total { get; set; }
        [ForeignKey("exhib_id")]
        public virtual exhib exhib { get; set; }
        [ForeignKey("invent_id")]
        public virtual invent invent { get; set;}
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<identity_products> identity_products { get; set; }
        [ForeignKey("RemovePro_id")]
        public virtual Remove_Product Remove_product { get; set; }
        [ForeignKey("Addpro_id")]
        public virtual Add_product Add_product { get; set; }
        [ForeignKey("Replacepro_id")]
        public virtual Replace_Product Replace_product { get; set; }
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
