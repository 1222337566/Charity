using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalTargetGroup
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? TargetCount { get; set; }
        public string? AgeRange { get; set; }
        public string? GenderNotes { get; set; }
        public string? SelectionCriteria { get; set; }
        public string? BenefitDescription { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
