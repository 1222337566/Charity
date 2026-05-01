using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AccountingIntegrationSourcesController : Controller
{
    private readonly AppDbContext _db;

    public AccountingIntegrationSourcesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _db.Set<AccountingIntegrationSourceDefinition>()
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateAccountingIntegrationSourceDefinitionVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountingIntegrationSourceDefinitionVm vm)
    {
        Normalize(vm);
        ValidateDynamicMap(vm);

        if (await _db.Set<AccountingIntegrationSourceDefinition>().AnyAsync(x => x.SourceType == vm.SourceType))
            ModelState.AddModelError(nameof(vm.SourceType), "كود المصدر موجود بالفعل");

        if (!ModelState.IsValid)
            return View(vm);

        var entity = new AccountingIntegrationSourceDefinition
        {
            Id = Guid.NewGuid(),
            SourceType = vm.SourceType,
            NameAr = vm.NameAr,
            NameEn = vm.NameEn,
            ModuleCode = vm.ModuleCode,
            Description = vm.Description,
            SortOrder = vm.SortOrder,
            IsActive = vm.IsActive,
            IsDynamicPostingEnabled = vm.IsDynamicPostingEnabled,
            EntityClrTypeName = vm.EntityClrTypeName,
            IdPropertyName = vm.IdPropertyName,
            DatePropertyName = vm.DatePropertyName,
            AmountPropertyName = vm.AmountPropertyName,
            NumberPropertyName = vm.NumberPropertyName,
            TitlePropertyName = vm.TitlePropertyName,
            DescriptionTemplate = vm.DescriptionTemplate,
            FinancialAccountIdPropertyName = vm.FinancialAccountIdPropertyName,
            ProjectIdPropertyName = vm.ProjectIdPropertyName,
            CostCenterIdPropertyName = vm.CostCenterIdPropertyName,
            EventCodePropertyName = vm.EventCodePropertyName,
            DonationTypePropertyName = vm.DonationTypePropertyName,
            TargetingScopeCodePropertyName = vm.TargetingScopeCodePropertyName,
            PurposeNamePropertyName = vm.PurposeNamePropertyName,
            AidTypeIdPropertyName = vm.AidTypeIdPropertyName,
            StoreMovementTypePropertyName = vm.StoreMovementTypePropertyName,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Set<AccountingIntegrationSourceDefinition>().Add(entity);
        await _db.SaveChangesAsync();

        TempData["Success"] = "تم حفظ مصدر الربط المحاسبي";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _db.Set<AccountingIntegrationSourceDefinition>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        var vm = new EditAccountingIntegrationSourceDefinitionVm
        {
            Id = entity.Id,
            SourceType = entity.SourceType,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            ModuleCode = entity.ModuleCode,
            Description = entity.Description,
            SortOrder = entity.SortOrder,
            IsActive = entity.IsActive,
            IsSystem = entity.IsSystem,
            IsDynamicPostingEnabled = entity.IsDynamicPostingEnabled,
            EntityClrTypeName = entity.EntityClrTypeName,
            IdPropertyName = entity.IdPropertyName,
            DatePropertyName = entity.DatePropertyName,
            AmountPropertyName = entity.AmountPropertyName,
            NumberPropertyName = entity.NumberPropertyName,
            TitlePropertyName = entity.TitlePropertyName,
            DescriptionTemplate = entity.DescriptionTemplate,
            FinancialAccountIdPropertyName = entity.FinancialAccountIdPropertyName,
            ProjectIdPropertyName = entity.ProjectIdPropertyName,
            CostCenterIdPropertyName = entity.CostCenterIdPropertyName,
            EventCodePropertyName = entity.EventCodePropertyName,
            DonationTypePropertyName = entity.DonationTypePropertyName,
            TargetingScopeCodePropertyName = entity.TargetingScopeCodePropertyName,
            PurposeNamePropertyName = entity.PurposeNamePropertyName,
            AidTypeIdPropertyName = entity.AidTypeIdPropertyName,
            StoreMovementTypePropertyName = entity.StoreMovementTypePropertyName
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAccountingIntegrationSourceDefinitionVm vm)
    {
        Normalize(vm);
        ValidateDynamicMap(vm);

        var entity = await _db.Set<AccountingIntegrationSourceDefinition>().FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (entity == null)
            return NotFound();

        if (await _db.Set<AccountingIntegrationSourceDefinition>().AnyAsync(x => x.Id != vm.Id && x.SourceType == vm.SourceType))
            ModelState.AddModelError(nameof(vm.SourceType), "كود المصدر مستخدم بالفعل");

        if (!ModelState.IsValid)
            return View(vm);

        entity.SourceType = vm.SourceType;
        entity.NameAr = vm.NameAr;
        entity.NameEn = vm.NameEn;
        entity.ModuleCode = vm.ModuleCode;
        entity.Description = vm.Description;
        entity.SortOrder = vm.SortOrder;
        entity.IsActive = vm.IsActive;
        entity.IsDynamicPostingEnabled = vm.IsDynamicPostingEnabled;
        entity.EntityClrTypeName = vm.EntityClrTypeName;
        entity.IdPropertyName = vm.IdPropertyName;
        entity.DatePropertyName = vm.DatePropertyName;
        entity.AmountPropertyName = vm.AmountPropertyName;
        entity.NumberPropertyName = vm.NumberPropertyName;
        entity.TitlePropertyName = vm.TitlePropertyName;
        entity.DescriptionTemplate = vm.DescriptionTemplate;
        entity.FinancialAccountIdPropertyName = vm.FinancialAccountIdPropertyName;
        entity.ProjectIdPropertyName = vm.ProjectIdPropertyName;
        entity.CostCenterIdPropertyName = vm.CostCenterIdPropertyName;
        entity.EventCodePropertyName = vm.EventCodePropertyName;
        entity.DonationTypePropertyName = vm.DonationTypePropertyName;
        entity.TargetingScopeCodePropertyName = vm.TargetingScopeCodePropertyName;
        entity.PurposeNamePropertyName = vm.PurposeNamePropertyName;
        entity.AidTypeIdPropertyName = vm.AidTypeIdPropertyName;
        entity.StoreMovementTypePropertyName = vm.StoreMovementTypePropertyName;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "تم تحديث مصدر الربط المحاسبي";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedDefaults()
    {
        var defaults = new[]
        {
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.Donation,
                NameAr = "التبرعات",
                ModuleCode = "Charity",
                SortOrder = 10,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation",
                DatePropertyName = "DonationDate",
                AmountPropertyName = "Amount",
                NumberPropertyName = "DonationNumber",
                TitlePropertyName = "DonationNumber",
                FinancialAccountIdPropertyName = "FinancialAccountId",
                DonationTypePropertyName = "DonationType",
                TargetingScopeCodePropertyName = "TargetingScopeCode",
                PurposeNamePropertyName = "GeneralPurposeName",
                AidTypeIdPropertyName = "AidTypeId",
                DescriptionTemplate = "إثبات تبرع رقم {DonationNumber}"
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.BeneficiaryAidDisbursement,
                NameAr = "صرف المساعدات للمستفيدين",
                ModuleCode = "Charity",
                SortOrder = 20,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries.BeneficiaryAidDisbursement",
                DatePropertyName = "DisbursementDate",
                AmountPropertyName = "Amount",
                NumberPropertyName = "Id",
                TitlePropertyName = "Notes",
                FinancialAccountIdPropertyName = "FinancialAccountId",
                ProjectIdPropertyName = "ProjectId",
                AidTypeIdPropertyName = "AidTypeId",
                DescriptionTemplate = "إثبات صرف مساعدة {Id}"
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.GrantInstallment,
                NameAr = "دفعات التمويل",
                ModuleCode = "Funding",
                SortOrder = 30,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Charity.Funders.GrantInstallment",
                DatePropertyName = "ReceivedDate",
                AmountPropertyName = "ReceivedAmount",
                NumberPropertyName = "InstallmentNumber",
                FinancialAccountIdPropertyName = "FinancialAccountId",
                DescriptionTemplate = "إثبات استلام دفعة تمويل رقم {InstallmentNumber}"
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.Expense,
                NameAr = "المصروفات",
                ModuleCode = "Finance",
                SortOrder = 40,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Expenses.Expensex",
                DatePropertyName = "ExpenseDateUtc",
                AmountPropertyName = "Amount",
                NumberPropertyName = "ExpenseNumber",
                DescriptionTemplate = "إثبات مصروف رقم {ExpenseNumber}"
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.Payroll,
                NameAr = "المرتبات",
                ModuleCode = "HR",
                SortOrder = 50,
                IsSystem = true,
                IsActive = true
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.StoreReceipt,
                NameAr = "الإضافة المخزنية",
                ModuleCode = "Stores",
                SortOrder = 60,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Charity.Stores.CharityStoreReceipt",
                DatePropertyName = "ReceiptDate",
                NumberPropertyName = "ReceiptNumber",
                StoreMovementTypePropertyName = "SourceType",
                DescriptionTemplate = "إثبات إضافة مخزنية رقم {ReceiptNumber}"
            },
            new AccountingIntegrationSourceDefinition
            {
                SourceType = AccountingSourceTypes.StoreIssue,
                NameAr = "الصرف المخزني",
                ModuleCode = "Stores",
                SortOrder = 70,
                IsSystem = true,
                IsActive = true,
                IsDynamicPostingEnabled = true,
                EntityClrTypeName = "InfrastrfuctureManagmentCore.Domains.Charity.Stores.CharityStoreIssue",
                DatePropertyName = "IssueDate",
                NumberPropertyName = "IssueNumber",
                ProjectIdPropertyName = "ProjectId",
                StoreMovementTypePropertyName = "IssueType",
                DescriptionTemplate = "إثبات صرف مخزني رقم {IssueNumber}"
            },
            new AccountingIntegrationSourceDefinition
{
    Id = Guid.NewGuid(),
    SourceType = AccountingSourceTypes.PurchaseInvoice,
    NameAr = "فاتورة شراء",
    Description = "ترحيل قيد محاسبي تلقائي عند إصدار فاتورة شراء",
    ModuleCode = "Purchasess",
                SortOrder = 80,
                IsSystem = true,
                  IsDynamicPostingEnabled = true,
    IsActive = true
},
            new AccountingIntegrationSourceDefinition
{
    SourceType = AccountingSourceTypes.SalesInvoiceCOGS,
    NameAr = "تكلفة البضاعة المباعة",
    NameEn = "Cost of Goods Sold",
    ModuleCode = "Sales",
    SortOrder = 92,
    IsSystem = true,
    IsActive = true,

    // Manual posting through OperationalJournalService.
    // Do not enable dynamic posting because the amount is calculated from the linked StoreIssue lines,
    // not directly from SalesInvoice.NetAmount.
    IsDynamicPostingEnabled = false,

    Description = "قيد تكلفة البضاعة المباعة الناتج عن فاتورة البيع"
},
new AccountingIntegrationSourceDefinition
{
    Id = Guid.NewGuid(),
    SourceType = AccountingSourceTypes.SalesInvoice,
    NameAr = "فاتورة بيع",
    Description = "ترحيل قيد محاسبي تلقائي عند إصدار فاتورة بيع",
     ModuleCode = "Saless",
                SortOrder = 90,
                IsSystem = true,
                  IsDynamicPostingEnabled = true,
    IsActive = true
}
        };

        foreach (var item in defaults)
        {
            var existing = await _db.Set<AccountingIntegrationSourceDefinition>().FirstOrDefaultAsync(x => x.SourceType == item.SourceType);
            if (existing == null)
            {
                item.Id = Guid.NewGuid();
                item.CreatedAtUtc = DateTime.UtcNow;
                _db.Set<AccountingIntegrationSourceDefinition>().Add(item);
            }
            else
            {
                existing.NameAr = item.NameAr;
                existing.ModuleCode = item.ModuleCode;
                existing.SortOrder = item.SortOrder;
                existing.IsSystem = true;
                existing.IsActive = true;
                existing.IsDynamicPostingEnabled = item.IsDynamicPostingEnabled;
                existing.EntityClrTypeName = item.EntityClrTypeName;
                existing.IdPropertyName = item.IdPropertyName;
                existing.DatePropertyName = item.DatePropertyName;
                existing.AmountPropertyName = item.AmountPropertyName;
                existing.NumberPropertyName = item.NumberPropertyName;
                existing.TitlePropertyName = item.TitlePropertyName;
                existing.DescriptionTemplate = item.DescriptionTemplate;
                existing.FinancialAccountIdPropertyName = item.FinancialAccountIdPropertyName;
                existing.ProjectIdPropertyName = item.ProjectIdPropertyName;
                existing.CostCenterIdPropertyName = item.CostCenterIdPropertyName;
                existing.EventCodePropertyName = item.EventCodePropertyName;
                existing.DonationTypePropertyName = item.DonationTypePropertyName;
                existing.TargetingScopeCodePropertyName = item.TargetingScopeCodePropertyName;
                existing.PurposeNamePropertyName = item.PurposeNamePropertyName;
                existing.AidTypeIdPropertyName = item.AidTypeIdPropertyName;
                existing.StoreMovementTypePropertyName = item.StoreMovementTypePropertyName;
            }
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "تم تهيئة مصادر الربط الأساسية. يمكنك إضافة مصادر أخرى أو تعديل الأسماء والخرائط الديناميكية من نفس الشاشة.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateDynamicMap(CreateAccountingIntegrationSourceDefinitionVm vm)
    {
        if (!vm.IsDynamicPostingEnabled)
            return;

        if (string.IsNullOrWhiteSpace(vm.EntityClrTypeName))
            ModelState.AddModelError(nameof(vm.EntityClrTypeName), "اسم كلاس الحركة مطلوب عند تفعيل التوليد الديناميكي.");

        if (string.IsNullOrWhiteSpace(vm.DatePropertyName))
            ModelState.AddModelError(nameof(vm.DatePropertyName), "حقل التاريخ مطلوب عند تفعيل التوليد الديناميكي.");

    }

    // ══ API: قائمة الـ Entities والخصائص بالـ Reflection ══
    [HttpGet]
    public IActionResult GetEntityTypes()
    {
        var asm = typeof(InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation).Assembly;
        var types = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.Namespace != null
                     && t.Namespace.StartsWith("InfrastrfuctureManagmentCore.Domains")
                     && t.GetProperty("Id") != null)
            .Select(t => new {
                fullName  = t.FullName ?? t.Name,
                shortName = t.Name,
                module    = (t.Namespace ?? "")
                    .Replace("InfrastrfuctureManagmentCore.Domains.", "")
                    .Split('.').FirstOrDefault() ?? ""
            })
            .OrderBy(t => t.module).ThenBy(t => t.shortName)
            .ToList();
        return Json(types);
    }

    [HttpGet]
    public IActionResult GetEntityProperties(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return Json(Array.Empty<object>());
        var asm  = typeof(InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation).Assembly;
        var type = asm.GetType(typeName);
        if (type == null) return Json(Array.Empty<object>());

        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Select(p => {
                var u = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                return new {
                    name     = p.Name,
                    kind     = u == typeof(DateTime) || u == typeof(DateTimeOffset) ? "date"
                             : u == typeof(decimal)  || u == typeof(double)          ? "amount"
                             : u == typeof(Guid)                                      ? "id"
                             : u == typeof(string)                                    ? "text"
                             : u == typeof(bool)                                      ? "bool"
                             : "other",
                    typeName = u.Name
                };
            })
            .Where(p => p.kind != "other")
            .OrderBy(p => p.name)
            .ToList();
        return Json(props);
    }

    private static void Normalize(CreateAccountingIntegrationSourceDefinitionVm vm)
    {
        vm.SourceType = vm.SourceType?.Trim() ?? string.Empty;
        vm.NameAr = vm.NameAr?.Trim() ?? string.Empty;
        vm.NameEn = NormalizeNullable(vm.NameEn);
        vm.ModuleCode = NormalizeNullable(vm.ModuleCode);
        vm.Description = NormalizeNullable(vm.Description);
        vm.EntityClrTypeName = NormalizeNullable(vm.EntityClrTypeName);
        vm.IdPropertyName = NormalizeNullable(vm.IdPropertyName) ?? "Id";
        vm.DatePropertyName = NormalizeNullable(vm.DatePropertyName);
        vm.AmountPropertyName = NormalizeNullable(vm.AmountPropertyName);
        vm.NumberPropertyName = NormalizeNullable(vm.NumberPropertyName);
        vm.TitlePropertyName = NormalizeNullable(vm.TitlePropertyName);
        vm.DescriptionTemplate = NormalizeNullable(vm.DescriptionTemplate);
        vm.FinancialAccountIdPropertyName = NormalizeNullable(vm.FinancialAccountIdPropertyName);
        vm.ProjectIdPropertyName = NormalizeNullable(vm.ProjectIdPropertyName);
        vm.CostCenterIdPropertyName = NormalizeNullable(vm.CostCenterIdPropertyName);
        vm.EventCodePropertyName = NormalizeNullable(vm.EventCodePropertyName);
        vm.DonationTypePropertyName = NormalizeNullable(vm.DonationTypePropertyName);
        vm.TargetingScopeCodePropertyName = NormalizeNullable(vm.TargetingScopeCodePropertyName);
        vm.PurposeNamePropertyName = NormalizeNullable(vm.PurposeNamePropertyName);
        vm.AidTypeIdPropertyName = NormalizeNullable(vm.AidTypeIdPropertyName);
        vm.StoreMovementTypePropertyName = NormalizeNullable(vm.StoreMovementTypePropertyName);
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
