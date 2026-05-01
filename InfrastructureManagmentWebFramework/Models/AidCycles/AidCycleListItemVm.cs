namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class AidCycleListItemVm
    {
        public Guid Id { get; set; }
        public string CycleNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CycleType { get; set; } = string.Empty;
        public string? AidType { get; set; }
        public int? PeriodYear { get; set; }
        public int? PeriodMonth { get; set; }
        public DateTime PlannedDisbursementDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int BeneficiariesCount { get; set; }
        public decimal? TotalPlannedAmount { get; set; }
        public decimal? TotalDisbursedAmount { get; set; }
    }
}
