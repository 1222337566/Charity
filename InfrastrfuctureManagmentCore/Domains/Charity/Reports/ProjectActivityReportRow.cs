namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class ProjectActivityReportRow
    {
        public Guid ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectStatus { get; set; }
        public string? ActivityTitle { get; set; }
        public string? ActivityStatus { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
    }
}
