using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Purchase
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class PurchaseInvoiceLineVm
    {
        [Required]
        public Guid ItemId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }

        public List<SelectListItem> Items { get; set; } = new();
    }
}
