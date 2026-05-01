using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.OutRequests
{
    public class CreateOutRequestVm
    {
        public Guid Id { get; set; }
        [Required, Display(Name = "الموظف")]
        public Guid EmployeeId { get; set; }
        [Display(Name = "التاريخ"), DataType(DataType.Date)]
        public DateTime OutDate { get; set; } = DateTime.Today;
        [Display(Name = "من")]
        public TimeSpan FromTime { get; set; } = new(9,0,0);
        [Display(Name = "إلى")]
        public TimeSpan ToTime { get; set; } = new(10,0,0);
        [Required, Display(Name = "السبب")]
        public string Reason { get; set; } = string.Empty;
        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending";
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
