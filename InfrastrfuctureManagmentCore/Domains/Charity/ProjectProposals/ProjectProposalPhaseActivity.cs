using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    /// <summary>تسكين نشاط في مرحلة مع نسبة المساهمة</summary>
    public class ProjectProposalPhaseActivity
    {
        public Guid Id { get; set; }
        public Guid PhaseId { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public int ContributionPercent { get; set; } = 100; // نسبة مساهمة المرحلة في النشاط
        public int? PlannedQuantity { get; set; }
        public int? DurationDays { get; set; }
        public int SortOrder { get; set; }
        public ProjectProposalPhase? Phase { get; set; }
    }
}
