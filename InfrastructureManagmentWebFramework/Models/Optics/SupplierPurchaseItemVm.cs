using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class SupplierPurchaseItemVm
    {
        public Guid PurchaseInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal NetAmount { get; set; }
        public string? Notes { get; set; }
    }
}
