using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Rfp
{
    public class HrEmployeeFundingAssignment
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid? FunderId { get; set; }
        public string FundingSourceName { get; set; } = null!;
        public Guid? CharityProjectId { get; set; }
        public string? GrantOrBudgetLine { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Today;
        public DateTime? ToDate { get; set; }
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
