using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerProjectAssignmentVm
    {
        public Guid Id { get; set; }

        [Required]
        public Guid VolunteerId { get; set; }

        [Display(Name = "المشروع")]
        public Guid? ProjectId { get; set; }

        [Required]
        [Display(Name = "الدور")]
        public string RoleTitle { get; set; } = string.Empty;

        [Display(Name = "نوع الإسناد")]
        public string AssignmentType { get; set; } = "Recurring";

        [DataType(DataType.Date)]
        [Display(Name = "من تاريخ")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "إلى تاريخ")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "الساعات المستهدفة")]
        public decimal? TargetHours { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Active";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> AssignmentTypes { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
