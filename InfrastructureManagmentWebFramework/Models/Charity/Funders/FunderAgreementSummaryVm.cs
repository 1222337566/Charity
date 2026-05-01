namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class FunderAgreementSummaryVm
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
    }
}
