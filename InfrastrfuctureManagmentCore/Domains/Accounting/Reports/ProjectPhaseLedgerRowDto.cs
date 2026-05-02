namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseLedgerRowDto
    {
        public string SourceType { get; set; } = string.Empty;
        public Guid SourceId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? BudgetLineName { get; set; }
        public string? CostCenterName { get; set; }
        public decimal Amount { get; set; }
    }
}
