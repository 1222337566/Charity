using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Domains.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Optics
{
    public class OpticalWorkOrder
    {
        public Guid Id { get; set; }

        public string WorkOrderNumber { get; set; } = string.Empty;

        public Guid SalesInvoiceId { get; set; }
        public SalesInvoice? SalesInvoice { get; set; }

        public Guid? CustomerId { get; set; }
        public CustomerClient? Customer { get; set; }

        public Guid? PrescriptionId { get; set; }
        public Prescription? Prescription { get; set; }

        public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDateUtc { get; set; }
        public DateTime? ReadyDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }

        public OpticalWorkOrderStatus Status { get; set; } = OpticalWorkOrderStatus.New;

        public bool IsUrgent { get; set; }

        public string? FrameNotes { get; set; }
        public string? LensNotes { get; set; }
        public string? WorkshopNotes { get; set; }
        public string? DeliveryNotes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
