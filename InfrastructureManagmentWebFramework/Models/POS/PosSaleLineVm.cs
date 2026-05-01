using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.POS
{
    
    public class PosSaleLineVm
    {
        public Guid ItemId { get; set; }

        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public string? Barcode { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        public decimal LineTotal { get; set; }
    }
}
