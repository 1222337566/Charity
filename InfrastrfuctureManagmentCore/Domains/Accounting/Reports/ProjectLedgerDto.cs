namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectLedgerDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public List<ProjectLedgerRowDto> Rows { get; set; } = new();
    }
}
