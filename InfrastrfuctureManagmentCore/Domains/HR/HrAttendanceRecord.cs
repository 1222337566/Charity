namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrAttendanceRecord
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public DateTime AttendanceDate { get; set; } = DateTime.Today;
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public Guid? ShiftId { get; set; }
        public HrShift? Shift { get; set; }
        public decimal WorkedHours { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public string Status { get; set; } = "Present";
        public string Source { get; set; } = "Manual";
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
