using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentWebFramework.Models.FinancialAccounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Web.Mvc;

public class FinancialAccountController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly IAccountRepository _accountRepository;

    public FinancialAccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountRepository.GetAllAsync();
        var allVms = accounts.Select(x => new AccountListItemVm
        {
            Id = x.Id,
            AccountCode = x.AccountCode,
            AccountNameAr = x.AccountNameAr,
            Level = x.Level,
            IsPosting = x.IsPosting,
            IsActive = x.IsActive,
            RequiresProject = x.RequiresProject,
            RequiresCostCenter = x.RequiresCostCenter,
            CashKind = x.CashKind,
            AllowNegativeCashBalance = x.AllowNegativeCashBalance,
            ParentAccountId = x.ParentAccountId,
            Category = x.Category.ToString()
        }).ToList();

        // ترتيب DFS: الجذر ثم أبناؤه بالتسلسل
        var byParent = allVms.ToLookup(x => x.ParentAccountId);
        var result   = new List<(AccountListItemVm Vm, int Depth, int ChildCount)>();

        void Dfs(Guid? parentId, int depth)
        {
            foreach (var vm in byParent[parentId].OrderBy(x => x.AccountCode))
            {
                var kids = byParent[vm.Id].Count();
                result.Add((vm, depth, kids));
                Dfs(vm.Id, depth + 1);
            }
        }
        Dfs(null, 0);

        ViewBag.Tree = result;
        return View(allVms);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet]
    public async Task<IActionResult> Create(Guid? parentId)
    {
        var vm = new CreateAccountVm { ParentAccountId = parentId };
        await FillParents(vm);

        // اقترح كود تلقائي إن عُرف الأب
        if (parentId.HasValue)
        {
            var accounts = await _accountRepository.GetAllAsync();
            var parent   = accounts.FirstOrDefault(x => x.Id == parentId.Value);
            if (parent != null)
            {
                var siblings = accounts.Where(x => x.ParentAccountId == parentId.Value).ToList();
                var seq      = siblings.Count + 1;
                vm.AccountCode     = $"{parent.AccountCode}{seq:00}";
                vm.ParentAccountId = parentId;
            }
        }
        return View(vm);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountVm vm)
    {
        await FillParents(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var code = vm.AccountCode.Trim();

        if (await _accountRepository.AccountCodeExistsAsync(code))
        {
            ModelState.AddModelError(nameof(vm.AccountCode), "كود الحساب موجود بالفعل");
            return View(vm);
        }

        FinancialAccount account;

        if (vm.ParentAccountId.HasValue)
        {
            var parent = await _accountRepository.GetByIdAsync(vm.ParentAccountId.Value);
            if (parent == null)
            {
                ModelState.AddModelError(nameof(vm.ParentAccountId), "الحساب الأب غير موجود");
                return View(vm);
            }

            if (parent.IsPosting)
            {
                ModelState.AddModelError(nameof(vm.ParentAccountId), "لا يمكن الإضافة تحت حساب حركي");
                return View(vm);
            }

            account = new FinancialAccount
            {
                Id = Guid.NewGuid(),
                AccountCode = code,
                AccountNameAr = vm.AccountNameAr.Trim(),
                AccountNameEn = vm.AccountNameEn?.Trim(),
                ParentAccountId = parent.Id,
                Category = parent.Category,
                Level = parent.Level + 1,
                IsPosting = vm.IsPosting,
                IsActive = true,
                RequiresProject = vm.RequiresProject,
                RequiresCostCenter = vm.RequiresCostCenter,
                CashKind = vm.IsPosting ? vm.CashKind : FinancialAccountCashKind.None,
                AllowNegativeCashBalance = vm.IsPosting && vm.AllowNegativeCashBalance,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
        else
        {
            if (!vm.Category.HasValue)
            {
                ModelState.AddModelError(nameof(vm.Category), "اختر فئة الحساب الرئيسي");
                return View(vm);
            }

            if (vm.IsPosting)
            {
                ModelState.AddModelError(nameof(vm.IsPosting), "الحساب الرئيسي يجب أن يكون تجميعيًا وليس حركيًا");
                return View(vm);
            }

            account = new FinancialAccount
            {
                Id = Guid.NewGuid(),
                AccountCode = code,
                AccountNameAr = vm.AccountNameAr.Trim(),
                AccountNameEn = vm.AccountNameEn?.Trim(),
                ParentAccountId = null,
                Category = vm.Category.Value,
                Level = 1,
                IsPosting = false,
                IsActive = true,
                RequiresProject = vm.RequiresProject,
                RequiresCostCenter = vm.RequiresCostCenter,
                CashKind = FinancialAccountCashKind.None,
                AllowNegativeCashBalance = false,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        await _accountRepository.AddAsync(account);

        TempData["Success"] = vm.ParentAccountId.HasValue
            ? "تم إضافة الحساب الفرعي بنجاح"
            : "تم إضافة الحساب الرئيسي بنجاح";

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAccountVm vm)
    {
        await FillParentsForEdit(vm, vm.Id);

        if (!ModelState.IsValid)
            return View(vm);

        var account = await _accountRepository.GetByIdAsync(vm.Id);
        if (account == null)
            return NotFound();

        if (await _accountRepository.AccountCodeExistsAsync(vm.AccountCode, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.AccountCode), "كود الحساب موجود بالفعل");
            return View(vm);
        }

        var parent = await _accountRepository.GetByIdAsync(vm.ParentAccountId!.Value);
        if (parent == null)
        {
            ModelState.AddModelError(nameof(vm.ParentAccountId), "الحساب الأب غير موجود");
            return View(vm);
        }

        if (parent.Id == account.Id)
        {
            ModelState.AddModelError(nameof(vm.ParentAccountId), "لا يمكن اختيار نفس الحساب كأب");
            return View(vm);
        }

        if (parent.IsPosting)
        {
            ModelState.AddModelError(nameof(vm.ParentAccountId), "لا يمكن ربط الحساب تحت حساب حركي");
            return View(vm);
        }

        var hasChildren = await _accountRepository.HasChildrenAsync(account.Id);

        // حساب عنده أبناء لازم يظل تجميعي
        if (hasChildren && vm.IsPosting)
        {
            ModelState.AddModelError(nameof(vm.IsPosting), "لا يمكن تحويل الحساب إلى حركي لأنه يحتوي على حسابات فرعية");
            return View(vm);
        }

        account.AccountCode = vm.AccountCode.Trim();
        account.AccountNameAr = vm.AccountNameAr.Trim();
        account.AccountNameEn = vm.AccountNameEn?.Trim();
        account.ParentAccountId = parent.Id;
        account.Category = parent.Category;
        account.Level = parent.Level + 1;
        account.IsPosting = vm.IsPosting;
        account.IsActive = vm.IsActive;
        account.RequiresProject = vm.RequiresProject;
        account.RequiresCostCenter = vm.RequiresCostCenter;
        account.CashKind = vm.IsPosting ? vm.CashKind : FinancialAccountCashKind.None;
        account.AllowNegativeCashBalance = vm.IsPosting && vm.AllowNegativeCashBalance;
        account.UpdatedAtUtc = DateTime.UtcNow;

        await _accountRepository.UpdateAsync(account);

        TempData["Success"] = "تم تعديل الحساب بنجاح";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return NotFound();

        var vm = new EditAccountVm
        {
            Id = account.Id,
            AccountCode = account.AccountCode,
            AccountNameAr = account.AccountNameAr,
            AccountNameEn = account.AccountNameEn,
            ParentAccountId = account.ParentAccountId,
            IsPosting = account.IsPosting,
            IsActive = account.IsActive,
            RequiresProject = account.RequiresProject,
            RequiresCostCenter = account.RequiresCostCenter,
            CashKind = account.CashKind,
            AllowNegativeCashBalance = account.AllowNegativeCashBalance
        };

        await FillParentsForEdit(vm, account.Id);

        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return NotFound();

        // لو الحساب تجميعي وعليه أبناء نشطين، ماينفعش نقفله
        if (account.IsActive)
        {
            var hasActiveChildren = await _accountRepository.HasActiveChildrenAsync(account.Id);
            if (hasActiveChildren)
            {
                TempData["Error"] = "لا يمكن تعطيل الحساب لأنه يحتوي على حسابات فرعية نشطة";
                return RedirectToAction(nameof(Index));
            }

            // لاحقًا هنا نضيف شرط:
            // لو الحساب عليه حركات محاسبية لا يمكن تعطيله
        }

        account.IsActive = !account.IsActive;
        account.UpdatedAtUtc = DateTime.UtcNow;

        await _accountRepository.UpdateAsync(account);

        TempData["Success"] = account.IsActive
            ? "تم تفعيل الحساب بنجاح"
            : "تم تعطيل الحساب بنجاح";

        return RedirectToAction(nameof(Index));
    }
    private async Task FillParents(CreateAccountVm vm)
    {
        var parents = await _accountRepository.GetAllParentsAsync();

        vm.ParentAccounts = parents
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToList();

        vm.Categories = Enum.GetValues<AccountCategory>()
            .Select(x => new SelectListItem
            {
                Value = ((int)x).ToString(),
                Text = GetCategoryText(x)
            })
            .ToList();

        vm.CashKinds = GetCashKindOptions();
    }
    private async Task FillParentsForEdit(EditAccountVm vm, Guid currentId)
    {
        var parents = await _accountRepository.GetAllParentsAsync();

        vm.ParentAccounts = parents
            .Where(x => x.Id != currentId) // لا يظهر نفسه
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AccountCode} - {x.AccountNameAr}"
            })
            .ToList();

        vm.CashKinds = GetCashKindOptions();
    }

    private static List<SelectListItem> GetCashKindOptions()
    {
        return Enum.GetValues<FinancialAccountCashKind>()
            .Select(x => new SelectListItem
            {
                Value = ((int)x).ToString(),
                Text = GetCashKindText(x)
            })
            .ToList();
    }

    private static string GetCashKindText(FinancialAccountCashKind kind)
    {
        return kind switch
        {
            FinancialAccountCashKind.Treasury => "خزينة",
            FinancialAccountCashKind.Bank => "بنك",
            _ => "غير نقدي"
        };
    }

    private static string GetCategoryText(AccountCategory category)
    {
        return category switch
        {
            AccountCategory.Asset => "أصول",
            AccountCategory.Liability => "خصوم",
            AccountCategory.Equity => "حقوق ملكية",
            AccountCategory.Revenue => "إيرادات",
            AccountCategory.Expense => "مصروفات",
            _ => category.ToString()
        };
    }
}