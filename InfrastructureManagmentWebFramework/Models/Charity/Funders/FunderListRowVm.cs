namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class FunderListRowVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FunderType { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int AgreementsCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
