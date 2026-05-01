namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Assessments
{
    public class BeneficiaryAssessmentListItemVm
    {
        public Guid Id { get; set; }
        public DateTime VisitDate { get; set; }
        public string? RecommendedAidType { get; set; }
        public decimal? RecommendationAmount { get; set; }
        public decimal? AssessmentScore { get; set; }
        public string? DecisionSuggested { get; set; }
        public string? RecommendationText { get; set; }
    }
}
