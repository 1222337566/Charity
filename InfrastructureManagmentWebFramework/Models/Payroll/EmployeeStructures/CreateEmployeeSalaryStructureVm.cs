using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Payroll.EmployeeStructures
{
    public class CreateEmployeeSalaryStructureVm
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required, Display(Name = "البند")]
        public Guid SalaryItemDefinitionId { get; set; }

        [Display(Name = "القيمة")]
        public decimal Value { get; set; }

        [Display(Name = "من تاريخ"), DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Display(Name = "إلى تاريخ"), DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> SalaryItems { get; set; } = new();
    }
}
