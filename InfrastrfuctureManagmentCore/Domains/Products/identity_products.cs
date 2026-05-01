using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Products
{
    public class identity_products
    {
        [Key]
        [Column("identity id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int identity_id { get; set; }

        
        [Column("Product id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Product_id { get; set; }

        
        [Column(Order = 2)]
        public double amount { get; set; }
        
        [Column(Order = 3)]
        public DateTime Date { get; set; }
        
        [Column(Order = 4)]
        public string Unit { get; set; }
        [ForeignKey("Product_id")]
        public virtual Product Product { get; set; }
        [ForeignKey("identity_id")]
        public virtual Identity identity { get; set; }
    }
}
