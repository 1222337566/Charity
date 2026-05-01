using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchCommitteeEvaluation
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }
        public DateTime CommitteeMeetingDate { get; set; }
        public string Decision { get; set; } = string.Empty; // ApproveAid / RejectAid / Suspend / NeedMoreDocuments
        public string? ApprovedAidType { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public int? DurationMonths { get; set; }
        public string? Notes { get; set; }
        public string? ApprovedByUserId { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
    }
}
