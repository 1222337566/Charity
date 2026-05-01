using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.JobTitles
{
    public class CreateJobTitleVm
    {
        [Required, Display(Name = "المسمى الوظيفي")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
