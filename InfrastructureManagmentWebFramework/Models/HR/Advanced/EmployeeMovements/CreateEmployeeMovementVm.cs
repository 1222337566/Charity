using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.EmployeeMovements
{
    public class CreateEmployeeMovementVm
    {
        public Guid Id { get; set; }
        [Display(Name = "الموظف")]
        [Required]
        public Guid EmployeeId { get; set; }
        [Display(Name = "نوع الحركة")]
        [Required]
        public string MovementType { get; set; } = "Transfer";
        [Display(Name = "تاريخ السريان")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;
        [Display(Name = "القسم السابق")]
        public Guid? FromDepartmentId { get; set; }
        [Display(Name = "القسم الجديد")]
        public Guid? ToDepartmentId { get; set; }
        [Display(Name = "الوظيفة السابقة")]
        public Guid? FromJobTitleId { get; set; }
        [Display(Name = "الوظيفة الجديدة")]
        public Guid? ToJobTitleId { get; set; }
        [Display(Name = "رقم القرار")]
        public string? DecisionNumber { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> JobTitles { get; set; } = new();
        public List<SelectListItem> MovementTypes { get; set; } = new();
    }
}
