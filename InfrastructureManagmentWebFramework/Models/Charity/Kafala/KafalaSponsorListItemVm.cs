namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class KafalaSponsorListItemVm
    {
        public Guid Id { get; set; }
        public string SponsorCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string SponsorType { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int ActiveCasesCount { get; set; }
    }
}
