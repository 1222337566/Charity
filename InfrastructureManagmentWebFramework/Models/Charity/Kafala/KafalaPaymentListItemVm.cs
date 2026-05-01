namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class KafalaPaymentListItemVm
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public string SponsorName { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string PeriodLabel { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? PaymentMethodName { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? AidCycleId { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
