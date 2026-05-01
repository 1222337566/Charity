using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentCore.Domains.Profiling;

public static class AccountSeedData
{
    public static List<FinancialAccount> GetInitialAccounts()
    {
        return new List<FinancialAccount>
        {
            new FinancialAccount
            {
                Id = AccountSeedIds.Assets,
                AccountCode = "1",
                AccountNameAr = "الأصول",
                Category = AccountCategory.Asset,
                Level = 1,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Liabilities,
                AccountCode = "2",
                AccountNameAr = "الالتزامات",
                Category = AccountCategory.Liability,
                Level = 1,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Equity,
                AccountCode = "3",
                AccountNameAr = "حقوق الملكية",
                Category = AccountCategory.Equity,
                Level = 1,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Revenue,
                AccountCode = "4",
                AccountNameAr = "الإيرادات",
                Category = AccountCategory.Revenue,
                Level = 1,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Expenses,
                AccountCode = "5",
                AccountNameAr = "المصروفات",
                Category = AccountCategory.Expense,
                Level = 1,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.CurrentAssets,
                AccountCode = "11",
                AccountNameAr = "الأصول المتداولة",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.Assets,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.FixedAssets,
                AccountCode = "12",
                AccountNameAr = "الأصول الثابتة",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.Assets,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.OtherAssets,
                AccountCode = "13",
                AccountNameAr = "أصول أخرى",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.Assets,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.CurrentLiabilities,
                AccountCode = "21",
                AccountNameAr = "الالتزامات المتداولة",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.Liabilities,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.LongTermLiabilities,
                AccountCode = "22",
                AccountNameAr = "الالتزامات طويلة الأجل",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.Liabilities,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.EquityMain,
                AccountCode = "31",
                AccountNameAr = "رأس المال وحقوق الملكية",
                Category = AccountCategory.Equity,
                ParentAccountId = AccountSeedIds.Equity,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.PartnersCurrent,
                AccountCode = "32",
                AccountNameAr = "جاري الشركاء",
                Category = AccountCategory.Equity,
                ParentAccountId = AccountSeedIds.Equity,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.RetainedEarnings,
                AccountCode = "33",
                AccountNameAr = "الأرباح والخسائر المرحلة",
                Category = AccountCategory.Equity,
                ParentAccountId = AccountSeedIds.Equity,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.OperatingRevenue,
                AccountCode = "41",
                AccountNameAr = "إيرادات النشاط",
                Category = AccountCategory.Revenue,
                ParentAccountId = AccountSeedIds.Revenue,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.OtherRevenue,
                AccountCode = "42",
                AccountNameAr = "إيرادات أخرى",
                Category = AccountCategory.Revenue,
                ParentAccountId = AccountSeedIds.Revenue,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.CostOfActivity,
                AccountCode = "51",
                AccountNameAr = "تكلفة النشاط",
                Category = AccountCategory.Expense,
                ParentAccountId = AccountSeedIds.Expenses,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.AdminExpenses,
                AccountCode = "52",
                AccountNameAr = "المصروفات الإدارية والعمومية",
                Category = AccountCategory.Expense,
                ParentAccountId = AccountSeedIds.Expenses,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.SellingExpenses,
                AccountCode = "53",
                AccountNameAr = "المصروفات البيعية والتسويقية",
                Category = AccountCategory.Expense,
                ParentAccountId = AccountSeedIds.Expenses,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.FinancingExpenses,
                AccountCode = "54",
                AccountNameAr = "المصروفات التمويلية",
                Category = AccountCategory.Expense,
                ParentAccountId = AccountSeedIds.Expenses,
                Level = 2,
                IsPosting = false,
                IsActive = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.Cash,
                AccountCode = "111",
                AccountNameAr = "النقدية",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Banks,
                AccountCode = "112",
                AccountNameAr = "البنوك",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Customers,
                AccountCode = "113",
                AccountNameAr = "العملاء وأوراق القبض",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Inventory,
                AccountCode = "114",
                AccountNameAr = "المخزون",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.AdvancesAndCustody,
                AccountCode = "115",
                AccountNameAr = "السلف والعهد",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.ProjectsUnderExecution,
                AccountCode = "116",
                AccountNameAr = "المشروعات تحت التنفيذ",
                Category = AccountCategory.Asset,
                ParentAccountId = AccountSeedIds.CurrentAssets,
                Level = 3,
                IsPosting = false,
                IsActive = true,
                RequiresProject = true
            },

            new FinancialAccount
            {
                Id = AccountSeedIds.Suppliers,
                AccountCode = "211",
                AccountNameAr = "الموردون",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.CurrentLiabilities,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Subcontractors,
                AccountCode = "212",
                AccountNameAr = "مقاولو الباطن",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.CurrentLiabilities,
                Level = 3,
                IsPosting = false,
                IsActive = true,
                RequiresProject = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.AccruedExpenses,
                AccountCode = "213",
                AccountNameAr = "المصروفات المستحقة",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.CurrentLiabilities,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.Taxes,
                AccountCode = "214",
                AccountNameAr = "الضرائب",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.CurrentLiabilities,
                Level = 3,
                IsPosting = false,
                IsActive = true
            },
            new FinancialAccount
            {
                Id = AccountSeedIds.CustomerAdvances,
                AccountCode = "215",
                AccountNameAr = "دفعات مقدمة من العملاء",
                Category = AccountCategory.Liability,
                ParentAccountId = AccountSeedIds.CurrentLiabilities,
                Level = 3,
                IsPosting = false,
                IsActive = true
            }
        };
    }
}