using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Attendance
{
    public class CreateAttendanceRecordVm
    {
        [Required, Display(Name = "الموظف")]
        public Guid EmployeeId { get; set; }

        [Required, Display(Name = "التاريخ"), DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Display(Name = "وقت الحضور")]
        public TimeSpan? CheckInTime { get; set; }

        [Display(Name = "وقت الانصراف")]
        public TimeSpan? CheckOutTime { get; set; }

        [Display(Name = "الشيفت")]
        public Guid? ShiftId { get; set; }

        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "Present";

        [Required, Display(Name = "المصدر")]
        public string Source { get; set; } = "Manual";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Employees { get; set; } = new();
        public List<SelectListItem> Shifts { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Sources { get; set; } = new();
    }
}
