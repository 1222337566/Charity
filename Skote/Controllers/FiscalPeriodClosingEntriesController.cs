using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.FiscalPeriodClosingEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class FiscalPeriodClosingEntriesController : Controller
    {
        private const string ClosingSourceType = "FiscalPeriodClosing";

        private readonly AppDbContext _db;

        public FiscalPeriodClosingEntriesController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var periods = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

            var vm = new FiscalPeriodClosingEntriesIndexVm();

            foreach (var period in periods)
            {
                var income = await BuildIncomeSummaryAsync(period, ct);

                var closingEntry = await _db.Set<JournalEntry>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.SourceType == ClosingSourceType &&
                        x.SourceId == period.Id &&
                        x.Status != JournalEntryStatus.Reversed,
                        ct);

                vm.Periods.Add(new FiscalPeriodClosingEntryPeriodRowVm
                {
                    FiscalPeriodId = period.Id,
                    PeriodName = period.PeriodNameAr,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    IsClosed = period.IsClosed,
                    HasClosingEntry = closingEntry != null,
                    ClosingEntryId = closingEntry?.Id,
                    ClosingEntryNumber = closingEntry?.EntryNumber,
                    TotalRevenue = income.TotalRevenue,
                    TotalExpense = income.TotalExpense
                });
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Prepare(Guid fiscalPeriodId, CancellationToken ct)
        {
            var vm = await BuildPrepareVmAsync(fiscalPeriodId, ct);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFiscalPeriodClosingEntryVm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });

            var period = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.FiscalPeriodId, ct);

            if (period == null)
                return NotFound();

            var existing = await _db.Set<JournalEntry>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.SourceType == ClosingSourceType &&
                    x.SourceId == period.Id &&
                    x.Status != JournalEntryStatus.Reversed,
                    ct);

            if (existing)
            {
                TempData["Error"] = "يوجد قيد إقفال سابق لهذه الفترة.";
                return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });
            }

            var retainedAccount = await _db.Set<FinancialAccount>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == model.RetainedSurplusAccountId, ct);

            if (retainedAccount == null)
            {
                TempData["Error"] = "حساب الفائض/العجز المرحّل غير صحيح.";
                return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });
            }

            var income = await BuildIncomeSummaryAsync(period, ct);

            if (!income.RevenueRows.Any() && !income.ExpenseRows.Any())
            {
                TempData["Error"] = "لا توجد أرصدة إيرادات أو مصروفات لإقفالها.";
                return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });
            }

            var entry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryNumber = await GenerateEntryNumberAsync(ct),
                PostedAtUtc = model.ClosingEntryDateUtc,
                Description = $"قيد إقفال الفترة المالية {period.PeriodNameAr}",
                SourceType = ClosingSourceType,
                SourceId = period.Id,
             
                Status = JournalEntryStatus.Posted,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Lines = new List<JournalEntryLine>()
            };

            // إقفال الإيرادات: مدين الإيراد
            foreach (var rev in income.RevenueRows.Where(x => x.NetCreditBalance > 0))
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    FinancialAccountId = rev.AccountId,
                    DebitAmount = rev.NetCreditBalance,
                    CreditAmount = 0,
                    Description = $"إقفال إيراد: {rev.AccountNameAr}"
                });
            }

            // إقفال المصروفات: دائن المصروف
            foreach (var exp in income.ExpenseRows.Where(x => x.NetDebitBalance > 0))
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    FinancialAccountId = exp.AccountId,
                    DebitAmount = 0,
                    CreditAmount = exp.NetDebitBalance,
                    Description = $"إقفال مصروف: {exp.AccountNameAr}"
                });
            }

            // ترحيل صافي الفائض/العجز إلى حساب حقوق الملكية / صافي الأصول
            if (income.NetSurplusOrDeficit > 0)
            {
                // فائض: دائن الفائض المرحل
                entry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    FinancialAccountId = model.RetainedSurplusAccountId,
                    DebitAmount = 0,
                    CreditAmount = income.NetSurplusOrDeficit,
                    Description = "ترحيل صافي فائض الفترة"
                });
            }
            else if (income.NetSurplusOrDeficit < 0)
            {
                // عجز: مدين الفائض/العجز المرحل
                entry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    FinancialAccountId = model.RetainedSurplusAccountId,
                    DebitAmount = Math.Abs(income.NetSurplusOrDeficit),
                    CreditAmount = 0,
                    Description = "ترحيل صافي عجز الفترة"
                });
            }

            var totalDebit = entry.Lines.Sum(x => x.DebitAmount);
            var totalCredit = entry.Lines.Sum(x => x.CreditAmount);

            if (totalDebit != totalCredit)
            {
                TempData["Error"] = $"قيد الإقفال غير متوازن. مدين: {totalDebit:N2} / دائن: {totalCredit:N2}";
                return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });
            }

            _db.Set<JournalEntry>().Add(entry);
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = $"تم إنشاء قيد الإقفال رقم {entry.EntryNumber}.";
            return RedirectToAction(nameof(Prepare), new { fiscalPeriodId = model.FiscalPeriodId });
        }

        private async Task<FiscalPeriodClosingEntryPrepareVm?> BuildPrepareVmAsync(Guid fiscalPeriodId, CancellationToken ct)
        {
            var period = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == fiscalPeriodId, ct);

            if (period == null)
                return null;

            var income = await BuildIncomeSummaryAsync(period, ct);

            var existing = await _db.Set<JournalEntry>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.SourceType == ClosingSourceType &&
                    x.SourceId == period.Id &&
                    x.Status != JournalEntryStatus.Reversed,
                    ct);

            var equityAccounts = await _db.Set<FinancialAccount>()
                .AsNoTracking()
                .Where(x => x.Category == AccountCategory.Equity && x.IsActive)
                .OrderBy(x => x.AccountCode)
                .ToListAsync(ct);

            return new FiscalPeriodClosingEntryPrepareVm
            {
                FiscalPeriodId = period.Id,
                PeriodName = period.PeriodNameAr,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                HasClosingEntry = existing != null,
                ExistingClosingEntryId = existing?.Id,
                ExistingClosingEntryNumber = existing?.EntryNumber,
                TotalRevenue = income.TotalRevenue,
                TotalExpense = income.TotalExpense,
                ClosingEntryDateUtc = period.EndDate,
                Notes = $"قيد إقفال الفترة {period.PeriodNameAr}",
                RevenueRows = income.RevenueRows,
                ExpenseRows = income.ExpenseRows,
                EquityAccounts = equityAccounts.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.AccountCode} - {x.AccountNameAr}"
                }).ToList()
            };
        }

        private async Task<IncomeSummary> BuildIncomeSummaryAsync(FiscalPeriod period, CancellationToken ct)
        {
            var lines = await _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Include(x => x.FinancialAccount)
                .Where(x =>
                    x.JournalEntry != null &&
                    x.FinancialAccount != null &&
                    x.JournalEntry.PostedAtUtc >= period.StartDate &&
                    x.JournalEntry.PostedAtUtc <= period.EndDate &&
                    x.JournalEntry.Status == JournalEntryStatus.Posted &&
                    x.JournalEntry.Status != JournalEntryStatus.Reversed &&
                    x.JournalEntry.SourceType != ClosingSourceType &&
                    (x.FinancialAccount.Category == AccountCategory.Revenue ||
                     x.FinancialAccount.Category == AccountCategory.Expense))
                .ToListAsync(ct);

            var grouped = lines
                .GroupBy(x => new
                {
                    x.FinancialAccount,
                    x.FinancialAccount.AccountCode,
                    x.FinancialAccount.AccountNameAr,
                    x.FinancialAccount.Category
                })
                .Select(g => new FiscalPeriodClosingEntryAccountRowVm
                {
                    AccountId = g.Key.FinancialAccount.Id,
                    AccountCode = g.Key.AccountCode,
                    AccountNameAr = g.Key.AccountNameAr,
                    Category = g.Key.Category.ToString(),
                    DebitAmount = g.Sum(x => x.DebitAmount),
                    CreditAmount = g.Sum(x => x.CreditAmount)
                })
                .ToList();

            var result = new IncomeSummary
            {
                RevenueRows = grouped
                    .Where(x => x.Category == AccountCategory.Revenue.ToString())
                    .Where(x => x.NetCreditBalance > 0)
                    .OrderBy(x => x.AccountCode)
                    .ToList(),

                ExpenseRows = grouped
                    .Where(x => x.Category == AccountCategory.Expense.ToString())
                    .Where(x => x.NetDebitBalance > 0)
                    .OrderBy(x => x.AccountCode)
                    .ToList()
            };

            return result;
        }

        private async Task<string> GenerateEntryNumberAsync(CancellationToken ct)
        {
            var count = await _db.Set<JournalEntry>().CountAsync(ct);
            return $"JE-CLOSE-{DateTime.Now:yyyyMM}-{(count + 1):0000}";
        }

        private class IncomeSummary
        {
            public List<FiscalPeriodClosingEntryAccountRowVm> RevenueRows { get; set; } = new();

            public List<FiscalPeriodClosingEntryAccountRowVm> ExpenseRows { get; set; } = new();

            public decimal TotalRevenue => RevenueRows.Sum(x => x.NetCreditBalance);

            public decimal TotalExpense => ExpenseRows.Sum(x => x.NetDebitBalance);

            public decimal NetSurplusOrDeficit => TotalRevenue - TotalExpense;
        }
    }
}
