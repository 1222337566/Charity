using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchReview
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }
        public string ReviewerUserId { get; set; } = string.Empty;
        public DateTime ReviewDateUtc { get; set; } = DateTime.UtcNow;
        public string Decision { get; set; } = string.Empty; // Approve / Reject / ReturnForRevision
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewerNotes { get; set; }
        public string? ReviewedByUserId { get; set; }
    }
}
