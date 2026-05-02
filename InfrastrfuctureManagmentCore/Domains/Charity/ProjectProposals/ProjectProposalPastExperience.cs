using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalPastExperience
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Donor { get; set; }
        public string? Location { get; set; }
        public string? DurationText { get; set; }
        public decimal? Budget { get; set; }
        public string? TargetGroup { get; set; }
        public string? ResultAchieved { get; set; }
        public string? Notes { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
