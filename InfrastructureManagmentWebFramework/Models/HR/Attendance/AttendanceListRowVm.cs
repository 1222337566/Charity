namespace InfrastructureManagmentWebFramework.Models.HR.Attendance
{
    public class AttendanceListRowVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string? ShiftName { get; set; }
        public decimal WorkedHours { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}
