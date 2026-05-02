namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class TreasuryBankDashboardVm
    {
        public int CashBankAccountCount { get; set; }
        public int TreasuryAccountCount { get; set; }
        public int BankAccountCount { get; set; }
        public decimal TotalCashBankBalance { get; set; }

        public List<CashBankVoucherListItemVm> RecentVouchers { get; set; } = new();
        public List<TreasuryBankTransferListItemVm> RecentTransfers { get; set; } = new();
    }
}
