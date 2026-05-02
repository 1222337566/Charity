using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.Search
{
    public class FindSalesRowVm
    {
        public Guid SalesInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public string? PrescriptionText { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMethodsText { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public string? WorkOrderNumber { get; set; }
    }
}
