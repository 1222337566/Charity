namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationAllocationListItemVm
    {
        public Guid Id { get; set; }
        public DateTime AllocatedDate { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? AidRequestSummary { get; set; }
        public string? DonationItemName { get; set; }
        public decimal? AllocatedQuantity { get; set; }
        public decimal? Amount { get; set; }
        public string? Notes { get; set; }
        public string ApprovalStatusCode { get; set; }
        public string ApprovalStatusName { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanEdit { get; set; }
        public bool HasFundingConsumption { get; set; }
        public int FundingConsumptionCount { get; set; }
    }
}
