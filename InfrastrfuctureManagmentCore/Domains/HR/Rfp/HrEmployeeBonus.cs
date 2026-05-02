using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Rfp
{
    public class HrEmployeeBonus
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public DateTime BonusDate { get; set; } = DateTime.Today;
        public string BonusType { get; set; } = "Performance";
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public Guid? PayrollMonthId { get; set; }
        public bool IsApproved { get; set; }
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
