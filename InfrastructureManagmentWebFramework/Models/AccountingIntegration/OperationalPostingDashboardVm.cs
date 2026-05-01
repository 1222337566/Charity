namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class OperationalPostingDashboardVm
    {
        public List<OperationalPostingRowVm> Donations { get; set; } = new();
        public List<OperationalPostingRowVm> BeneficiaryAidDisbursements { get; set; } = new();
        public List<OperationalPostingRowVm> GrantInstallments { get; set; } = new();
        public List<OperationalPostingRowVm> Expenses { get; set; } = new();
        public List<OperationalPostingRowVm> PayrollMonths { get; set; } = new();
        public List<OperationalPostingRowVm> StoreIssues { get; set; } = new();
        public List<OperationalPostingRowVm> StoreReceipts { get; set; } = new();
    }
}
