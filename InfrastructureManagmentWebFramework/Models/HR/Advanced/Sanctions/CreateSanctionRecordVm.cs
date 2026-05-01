using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.Sanctions
{
    public class CreateSanctionRecordVm
    {
        public Guid Id { get; set; }
        [Required, Display(Name = "الموظف")]
        public Guid EmployeeId { get; set; }
        [Required, Display(Name = "نوع الجزاء")]
        public string SanctionType { get; set; } = "Warning";
        [DataType(DataType.Date), Display(Name = "تاريخ الجزاء")]
        public DateTime SanctionDate { get; set; } = DateTime.Today;
        [Display(Name = "قيمة الخصم")]
        public decimal? Amount { get; set; }
        [Required, Display(Name = "السبب")]
        public string Reason { get; set; } = string.Empty;
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> SanctionTypes { get; set; } = new();
    }
}
