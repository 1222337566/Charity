using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Stock
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreateOpeningBalanceVm
    {
        [Required]
        [Display(Name = "الصنف")]
        public Guid ItemId { get; set; }

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "التكلفة")]
        public decimal UnitCost { get; set; }

        [Display(Name = "تاريخ الحركة")]
        public DateTime? TransactionDateUtc { get; set; }

        [Display(Name = "رقم المرجع")]
        public string? ReferenceNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Items { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}
