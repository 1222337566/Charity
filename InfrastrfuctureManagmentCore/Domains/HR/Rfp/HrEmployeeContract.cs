using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Rfp
{
    public class HrEmployeeContract
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public string ContractType { get; set; } = "Permanent";
        public string? ContractNumber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? GrossSalary { get; set; }
        public string? FundingNotes { get; set; }
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
