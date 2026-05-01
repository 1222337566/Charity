namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseLedgerDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProjectPhaseLedgerRowDto> Rows { get; set; } = new();
    }
}
