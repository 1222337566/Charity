using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

public static class AccountSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedLevel1Async(db);
        await db.SaveChangesAsync();
        await SeedLevel2Async(db);
        await db.SaveChangesAsync();
        await SeedLevel3Async(db);
        await db.SaveChangesAsync();
        await SeedLevel4Async(db);
        await db.SaveChangesAsync();


    }

    // =========================
    // المستوى الأول
    // =========================
    private static async Task SeedLevel1Async(AppDbContext db)
    {
        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "1",
            AccountNameAr = "الأصول",
            Category = AccountCategory.Asset,
            Level = 1,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "2",
            AccountNameAr = "الالتزامات",
            Category = AccountCategory.Liability,
            Level = 1,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "3",
            AccountNameAr = "حقوق الملكية",
            Category = AccountCategory.Equity,
            Level = 1,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "4",
            AccountNameAr = "الإيرادات",
            Category = AccountCategory.Revenue,
            Level = 1,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "5",
            AccountNameAr = "المصروفات",
            Category = AccountCategory.Expense,
            Level = 1,
            IsPosting = false
        });
    }

    // =========================
    // المستوى الثاني
    // =========================
    private static async Task SeedLevel2Async(AppDbContext db)
    {
        var assets = await db.Accounts.FirstAsync(x => x.AccountCode == "1");
        var liabilities = await db.Accounts.FirstAsync(x => x.AccountCode == "2");
        var equity = await db.Accounts.FirstAsync(x => x.AccountCode == "3");
        var revenue = await db.Accounts.FirstAsync(x => x.AccountCode == "4");
        var expenses = await db.Accounts.FirstAsync(x => x.AccountCode == "5");

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11",
            AccountNameAr = "الأصول المتداولة",
            Category = AccountCategory.Asset,
            ParentAccountId = assets.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "12",
            AccountNameAr = "الأصول الثابتة",
            Category = AccountCategory.Asset,
            ParentAccountId = assets.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "13",
            AccountNameAr = "أصول أخرى",
            Category = AccountCategory.Asset,
            ParentAccountId = assets.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21",
            AccountNameAr = "الالتزامات المتداولة",
            Category = AccountCategory.Liability,
            ParentAccountId = liabilities.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "22",
            AccountNameAr = "الالتزامات طويلة الأجل",
            Category = AccountCategory.Liability,
            ParentAccountId = liabilities.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "31",
            AccountNameAr = "رأس المال وحقوق الملكية",
            Category = AccountCategory.Equity,
            ParentAccountId = equity.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "32",
            AccountNameAr = "جاري الشركاء",
            Category = AccountCategory.Equity,
            ParentAccountId = equity.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "33",
            AccountNameAr = "الأرباح والخسائر المرحلة",
            Category = AccountCategory.Equity,
            ParentAccountId = equity.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "41",
            AccountNameAr = "إيرادات النشاط",
            Category = AccountCategory.Revenue,
            ParentAccountId = revenue.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "42",
            AccountNameAr = "إيرادات أخرى",
            Category = AccountCategory.Revenue,
            ParentAccountId = revenue.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51",
            AccountNameAr = "تكلفة النشاط",
            Category = AccountCategory.Expense,
            ParentAccountId = expenses.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "52",
            AccountNameAr = "المصروفات الإدارية والعمومية",
            Category = AccountCategory.Expense,
            ParentAccountId = expenses.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "53",
            AccountNameAr = "المصروفات البيعية والتسويقية",
            Category = AccountCategory.Expense,
            ParentAccountId = expenses.Id,
            Level = 2,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "54",
            AccountNameAr = "المصروفات التمويلية",
            Category = AccountCategory.Expense,
            ParentAccountId = expenses.Id,
            Level = 2,
            IsPosting = false
        });
    }

    // =========================
    // المستوى الثالث
    // =========================
    private static async Task SeedLevel3Async(AppDbContext db)
    {
        var currentAssets = await db.Accounts.FirstAsync(x => x.AccountCode == "11");
        var currentLiabilities = await db.Accounts.FirstAsync(x => x.AccountCode == "21");
        var operatingRevenue = await db.Accounts.FirstAsync(x => x.AccountCode == "41");
        var costOfActivity = await db.Accounts.FirstAsync(x => x.AccountCode == "51");

            await AddIfNotExistsAsync(db, new   FinancialAccount
        {
            AccountCode = "111",
            AccountNameAr = "النقدية",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "112",
            AccountNameAr = "البنوك",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "113",
            AccountNameAr = "العملاء وأوراق القبض",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "114",
            AccountNameAr = "المخزون",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "115",
            AccountNameAr = "السلف والعهد",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "116",
            AccountNameAr = "المشروعات تحت التنفيذ",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "117",
            AccountNameAr = "دفعات مقدمة للغير",
            Category = AccountCategory.Asset,
            ParentAccountId = currentAssets.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "211",
            AccountNameAr = "الموردون",
            Category = AccountCategory.Liability,
            ParentAccountId = currentLiabilities.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "212",
            AccountNameAr = "مقاولو الباطن",
            Category = AccountCategory.Liability,
            ParentAccountId = currentLiabilities.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "213",
            AccountNameAr = "المصروفات المستحقة",
            Category = AccountCategory.Liability,
            ParentAccountId = currentLiabilities.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "214",
            AccountNameAr = "الضرائب",
            Category = AccountCategory.Liability,
            ParentAccountId = currentLiabilities.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "215",
            AccountNameAr = "دفعات مقدمة من العملاء",
            Category = AccountCategory.Liability,
            ParentAccountId = currentLiabilities.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "411",
            AccountNameAr = "إيرادات عقود المقاولات",
            Category = AccountCategory.Revenue,
            ParentAccountId = operatingRevenue.Id,
            Level = 3,
            IsPosting = false
        });

            await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "412",
            AccountNameAr = "إيرادات أخرى للنشاط",
            Category = AccountCategory.Revenue,
            ParentAccountId = operatingRevenue.Id,
            Level = 3,
            IsPosting = false
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "511",
            AccountNameAr = "تكلفة مواد مباشرة",
            Category = AccountCategory.Expense,
            ParentAccountId = costOfActivity.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "512",
            AccountNameAr = "تكلفة أجور مباشرة",
            Category = AccountCategory.Expense,
            ParentAccountId = costOfActivity.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "513",
            AccountNameAr = "تكلفة مقاولي باطن",
            Category = AccountCategory.Expense,
            ParentAccountId = costOfActivity.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "514",
            AccountNameAr = "مصروفات مواقع ومشروعات",
            Category = AccountCategory.Expense,
            ParentAccountId = costOfActivity.Id,
            Level = 3,
            IsPosting = false,
            RequiresProject = true
        });
    }

    // =========================
    // المستوى الرابع
    // =========================
    private static async Task SeedLevel4Async(AppDbContext db)
    {
        var cash = await db.Accounts.FirstAsync(x => x.AccountCode == "111");
        var banks = await db.Accounts.FirstAsync(x => x.AccountCode == "112");
        var customers = await db.Accounts.FirstAsync(x => x.AccountCode == "113");
        var inventory = await db.Accounts.FirstAsync(x => x.AccountCode == "114");
        var custody = await db.Accounts.FirstAsync(x => x.AccountCode == "115");
        var projects = await db.Accounts.FirstAsync(x => x.AccountCode == "116");
        var advances = await db.Accounts.FirstAsync(x => x.AccountCode == "117");

        var suppliers = await db.Accounts.FirstAsync(x => x.AccountCode == "211");
        var subcontractors = await db.Accounts.FirstAsync(x => x.AccountCode == "212");
        var taxes = await db.Accounts.FirstAsync(x => x.AccountCode == "214");
        var customerAdvances = await db.Accounts.FirstAsync(x => x.AccountCode == "215");

        var contractRevenue = await db.Accounts.FirstAsync(x => x.AccountCode == "411");
        var otherOperatingRevenue = await db.Accounts.FirstAsync(x => x.AccountCode == "412");

        var directMaterials = await db.Accounts.FirstAsync(x => x.AccountCode == "511");
        var directWages = await db.Accounts.FirstAsync(x => x.AccountCode == "512");
        var subcontractorCost = await db.Accounts.FirstAsync(x => x.AccountCode == "513");
        var projectExpenses = await db.Accounts.FirstAsync(x => x.AccountCode == "514");

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11101",
            AccountNameAr = "الخزنة الرئيسية",
            Category = AccountCategory.Asset,
            ParentAccountId = cash.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11102",
            AccountNameAr = "خزنة الموقع",
            Category = AccountCategory.Asset,
            ParentAccountId = cash.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11103",
            AccountNameAr = "خزنة المصروفات اليومية",
            Category = AccountCategory.Asset,
            ParentAccountId = cash.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11201",
            AccountNameAr = "البنك الأهلي",
            Category = AccountCategory.Asset,
            ParentAccountId = banks.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11202",
            AccountNameAr = "بنك مصر",
            Category = AccountCategory.Asset,
            ParentAccountId = banks.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11203",
            AccountNameAr = "البنك التجاري الدولي",
            Category = AccountCategory.Asset,
            ParentAccountId = banks.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11301",
            AccountNameAr = "العملاء المحليون",
            Category = AccountCategory.Asset,
            ParentAccountId = customers.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11302",
            AccountNameAr = "أوراق القبض",
            Category = AccountCategory.Asset,
            ParentAccountId = customers.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11303",
            AccountNameAr = "مستخلصات تحت التحصيل",
            Category = AccountCategory.Asset,
            ParentAccountId = customers.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11401",
            AccountNameAr = "مخزون مواد خام",
            Category = AccountCategory.Asset,
            ParentAccountId = inventory.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new   FinancialAccount
        {
            AccountCode = "11402",
            AccountNameAr = "مخزون أدوات ومهمات",
            Category = AccountCategory.Asset,
            ParentAccountId = inventory.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11403",
            AccountNameAr = "مخزون قطع غيار",
            Category = AccountCategory.Asset,
            ParentAccountId = inventory.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11501",
            AccountNameAr = "عهد موظفين",
            Category = AccountCategory.Asset,
            ParentAccountId = custody.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11502",
            AccountNameAr = "عهد مهندسين مواقع",
            Category = AccountCategory.Asset,
            ParentAccountId = custody.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11503",
            AccountNameAr = "سلف عاملين",
            Category = AccountCategory.Asset,
            ParentAccountId = custody.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11601",
            AccountNameAr = "مشروع 1 تحت التنفيذ",
            Category = AccountCategory.Asset,
            ParentAccountId = projects.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11602",
            AccountNameAr = "مشروع 2 تحت التنفيذ",
            Category = AccountCategory.Asset,
            ParentAccountId = projects.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11701",
            AccountNameAr = "دفعات مقدمة للموردين",
            Category = AccountCategory.Asset,
            ParentAccountId = advances.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "11702",
            AccountNameAr = "دفعات مقدمة لمقاولي الباطن",
            Category = AccountCategory.Asset,
            ParentAccountId = advances.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21101",
            AccountNameAr = "الموردون المحليون",
            Category = AccountCategory.Liability,
            ParentAccountId = suppliers.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21102",
            AccountNameAr = "الموردون الخارجيون",
            Category = AccountCategory.Liability,
            ParentAccountId = suppliers.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21201",
            AccountNameAr = "مقاولو باطن محليون",
            Category = AccountCategory.Liability,
            ParentAccountId = subcontractors.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21202",
            AccountNameAr = "مقاولو باطن متخصصون",
            Category = AccountCategory.Liability,
            ParentAccountId = subcontractors.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21401",
            AccountNameAr = "ضريبة القيمة المضافة",
            Category = AccountCategory.Liability,
            ParentAccountId = taxes.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21402",
            AccountNameAr = "ضرائب خصم وإضافة",
            Category = AccountCategory.Liability,
            ParentAccountId = taxes.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "21501",
            AccountNameAr = "دفعات مقدمة من العملاء",
            Category = AccountCategory.Liability,
            ParentAccountId = customerAdvances.Id,
            Level = 4,
            IsPosting = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "41101",
            AccountNameAr = "إيرادات العقود الرئيسية",
            Category = AccountCategory.Revenue,
            ParentAccountId = contractRevenue.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "41102",
            AccountNameAr = "إيرادات المستخلصات",
            Category = AccountCategory.Revenue,
            ParentAccountId = contractRevenue.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "41201",
            AccountNameAr = "إيرادات أعمال إضافية",
            Category = AccountCategory.Revenue,
            ParentAccountId = otherOperatingRevenue.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51101",
            AccountNameAr = "تكلفة مواد مباشرة للمشروع",
            Category = AccountCategory.Expense,
            ParentAccountId = directMaterials.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51201",
            AccountNameAr = "أجور مباشرة للمشروع",
            Category = AccountCategory.Expense,
            ParentAccountId = directWages.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51301",
            AccountNameAr = "تكلفة مقاولي الباطن للمشروع",
            Category = AccountCategory.Expense,
            ParentAccountId = subcontractorCost.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51401",
            AccountNameAr = "مصروفات موقع",
            Category = AccountCategory.Expense,
            ParentAccountId = projectExpenses.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51402",
            AccountNameAr = "نقل وشحن للمشروع",
            Category = AccountCategory.Expense,
            ParentAccountId = projectExpenses.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });

        await AddIfNotExistsAsync(db, new FinancialAccount
        {
            AccountCode = "51403",
            AccountNameAr = "وقود وزيوت للمشروع",
            Category = AccountCategory.Expense,
            ParentAccountId = projectExpenses.Id,
            Level = 4,
            IsPosting = true,
            RequiresProject = true
        });
    }

    private static async Task AddIfNotExistsAsync(AppDbContext db, FinancialAccount account)
    {
        var exists = await db.Accounts.AnyAsync(x => x.AccountCode == account.AccountCode);
        if (!exists)
        {
            account.Id = Guid.NewGuid();
            account.IsActive = true;
            await db.Accounts.AddAsync(account);
        }
    }
}