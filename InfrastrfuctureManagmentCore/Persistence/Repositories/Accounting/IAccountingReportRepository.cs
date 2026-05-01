using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IAccountingReportRepository
    {
        Task<List<TrialBalanceRow>> GetTrialBalanceAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
        Task<List<GeneralLedgerSummaryRow>> GetGeneralLedgerAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
        Task<AccountStatementResult?> GetAccountStatementAsync(Guid accountId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
        Task<CostCenterStatementResult?> GetCostCenterStatementAsync(Guid costCenterId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
        Task<IncomeStatementResult> GetIncomeStatementAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
        Task<BalanceSheetResult> GetBalanceSheetAsync(Guid? fiscalPeriodId, DateTime? asOfDate, bool postedOnly = true);
        Task<RevenueExpenseSummaryResult> GetRevenueExpenseSummaryAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true);
    }
}
