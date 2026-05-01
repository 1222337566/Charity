using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.WorkOrders
{
    public class OpticalWorkOrderDetailsVm
    {
        public Guid Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? PrescriptionText { get; set; }

        public DateTime OrderDateUtc { get; set; }
        public DateTime? ExpectedDeliveryDateUtc { get; set; }
        public DateTime? ReadyDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }

        public string StatusText { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }

        public string? FrameNotes { get; set; }
        public string? LensNotes { get; set; }
        public string? WorkshopNotes { get; set; }
        public string? DeliveryNotes { get; set; }

        public List<OpticalWorkOrderLineVm> Items { get; set; } = new();
    }
}
