using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalMonitoringIndicator
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public string Indicator { get; set; } = string.Empty;
        public string? TargetValue { get; set; }
        public string? VerificationMethod { get; set; }
        public string? ReportingFrequency { get; set; }
        public ProjectProposal? Proposal { get; set; }
    }
}
