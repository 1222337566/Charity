namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectLedgerRowDto
    {
        public DateTime EntryDate { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? CostCenterName { get; set; }
        public string? Description { get; set; }
        public string? SourceType { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
