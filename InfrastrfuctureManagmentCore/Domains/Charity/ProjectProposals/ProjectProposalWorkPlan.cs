using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalWorkPlan
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string GoalTitle { get; set; } = string.Empty;
        public string ActivityTitle { get; set; } = string.Empty;
        public string? Responsible { get; set; }
        public string? Resources { get; set; }
        public string? PhaseName { get; set; }
        public int ContributionPercent { get; set; } = 100;
        public int? PlannedQuantity { get; set; }
        public int? DurationDays { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
