namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class CashBankVoucherListItemVm
    {
        public Guid Id { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; }
        public string VoucherTypeName { get; set; } = string.Empty;
        public string FinancialAccountName { get; set; } = string.Empty;
        public string OppositeAccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? JournalEntryId { get; set; }
        public string? Notes { get; set; }
    }
}
