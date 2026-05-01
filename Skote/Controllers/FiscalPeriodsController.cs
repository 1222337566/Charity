using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentWebFramework.Models.Accounting;
using Microsoft.AspNetCore.Mvc;

public class FiscalPeriodsController : Controller
{
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly AppDbContext _db;

    public FiscalPeriodsController(IFiscalPeriodRepository fiscalPeriodRepository, AppDbContext db)
    {
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _fiscalPeriodRepository.GetAllAsync();
        var model = items.Select(x => new FiscalPeriodListItemVm
        {
            Id = x.Id,
            PeriodCode = x.PeriodCode,
            PeriodNameAr = x.PeriodNameAr,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            IsCurrent = x.IsCurrent,
            IsClosed = x.IsClosed
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateFiscalPeriodVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFiscalPeriodVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (vm.EndDate < vm.StartDate)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
            return View(vm);
        }

        if (await _fiscalPeriodRepository.CodeExistsAsync(vm.PeriodCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.PeriodCode), "كود الفترة موجود بالفعل");
            return View(vm);
        }

        if (await _fiscalPeriodRepository.HasOverlappingPeriodAsync(vm.StartDate, vm.EndDate))
        {
            ModelState.AddModelError(string.Empty, "هناك فترة مالية متداخلة مع نفس المدة");
            return View(vm);
        }

        var entity = new FiscalPeriod
        {
            Id = Guid.NewGuid(),
            PeriodCode = vm.PeriodCode.Trim(),
            PeriodNameAr = vm.PeriodNameAr.Trim(),
            PeriodNameEn = vm.PeriodNameEn?.Trim(),
            StartDate = vm.StartDate.Date,
            EndDate = vm.EndDate.Date,
            IsCurrent = vm.IsCurrent,
            IsClosed = false,
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _fiscalPeriodRepository.AddAsync(entity);
        if (entity.IsCurrent)
            await _fiscalPeriodRepository.ClearCurrentFlagAsync(entity.Id);

        TempData["Success"] = "تم إضافة الفترة المالية بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _fiscalPeriodRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditFiscalPeriodVm
        {
            Id = entity.Id,
            PeriodCode = entity.PeriodCode,
            PeriodNameAr = entity.PeriodNameAr,
            PeriodNameEn = entity.PeriodNameEn,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsCurrent = entity.IsCurrent,
            IsClosed = entity.IsClosed,
            Notes = entity.Notes
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditFiscalPeriodVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _fiscalPeriodRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (vm.EndDate < vm.StartDate)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
            return View(vm);
        }

        if (await _fiscalPeriodRepository.CodeExistsAsync(vm.PeriodCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.PeriodCode), "كود الفترة موجود بالفعل");
            return View(vm);
        }

        if (await _fiscalPeriodRepository.HasOverlappingPeriodAsync(vm.StartDate, vm.EndDate, vm.Id))
        {
            ModelState.AddModelError(string.Empty, "هناك فترة مالية متداخلة مع نفس المدة");
            return View(vm);
        }

        entity.PeriodCode = vm.PeriodCode.Trim();
        entity.PeriodNameAr = vm.PeriodNameAr.Trim();
        entity.PeriodNameEn = vm.PeriodNameEn?.Trim();
        entity.StartDate = vm.StartDate.Date;
        entity.EndDate = vm.EndDate.Date;
        entity.IsCurrent = vm.IsCurrent;
        entity.IsClosed = vm.IsClosed;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _fiscalPeriodRepository.UpdateAsync(entity);
        if (entity.IsCurrent)
            await _fiscalPeriodRepository.ClearCurrentFlagAsync(entity.Id);

        TempData["Success"] = "تم تعديل الفترة المالية بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCurrent(Guid id)
    {
        var all = await _fiscalPeriodRepository.GetAllAsync();
        foreach (var fp in all) { fp.IsCurrent = false; }
        var target = all.FirstOrDefault(x => x.Id == id);
        if (target != null) target.IsCurrent = true;
        await _db.SaveChangesAsync();
        TempData["Success"] = "تم تعيين الفترة الحالية بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ClosePeriodAndCarryForward(
        Guid periodId, Guid nextPeriodId,
        string? retainedEarningsAccount,
        bool closeIncomeAccounts, bool carryBalanceAccounts)
    {
        try
        {
            var period     = await _db.Set<FiscalPeriod>().FindAsync(periodId);
            var nextPeriod = await _db.Set<FiscalPeriod>().FindAsync(nextPeriodId);
            if (period == null || nextPeriod == null)
            {
                TempData["Error"] = "الفترة المالية غير موجودة";
                return RedirectToAction(nameof(Index));
            }
            if (!period.IsClosed)
            {
                TempData["Error"] = "يجب إغلاق الفترة أولاً قبل الترحيل";
                return RedirectToAction(nameof(Index));
            }

            var entryDate = period.EndDate;
            var entryNum  = $"CLOSE-{period.PeriodCode}-{DateTime.Now:HHmmss}";

            // ── جيب الحسابات المتحركة خلال الفترة ──
            var lines = await _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Include(x => x.FinancialAccount)
                .Where(x => x.JournalEntry!.FiscalPeriodId == periodId
                          && x.JournalEntry.Status == JournalEntryStatus.Posted)
                .ToListAsync();

            // احسب رصيد كل حساب خلال الفترة
            var balances = lines
                .GroupBy(x => x.FinancialAccountId)
                .Select(g => new {
                    AccountId = g.Key,
                    Account   = g.First().FinancialAccount,
                    NetBalance = g.Sum(l => l.DebitAmount - l.CreditAmount)
                })
                .Where(x => x.NetBalance != 0)
                .ToList();

            var closingLines = new List<JournalEntryLine>();
            decimal netIncome = 0;

            // ── خطوة 1: إغلاق الإيرادات والمصروفات ──
            if (closeIncomeAccounts)
            {
                foreach (var bal in balances.Where(b => b.Account?.Category == AccountCategory.Revenue
                                                     || b.Account?.Category == AccountCategory.Expense))
                {
                    netIncome += bal.Account!.Category == AccountCategory.Revenue
                        ? bal.NetBalance : -bal.NetBalance;

                    closingLines.Add(new JournalEntryLine
                    {
                        Id           = Guid.NewGuid(),
                        FinancialAccountId = bal.AccountId,
                        Description  = $"إغلاق {bal.Account.AccountNameAr}",
                        DebitAmount  = bal.NetBalance < 0 ? Math.Abs(bal.NetBalance) : 0,
                        CreditAmount = bal.NetBalance > 0 ? bal.NetBalance : 0,
                    });
                }
            }

            // ── خطوة 2: ترحيل أرصدة الميزانية للفترة الجديدة ──
            if (carryBalanceAccounts)
            {
                var balanceAccounts = balances.Where(b =>
                    b.Account?.Category == AccountCategory.Asset    ||
                    b.Account?.Category == AccountCategory.Liability ||
                    b.Account?.Category == AccountCategory.Equity);

                foreach (var bal in balanceAccounts)
                {
                    // قيد ترحيل افتتاحي للفترة الجديدة
                    var openingEntry = new JournalEntry
                    {
                        Id             = Guid.NewGuid(),
                        EntryNumber    = $"OB-{nextPeriod.PeriodCode}-{bal.AccountId.ToString()[..6].ToUpper()}",
                        EntryDate      = nextPeriod.StartDate,
                        Description    = $"رصيد افتتاحي من {period.PeriodNameAr}",
                        FiscalPeriodId = nextPeriodId,
                        Status         = JournalEntryStatus.Posted,
                        TotalDebit     = bal.NetBalance > 0 ? bal.NetBalance : 0,
                        TotalCredit    = bal.NetBalance < 0 ? Math.Abs(bal.NetBalance) : 0,
                        Lines          = new List<JournalEntryLine>
                        {
                            new JournalEntryLine {
                                Id           = Guid.NewGuid(),
                                FinancialAccountId    = bal.AccountId,
                                Description  = $"رصيد مرحّل",
                                DebitAmount  = bal.NetBalance > 0 ? bal.NetBalance : 0,
                                CreditAmount = bal.NetBalance < 0 ? Math.Abs(bal.NetBalance) : 0,
                            }
                        }
                    };
                    _db.Set<JournalEntry>().Add(openingEntry);
                }
            }

            // ── حفظ قيد الإغلاق ──
            if (closingLines.Any())
            {
                var closingEntry = new JournalEntry
                {
                    Id             = Guid.NewGuid(),
                    EntryNumber    = entryNum,
                    EntryDate      = entryDate,
                    Description    = $"قيد إغلاق الفترة {period.PeriodNameAr}",
                    FiscalPeriodId = periodId,
                    Status         = JournalEntryStatus.Posted,
                    TotalDebit     = closingLines.Sum(l => l.DebitAmount),
                    TotalCredit    = closingLines.Sum(l => l.CreditAmount),
                    Lines          = closingLines
                };
                _db.Set<JournalEntry>().Add(closingEntry);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم ترحيل الأرصدة بنجاح إلى {nextPeriod.PeriodNameAr} — قيود الإغلاق: {closingLines.Count}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "خطأ في الترحيل: " + ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleClosed(Guid id)
    {
        var entity = await _fiscalPeriodRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.IsClosed = !entity.IsClosed;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _fiscalPeriodRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsClosed ? "تم قفل الفترة بنجاح" : "تم إعادة فتح الفترة بنجاح";
        return RedirectToAction(nameof(Index));
    }
}
