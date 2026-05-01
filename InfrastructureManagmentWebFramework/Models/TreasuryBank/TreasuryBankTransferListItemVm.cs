namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class TreasuryBankTransferListItemVm
    {
        public Guid Id { get; set; }
        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public string FromAccountName { get; set; } = string.Empty;
        public string ToAccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? JournalEntryId { get; set; }
        public string? Notes { get; set; }
    }
}
