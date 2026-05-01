namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.CommitteeDecisions
{
    public class BeneficiaryCommitteeDecisionListItemVm
    {
        public Guid Id { get; set; }
        public DateTime DecisionDate { get; set; }
        public string DecisionType { get; set; } = string.Empty;
        public string? ApprovedAidType { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public int? DurationInMonths { get; set; }
        public string? CommitteeNotes { get; set; }
        public bool ApprovedStatus { get; set; }
    }
}
