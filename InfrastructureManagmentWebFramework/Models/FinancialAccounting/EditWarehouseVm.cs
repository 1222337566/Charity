using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
   
    public class EditWarehouseVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود المخزن")]
        public string WarehouseCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم المخزن")]
        public string WarehouseNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? WarehouseNameEn { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "مخزن رئيسي")]
        public bool IsMain { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }
    }
}
