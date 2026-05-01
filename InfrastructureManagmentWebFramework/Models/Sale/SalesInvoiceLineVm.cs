using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Sale
{
  
    public class SalesInvoiceLineVm
    {
        [Required]
        public Guid ItemId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        public List<SelectListItem> Items { get; set; } = new();
    }
}
