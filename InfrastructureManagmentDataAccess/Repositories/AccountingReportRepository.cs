using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class AccountingReportRepository : IAccountingReportRepository
    {
        private readonly AppDbContext _db;

        public AccountingReportRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TrialBalanceRow>> GetTrialBalanceAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var query = BuildBaseLinesQuery(fiscalPeriodId, postedOnly);

            if (fromDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            return await query
                .GroupBy(x => new
                {
                    x.FinancialAccountId,
                    x.FinancialAccount!.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new TrialBalanceRow
                {
                    AccountId = g.Key.FinancialAccountId,
                    AccountCode = g.Key.AccountCode,
                    AccountNameAr = g.Key.AccountNameAr,
                    Category = g.Key.Category.ToString(),
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount),
                    NetBalance = g.Sum(x => x.DebitAmount - x.CreditAmount)
                })
                .OrderBy(x => x.AccountCode)
                .ToListAsync();
        }

        public async Task<List<GeneralLedgerSummaryRow>> GetGeneralLedgerAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var baseQuery = BuildBaseLinesQuery(fiscalPeriodId, postedOnly);

            var openingQuery = baseQuery;
            if (fromDate.HasValue)
                openingQuery = openingQuery.Where(x => x.JournalEntry!.EntryDate.Date < fromDate.Value.Date);
            else
                openingQuery = openingQuery.Where(x => false);

            var periodQuery = baseQuery;
            if (fromDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var openingRows = await openingQuery
                .GroupBy(x => new
                {
                    x.FinancialAccountId,
                    x.FinancialAccount!.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new
                {
                    g.Key.FinancialAccountId,
                    g.Key.AccountCode,
                    g.Key.AccountNameAr,
                    g.Key.Category,
                    OpeningBalance = g.Sum(x => x.DebitAmount - x.CreditAmount)
                })
                .ToListAsync();

            var periodRows = await periodQuery
                .GroupBy(x => new
                {
                    x.FinancialAccountId,
                    x.FinancialAccount!.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new
                {
                    g.Key.FinancialAccountId,
                    g.Key.AccountCode,
                    g.Key.AccountNameAr,
                    g.Key.Category,
                    PeriodDebit = g.Sum(x => x.DebitAmount),
                    PeriodCredit = g.Sum(x => x.CreditAmount)
                })
                .ToListAsync();

            var results = openingRows
                .Select(x => new GeneralLedgerSummaryRow
                {
                    AccountId = x.FinancialAccountId,
                    AccountCode = x.AccountCode,
                    AccountNameAr = x.AccountNameAr,
                    Category = x.Category.ToString(),
                    OpeningBalance = x.OpeningBalance
                })
                .Concat(periodRows.Select(x => new GeneralLedgerSummaryRow
                {
                    AccountId = x.FinancialAccountId,
                    AccountCode = x.AccountCode,
                    AccountNameAr = x.AccountNameAr,
                    Category = x.Category.ToString(),
                    PeriodDebit = x.PeriodDebit,
                    PeriodCredit = x.PeriodCredit
                }))
                .GroupBy(x => new { x.AccountId, x.AccountCode, x.AccountNameAr, x.Category })
                .Select(g => new GeneralLedgerSummaryRow
                {
                    AccountId = g.Key.AccountId,
                    AccountCode = g.Key.AccountCode,
                    AccountNameAr = g.Key.AccountNameAr,
                    Category = g.Key.Category,
                    OpeningBalance = g.Sum(x => x.OpeningBalance),
                    PeriodDebit = g.Sum(x => x.PeriodDebit),
                    PeriodCredit = g.Sum(x => x.PeriodCredit),
                    ClosingBalance = g.Sum(x => x.OpeningBalance + x.PeriodDebit - x.PeriodCredit)
                })
                .OrderBy(x => x.AccountCode)
                .ToList();

            return results;
        }

        public async Task<AccountStatementResult?> GetAccountStatementAsync(Guid accountId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var account = await _db.Set<InfrastrfuctureManagmentCore.Domains.Financial.FinancialAccount>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == accountId);

            if (account == null)
                return null;

            var baseQuery = BuildBaseLinesQuery(fiscalPeriodId, postedOnly)
                .Where(x => x.FinancialAccountId == accountId);

            decimal openingBalance = 0m;
            if (fromDate.HasValue)
            {
                openingBalance = await baseQuery
                    .Where(x => x.JournalEntry!.EntryDate.Date < fromDate.Value.Date)
                    .SumAsync(x => x.DebitAmount - x.CreditAmount);
            }

            var periodQuery = baseQuery;
            if (fromDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var periodRows = await periodQuery
                .OrderBy(x => x.JournalEntry!.EntryDate)
                .ThenBy(x => x.JournalEntry!.EntryNumber)
                .ThenBy(x => x.CreatedAtUtc)
                .Select(x => new
                {
                    x.Id,
                    x.JournalEntryId,
                    x.JournalEntry!.EntryDate,
                    x.JournalEntry.EntryNumber,
                    LineDescription = x.Description,
                    EntryDescription = x.JournalEntry.Description,
                    CostCenterNameAr = x.CostCenter != null ? x.CostCenter.NameAr : null,
                    x.ProjectId,
                    x.JournalEntry.SourceType,
                    x.JournalEntry.SourceId,
                    x.DebitAmount,
                    x.CreditAmount
                })
                .ToListAsync();

            var result = new AccountStatementResult
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountNameAr = account.AccountNameAr,
                Category = account.Category.ToString(),
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = openingBalance
            };

            decimal runningBalance = openingBalance;
            foreach (var row in periodRows)
            {
                runningBalance += row.DebitAmount - row.CreditAmount;
                result.Rows.Add(new LedgerEntryRow
                {
                    JournalEntryId = row.JournalEntryId,
                    JournalEntryLineId = row.Id,
                    EntryDate = row.EntryDate,
                    EntryNumber = row.EntryNumber,
                    Description = string.IsNullOrWhiteSpace(row.LineDescription) ? row.EntryDescription : row.LineDescription!,
                    CostCenterNameAr = row.CostCenterNameAr,
                    ProjectId = row.ProjectId,
                    SourceType = row.SourceType,
                    SourceId = row.SourceId,
                    DebitAmount = row.DebitAmount,
                    CreditAmount = row.CreditAmount,
                    RunningBalance = runningBalance
                });
            }

            result.TotalDebit = result.Rows.Sum(x => x.DebitAmount);
            result.TotalCredit = result.Rows.Sum(x => x.CreditAmount);
            result.ClosingBalance = result.OpeningBalance + result.TotalDebit - result.TotalCredit;

            return result;
        }

        public async Task<CostCenterStatementResult?> GetCostCenterStatementAsync(Guid costCenterId, Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var costCenter = await _db.Set<CostCenter>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == costCenterId);

            if (costCenter == null)
                return null;

            var baseQuery = BuildBaseLinesQuery(fiscalPeriodId, postedOnly)
                .Where(x => x.CostCenterId == costCenterId);

            decimal openingBalance = 0m;
            if (fromDate.HasValue)
            {
                openingBalance = await baseQuery
                    .Where(x => x.JournalEntry!.EntryDate.Date < fromDate.Value.Date)
                    .SumAsync(x => x.DebitAmount - x.CreditAmount);
            }

            var periodQuery = baseQuery;
            if (fromDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                periodQuery = periodQuery.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var periodRows = await periodQuery
                .OrderBy(x => x.JournalEntry!.EntryDate)
                .ThenBy(x => x.JournalEntry!.EntryNumber)
                .ThenBy(x => x.CreatedAtUtc)
                .Select(x => new
                {
                    x.Id,
                    x.JournalEntryId,
                    x.JournalEntry!.EntryDate,
                    x.JournalEntry.EntryNumber,
                    AccountCode = x.FinancialAccount != null ? x.FinancialAccount.AccountCode : string.Empty,
                    AccountNameAr = x.FinancialAccount != null ? x.FinancialAccount.AccountNameAr : string.Empty,
                    LineDescription = x.Description,
                    EntryDescription = x.JournalEntry.Description,
                    x.ProjectId,
                    x.JournalEntry.SourceType,
                    x.JournalEntry.SourceId,
                    x.DebitAmount,
                    x.CreditAmount
                })
                .ToListAsync();

            var result = new CostCenterStatementResult
            {
                CostCenterId = costCenter.Id,
                CostCenterCode = costCenter.CostCenterCode,
                CostCenterNameAr = costCenter.NameAr,
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = openingBalance
            };

            decimal runningBalance = openingBalance;
            foreach (var row in periodRows)
            {
                runningBalance += row.DebitAmount - row.CreditAmount;
                result.Rows.Add(new CostCenterLedgerEntryRow
                {
                    JournalEntryId = row.JournalEntryId,
                    JournalEntryLineId = row.Id,
                    EntryDate = row.EntryDate,
                    EntryNumber = row.EntryNumber,
                    AccountCode = row.AccountCode,
                    AccountNameAr = row.AccountNameAr,
                    Description = string.IsNullOrWhiteSpace(row.LineDescription) ? row.EntryDescription : row.LineDescription!,
                    ProjectId = row.ProjectId,
                    SourceType = row.SourceType,
                    SourceId = row.SourceId,
                    DebitAmount = row.DebitAmount,
                    CreditAmount = row.CreditAmount,
                    RunningBalance = runningBalance
                });
            }

            result.TotalDebit = result.Rows.Sum(x => x.DebitAmount);
            result.TotalCredit = result.Rows.Sum(x => x.CreditAmount);
            result.ClosingBalance = result.OpeningBalance + result.TotalDebit - result.TotalCredit;

            return result;
        }

        public async Task<IncomeStatementResult> GetIncomeStatementAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var query = BuildBaseLinesQuery(fiscalPeriodId, postedOnly)
                .Where(x => x.FinancialAccount!.Category == AccountCategory.Revenue
                         || x.FinancialAccount!.Category == AccountCategory.Expense);

            if (fromDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var rows = await query
                .GroupBy(x => new
                {
                    x.FinancialAccountId,
                    x.FinancialAccount!.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new
                {
                    AccountId = g.Key.FinancialAccountId,
                    g.Key.AccountCode,
                    g.Key.AccountNameAr,
                    g.Key.Category,
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount)
                })
                .OrderBy(x => x.AccountCode)
                .ToListAsync();

            var result = new IncomeStatementResult
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate
            };

            foreach (var row in rows)
            {
                var amount = row.Category == AccountCategory.Revenue
                    ? row.TotalCredit - row.TotalDebit
                    : row.TotalDebit - row.TotalCredit;

                var dto = new IncomeStatementRow
                {
                    AccountId = row.AccountId,
                    AccountCode = row.AccountCode,
                    AccountNameAr = row.AccountNameAr,
                    Category = row.Category.ToString(),
                    TotalDebit = row.TotalDebit,
                    TotalCredit = row.TotalCredit,
                    Amount = amount
                };

                if (row.Category == AccountCategory.Revenue)
                    result.RevenueRows.Add(dto);
                else if (row.Category == AccountCategory.Expense)
                    result.ExpenseRows.Add(dto);
            }

            return result;
        }

        public async Task<BalanceSheetResult> GetBalanceSheetAsync(Guid? fiscalPeriodId, DateTime? asOfDate, bool postedOnly = true)
        {
            var query = BuildBaseLinesQuery(fiscalPeriodId, postedOnly)
                .Where(x => x.FinancialAccount!.Category == AccountCategory.Asset
                         || x.FinancialAccount!.Category == AccountCategory.Liability
                         || x.FinancialAccount!.Category == AccountCategory.Equity
                         || x.FinancialAccount!.Category == AccountCategory.Revenue
                         || x.FinancialAccount!.Category == AccountCategory.Expense);

            if (asOfDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date <= asOfDate.Value.Date);

            var rows = await query
                .GroupBy(x => new
                {
                    x.FinancialAccountId,
                    x.FinancialAccount!.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new
                {
                    AccountId = g.Key.FinancialAccountId,
                    g.Key.AccountCode,
                    g.Key.AccountNameAr,
                    g.Key.Category,
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount)
                })
                .OrderBy(x => x.AccountCode)
                .ToListAsync();

            var result = new BalanceSheetResult
            {
                FiscalPeriodId = fiscalPeriodId,
                AsOfDate = asOfDate
            };

            foreach (var row in rows)
            {
                if (row.Category == AccountCategory.Revenue)
                {
                    result.CurrentPeriodSurplusOrDeficit += row.TotalCredit - row.TotalDebit;
                    continue;
                }

                if (row.Category == AccountCategory.Expense)
                {
                    result.CurrentPeriodSurplusOrDeficit -= row.TotalDebit - row.TotalCredit;
                    continue;
                }

                var balance = row.Category == AccountCategory.Asset
                    ? row.TotalDebit - row.TotalCredit
                    : row.TotalCredit - row.TotalDebit;

                var dto = new BalanceSheetRow
                {
                    AccountId = row.AccountId,
                    AccountCode = row.AccountCode,
                    AccountNameAr = row.AccountNameAr,
                    Category = row.Category.ToString(),
                    TotalDebit = row.TotalDebit,
                    TotalCredit = row.TotalCredit,
                    Balance = balance
                };

                if (row.Category == AccountCategory.Asset)
                    result.AssetRows.Add(dto);
                else if (row.Category == AccountCategory.Liability)
                    result.LiabilityRows.Add(dto);
                else if (row.Category == AccountCategory.Equity)
                    result.EquityRows.Add(dto);
            }

            return result;
        }

        public async Task<RevenueExpenseSummaryResult> GetRevenueExpenseSummaryAsync(Guid? fiscalPeriodId, DateTime? fromDate, DateTime? toDate, bool postedOnly = true)
        {
            var income = await GetIncomeStatementAsync(fiscalPeriodId, fromDate, toDate, postedOnly);
            var balance = await GetBalanceSheetAsync(fiscalPeriodId, toDate, postedOnly);

            return new RevenueExpenseSummaryResult
            {
                FiscalPeriodId = fiscalPeriodId,
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = income.TotalRevenue,
                TotalExpense = income.TotalExpense,
                TotalAssets = balance.TotalAssets,
                TotalLiabilities = balance.TotalLiabilities,
                TotalEquity = balance.TotalEquityAfterSurplus
            };
        }

        private IQueryable<JournalEntryLine> BuildBaseLinesQuery(Guid? fiscalPeriodId, bool postedOnly)
        {
            var query = _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Include(x => x.FinancialAccount)
                .Include(x => x.CostCenter)
                .AsQueryable();

            if (postedOnly)
                query = query.Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted);

            if (fiscalPeriodId.HasValue)
                query = query.Where(x => x.JournalEntry!.FiscalPeriodId == fiscalPeriodId.Value);

            return query;
        }
    }
}
