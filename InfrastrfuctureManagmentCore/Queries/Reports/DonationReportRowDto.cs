namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class DonationReportRowDto
    {
        public Guid DonationId { get; set; }
        public DateTime DonationDate { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string DonationType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public string FinancialAccountName { get; set; } = string.Empty;
        public bool IsRestricted { get; set; }
        public string? CampaignName { get; set; }
    }
}
