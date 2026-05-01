namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile
{
    public class GrantAgreementHeaderVm
    {
        public Guid Id { get; set; }
        public Guid FunderId { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string FunderName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
