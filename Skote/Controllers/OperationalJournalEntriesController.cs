using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OperationalJournalEntriesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IOperationalJournalService _operationalJournalService;

    public OperationalJournalEntriesController(AppDbContext db, IOperationalJournalService operationalJournalService)
    {
        _db = db;
        _operationalJournalService = operationalJournalService;
    }

    public async Task<IActionResult> Index()
    {
        // ── قراءة جميع المصادر النشطة بالـ Dynamic Posting من جدول المصادر ──
        var sources = await _db.Set<AccountingIntegrationSourceDefinition>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsDynamicPostingEnabled
                     && x.EntityClrTypeName != null)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        // ── جيب القيود الموجودة للمقارنة ──
        var journalRefs = await _db.Set<JournalEntry>()
            .AsNoTracking()
            .Where(x => x.SourceId != null)
            .Select(x => new { x.SourceType, x.SourceId, x.Id })
            .ToListAsync();

        Guid? FindJournalEntryId(string sourceType, Guid sourceId)
            => journalRefs.FirstOrDefault(x => x.SourceType == sourceType
                                            && x.SourceId == sourceId)?.Id;

        var asm = typeof(InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation).Assembly;

        var sections = new List<OperationalPostingSectionVm>();

        foreach (var src in sources)
        {
            var type = asm.GetType(src.EntityClrTypeName!);
            if (type == null) continue;

            // جيب آخر 20 سجل من الجدول
            var dbSet = _db.GetType()
                .GetMethod("Set", Array.Empty<Type>())!
                .MakeGenericMethod(type)
                .Invoke(_db, null) as System.Collections.IEnumerable;

            if (dbSet == null) continue;

            var rows = new List<OperationalPostingRowVm>();
            try
            {
                // استخدم Reflection لقراءة الخصائص
                var queryable = (IQueryable<object>)dbSet;
                var records   = await queryable
                    .OrderByDescending(x => EF.Property<DateTime>(x, src.DatePropertyName ?? "CreatedAtUtc"))
                    .Take(20)
                    .ToListAsync();

                foreach (var rec in records)
                {
                    var recType = rec.GetType();
                    T? Prop<T>(string? name) where T : struct
                    {
                        if (string.IsNullOrEmpty(name)) return null;
                        var v = recType.GetProperty(name)?.GetValue(rec);
                        return v is T t ? t : null;
                    }
                    string? PropStr(string? name)
                    {
                        if (string.IsNullOrEmpty(name)) return null;
                        return recType.GetProperty(name)?.GetValue(rec)?.ToString();
                    }

                    var id     = Prop<Guid>("Id") ?? Guid.Empty;
                    var date   = Prop<DateTime>(src.DatePropertyName) ?? DateTime.MinValue;
                    var amount = Prop<decimal>(src.AmountPropertyName) ?? 0m;
                    var num    = PropStr(src.NumberPropertyName) ?? id.ToString()[..8].ToUpper();
                    var title  = PropStr(src.TitlePropertyName)  ?? src.NameAr;

                    rows.Add(new OperationalPostingRowVm
                    {
                        SourceId        = id,
                        SourceType      = src.SourceType,
                        ReferenceNumber = num,
                        Title           = title,
                        DocumentDate    = date,
                        Amount          = amount,
                        JournalEntryId  = FindJournalEntryId(src.SourceType, id)
                    });
                }
            }
            catch { /* تجاهل المصادر التي يصعب قراءتها */ }

            sections.Add(new OperationalPostingSectionVm
            {
                Title      = src.NameAr,
                ActionName = "GenerateDynamic",
                SourceType = src.SourceType,
                Rows       = rows
            });
        }

        return View(sections);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateDonation(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateDonationEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateBeneficiaryAidDisbursement(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateBeneficiaryAidDisbursementEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateGrantInstallment(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateGrantInstallmentEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateExpense(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateExpenseEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePayroll(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreatePayrollEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateStoreIssue(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateStoreIssueEntryAsync(id));

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> GeneratePurchaseInvoice(Guid id)
    //    => await GenerateAsync(() => _operationalJournalService.CreateDynamicEntryAsync(AccountingSourceTypes.PurchaseInvoice, id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateStoreReceipt(Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateStoreReceiptEntryAsync(id));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateDynamic(string sourceType, Guid id)
        => await GenerateAsync(() => _operationalJournalService.CreateDynamicEntryAsync(sourceType, id));

    private async Task<IActionResult> GenerateAsync(Func<Task<JournalEntry>> action)
    {
        try
        {
            var entry = await action();
            TempData["Success"] = "تم إنشاء/استرجاع القيد المحاسبي بنجاح";
            return RedirectToAction("Details", "JournalEntries", new { id = entry.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
