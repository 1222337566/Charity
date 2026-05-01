using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Domains.Products
{
    public class miss_products
    {
        [Key]
        [Column("miss id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int miss_id { get; set; }

        
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
        [ForeignKey("miss_id")]
        public virtual missingitem miss { get; set; }
    }
}
