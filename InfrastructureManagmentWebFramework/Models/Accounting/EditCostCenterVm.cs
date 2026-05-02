using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class EditCostCenterVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود المركز")]
        public string CostCenterCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم المركز")]
        public string CostCenterNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? CostCenterNameEn { get; set; }

        [Display(Name = "المركز الأب")]
        public Guid? ParentCostCenterId { get; set; }

        [Display(Name = "مرتبط بالمشروعات")]
        public bool IsProjectRelated { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> ParentCostCenters { get; set; } = new();
    }
}
