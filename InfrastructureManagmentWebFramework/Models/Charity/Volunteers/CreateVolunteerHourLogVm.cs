using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerHourLogVm
    {
        public Guid Id { get; set; }

        [Required]
        public Guid VolunteerId { get; set; }

        [Display(Name = "الإسناد")]
        public Guid? AssignmentId { get; set; }

        [Display(Name = "المشروع")]
        public Guid? ProjectId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ العمل")]
        public DateTime WorkDate { get; set; } = DateTime.Today;

        [Required]
        [Range(0.25, 24)]
        [Display(Name = "عدد الساعات")]
        public decimal Hours { get; set; }

        [Required]
        [Display(Name = "عنوان النشاط")]
        public string ActivityTitle { get; set; } = string.Empty;

        [Display(Name = "النتيجة")]
        public string? Outcome { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Assignments { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
    }
}
