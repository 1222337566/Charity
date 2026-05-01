using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastrfuctureManagmentCore.Queries.Reports;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InfrastructureManagmentDataAccess.Repositories.Reports
{
    public class CharityDashboardRepository : ICharityDashboardRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "charity_dashboard_snapshot";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public CharityDashboardRepository(
            IDbContextFactory<AppDbContext> factory,
            IMemoryCache cache)
        {
            _factory = factory;
            _cache   = cache;
        }

        public async Task<CharityDashboardSnapshotDto> GetSnapshotAsync(
            int donationMonths = 6,
            int topProjects    = 5,
            int payrollTake    = 6)
        {
            // Cache hit → فوري
            if (_cache.TryGetValue(CacheKey, out CharityDashboardSnapshotDto? cached) && cached != null)
                return cached;

            var today      = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-(donationMonths - 1));

            // ── كل query على DbContext مستقل → Task.WhenAll آمن ──
            // 40 query متتالية × 200ms = 8s  →  max(200ms) = ~200ms

            await using var c0  = await _factory.CreateDbContextAsync();
            await using var c1  = await _factory.CreateDbContextAsync();
            await using var c2  = await _factory.CreateDbContextAsync();
            await using var c3  = await _factory.CreateDbContextAsync();
            await using var c4  = await _factory.CreateDbContextAsync();
            await using var c5  = await _factory.CreateDbContextAsync();
            await using var c6  = await _factory.CreateDbContextAsync();
            await using var c7  = await _factory.CreateDbContextAsync();
            await using var c8  = await _factory.CreateDbContextAsync();
            await using var c9  = await _factory.CreateDbContextAsync();
            await using var c10 = await _factory.CreateDbContextAsync();
            await using var c11 = await _factory.CreateDbContextAsync();
            await using var c12 = await _factory.CreateDbContextAsync();
            await using var c13 = await _factory.CreateDbContextAsync();
            await using var c14 = await _factory.CreateDbContextAsync();
            await using var c15 = await _factory.CreateDbContextAsync();
            await using var c16 = await _factory.CreateDbContextAsync();
            await using var c17 = await _factory.CreateDbContextAsync();
            await using var c18 = await _factory.CreateDbContextAsync();
            await using var c19 = await _factory.CreateDbContextAsync();
            await using var c20 = await _factory.CreateDbContextAsync();
            await using var c21 = await _factory.CreateDbContextAsync();
            await using var c22 = await _factory.CreateDbContextAsync();
            await using var c23 = await _factory.CreateDbContextAsync();
            await using var c24 = await _factory.CreateDbContextAsync();
            await using var c25 = await _factory.CreateDbContextAsync();
            await using var c26 = await _factory.CreateDbContextAsync();
            await using var c27 = await _factory.CreateDbContextAsync();
            await using var c28 = await _factory.CreateDbContextAsync();
            await using var c29 = await _factory.CreateDbContextAsync();
            await using var c30 = await _factory.CreateDbContextAsync();
            await using var c31 = await _factory.CreateDbContextAsync();
            await using var c32 = await _factory.CreateDbContextAsync();
            await using var c33 = await _factory.CreateDbContextAsync();
            await using var c34 = await _factory.CreateDbContextAsync();
            await using var c35 = await _factory.CreateDbContextAsync();
            await using var c36 = await _factory.CreateDbContextAsync();
            await using var c37 = await _factory.CreateDbContextAsync();
            await using var c38 = await _factory.CreateDbContextAsync();

            // Group A — Counts & Totals
            var tBenef    = c0.Set<Beneficiary>().AsNoTracking().CountAsync();
            var tDonors   = c1.Set<Donor>().AsNoTracking().CountAsync();
            var tFunders  = c2.Set<Funder>().AsNoTracking().CountAsync();
            var tProjects = c3.Set<CharityProject>().AsNoTracking().CountAsync();
            var tEmployees= c4.Set<HrEmployee>().AsNoTracking().CountAsync();
            var tDonAmt   = c5.Set<Donation>().AsNoTracking()
                              .Where(x => x.Amount.HasValue).SumAsync(x => x.Amount ?? 0m);
            var tGrants   = c6.Set<GrantInstallment>().AsNoTracking()
                              .Where(x => x.ReceivedAmount.HasValue).SumAsync(x => x.ReceivedAmount ?? 0m);
            var tAid      = c7.Set<BeneficiaryAidDisbursement>().AsNoTracking()
                              .Where(x => x.Amount.HasValue).SumAsync(x => x.Amount ?? 0m);
            var tPayroll  = c8.Set<PayrollEmployee>().AsNoTracking().SumAsync(x => x.NetAmount);

            // Group B — Lookups & Groups
            var tStatuses     = c9.Set<BeneficiaryStatusLookup>().AsNoTracking()
                                  .OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync();
            var tStatusCounts = c10.Set<Beneficiary>().AsNoTracking()
                                   .GroupBy(x => x.StatusId)
                                   .Select(g => new { StatusId = g.Key, Count = g.Count() }).ToListAsync();
            var tMonthlyDon   = c11.Set<Donation>().AsNoTracking()
                                   .Where(x => x.DonationDate >= monthStart && x.Amount.HasValue)
                                   .GroupBy(x => new { x.DonationDate.Year, x.DonationDate.Month })
                                   .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.Amount ?? 0m) })
                                   .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tTopProj      = c12.Set<CharityProject>().AsNoTracking()
                                   .Select(x => new ProjectOverviewDto
                                   {
                                       ProjectId = x.Id, Code = x.Code, Name = x.Name,
                                       Status = x.Status, Budget = x.Budget,
                                       PlannedBudgetLines  = x.BudgetLines.Sum(b => b.PlannedAmount),
                                       ActualBudgetLines   = x.BudgetLines.Sum(b => b.ActualAmount),
                                       AllocatedGrants     = x.Grants.Sum(g => g.AllocatedAmount),
                                       BeneficiariesCount  = x.Beneficiaries.Count,
                                       ActivitiesCount     = x.Activities.Count
                                   })
                                   .OrderByDescending(x => x.Budget).ThenBy(x => x.Name).Take(topProjects).ToListAsync();
            var tPayrollRows  = c13.Set<PayrollMonth>().AsNoTracking()
                                   .Select(x => new PayrollMonthOverviewDto
                                   {
                                       PayrollMonthId = x.Id, Year = x.Year, Month = x.Month,
                                       Status = x.Status,
                                       EmployeesCount = x.Employees.Count,
                                       TotalNet = x.Employees.Sum(e => e.NetAmount)
                                   })
                                   .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Take(payrollTake).ToListAsync();
            var tReceipts     = c14.Set<CharityStoreReceipt>().AsNoTracking().Include(x => x.Warehouse).Include(x => x.Lines).ToListAsync();
            var tIssues       = c15.Set<CharityStoreIssue>().AsNoTracking().Include(x => x.Warehouse).Include(x => x.Lines).ToListAsync();

            // Group C — Kafala
            var tKafTotal    = c16.Set<KafalaCase>().AsNoTracking().CountAsync();
            var tKafActive   = c17.Set<KafalaCase>().AsNoTracking().CountAsync(x => x.Status == "Active");
            var tKafSusp     = c18.Set<KafalaCase>().AsNoTracking().CountAsync(x => x.Status == "Suspended");
            var tKafClosed   = c19.Set<KafalaCase>().AsNoTracking().CountAsync(x => x.Status == "Closed");
            var tKafSponsors = c20.Set<KafalaSponsor>().AsNoTracking().CountAsync(x => x.IsActive);
            var tKafCollect  = c21.Set<KafalaPayment>().AsNoTracking()
                                  .Where(x => x.Direction == "Received" && x.Status == "Confirmed")
                                  .SumAsync(x => (decimal?)x.Amount);
            var tKafDisburse = c22.Set<KafalaPayment>().AsNoTracking()
                                  .Where(x => x.Direction == "Disbursed" && x.Status == "Confirmed")
                                  .SumAsync(x => (decimal?)x.Amount);
            var tKafByType   = c23.Set<KafalaCase>().AsNoTracking()
                                  .GroupBy(x => x.SponsorshipType)
                                  .Select(g => new StatusCountDto { Name = g.Key, Count = g.Count() }).ToListAsync();
            var tKafByFreq   = c24.Set<KafalaCase>().AsNoTracking()
                                  .Where(x => x.Status == "Active")
                                  .GroupBy(x => x.Frequency)
                                  .Select(g => new StatusCountDto { Name = g.Key, Count = g.Count() }).ToListAsync();
            var tKafMonCollect= c25.Set<KafalaPayment>().AsNoTracking()
                                   .Where(x => x.Direction == "Received" && x.Status == "Confirmed" && x.PaymentDate >= monthStart)
                                   .GroupBy(x => new { x.PaymentDate.Year, x.PaymentDate.Month })
                                   .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.Amount) })
                                   .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tKafMonDisb  = c26.Set<KafalaPayment>().AsNoTracking()
                                  .Where(x => x.Direction == "Disbursed" && x.Status == "Confirmed" && x.PaymentDate >= monthStart)
                                  .GroupBy(x => new { x.PaymentDate.Year, x.PaymentDate.Month })
                                  .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.Amount) })
                                  .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

            // Group D — Accounting
            var tAssets  = c27.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Asset)
                              .SumAsync(x => (decimal?)(x.DebitAmount - x.CreditAmount));
            var tLiab    = c28.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Liability)
                              .SumAsync(x => (decimal?)(x.CreditAmount - x.DebitAmount));
            var tEquity  = c29.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Equity)
                              .SumAsync(x => (decimal?)(x.CreditAmount - x.DebitAmount));
            var tRev     = c30.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Revenue)
                              .SumAsync(x => (decimal?)(x.CreditAmount - x.DebitAmount));
            var tExp     = c31.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Expense)
                              .SumAsync(x => (decimal?)(x.DebitAmount - x.CreditAmount));
            var tAccByCat= c32.Set<FinancialAccount>().AsNoTracking()
                              .Where(x => x.IsActive && x.IsPosting)
                              .GroupBy(x => x.Category)
                              .Select(g => new StatusCountDto { Name = g.Key.ToString(), Count = g.Count() }).ToListAsync();
            var tMonRev  = c33.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Revenue && x.JournalEntry.EntryDate >= monthStart)
                              .GroupBy(x => new { x.JournalEntry!.EntryDate.Year, x.JournalEntry.EntryDate.Month })
                              .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.CreditAmount - x.DebitAmount) })
                              .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tMonExp  = c34.Set<JournalEntryLine>().AsNoTracking()
                              .Where(x => x.JournalEntry!.Status == JournalEntryStatus.Posted && x.FinancialAccount!.Category == AccountCategory.Expense && x.JournalEntry.EntryDate >= monthStart)
                              .GroupBy(x => new { x.JournalEntry!.EntryDate.Year, x.JournalEntry.EntryDate.Month })
                              .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.DebitAmount - x.CreditAmount) })
                              .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

            // Group E — Historical
            var tMonBenef = c35.Set<Beneficiary>().AsNoTracking()
                               .Where(x => x.CreatedAtUtc >= monthStart)
                               .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
                               .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Count() })
                               .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tMonAidReq= c36.Set<BeneficiaryAidRequest>().AsNoTracking()
                               .Where(x => x.CreatedAtUtc >= monthStart)
                               .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
                               .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Count() })
                               .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tMonAidDisb=c37.Set<BeneficiaryAidDisbursement>().AsNoTracking()
                               .Where(x => x.DisbursementDate >= monthStart)
                               .GroupBy(x => new { x.DisbursementDate.Year, x.DisbursementDate.Month })
                               .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Count() })
                               .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

            await using var c39 = await _factory.CreateDbContextAsync();
            await using var c40 = await _factory.CreateDbContextAsync();
            await using var c41 = await _factory.CreateDbContextAsync();
            await using var c42 = await _factory.CreateDbContextAsync();
            await using var c43 = await _factory.CreateDbContextAsync();
            await using var c44 = await _factory.CreateDbContextAsync();

            var tMonAidAmt = c38.Set<BeneficiaryAidDisbursement>().AsNoTracking()
                                .Where(x => x.DisbursementDate >= monthStart && x.Amount.HasValue)
                                .GroupBy(x => new { x.DisbursementDate.Year, x.DisbursementDate.Month })
                                .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.Amount ?? 0m) })
                                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tMonNewDon = c39.Set<Donor>().AsNoTracking()
                                .Where(x => x.CreatedAtUtc >= monthStart)
                                .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
                                .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Count() })
                                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tMonGrants = c40.Set<GrantInstallment>().AsNoTracking()
                                .Where(x => x.ReceivedDate.HasValue && x.ReceivedDate.Value >= monthStart && x.ReceivedAmount.HasValue)
                                .GroupBy(x => new { x.ReceivedDate!.Value.Year, x.ReceivedDate.Value.Month })
                                .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.ReceivedAmount ?? 0m) })
                                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tGrantStatus=c41.Set<GrantAgreement>().AsNoTracking()
                                .GroupBy(x => x.Status)
                                .Select(g => new StatusCountDto { Name = g.Key, Count = g.Count() }).ToListAsync();
            var tMonNewProj= c42.Set<CharityProject>().AsNoTracking()
                                .Where(x => x.CreatedAtUtc >= monthStart)
                                .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
                                .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Count() })
                                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            var tProjStatus= c43.Set<CharityProject>().AsNoTracking()
                                .GroupBy(x => x.Status)
                                .Select(g => new StatusCountDto { Name = g.Key, Count = g.Count() }).ToListAsync();
            var tMonProjBud= c44.Set<CharityProject>().AsNoTracking()
                                .Where(x => x.CreatedAtUtc >= monthStart)
                                .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
                                .Select(g => new MonthlyAmountDto { Year = g.Key.Year, Month = g.Key.Month, Amount = g.Sum(x => x.Budget) })
                                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

            // كلهم يبدأوا مع بعض — ننتظر الأبطأ واحدة
            await Task.WhenAll(
                tBenef, tDonors, tFunders, tProjects, tEmployees,
                tDonAmt, tGrants, tAid, tPayroll,
                tStatuses, tStatusCounts, tMonthlyDon, tTopProj, tPayrollRows, tReceipts, tIssues,
                tKafTotal, tKafActive, tKafSusp, tKafClosed, tKafSponsors,
                tKafCollect, tKafDisburse, tKafByType, tKafByFreq, tKafMonCollect, tKafMonDisb,
                tAssets, tLiab, tEquity, tRev, tExp, tAccByCat, tMonRev, tMonExp,
                tMonBenef, tMonAidReq, tMonAidDisb, tMonAidAmt, tMonNewDon,
                tMonGrants, tGrantStatus, tMonNewProj, tProjStatus, tMonProjBud
            );

            // تجميع store movements
            var receipts     = await tReceipts;
            var issues       = await tIssues;
            var statuses     = await tStatuses;
            var statusCounts = await tStatusCounts;

            var storeMovements = receipts
                .GroupBy(x => new { x.WarehouseId, Name = x.Warehouse?.WarehouseNameAr ?? string.Empty })
                .Select(g => new StoreMovementOverviewDto
                {
                    WarehouseId      = g.Key.WarehouseId,
                    WarehouseName    = g.Key.Name,
                    ReceiptsCount    = g.Count(),
                    ReceiptQuantity  = g.SelectMany(x => x.Lines).Sum(l => l.Quantity),
                    IssuesCount      = issues.Count(i => i.WarehouseId == g.Key.WarehouseId),
                    IssueQuantity    = issues.Where(i => i.WarehouseId == g.Key.WarehouseId)
                                             .SelectMany(i => i.Lines).Sum(l => l.Quantity)
                })
                .OrderByDescending(x => x.ReceiptQuantity + x.IssueQuantity)
                .ToList();

            var snapshot = new CharityDashboardSnapshotDto
            {
                BeneficiariesCount   = await tBenef,
                DonorsCount          = await tDonors,
                FundersCount         = await tFunders,
                ProjectsCount        = await tProjects,
                EmployeesCount       = await tEmployees,
                TotalDonations       = await tDonAmt,
                TotalReceivedGrants  = await tGrants,
                TotalAidDisbursed    = await tAid,
                TotalPayrollNet      = await tPayroll,
                BeneficiaryStatuses  = statuses.Select(s => new StatusCountDto
                {
                    Name  = s.NameAr,
                    Count = statusCounts.FirstOrDefault(x => x.StatusId == s.Id)?.Count ?? 0
                }).ToList(),
                MonthlyDonations     = await tMonthlyDon,
                TopProjects          = await tTopProj,
                RecentPayrollMonths  = await tPayrollRows,
                StoreMovements       = storeMovements,
                // Kafala
                KafalaCasesTotal        = await tKafTotal,
                KafalaCasesActive       = await tKafActive,
                KafalaCasesSuspended    = await tKafSusp,
                KafalaCasesClosed       = await tKafClosed,
                KafalaSponsorsCount     = await tKafSponsors,
                KafalaTotalCollected    = await tKafCollect  ?? 0m,
                KafalaTotalDisbursed    = await tKafDisburse ?? 0m,
                KafalaBySponsorshipType = await tKafByType,
                KafalaByFrequency       = await tKafByFreq,
                KafalaMonthlyCollected  = await tKafMonCollect,
                KafalaMonthlyDisbursed  = await tKafMonDisb,
                // Accounting
                TotalAssets          = await tAssets  ?? 0m,
                TotalLiabilities     = await tLiab    ?? 0m,
                TotalEquity          = await tEquity  ?? 0m,
                TotalRevenue         = await tRev     ?? 0m,
                TotalExpenses        = await tExp     ?? 0m,
                AccountsByCategory   = await tAccByCat,
                MonthlyRevenue       = await tMonRev,
                MonthlyExpenses      = await tMonExp,
                // Historical
                MonthlyNewBeneficiaries   = await tMonBenef,
                MonthlyAidRequests        = await tMonAidReq,
                MonthlyAidDisbursements   = await tMonAidDisb,
                MonthlyAidDisbursedAmount = await tMonAidAmt,
                MonthlyNewDonors          = await tMonNewDon,
                MonthlyGrantsReceived     = await tMonGrants,
                GrantAgreementsByStatus   = await tGrantStatus,
                MonthlyNewProjects        = await tMonNewProj,
                ProjectsByStatus          = await tProjStatus,
                MonthlyProjectBudget      = await tMonProjBud,
            };

            _cache.Set(CacheKey, snapshot, CacheTtl);
            return snapshot;
        }
    }
}
