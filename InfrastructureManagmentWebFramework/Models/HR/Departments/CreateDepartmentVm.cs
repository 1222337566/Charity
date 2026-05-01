using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Departments
{
    public class CreateDepartmentVm
    {
        [Required, Display(Name = "اسم القسم")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
