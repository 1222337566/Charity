using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryAssessment
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public DateTime VisitDate { get; set; }
        public string? ResearcherUserId { get; set; }

        public string? HousingCondition { get; set; }
        public string? EconomicCondition { get; set; }
        public string? HealthCondition { get; set; }
        public string? SocialCondition { get; set; }

        public Guid? RecommendedAidTypeId { get; set; }
        public AidTypeLookup? RecommendedAidType { get; set; }

        public decimal? RecommendationAmount { get; set; }
        public decimal? AssessmentScore { get; set; }

        public string? RecommendationText { get; set; }
        public string? DecisionSuggested { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
