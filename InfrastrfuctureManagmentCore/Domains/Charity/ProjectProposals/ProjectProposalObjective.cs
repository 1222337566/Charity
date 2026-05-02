using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalObjective
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string ObjectiveType { get; set; } = string.Empty; // General / Specific / Result
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
