namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantAgreements
{
    public class GrantAgreementListItemVm
    {
        public Guid Id { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime AgreementDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int InstallmentsCount { get; set; }
        public int ConditionsCount { get; set; }
        public decimal ReceivedAmount { get; set; }

        public string? FunderName { get; set; }
    }
}
