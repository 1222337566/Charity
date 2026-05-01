namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class FunderDetailsVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FunderType { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int AgreementsCount { get; set; }
        public decimal TotalFunding { get; set; }
        public List<FunderAgreementSummaryVm> RecentAgreements { get; set; } = new();
    }
}
