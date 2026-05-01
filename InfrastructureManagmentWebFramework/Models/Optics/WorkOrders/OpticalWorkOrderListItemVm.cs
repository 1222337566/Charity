using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.WorkOrders
{
    public class OpticalWorkOrderListItemVm
    {
        public Guid Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDateUtc { get; set; }
        public DateTime? ExpectedDeliveryDateUtc { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
    }
}
