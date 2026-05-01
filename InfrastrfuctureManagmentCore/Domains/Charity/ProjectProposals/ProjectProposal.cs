using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposal
    {
        public Guid Id { get; set; }
        public string ProposalNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public string? OrganizationName { get; set; }
        public string? ProjectLocation { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Today;
        public int DurationMonths { get; set; }
        public decimal RequestedBudget { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } = "Draft";

        public string? RegistrationNumber { get; set; }
        public int? RegistrationYear { get; set; }
        public string? Vision { get; set; }
        public string? Mission { get; set; }
        public string? ExpertiseSummary { get; set; }
        public int? EmployeesCount { get; set; }
        public int? VolunteersCount { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Address { get; set; }

        public string? ProblemBackground { get; set; }
        public string? ProblemAnalysis { get; set; }
        public string? NationalAlignment { get; set; }
        public string? ProposedApproach { get; set; }
        public string? ProposedSolution { get; set; }
        public string? RisksAndExternalFactors { get; set; }
        public string? ExecutiveSummary { get; set; }
        public string? GeneralGoal { get; set; }
        public string? ExpectedResults { get; set; }
        public string? PreparatoryRequirements { get; set; }
        public string? ImplementationTeamSummary { get; set; }
        public string? SustainabilityPlan { get; set; }
        public string? Notes { get; set; }

        public Guid? CharityProjectId { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? ApprovedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProjectProposalPastExperience> PastExperiences { get; set; } = new List<ProjectProposalPastExperience>();
        public ICollection<ProjectProposalTargetGroup> TargetGroups { get; set; } = new List<ProjectProposalTargetGroup>();
        public ICollection<ProjectProposalObjective> Objectives { get; set; } = new List<ProjectProposalObjective>();
        public ICollection<ProjectProposalActivity> Activities { get; set; } = new List<ProjectProposalActivity>();
        public ICollection<ProjectProposalWorkPlan> WorkPlanItems { get; set; } = new List<ProjectProposalWorkPlan>();
        public ICollection<ProjectProposalMonitoringIndicator> MonitoringIndicators { get; set; } = new List<ProjectProposalMonitoringIndicator>();
        public ICollection<ProjectProposalTeamMember> TeamMembers { get; set; } = new List<ProjectProposalTeamMember>();
        public ICollection<ProjectProposalAttachment> Attachments { get; set; } = new List<ProjectProposalAttachment>();
        public ICollection<ProjectProposalPhase> Phases { get; set; } = new List<ProjectProposalPhase>();
    }
}
