using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tracking;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectTrackingBoardVm
    {
        public ProjectHeaderVm ProjectHeader { get; set; } = new();
        public List<ProjectTrackingPhaseSummaryVm> PhaseSummaries { get; set; } = new();
        public List<ProjectTrackingLogListItemVm> RecentLogs { get; set; } = new();

        // Overall project progress
        public decimal OverallProgress => PhaseSummaries.Count == 0 ? 0
            : Math.Round(PhaseSummaries.Average(x => x.ProgressPercent), 0);

        public int CompletedPhases  => PhaseSummaries.Count(x => x.Status is "Completed");
        public int InProgressPhases => PhaseSummaries.Count(x => x.Status is "InProgress");
        public int DelayedPhases    => PhaseSummaries.Count(x => x.RequiresAttention);
        public int RemainingPhases  => PhaseSummaries.Count(x => x.Status is "Planned" or "Pending");

        // Milestones
        public List<ProjectMilestoneSummaryVm> Milestones { get; set; } = new();
        public int MilestoneDone    => Milestones.Count(x => x.Status == "Completed");
        public int MilestonePending => Milestones.Count(x => x.Status != "Completed");

        // Activities
        public List<ProjectActivitySummaryVm> Activities { get; set; } = new();
        public int ActivityDone      => Activities.Count(x => x.IsCompleted);
        public int ActivityRemaining => Activities.Count(x => !x.IsCompleted);

        // Budget
        public decimal TotalPlannedBudget { get; set; }
        public decimal TotalActualSpent   { get; set; }
        public decimal BudgetVariance     => TotalPlannedBudget - TotalActualSpent;
        public decimal BudgetUtilization  => TotalPlannedBudget > 0
            ? Math.Round(TotalActualSpent / TotalPlannedBudget * 100, 1) : 0;
    }

    public class ProjectMilestoneSummaryVm
    {
        public Guid Id { get; set; }
        public string PhaseName   { get; set; } = string.Empty;
        public string Title       { get; set; } = string.Empty;
        public DateTime DueDate   { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status      { get; set; } = string.Empty;
        public bool IsOverdue     => Status != "Completed" && DueDate.Date < DateTime.Today;
        public bool IsDueSoon     => Status != "Completed" && !IsOverdue
                                     && DueDate.Date <= DateTime.Today.AddDays(7);
    }

    public class ProjectActivitySummaryVm
    {
        public Guid Id           { get; set; }
        public string Title      { get; set; } = string.Empty;
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Status     { get; set; } = string.Empty;
        public decimal PlannedCost { get; set; }
        public decimal ActualCost  { get; set; }
        public bool IsCompleted  => ActualDate.HasValue || Status == "Completed";
        public bool IsOverdue    => !IsCompleted && PlannedDate.Date < DateTime.Today;
    }
}
