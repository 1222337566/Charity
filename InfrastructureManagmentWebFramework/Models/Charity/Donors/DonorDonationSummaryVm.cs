namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class DonorDonationSummaryVm
    {
        public Guid Id { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
