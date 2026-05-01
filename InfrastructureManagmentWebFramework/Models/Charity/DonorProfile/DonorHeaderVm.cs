namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile
{
    public class DonorHeaderVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DonorType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ContactPerson { get; set; }
        public bool IsActive { get; set; }
    }
}
