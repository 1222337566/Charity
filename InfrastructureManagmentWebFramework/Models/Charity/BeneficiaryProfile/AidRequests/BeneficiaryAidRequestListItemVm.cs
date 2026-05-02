namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidRequests
{
    public class BeneficiaryAidRequestListItemVm
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? BeneficiaryCode { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime RequestDate { get; set; }
        public string? AidType { get; set; }
        public decimal? RequestedAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal RemainingToFundAmount { get; set; }
        public decimal RemainingToDisburseAmount { get; set; }
        public decimal RemainingOnRequestAmount { get; set; }
        public string? UrgencyLevel { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string FundingStatusCode { get; set; } = string.Empty;
        public string FundingStatusName { get; set; } = string.Empty;
        public string DisbursementStatusCode { get; set; } = string.Empty;
        public string DisbursementStatusName { get; set; } = string.Empty;
        public string OperationalStatusCode { get; set; } = string.Empty;
        public string OperationalStatusName { get; set; } = string.Empty;
        public bool IsOperationallyClosed { get; set; }
        public List<BeneficiaryAidRequestLineVm> Lines { get; set; } = new();
    }
}
