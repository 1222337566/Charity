using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Payroll.SalaryItems
{
    public class CreateSalaryItemDefinitionVm
    {
        [Required, Display(Name = "الاسم")]
        public string Name { get; set; } = string.Empty;

        [Required, Display(Name = "النوع")]
        public string ItemType { get; set; } = "Addition";

        [Required, Display(Name = "طريقة الحساب")]
        public string CalculationMethod { get; set; } = "Fixed";

        [Display(Name = "القيمة الافتراضية")]
        public decimal? DefaultValue { get; set; }

        [Display(Name = "خاضع للضريبة")]
        public bool IsTaxable { get; set; }

        [Display(Name = "ضمن التأمين")]
        public bool IsInsuranceIncluded { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> ItemTypes { get; set; } = new();
        public List<SelectListItem> CalculationMethods { get; set; } = new();
    }
}
