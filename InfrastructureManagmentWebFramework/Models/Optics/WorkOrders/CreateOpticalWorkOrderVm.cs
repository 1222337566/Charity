using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics.WorkOrders
{
    using System.ComponentModel.DataAnnotations;

    public class CreateOpticalWorkOrderVm
    {
        public Guid SalesInvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? PrescriptionText { get; set; }

        [Required]
        [Display(Name = "رقم أمر الشغل")]
        public string WorkOrderNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ أمر الشغل")]
        public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime? ExpectedDeliveryDateUtc { get; set; }

        [Display(Name = "عاجل")]
        public bool IsUrgent { get; set; }

        [Display(Name = "ملاحظات الإطار")]
        public string? FrameNotes { get; set; }

        [Display(Name = "ملاحظات العدسة")]
        public string? LensNotes { get; set; }

        [Display(Name = "ملاحظات المعمل")]
        public string? WorkshopNotes { get; set; }

        [Display(Name = "ملاحظات التسليم")]
        public string? DeliveryNotes { get; set; }
    }
}
