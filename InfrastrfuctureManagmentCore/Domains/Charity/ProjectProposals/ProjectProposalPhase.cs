using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalPhase
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StartMonth { get; set; } = 1;
        public int EndMonth { get; set; } = 3;
        public ProjectProposal? Proposal { get; set; }
        public List<ProjectProposalPhaseActivity> Activities { get; set; } = new();
    }
}
