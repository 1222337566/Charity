namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseAlertRowDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? RelatedDate { get; set; }
        public decimal? PlannedCost { get; set; }
        public decimal? ActualCost { get; set; }
    }
}
