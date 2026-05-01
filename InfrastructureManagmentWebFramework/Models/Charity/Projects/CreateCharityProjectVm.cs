using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Projects
{
    public class CreateCharityProjectVm
    {
        [Required]
        [Display(Name = "كود المشروع")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم المشروع")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "الموازنة")]
        [Range(0, 999999999)]
        public decimal Budget { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "عدد المستفيدين المستهدف")]
        public int? TargetBeneficiariesCount { get; set; }

        [Display(Name = "الموقع")]
        public string? Location { get; set; }

        [Display(Name = "الأهداف")]
        public string? Objectives { get; set; }

        [Display(Name = "المؤشرات")]
        public string? Kpis { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
