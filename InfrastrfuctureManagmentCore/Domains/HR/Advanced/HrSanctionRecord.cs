using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Advanced
{
    public class HrSanctionRecord
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public string SanctionType { get; set; } = "Warning";
        public DateTime SanctionDate { get; set; } = DateTime.Today;
        public decimal? Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
