namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class DonorDetailsVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DonorType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? NationalIdOrTaxNo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public string? GovernorateName { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? PreferredCommunicationMethod { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int DonationsCount { get; set; }
        public decimal TotalDonationsAmount { get; set; }
        public List<DonorDonationSummaryVm> RecentDonations { get; set; } = new();
    }
}
