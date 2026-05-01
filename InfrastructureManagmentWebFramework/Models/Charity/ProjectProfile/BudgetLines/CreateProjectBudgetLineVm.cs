using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.BudgetLines
{
    public class CreateProjectBudgetLineVm
    {
        public Guid ProjectId { get; set; }

        [Required]
        [Display(Name = "اسم البند")]
        public string LineName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "نوع البند")]
        public string LineType { get; set; } = string.Empty;

        [Display(Name = "المخطط")]
        [Range(0, 999999999)]
        public decimal PlannedAmount { get; set; }

        [Display(Name = "الفعلي")]
        [Range(0, 999999999)]
        public decimal ActualAmount { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
