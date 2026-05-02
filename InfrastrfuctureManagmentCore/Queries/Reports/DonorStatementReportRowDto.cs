namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class DonorStatementReportRowDto
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string DonationType { get; set; } = string.Empty;
        public decimal DonationAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string? CampaignName { get; set; }
        public bool IsRestricted { get; set; }
    }
}
