namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectCostSummaryRowDto
    {
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? CostCenterName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal NetCost => TotalDebit - TotalCredit;
    }
}
