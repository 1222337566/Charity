using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Accounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class JournalEntriesController : Controller
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICostCenterRepository _costCenterRepository;
    private readonly AppDbContext _db;

    public JournalEntriesController(
        IJournalEntryRepository journalEntryRepository,
        IFiscalPeriodRepository fiscalPeriodRepository,
        IAccountRepository accountRepository,
        ICostCenterRepository costCenterRepository,
        AppDbContext db)
    {
        _journalEntryRepository = journalEntryRepository;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _accountRepository = accountRepository;
        _costCenterRepository = costCenterRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(string? q = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null, string? sourceType = null)
    {
        var items = await _journalEntryRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim();
            items = items.Where(x =>
                    (x.EntryNumber ?? string.Empty).Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || (x.Description ?? string.Empty).Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || (x.Notes ?? string.Empty).Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (fromDate.HasValue)
            items = items.Where(x => x.EntryDate.Date >= fromDate.Value.Date).ToList();

        if (toDate.HasValue)
            items = items.Where(x => x.EntryDate.Date <= toDate.Value.Date).ToList();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<JournalEntryStatus>(status, true, out var parsedStatus))
            items = items.Where(x => x.Status == parsedStatus).ToList();

        if (!string.IsNullOrWhiteSpace(sourceType))
        {
            if (string.Equals(sourceType, "Manual", StringComparison.OrdinalIgnoreCase))
                items = items.Where(x => string.Equals(x.SourceType, "Manual", StringComparison.OrdinalIgnoreCase)).ToList();
            else if (string.Equals(sourceType, "Automatic", StringComparison.OrdinalIgnoreCase))
                items = items.Where(x => !string.Equals(x.SourceType, "Manual", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        ViewBag.FilterQ = q;
        ViewBag.FilterFromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.FilterToDate = toDate?.ToString("yyyy-MM-dd");
        ViewBag.FilterStatus = status;
        ViewBag.FilterSourceType = sourceType;
        ViewBag.TotalDebit = items.Sum(x => x.Lines?.Sum(l => l.DebitAmount) ?? x.TotalDebit);
        ViewBag.TotalCredit = items.Sum(x => x.Lines?.Sum(l => l.CreditAmount) ?? x.TotalCredit);

        var model = items.Select(x =>
        {
            var totalDebit = x.Lines?.Sum(l => l.DebitAmount) ?? x.TotalDebit;
            var totalCredit = x.Lines?.Sum(l => l.CreditAmount) ?? x.TotalCredit;

            return new JournalEntryListItemVm
            {
                Id = x.Id,
                EntryNumber = x.EntryNumber,
                EntryDate = x.EntryDate,
                FiscalPeriodName = x.FiscalPeriod?.PeriodNameAr ?? string.Empty,
                Description = x.Description,
                Status = x.Status.ToString(),
                SourceType = x.SourceType,
                IsManual = string.Equals(x.SourceType, "Manual", StringComparison.OrdinalIgnoreCase),
                IsBalanced = totalDebit == totalCredit && totalDebit > 0,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                LinesCount = x.Lines.Count
            };
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateJournalEntryVm();
        await FillFiscalPeriods(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJournalEntryVm vm)
    {
        await FillFiscalPeriods(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _journalEntryRepository.EntryNumberExistsAsync(vm.EntryNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.EntryNumber), "رقم القيد موجود بالفعل");
            return View(vm);
        }

        var fiscalPeriod = await ValidateFiscalPeriod(vm.FiscalPeriodId, vm.EntryDate);
        if (fiscalPeriod == null)
            return View(vm);

        var entity = new JournalEntry
        {
            Id = Guid.NewGuid(),
            EntryNumber = vm.EntryNumber.Trim(),
            EntryDate = vm.EntryDate,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            FiscalPeriodId = fiscalPeriod.Id,
            SourceType = vm.SourceType?.Trim(),
            SourceId = vm.SourceId,
            Status = JournalEntryStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _journalEntryRepository.AddAsync(entity);
        TempData["Success"] = "تم إنشاء القيد كمسودة بنجاح";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }


    [HttpGet]
    public async Task<IActionResult> CreateManual()
    {
        var vm = new CreateManualJournalEntryVm
        {
            EntryNumber = await GenerateManualEntryNumberAsync(),
            EntryDate = DateTime.Today
        };

        await FillManualLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateManual(CreateManualJournalEntryVm vm)
    {
        NormalizeManualLines(vm);
        await FillManualLookupsAsync(vm);

        if (string.IsNullOrWhiteSpace(vm.EntryNumber))
            vm.EntryNumber = await GenerateManualEntryNumberAsync();

        if (await _journalEntryRepository.EntryNumberExistsAsync(vm.EntryNumber.Trim()))
            ModelState.AddModelError(nameof(vm.EntryNumber), "رقم القيد موجود بالفعل");

        var fiscalPeriod = await ValidateFiscalPeriod(vm.FiscalPeriodId, vm.EntryDate);
        await ValidateManualJournalEntryAsync(vm);

        if (!ModelState.IsValid || fiscalPeriod == null)
        {
            if (vm.Lines.Count < 2)
            {
                while (vm.Lines.Count < 2)
                    vm.Lines.Add(new CreateManualJournalEntryLineVm());
            }

            return View(vm);
        }

        var now = DateTime.UtcNow;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var totalDebit = vm.Lines.Sum(x => x.DebitAmount ?? 0m);
        var totalCredit = vm.Lines.Sum(x => x.CreditAmount ?? 0m);

        var entity = new JournalEntry
        {
            Id = Guid.NewGuid(),
            EntryNumber = vm.EntryNumber.Trim(),
            EntryDate = vm.EntryDate,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            FiscalPeriodId = fiscalPeriod.Id,
            SourceType = "Manual",
            SourceId = null,
            Status = vm.PostImmediately ? JournalEntryStatus.Posted : JournalEntryStatus.Draft,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            CreatedAtUtc = now,
            PostedAtUtc = vm.PostImmediately ? (DateTime?)now : null,
            PostedByUserId = vm.PostImmediately ? userId : null
        };

        foreach (var line in vm.Lines)
        {
            entity.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entity.Id,
                FinancialAccountId = line.FinancialAccountId!.Value,
                CostCenterId = line.CostCenterId,
                ProjectId = line.ProjectId,
                Description = string.IsNullOrWhiteSpace(line.Description)
                    ? vm.Description.Trim()
                    : line.Description.Trim(),
                DebitAmount = line.DebitAmount ?? 0m,
                CreditAmount = line.CreditAmount ?? 0m,
                CreatedAtUtc = now
            });
        }

        await _journalEntryRepository.AddAsync(entity);
        TempData["Success"] = vm.PostImmediately
            ? $"✅ تم إنشاء القيد اليدوي رقم ({entity.EntryNumber}) وترحيله بنجاح — تم عمل هذا القيد يدوياً"
            : $"✅ تم حفظ القيد اليدوي رقم ({entity.EntryNumber}) كمسودة — تم عمل هذا القيد يدوياً";

        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _journalEntryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (entity.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن تعديل قيد بعد ترحيله أو عكسه";
            return RedirectToAction(nameof(Details), new { id });
        }

        var vm = new EditJournalEntryVm
        {
            Id = entity.Id,
            EntryNumber = entity.EntryNumber,
            EntryDate = entity.EntryDate,
            FiscalPeriodId = entity.FiscalPeriodId,
            Description = entity.Description,
            Notes = entity.Notes,
            SourceType = entity.SourceType,
            SourceId = entity.SourceId
        };

        await FillFiscalPeriods(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditJournalEntryVm vm)
    {
        await FillFiscalPeriods(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _journalEntryRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (entity.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن تعديل قيد بعد ترحيله أو عكسه";
            return RedirectToAction(nameof(Details), new { id = vm.Id });
        }

        if (await _journalEntryRepository.EntryNumberExistsAsync(vm.EntryNumber.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.EntryNumber), "رقم القيد موجود بالفعل");
            return View(vm);
        }

        var fiscalPeriod = await ValidateFiscalPeriod(vm.FiscalPeriodId, vm.EntryDate);
        if (fiscalPeriod == null)
            return View(vm);

        entity.EntryNumber = vm.EntryNumber.Trim();
        entity.EntryDate = vm.EntryDate;
        entity.Description = vm.Description.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.FiscalPeriodId = fiscalPeriod.Id;
        entity.SourceType = vm.SourceType?.Trim();
        entity.SourceId = vm.SourceId;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _journalEntryRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل القيد بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _journalEntryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.TotalDebit = entity.Lines.Sum(x => x.DebitAmount);
        entity.TotalCredit = entity.Lines.Sum(x => x.CreditAmount);

        // جيب أسماء المشاريع
        var projectIds = entity.Lines
            .Where(x => x.ProjectId.HasValue)
            .Select(x => x.ProjectId!.Value)
            .Distinct()
            .ToList();
        var projectNames = new Dictionary<Guid, string>();
        if (projectIds.Any())
        {
            var projects = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Projects.CharityProject>()
                .AsNoTracking()
                .Where(x => projectIds.Contains(x.Id))
                .ToListAsync();
            projectNames = projects.ToDictionary(x => x.Id, x => $"{x.Code} - {x.Name}");
        }

        var isManual = string.Equals(entity.SourceType, "Manual", StringComparison.OrdinalIgnoreCase);

        var vm = new JournalEntryDetailsVm
        {
            Id = entity.Id,
            EntryNumber = entity.EntryNumber,
            EntryDate = entity.EntryDate,
            FiscalPeriodName = entity.FiscalPeriod?.PeriodNameAr ?? string.Empty,
            Description = entity.Description,
            Notes = entity.Notes,
            Status = entity.Status.ToString(),
            SourceType = entity.SourceType,
            SourceId = entity.SourceId,
            RelatedJournalEntryId = entity.RelatedJournalEntryId,
            IsManual = isManual,
            TotalDebit = entity.TotalDebit,
            TotalCredit = entity.TotalCredit,
            IsBalanced = entity.TotalDebit == entity.TotalCredit && entity.TotalDebit > 0,
            Lines = entity.Lines.Select(x => new JournalEntryLineListItemVm
            {
                Id = x.Id,
                AccountCode = x.FinancialAccount?.AccountCode ?? string.Empty,
                AccountNameAr = x.FinancialAccount?.AccountNameAr ?? string.Empty,
                CostCenterName = x.CostCenter?.NameAr,
                ProjectId = x.ProjectId,
                ProjectName = x.ProjectId.HasValue && projectNames.TryGetValue(x.ProjectId.Value, out var pn) ? pn : null,
                Description = x.Description,
                DebitAmount = x.DebitAmount,
                CreditAmount = x.CreditAmount
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Print(Guid id)
    {
        var entity = await _journalEntryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.TotalDebit = entity.Lines.Sum(x => x.DebitAmount);
        entity.TotalCredit = entity.Lines.Sum(x => x.CreditAmount);

        var vm = new JournalEntryDetailsVm
        {
            Id = entity.Id,
            EntryNumber = entity.EntryNumber,
            EntryDate = entity.EntryDate,
            FiscalPeriodName = entity.FiscalPeriod?.PeriodNameAr ?? string.Empty,
            Description = entity.Description,
            Notes = entity.Notes,
            Status = entity.Status.ToString(),
            SourceType = entity.SourceType,
            SourceId = entity.SourceId,
            RelatedJournalEntryId = entity.RelatedJournalEntryId,
            TotalDebit = entity.TotalDebit,
            TotalCredit = entity.TotalCredit,
            IsBalanced = entity.TotalDebit == entity.TotalCredit && entity.TotalDebit > 0,
            Lines = entity.Lines.Select(x => new JournalEntryLineListItemVm
            {
                Id = x.Id,
                AccountCode = x.FinancialAccount?.AccountCode ?? string.Empty,
                AccountNameAr = x.FinancialAccount?.AccountNameAr ?? string.Empty,
                CostCenterName = x.CostCenter?.NameAr,
                ProjectId = x.ProjectId,
                Description = x.Description,
                DebitAmount = x.DebitAmount,
                CreditAmount = x.CreditAmount
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id);
        if (entry == null)
            return NotFound();

        if (entry.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن إلغاء إلا القيود في حالة مسودة. القيد المرحل يتم عكسه بقيد عكسي.";
            return RedirectToAction(nameof(Details), new { id });
        }

        entry.Status = JournalEntryStatus.Cancelled;
        entry.UpdatedAtUtc = DateTime.UtcNow;
        await _journalEntryRepository.UpdateAsync(entry);

        TempData["Success"] = "تم إلغاء القيد المسودة بدون حذف بياناته.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> AddLine(Guid journalEntryId)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(journalEntryId);
        if (entry == null)
            return NotFound();

        if (entry.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن إضافة سطور إلى قيد غير مسودة";
            return RedirectToAction(nameof(Details), new { id = journalEntryId });
        }

        var vm = new CreateJournalEntryLineVm { JournalEntryId = journalEntryId };
        await FillLineLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLine(CreateJournalEntryLineVm vm)
    {
        await FillLineLookups(vm);

        var entry = await _journalEntryRepository.GetByIdAsync(vm.JournalEntryId);
        if (entry == null)
            return NotFound();

        if (entry.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن إضافة سطور إلى قيد غير مسودة";
            return RedirectToAction(nameof(Details), new { id = vm.JournalEntryId });
        }

        var account = await ValidateLineModel(vm);
        if (account == null)
            return View(vm);

        var line = new JournalEntryLine
        {
            Id = Guid.NewGuid(),
            JournalEntryId = vm.JournalEntryId,
            FinancialAccountId = account.Id,
            CostCenterId = vm.CostCenterId,
            ProjectId = vm.ProjectId,
            Description = vm.Description?.Trim(),
            DebitAmount = vm.DebitAmount,
            CreditAmount = vm.CreditAmount,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _journalEntryRepository.AddLineAsync(line);
        TempData["Success"] = "تم إضافة سطر القيد بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.JournalEntryId });
    }

    [HttpGet]
    public async Task<IActionResult> EditLine(Guid id)
    {
        var line = await _journalEntryRepository.GetLineByIdAsync(id);
        if (line == null)
            return NotFound();

        if (line.JournalEntry?.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن تعديل سطر داخل قيد غير مسودة";
            return RedirectToAction(nameof(Details), new { id = line.JournalEntryId });
        }

        var vm = new EditJournalEntryLineVm
        {
            Id = line.Id,
            JournalEntryId = line.JournalEntryId,
            FinancialAccountId = line.FinancialAccountId,
            CostCenterId = line.CostCenterId,
            ProjectId = line.ProjectId,
            Description = line.Description,
            DebitAmount = line.DebitAmount,
            CreditAmount = line.CreditAmount
        };
        await FillLineLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLine(EditJournalEntryLineVm vm)
    {
        await FillLineLookups(vm);

        var line = await _journalEntryRepository.GetLineByIdAsync(vm.Id);
        if (line == null)
            return NotFound();

        if (line.JournalEntry?.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن تعديل سطر داخل قيد غير مسودة";
            return RedirectToAction(nameof(Details), new { id = line.JournalEntryId });
        }

        var account = await ValidateLineModel(vm);
        if (account == null)
            return View(vm);

        line.FinancialAccountId = account.Id;
        line.CostCenterId = vm.CostCenterId;
        line.ProjectId = vm.ProjectId;
        line.Description = vm.Description?.Trim();
        line.DebitAmount = vm.DebitAmount;
        line.CreditAmount = vm.CreditAmount;
        line.UpdatedAtUtc = DateTime.UtcNow;

        await _journalEntryRepository.UpdateLineAsync(line);
        TempData["Success"] = "تم تعديل سطر القيد بنجاح";
        return RedirectToAction(nameof(Details), new { id = line.JournalEntryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLine(Guid id)
    {
        var line = await _journalEntryRepository.GetLineByIdAsync(id);
        if (line == null)
            return NotFound();

        if (line.JournalEntry?.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن حذف سطر من قيد غير مسودة";
            return RedirectToAction(nameof(Details), new { id = line.JournalEntryId });
        }

        var journalEntryId = line.JournalEntryId;
        await _journalEntryRepository.DeleteLineAsync(line);
        TempData["Success"] = "تم حذف سطر القيد بنجاح";
        return RedirectToAction(nameof(Details), new { id = journalEntryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(Guid id)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id);
        if (entry == null)
            return NotFound();

        if (entry.Status != JournalEntryStatus.Draft)
        {
            TempData["Error"] = "لا يمكن ترحيل إلا القيود في حالة مسودة";
            return RedirectToAction(nameof(Details), new { id });
        }

        var errors = await ValidateEntryBeforePosting(entry);
        if (errors.Any())
        {
            TempData["Error"] = string.Join(" | ", errors);
            return RedirectToAction(nameof(Details), new { id });
        }

        entry.TotalDebit = entry.Lines.Sum(x => x.DebitAmount);
        entry.TotalCredit = entry.Lines.Sum(x => x.CreditAmount);
        entry.Status = JournalEntryStatus.Posted;
        entry.PostedAtUtc = DateTime.UtcNow;
        entry.PostedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entry.UpdatedAtUtc = DateTime.UtcNow;

        await _journalEntryRepository.UpdateAsync(entry);
        TempData["Success"] = "تم ترحيل القيد بنجاح";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reverse(Guid id)
    {
        var entry = await _journalEntryRepository.GetByIdAsync(id);
        if (entry == null)
            return NotFound();

        if (entry.Status != JournalEntryStatus.Posted)
        {
            TempData["Error"] = "لا يمكن عكس إلا القيود المرحلة";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (entry.RelatedJournalEntryId.HasValue)
        {
            TempData["Error"] = "تم عكس هذا القيد من قبل";
            return RedirectToAction(nameof(Details), new { id });
        }

        var reversal = new JournalEntry
        {
            Id = Guid.NewGuid(),
            EntryNumber = $"{entry.EntryNumber}-REV",
            EntryDate = DateTime.Today,
            Description = $"قيد عكسي للقيد رقم {entry.EntryNumber}",
            Notes = entry.Notes,
            FiscalPeriodId = entry.FiscalPeriodId,
            SourceType = "ReverseJournalEntry",
            SourceId = entry.Id,
            RelatedJournalEntryId = entry.Id,
            Status = JournalEntryStatus.Posted,
            TotalDebit = entry.TotalCredit,
            TotalCredit = entry.TotalDebit,
            CreatedAtUtc = DateTime.UtcNow,
            PostedAtUtc = DateTime.UtcNow,
            PostedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        foreach (var line in entry.Lines)
        {
            reversal.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = reversal.Id,
                FinancialAccountId = line.FinancialAccountId,
                CostCenterId = line.CostCenterId,
                ProjectId = line.ProjectId,
                Description = line.Description,
                DebitAmount = line.CreditAmount,
                CreditAmount = line.DebitAmount,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        if (await _journalEntryRepository.EntryNumberExistsAsync(reversal.EntryNumber))
        {
            reversal.EntryNumber = $"{entry.EntryNumber}-REV-{DateTime.Now:HHmmss}";
        }

        await _journalEntryRepository.AddAsync(reversal);

        entry.Status = JournalEntryStatus.Reversed;
        entry.RelatedJournalEntryId = reversal.Id;
        entry.UpdatedAtUtc = DateTime.UtcNow;
        await _journalEntryRepository.UpdateAsync(entry);

        TempData["Success"] = "تم إنشاء القيد العكسي وتحديث القيد الأصلي";
        return RedirectToAction(nameof(Details), new { id = reversal.Id });
    }

    private async Task FillFiscalPeriods(CreateJournalEntryVm vm)
    {
        var periods = await _fiscalPeriodRepository.GetAllAsync();
        vm.FiscalPeriods = periods
            .Where(x => !x.IsClosed)
            .OrderByDescending(x => x.StartDate)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.PeriodCode} - {x.PeriodNameAr}"
            })
            .ToList();
    }

    private async Task FillLineLookups(CreateJournalEntryLineVm vm)
    {
        var accounts = await _accountRepository.GetAllAsync();
        vm.Accounts = accounts
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToList();

        var costCenters = await _costCenterRepository.GetAllAsync();
        vm.CostCenters = costCenters
            .Where(x => x.IsActive)
            .OrderBy(x => x.CostCenterCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.CostCenterCode} - {x.NameAr}"
            })
            .ToList();
    }


    private async Task FillManualLookupsAsync(CreateManualJournalEntryVm vm)
    {
        var periods = await _fiscalPeriodRepository.GetAllAsync();
        vm.FiscalPeriods = periods
            .Where(x => !x.IsClosed)
            .OrderByDescending(x => x.StartDate)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.PeriodCode} - {x.PeriodNameAr}"
            })
            .ToList();

        var accounts = await _accountRepository.GetAllAsync();
        vm.Accounts = accounts
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToList();

        var costCenters = await _costCenterRepository.GetAllAsync();
        vm.CostCenters = costCenters
            .Where(x => x.IsActive)
            .OrderBy(x => x.CostCenterCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.CostCenterCode} - {x.NameAr}"
            })
            .ToList();

        var projects = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Projects.CharityProject>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync();
        vm.Projects = projects
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Code} - {x.Name}"
            })
            .ToList();
    }

    private void NormalizeManualLines(CreateManualJournalEntryVm vm)
    {
        vm.Lines ??= new List<CreateManualJournalEntryLineVm>();
        vm.Lines = vm.Lines
            .Where(x =>
                x.FinancialAccountId.HasValue ||
                x.CostCenterId.HasValue ||
                x.ProjectId.HasValue ||
                (x.DebitAmount ?? 0m) > 0m ||
                (x.CreditAmount ?? 0m) > 0m ||
                !string.IsNullOrWhiteSpace(x.Description))
            .ToList();
    }

    private async Task ValidateManualJournalEntryAsync(CreateManualJournalEntryVm vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Description))
            ModelState.AddModelError(nameof(vm.Description), "بيان القيد مطلوب");

        if (vm.EntryDate == default)
            ModelState.AddModelError(nameof(vm.EntryDate), "تاريخ القيد مطلوب");

        if (vm.Lines.Count < 2)
            ModelState.AddModelError(string.Empty, "يجب إدخال سطرين على الأقل في القيد اليدوي");

        var totalDebit = vm.Lines.Sum(x => x.DebitAmount ?? 0m);
        var totalCredit = vm.Lines.Sum(x => x.CreditAmount ?? 0m);

        if (totalDebit <= 0m || totalCredit <= 0m)
            ModelState.AddModelError(string.Empty, "إجمالي المدين والدائن يجب أن يكون أكبر من صفر");

        if (totalDebit != totalCredit)
            ModelState.AddModelError(string.Empty, $"القيد غير متوازن. إجمالي المدين {totalDebit:N2} وإجمالي الدائن {totalCredit:N2}");

        for (var i = 0; i < vm.Lines.Count; i++)
        {
            var line = vm.Lines[i];
            var debit = line.DebitAmount ?? 0m;
            var credit = line.CreditAmount ?? 0m;

            if (!line.FinancialAccountId.HasValue || line.FinancialAccountId == Guid.Empty)
            {
                ModelState.AddModelError($"Lines[{i}].FinancialAccountId", "الحساب مطلوب");
                continue;
            }

            if (debit < 0m || credit < 0m)
                ModelState.AddModelError($"Lines[{i}].DebitAmount", "لا يمكن إدخال قيم سالبة");

            if (debit == 0m && credit == 0m)
                ModelState.AddModelError($"Lines[{i}].DebitAmount", "يجب إدخال مبلغ مدين أو دائن");

            if (debit > 0m && credit > 0m)
                ModelState.AddModelError($"Lines[{i}].DebitAmount", "لا يجوز أن يكون السطر مدينًا ودائنًا في نفس الوقت");

            var account = await _accountRepository.GetByIdAsync(line.FinancialAccountId.Value);
            if (account == null)
            {
                ModelState.AddModelError($"Lines[{i}].FinancialAccountId", "الحساب غير موجود");
                continue;
            }

            if (!account.IsActive || !account.IsPosting)
                ModelState.AddModelError($"Lines[{i}].FinancialAccountId", "يجب اختيار حساب حركي نشط");

            if (account.RequiresCostCenter && !line.CostCenterId.HasValue)
                ModelState.AddModelError($"Lines[{i}].CostCenterId", "هذا الحساب يتطلب مركز تكلفة");

            if (account.RequiresProject && !line.ProjectId.HasValue)
                ModelState.AddModelError($"Lines[{i}].ProjectId", "هذا الحساب يتطلب مشروعًا");
        }
    }

    private async Task<string> GenerateManualEntryNumberAsync()
    {
        var prefix = $"MJ-{DateTime.Today:yyyyMMdd}-";

        for (var i = 1; i <= 9999; i++)
        {
            var entryNumber = $"{prefix}{i:0000}";
            if (!await _journalEntryRepository.EntryNumberExistsAsync(entryNumber))
                return entryNumber;
        }

        return $"{prefix}{DateTime.Now:HHmmss}";
    }

    private async Task<FiscalPeriod?> ValidateFiscalPeriod(Guid? fiscalPeriodId, DateTime entryDate)
    {
        if (!fiscalPeriodId.HasValue)
        {
            ModelState.AddModelError(nameof(CreateJournalEntryVm.FiscalPeriodId), "الفترة المالية مطلوبة");
            return null;
        }

        var fiscalPeriod = await _fiscalPeriodRepository.GetByIdAsync(fiscalPeriodId.Value);
        if (fiscalPeriod == null)
        {
            ModelState.AddModelError(nameof(CreateJournalEntryVm.FiscalPeriodId), "الفترة المالية غير موجودة");
            return null;
        }

        if (fiscalPeriod.IsClosed)
        {
            ModelState.AddModelError(nameof(CreateJournalEntryVm.FiscalPeriodId), "الفترة المالية مغلقة");
            return null;
        }

        if (entryDate.Date < fiscalPeriod.StartDate.Date || entryDate.Date > fiscalPeriod.EndDate.Date)
        {
            ModelState.AddModelError(nameof(CreateJournalEntryVm.EntryDate), "تاريخ القيد خارج حدود الفترة المالية");
            return null;
        }

        return fiscalPeriod;
    }

    private async Task<FinancialAccount?> ValidateLineModel(CreateJournalEntryLineVm vm)
    {
        if (!ModelState.IsValid)
            return null;

        if ((! (vm.DebitAmount > 0)) && (!(vm.CreditAmount > 0)))
        {
            ModelState.AddModelError(string.Empty, "يجب إدخال مبلغ مدين أو دائن");
            return null;
        }

        if (vm.DebitAmount > 0 && vm.CreditAmount > 0)
        {
            ModelState.AddModelError(string.Empty, "لا يمكن أن يكون السطر مدينًا ودائنًا في نفس الوقت");
            return null;
        }

        if (!vm.FinancialAccountId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.FinancialAccountId), "الحساب مطلوب");
            return null;
        }

        var account = await _accountRepository.GetByIdAsync(vm.FinancialAccountId.Value);
        if (account == null)
        {
            ModelState.AddModelError(nameof(vm.FinancialAccountId), "الحساب غير موجود");
            return null;
        }

        if (!account.IsActive || !account.IsPosting)
        {
            ModelState.AddModelError(nameof(vm.FinancialAccountId), "يجب اختيار حساب حركي نشط");
            return null;
        }

        if (account.RequiresCostCenter && !vm.CostCenterId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.CostCenterId), "هذا الحساب يتطلب مركز تكلفة");
            return null;
        }

        if (account.RequiresProject && !vm.ProjectId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.ProjectId), "هذا الحساب يتطلب مشروعًا");
            return null;
        }

        return account;
    }

    private async Task<List<string>> ValidateEntryBeforePosting(JournalEntry entry)
    {
        var errors = new List<string>();

        var fiscalPeriod = await _fiscalPeriodRepository.GetByIdAsync(entry.FiscalPeriodId);
        if (fiscalPeriod == null)
        {
            errors.Add("الفترة المالية غير موجودة");
            return errors;
        }

        if (fiscalPeriod.IsClosed)
            errors.Add("الفترة المالية مغلقة");

        if (entry.EntryDate.Date < fiscalPeriod.StartDate.Date || entry.EntryDate.Date > fiscalPeriod.EndDate.Date)
            errors.Add("تاريخ القيد خارج حدود الفترة المالية");

        if (entry.Lines == null || !entry.Lines.Any())
        {
            errors.Add("لا يوجد أي سطور داخل القيد");
            return errors;
        }

        if (entry.Lines.Any(x => x.DebitAmount < 0 || x.CreditAmount < 0))
            errors.Add("لا يمكن أن تحتوي السطور على قيم سالبة");

        if (entry.Lines.Any(x => (x.DebitAmount <= 0 && x.CreditAmount <= 0) || (x.DebitAmount > 0 && x.CreditAmount > 0)))
            errors.Add("يوجد سطر أو أكثر غير صحيح من حيث المدين والدائن");

        var totalDebit = entry.Lines.Sum(x => x.DebitAmount);
        var totalCredit = entry.Lines.Sum(x => x.CreditAmount);
        if (totalDebit <= 0 || totalCredit <= 0)
            errors.Add("إجمالي المدين والدائن يجب أن يكون أكبر من صفر");

        if (totalDebit != totalCredit)
            errors.Add("القيد غير متوازن");

        foreach (var line in entry.Lines)
        {
            var account = await _accountRepository.GetByIdAsync(line.FinancialAccountId);
            if (account == null)
            {
                errors.Add("يوجد سطر بحساب غير موجود");
                continue;
            }

            if (!account.IsActive || !account.IsPosting)
                errors.Add($"الحساب {account.AccountCode} يجب أن يكون حركيًا ونشطًا");

            if (account.RequiresCostCenter && !line.CostCenterId.HasValue)
                errors.Add($"الحساب {account.AccountCode} يتطلب مركز تكلفة");

            if (account.RequiresProject && !line.ProjectId.HasValue)
                errors.Add($"الحساب {account.AccountCode} يتطلب مشروعًا");
        }

        return errors.Distinct().ToList();
    }
}
