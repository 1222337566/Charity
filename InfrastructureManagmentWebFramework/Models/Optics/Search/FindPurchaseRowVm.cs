using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Search
{
    public class FindPurchaseRowVm
    {
        public Guid PurchaseInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; }

        public string SupplierName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public decimal NetAmount { get; set; }
        public string? SupplierInvoiceNumber { get; set; }
        public string? Notes { get; set; }
    }
}
