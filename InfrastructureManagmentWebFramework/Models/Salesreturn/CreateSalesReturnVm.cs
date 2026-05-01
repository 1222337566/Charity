using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Salesreturn
{
    using System.ComponentModel.DataAnnotations;

    public class CreateSalesReturnVm
    {
        [Required]
        [Display(Name = "رقم المرتجع")]
        public string ReturnNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ المرتجع")]
        public DateTime ReturnDateUtc { get; set; } = DateTime.UtcNow;

        public Guid OriginalSalesInvoiceId { get; set; }

        [Display(Name = "رقم الفاتورة الأصلية")]
        public string OriginalInvoiceNumber { get; set; } = string.Empty;

        [Display(Name = "العميل")]
        public string CustomerName { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SalesReturnLineVm> Lines { get; set; } = new();
    }
}
