using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalTeamMember
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Responsibility { get; set; }
        public bool IsInternal { get; set; } = true;
        public Guid? EmployeeId { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
