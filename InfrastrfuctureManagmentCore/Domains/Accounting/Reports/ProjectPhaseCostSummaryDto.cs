namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseCostSummaryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<ProjectPhaseCostSummaryRowDto> Rows { get; set; } = new();
    }
}
