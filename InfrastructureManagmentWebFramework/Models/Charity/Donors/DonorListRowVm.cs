namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class DonorListRowVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DonorType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? PhoneNumber { get; set; }
        public string? GovernorateName { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalDonations { get; set; }
    }
}
