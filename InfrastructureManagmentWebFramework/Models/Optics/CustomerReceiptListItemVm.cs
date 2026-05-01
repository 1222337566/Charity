using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class CustomerReceiptListItemVm
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDateUtc { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? PaymentMethodName { get; set; }
        public decimal Amount { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }
}
