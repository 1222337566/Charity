namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrShift
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GraceMinutes { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<HrAttendanceRecord> AttendanceRecords { get; set; } = new List<HrAttendanceRecord>();
    }
}
