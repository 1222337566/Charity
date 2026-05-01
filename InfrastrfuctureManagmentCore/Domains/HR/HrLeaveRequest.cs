namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrLeaveRequest
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid LeaveTypeId { get; set; }
        public HrLeaveType? LeaveType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public bool IsHalfDay { get; set; }
        public string? HalfDayPeriod { get; set; } // Morning | Afternoon

        public string Status { get; set; } = "Pending"; // Pending|Approved|Rejected|Cancelled
        public string? Reason { get; set; }
        public string? AttachmentPath { get; set; }      // مسار المستند المرفق

        // الاعتماد
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? RejectionReason { get; set; }

        // الاستئناف من الإجازة
        public DateTime? ActualReturnDate { get; set; }
        public bool ReturnedEarly { get; set; }
        public string? ReturnNotes { get; set; }

        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
