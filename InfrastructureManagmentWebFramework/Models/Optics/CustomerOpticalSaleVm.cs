using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    using InfrastructureManagmentWebFramework.Models.Sale;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CustomerOpticalSaleVm
    {
        public Guid CustomerId { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "رقم الفاتورة")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ الفاتورة")]
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Display(Name = "Prescription")]
        public Guid? PrescriptionId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public Guid? FrameItemId { get; set; }
        public decimal FrameQty { get; set; } = 1;
        public decimal FrameUnitPrice { get; set; }

        public Guid? LensItemId { get; set; }
        public decimal LensQty { get; set; } = 2;
        public decimal LensUnitPrice { get; set; }

        public Guid? ContactLensItemId { get; set; }
        public decimal ContactLensQty { get; set; }
        public decimal ContactLensUnitPrice { get; set; }

        public Guid? AccessoryItemId { get; set; }
        public decimal AccessoryQty { get; set; }
        public decimal AccessoryUnitPrice { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> Prescriptions { get; set; } = new();

        public List<OpticalItemOptionVm> Frames { get; set; } = new();
        public List<OpticalItemOptionVm> Lenses { get; set; } = new();
        public List<OpticalItemOptionVm> ContactLenses { get; set; } = new();
        public List<OpticalItemOptionVm> Accessories { get; set; } = new();

        public List<SalesPaymentVm> Payments { get; set; } = new();
    }
}
