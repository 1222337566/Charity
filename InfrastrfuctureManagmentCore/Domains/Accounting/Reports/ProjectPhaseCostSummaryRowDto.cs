namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class ProjectPhaseCostSummaryRowDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string PhaseStatus { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public decimal VarianceAmount => ActualCost - PlannedCost;
        public decimal VariancePercent => PlannedCost == 0 ? 0 : ((ActualCost - PlannedCost) / PlannedCost) * 100m;
        public int MilestonesCount { get; set; }
        public int CompletedMilestonesCount { get; set; }
    }
}
