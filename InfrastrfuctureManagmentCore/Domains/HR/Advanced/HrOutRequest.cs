using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Advanced
{
    public class HrOutRequest
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public DateTime OutDate { get; set; } = DateTime.Today;
        public TimeSpan FromTime { get; set; } = new(9, 0, 0);
        public TimeSpan ToTime { get; set; } = new(10, 0, 0);
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
