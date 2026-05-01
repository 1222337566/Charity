using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities
{
    public class CreateProjectActivityVm
    {
        public Guid ProjectId { get; set; }

        [Required]
        [Display(Name = "عنوان النشاط")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "التاريخ المخطط")]
        [DataType(DataType.Date)]
        public DateTime PlannedDate { get; set; } = DateTime.Today;

        [Display(Name = "التاريخ الفعلي")]
        [DataType(DataType.Date)]
        public DateTime? ActualDate { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Planned";

        [Display(Name = "التكلفة المخططة")]
        public decimal PlannedCost { get; set; }

        [Display(Name = "التكلفة الفعلية")]
        public decimal ActualCost { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
