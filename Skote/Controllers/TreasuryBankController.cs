using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Accounting.Treasury;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.TreasuryBank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class TreasuryBankController : Controller
{
    private readonly AppDbContext _db;

    public TreasuryBankController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var cashBankAccounts = await GetCashBankAccountsAsync();
        var totalBalance = 0m;
        foreach (var account in cashBankAccounts)
            totalBalance += await GetAccountBalanceAsync(account.Id, DateTime.Today);

        var recentVouchers = await _db.Set<CashBankVoucher>()
            .AsNoTracking()
            .Include(x => x.FinancialAccount)
            .Include(x => x.OppositeAccount)
            .OrderByDescending(x => x.VoucherDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Take(10)
            .Select(x => new CashBankVoucherListItemVm
            {
                Id = x.Id,
                VoucherNumber = x.VoucherNumber,
                VoucherDate = x.VoucherDate,
                VoucherTypeName = x.VoucherType == CashBankVoucherType.Receipt ? "سند قبض" : "سند صرف",
                FinancialAccountName = x.FinancialAccount == null ? "" : x.FinancialAccount.AccountNameAr,
                OppositeAccountName = x.OppositeAccount == null ? "" : x.OppositeAccount.AccountNameAr,
                Amount = x.Amount,
                StatusName = x.Status == CashBankVoucherStatus.Posted ? "مرحل" : x.Status.ToString(),
                JournalEntryId = x.JournalEntryId,
                Notes = x.Notes
            })
            .ToListAsync();

        var recentTransfers = await _db.Set<TreasuryBankTransfer>()
            .AsNoTracking()
            .Include(x => x.FromFinancialAccount)
            .Include(x => x.ToFinancialAccount)
            .OrderByDescending(x => x.TransferDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Take(10)
            .Select(x => new TreasuryBankTransferListItemVm
            {
                Id = x.Id,
                TransferNumber = x.TransferNumber,
                TransferDate = x.TransferDate,
                FromAccountName = x.FromFinancialAccount == null ? "" : x.FromFinancialAccount.AccountNameAr,
                ToAccountName = x.ToFinancialAccount == null ? "" : x.ToFinancialAccount.AccountNameAr,
                Amount = x.Amount,
                StatusName = x.Status == TreasuryBankTransferStatus.Posted ? "مرحل" : x.Status.ToString(),
                JournalEntryId = x.JournalEntryId,
                Notes = x.Notes
            })
            .ToListAsync();

        var model = new TreasuryBankDashboardVm
        {
            CashBankAccountCount = cashBankAccounts.Count,
            TreasuryAccountCount = cashBankAccounts.Count(x => x.CashKind == FinancialAccountCashKind.Treasury),
            BankAccountCount = cashBankAccounts.Count(x => x.CashKind == FinancialAccountCashKind.Bank),
            TotalCashBankBalance = totalBalance,
            RecentVouchers = recentVouchers,
            RecentTransfers = recentTransfers
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Receipt()
    {
        var vm = new CreateCashBankVoucherVm
        {
            VoucherDate = DateTime.Today,
            Description = "سند قبض"
        };
        await FillVoucherLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receipt(CreateCashBankVoucherVm vm)
    {
        await FillVoucherLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var validation = await ValidateCashVoucherAccountsAsync(vm.FinancialAccountId, vm.OppositeAccountId);
        if (!validation.IsValid)
        {
            ModelState.AddModelError(string.Empty, validation.Message);
            return View(vm);
        }

        var voucher = new CashBankVoucher
        {
            Id = Guid.NewGuid(),
            VoucherNumber = await GenerateDocumentNumberAsync("RCV", vm.VoucherDate),
            VoucherDate = vm.VoucherDate.Date,
            VoucherType = CashBankVoucherType.Receipt,
            Status = CashBankVoucherStatus.Posted,
            FinancialAccountId = vm.FinancialAccountId,
            OppositeAccountId = vm.OppositeAccountId,
            Amount = vm.Amount,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedByUserName = User.Identity?.Name,
            CreatedAtUtc = DateTime.UtcNow
        };

        await using var tx = await _db.Database.BeginTransactionAsync();
        await _db.Set<CashBankVoucher>().AddAsync(voucher);
        await _db.SaveChangesAsync();

        var entry = await CreateJournalEntryAsync(
            AccountingSourceTypes.CashReceiptVoucher,
            voucher.Id,
            voucher.VoucherDate,
            $"سند قبض رقم {voucher.VoucherNumber} - {voucher.Description}",
            debitAccountId: voucher.FinancialAccountId,
            creditAccountId: voucher.OppositeAccountId,
            amount: voucher.Amount);

        voucher.JournalEntryId = entry.Id;
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["Success"] = $"تم تسجيل سند القبض وترحيله بقيد رقم {entry.EntryNumber}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Payment()
    {
        var vm = new CreateCashBankVoucherVm
        {
            VoucherDate = DateTime.Today,
            Description = "سند صرف"
        };
        await FillVoucherLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(CreateCashBankVoucherVm vm)
    {
        await FillVoucherLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var validation = await ValidateCashVoucherAccountsAsync(vm.FinancialAccountId, vm.OppositeAccountId);
        if (!validation.IsValid)
        {
            ModelState.AddModelError(string.Empty, validation.Message);
            return View(vm);
        }

        var cashAccount = validation.CashAccount!;
        if (!cashAccount.AllowNegativeCashBalance)
        {
            var balance = await GetAccountBalanceAsync(cashAccount.Id, vm.VoucherDate);
            if (balance < vm.Amount)
            {
                ModelState.AddModelError(nameof(vm.Amount), $"رصيد الحساب غير كافٍ. الرصيد الحالي {balance:N2}.");
                return View(vm);
            }
        }

        var voucher = new CashBankVoucher
        {
            Id = Guid.NewGuid(),
            VoucherNumber = await GenerateDocumentNumberAsync("PAY", vm.VoucherDate),
            VoucherDate = vm.VoucherDate.Date,
            VoucherType = CashBankVoucherType.Payment,
            Status = CashBankVoucherStatus.Posted,
            FinancialAccountId = vm.FinancialAccountId,
            OppositeAccountId = vm.OppositeAccountId,
            Amount = vm.Amount,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedByUserName = User.Identity?.Name,
            CreatedAtUtc = DateTime.UtcNow
        };

        await using var tx = await _db.Database.BeginTransactionAsync();
        await _db.Set<CashBankVoucher>().AddAsync(voucher);
        await _db.SaveChangesAsync();

        var entry = await CreateJournalEntryAsync(
            AccountingSourceTypes.CashPaymentVoucher,
            voucher.Id,
            voucher.VoucherDate,
            $"سند صرف رقم {voucher.VoucherNumber} - {voucher.Description}",
            debitAccountId: voucher.OppositeAccountId,
            creditAccountId: voucher.FinancialAccountId,
            amount: voucher.Amount);

        voucher.JournalEntryId = entry.Id;
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["Success"] = $"تم تسجيل سند الصرف وترحيله بقيد رقم {entry.EntryNumber}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Transfer()
    {
        var vm = new CreateTreasuryBankTransferVm
        {
            TransferDate = DateTime.Today,
            Description = "تحويل بين خزينة وبنك"
        };
        await FillTransferLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(CreateTreasuryBankTransferVm vm)
    {
        await FillTransferLookupsAsync(vm);

        if (vm.FromFinancialAccountId == vm.ToFinancialAccountId)
            ModelState.AddModelError(nameof(vm.ToFinancialAccountId), "لا يمكن التحويل إلى نفس الحساب.");

        if (!ModelState.IsValid)
            return View(vm);

        var fromAccount = await GetCashBankAccountByIdAsync(vm.FromFinancialAccountId);
        var toAccount = await GetCashBankAccountByIdAsync(vm.ToFinancialAccountId);

        if (fromAccount == null)
            ModelState.AddModelError(nameof(vm.FromFinancialAccountId), "حساب التحويل منه يجب أن يكون خزينة أو بنك نشط وحركي.");

        if (toAccount == null)
            ModelState.AddModelError(nameof(vm.ToFinancialAccountId), "حساب التحويل إليه يجب أن يكون خزينة أو بنك نشط وحركي.");

        if (!ModelState.IsValid)
            return View(vm);

        if (!fromAccount!.AllowNegativeCashBalance)
        {
            var balance = await GetAccountBalanceAsync(fromAccount.Id, vm.TransferDate);
            if (balance < vm.Amount)
            {
                ModelState.AddModelError(nameof(vm.Amount), $"رصيد الحساب المحول منه غير كافٍ. الرصيد الحالي {balance:N2}.");
                return View(vm);
            }
        }

        var transfer = new TreasuryBankTransfer
        {
            Id = Guid.NewGuid(),
            TransferNumber = await GenerateDocumentNumberAsync("TRF", vm.TransferDate),
            TransferDate = vm.TransferDate.Date,
            FromFinancialAccountId = vm.FromFinancialAccountId,
            ToFinancialAccountId = vm.ToFinancialAccountId,
            Amount = vm.Amount,
            Status = TreasuryBankTransferStatus.Posted,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedByUserName = User.Identity?.Name,
            CreatedAtUtc = DateTime.UtcNow
        };

        await using var tx = await _db.Database.BeginTransactionAsync();
        await _db.Set<TreasuryBankTransfer>().AddAsync(transfer);
        await _db.SaveChangesAsync();

        var entry = await CreateJournalEntryAsync(
            AccountingSourceTypes.TreasuryBankTransfer,
            transfer.Id,
            transfer.TransferDate,
            $"تحويل رقم {transfer.TransferNumber} - {transfer.Description}",
            debitAccountId: transfer.ToFinancialAccountId,
            creditAccountId: transfer.FromFinancialAccountId,
            amount: transfer.Amount);

        transfer.JournalEntryId = entry.Id;
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["Success"] = $"تم تسجيل التحويل وترحيله بقيد رقم {entry.EntryNumber}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Movement(Guid? financialAccountId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new TreasuryBankMovementVm
        {
            FinancialAccountId = financialAccountId,
            FromDate = fromDate,
            ToDate = toDate
        };

        await FillMovementLookupsAsync(vm);

        if (!financialAccountId.HasValue)
            return View(vm);

        var account = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == financialAccountId.Value);

        if (account == null)
            return View(vm);

        vm.FinancialAccountName = $"{account.AccountCode} - {account.AccountNameAr}";

        var baseQuery = _db.Set<JournalEntryLine>()
            .AsNoTracking()
            .Include(x => x.JournalEntry)
            .Where(x => x.FinancialAccountId == financialAccountId.Value
                && x.JournalEntry != null
                && x.JournalEntry.Status == JournalEntryStatus.Posted);

        if (fromDate.HasValue)
        {
            vm.OpeningBalance = await baseQuery
                .Where(x => x.JournalEntry!.EntryDate.Date < fromDate.Value.Date)
                .SumAsync(x => x.DebitAmount - x.CreditAmount);

            baseQuery = baseQuery.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
            baseQuery = baseQuery.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

        var rows = await baseQuery
            .OrderBy(x => x.JournalEntry!.EntryDate)
            .ThenBy(x => x.JournalEntry!.EntryNumber)
            .Select(x => new
            {
                x.JournalEntry!.EntryDate,
                x.JournalEntry.EntryNumber,
                x.JournalEntry.Description,
                SourceType = x.JournalEntry.SourceType ?? "",
                x.DebitAmount,
                x.CreditAmount
            })
            .ToListAsync();

        var running = vm.OpeningBalance;
        foreach (var row in rows)
        {
            running += row.DebitAmount - row.CreditAmount;
            vm.Rows.Add(new TreasuryBankMovementRowVm
            {
                EntryDate = row.EntryDate,
                EntryNumber = row.EntryNumber,
                Description = row.Description,
                SourceType = row.SourceType,
                DebitAmount = row.DebitAmount,
                CreditAmount = row.CreditAmount,
                RunningBalance = running
            });
        }

        vm.TotalDebit = vm.Rows.Sum(x => x.DebitAmount);
        vm.TotalCredit = vm.Rows.Sum(x => x.CreditAmount);
        vm.ClosingBalance = vm.OpeningBalance + vm.TotalDebit - vm.TotalCredit;

        return View(vm);
    }

    private async Task FillVoucherLookupsAsync(CreateCashBankVoucherVm vm)
    {
        vm.CashBankAccounts = (await GetCashBankAccountsAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr} ({GetCashKindText(x.CashKind)})"
            })
            .ToList();

        vm.OppositeAccounts = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToListAsync();
    }

    private async Task FillTransferLookupsAsync(CreateTreasuryBankTransferVm vm)
    {
        vm.CashBankAccounts = (await GetCashBankAccountsAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr} ({GetCashKindText(x.CashKind)})"
            })
            .ToList();
    }

    private async Task FillMovementLookupsAsync(TreasuryBankMovementVm vm)
    {
        vm.CashBankAccounts = (await GetCashBankAccountsAsync())
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr} ({GetCashKindText(x.CashKind)})"
            })
            .ToList();
    }

    private async Task<List<FinancialAccount>> GetCashBankAccountsAsync()
    {
        var marked = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting && x.CashKind != FinancialAccountCashKind.None)
            .OrderBy(x => x.AccountCode)
            .ToListAsync();

        if (marked.Count > 0)
            return marked;

        // Fallback حتى لا تتوقف الشاشة قبل تصنيف الحسابات القديمة.
        return await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting && x.Category == AccountCategory.Asset)
            .OrderBy(x => x.AccountCode)
            .ToListAsync();
    }

    private Task<FinancialAccount?> GetCashBankAccountByIdAsync(Guid accountId)
    {
        return _db.Set<FinancialAccount>()
            .FirstOrDefaultAsync(x => x.Id == accountId && x.IsActive && x.IsPosting
                && (x.CashKind != FinancialAccountCashKind.None || x.Category == AccountCategory.Asset));
    }

    private async Task<(bool IsValid, string Message, FinancialAccount? CashAccount)> ValidateCashVoucherAccountsAsync(Guid cashAccountId, Guid oppositeAccountId)
    {
        var cashAccount = await GetCashBankAccountByIdAsync(cashAccountId);
        if (cashAccount == null)
            return (false, "حساب الخزينة / البنك يجب أن يكون حسابًا نشطًا وحركيًا ومصنفًا خزينة أو بنك.", null);

        var oppositeAccount = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == oppositeAccountId && x.IsActive && x.IsPosting);

        if (oppositeAccount == null)
            return (false, "الحساب المقابل غير صالح للترحيل.", cashAccount);

        if (cashAccount.Id == oppositeAccount.Id)
            return (false, "لا يمكن اختيار نفس الحساب كحساب نقدية وكحساب مقابل.", cashAccount);

        return (true, string.Empty, cashAccount);
    }

    private async Task<decimal> GetAccountBalanceAsync(Guid accountId, DateTime upToDate)
    {
        return await _db.Set<JournalEntryLine>()
            .AsNoTracking()
            .Where(x => x.FinancialAccountId == accountId
                && x.JournalEntry != null
                && x.JournalEntry.Status == JournalEntryStatus.Posted
                && x.JournalEntry.EntryDate.Date <= upToDate.Date)
            .SumAsync(x => x.DebitAmount - x.CreditAmount);
    }

    private async Task<string> GenerateDocumentNumberAsync(string prefix, DateTime date)
    {
        var year = date.Year;
        var voucherCount = await _db.Set<CashBankVoucher>()
            .CountAsync(x => x.VoucherDate.Year == year);
        var transferCount = await _db.Set<TreasuryBankTransfer>()
            .CountAsync(x => x.TransferDate.Year == year);

        return $"{prefix}-{year}-{(voucherCount + transferCount + 1):D5}";
    }

    private async Task<JournalEntry> CreateJournalEntryAsync(string sourceType, Guid sourceId, DateTime entryDate, string description,
        Guid debitAccountId, Guid creditAccountId, decimal amount)
    {
        var existing = await _db.Set<JournalEntry>()
            .FirstOrDefaultAsync(x => x.SourceType == sourceType && x.SourceId == sourceId && x.Status != JournalEntryStatus.Reversed);

        if (existing != null)
            return existing;

        var fiscalPeriod = await _db.Set<FiscalPeriod>()
            .Where(x => x.IsActive && x.IsOpen && x.StartDate <= entryDate.Date && x.EndDate >= entryDate.Date)
            .OrderBy(x => x.StartDate)
            .FirstOrDefaultAsync();

        if (fiscalPeriod == null)
            throw new InvalidOperationException("لا توجد فترة مالية مفتوحة لتاريخ الحركة.");

        var count = await _db.Set<JournalEntry>().CountAsync(x => x.EntryDate.Year == entryDate.Year);
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            EntryNumber = $"JV-{entryDate.Year}-{(count + 1):D5}",
            EntryDate = entryDate.Date,
            Description = description,
            FiscalPeriodId = fiscalPeriod.Id,
            Status = JournalEntryStatus.Posted,
            SourceType = sourceType,
            SourceId = sourceId,
            TotalDebit = amount,
            TotalCredit = amount,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedByUserName = User.Identity?.Name,
            CreatedAtUtc = DateTime.UtcNow,
            PostedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            PostedByUserName = User.Identity?.Name,
            PostedAtUtc = DateTime.UtcNow,
            Lines = new List<JournalEntryLine>
            {
                new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    FinancialAccountId = debitAccountId,
                    DebitAmount = amount,
                    CreditAmount = 0m,
                    Description = description,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    FinancialAccountId = creditAccountId,
                    DebitAmount = 0m,
                    CreditAmount = amount,
                    Description = description,
                    CreatedAtUtc = DateTime.UtcNow
                }
            }
        };

        foreach (var line in entry.Lines)
            line.JournalEntryId = entry.Id;

        await _db.Set<JournalEntry>().AddAsync(entry);
        await _db.SaveChangesAsync();

        return entry;
    }

    private static string GetCashKindText(FinancialAccountCashKind kind)
    {
        return kind switch
        {
            FinancialAccountCashKind.Treasury => "خزينة",
            FinancialAccountCashKind.Bank => "بنك",
            _ => "غير مصنف"
        };
    }
}
