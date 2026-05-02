namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectCostSummaryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<ProjectCostSummaryRowDto> Rows { get; set; } = new();
    }
}
