namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantInstallments
{
    public class GrantInstallmentListItemVm
    {
        public Guid Id { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? ReceivedAmount { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? FinancialAccount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }

        public string? AgreementTitle { get; set; }
        public string? FunderName { get; set; }
    }
}
