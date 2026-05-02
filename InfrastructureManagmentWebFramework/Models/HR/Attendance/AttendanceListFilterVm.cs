using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.HR.Attendance
{
    public class AttendanceListFilterVm
    {
        public Guid? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
