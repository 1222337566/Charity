using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CreateCostCenterVm
    {
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
        public bool IsProjectRelated { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> ParentCostCenters { get; set; } = new();
    }
}
