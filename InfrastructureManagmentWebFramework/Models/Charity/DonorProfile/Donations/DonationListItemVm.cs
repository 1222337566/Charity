namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationListItemVm
    {
        public Guid Id { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? FinancialAccount { get; set; }
        public string? ReceiptNumber { get; set; }
        public bool IsRestricted { get; set; }
        public string? CampaignName { get; set; }
        public string? TargetingScopeCode { get; set; }
        public string? TargetingScopeName { get; set; }
        public string? GeneralPurposeName { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? DonorName { get; set; }
    }
}
