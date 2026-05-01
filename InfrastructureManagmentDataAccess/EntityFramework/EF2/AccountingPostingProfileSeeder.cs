using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public static class AccountingPostingProfileSeeder
    {
        private const string CashDonationType = "نقدي";
        private const string InKindDonationType = "عيني";

        public static async Task SeedAsync(AppDbContext db)
        {
            await EnsureCharityAccountsAsync(db);
            await db.SaveChangesAsync();

            var mainCash = await AccountByCodeAsync(db, "11101");
            var inKindInventory = await AccountByCodeAsync(db, "11410");
            var generalDonations = await AccountByCodeAsync(db, "41301");
            var restrictedAidDonations = await AccountByCodeAsync(db, "41302");
            var zakatFitrDonations = await AccountByCodeAsync(db, "41303");
            var ramadanBagDonations = await AccountByCodeAsync(db, "41304");
            var inKindDonations = await AccountByCodeAsync(db, "41305");
            var cashAidExpense = await AccountByCodeAsync(db, "51501");
            var inKindAidExpense = await AccountByCodeAsync(db, "51502");

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-GENERAL-CASH",
                NameAr = "تحصيل تبرع عام نقدي",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.DonationReceipt,
                DonationType = CashDonationType,
                TargetingScopeCode = "GeneralFund",
                DebitAccountId = mainCash.Id,
                CreditAccountId = generalDonations.Id,
                UseSourceFinancialAccountAsDebit = true,
                Notes = "Dr Cash/Bank - Cr General Donations. The debit side can be replaced by Donation.FinancialAccountId."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-RESTRICTED-AID",
                NameAr = "تحصيل تبرع مقيد لطلب مساعدة",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.DonationReceipt,
                DonationType = CashDonationType,
                TargetingScopeCode = "SpecificRequests",
                DebitAccountId = mainCash.Id,
                CreditAccountId = restrictedAidDonations.Id,
                UseSourceFinancialAccountAsDebit = true,
                Notes = "Dr Cash/Bank - Cr Restricted Donations for approved aid requests."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-ZAKAT-FITR",
                NameAr = "تحصيل زكاة الفطر",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.DonationReceipt,
                DonationType = CashDonationType,
                TargetingScopeCode = "GeneralPurpose",
                PurposeName = "زكاة الفطر",
                DebitAccountId = mainCash.Id,
                CreditAccountId = zakatFitrDonations.Id,
                UseSourceFinancialAccountAsDebit = true,
                Notes = "Dr Cash/Bank - Cr Restricted Donations - Zakat Al-Fitr."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-RAMADAN-BAG-CASH",
                NameAr = "تحصيل تبرع نقدي لشنطة رمضان",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.DonationReceipt,
                DonationType = CashDonationType,
                TargetingScopeCode = "GeneralPurpose",
                PurposeName = "شنطة رمضان",
                DebitAccountId = mainCash.Id,
                CreditAccountId = ramadanBagDonations.Id,
                UseSourceFinancialAccountAsDebit = true,
                Notes = "Dr Cash/Bank - Cr Restricted Donations - Ramadan Bag."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-INKIND-RECEIPT",
                NameAr = "استلام تبرع عيني",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.InKindDonationReceipt,
                DonationType = InKindDonationType,
                DebitAccountId = inKindInventory.Id,
                CreditAccountId = inKindDonations.Id,
                Notes = "Dr In-Kind Donations Inventory - Cr In-Kind Donations."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "DON-RAMADAN-BAG-INKIND",
                NameAr = "استلام تبرع عيني لشنطة رمضان",
                ModuleCode = AccountingPostingModuleCodes.Donations,
                EventCode = AccountingPostingEventCodes.InKindDonationReceipt,
                DonationType = InKindDonationType,
                TargetingScopeCode = "GeneralPurpose",
                PurposeName = "شنطة رمضان",
                DebitAccountId = inKindInventory.Id,
                CreditAccountId = ramadanBagDonations.Id,
                Notes = "Dr In-Kind Donations Inventory - Cr Restricted Donations - Ramadan Bag."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "AID-CASH-PAYMENT",
                NameAr = "صرف مساعدة نقدية",
                ModuleCode = AccountingPostingModuleCodes.Aid,
                EventCode = AccountingPostingEventCodes.AidCashDisbursement,
                DebitAccountId = cashAidExpense.Id,
                CreditAccountId = mainCash.Id,
                UseSourceFinancialAccountAsCredit = true,
                Notes = "Dr Cash Aid Expense - Cr Cash/Bank. The credit side can be replaced by the payment voucher financial account."
            });

            await AddOrUpdateProfileAsync(db, new AccountingPostingProfile
            {
                Code = "AID-INKIND-ISSUE",
                NameAr = "صرف مساعدة عينية من المخزن",
                ModuleCode = AccountingPostingModuleCodes.Aid,
                EventCode = AccountingPostingEventCodes.AidInKindIssue,
                DonationType = InKindDonationType,
                DebitAccountId = inKindAidExpense.Id,
                CreditAccountId = inKindInventory.Id,
                Notes = "Dr In-Kind Aid Expense - Cr In-Kind Donations Inventory."
            });

            await db.SaveChangesAsync();
        }

        private static async Task EnsureCharityAccountsAsync(AppDbContext db)
        {
            var inventory = await AccountByCodeAsync(db, "114");
            var operatingRevenue = await AccountByCodeAsync(db, "41");
            var costOfActivity = await AccountByCodeAsync(db, "51");

            await AddAccountIfMissingAsync(db, "11410", "مخزون التبرعات العينية", AccountCategory.Asset, inventory.Id, 4, true);

            await AddAccountIfMissingAsync(db, "413", "تبرعات الجمعية", AccountCategory.Revenue, operatingRevenue.Id, 3, false);
            await db.SaveChangesAsync();
            var charityDonations = await AccountByCodeAsync(db, "413");
            await AddAccountIfMissingAsync(db, "41301", "التبرعات العامة", AccountCategory.Revenue, charityDonations.Id, 4, true);
            await AddAccountIfMissingAsync(db, "41302", "التبرعات المقيدة لطلبات المساعدة", AccountCategory.Revenue, charityDonations.Id, 4, true);
            await AddAccountIfMissingAsync(db, "41303", "تبرعات مقيدة - زكاة الفطر", AccountCategory.Revenue, charityDonations.Id, 4, true);
            await AddAccountIfMissingAsync(db, "41304", "تبرعات مقيدة - شنطة رمضان", AccountCategory.Revenue, charityDonations.Id, 4, true);
            await AddAccountIfMissingAsync(db, "41305", "التبرعات العينية", AccountCategory.Revenue, charityDonations.Id, 4, true);

            await AddAccountIfMissingAsync(db, "515", "مصروفات المساعدات", AccountCategory.Expense, costOfActivity.Id, 3, false);
            await db.SaveChangesAsync();
            var aidExpenses = await AccountByCodeAsync(db, "515");
            await AddAccountIfMissingAsync(db, "51501", "مصروف مساعدات نقدية", AccountCategory.Expense, aidExpenses.Id, 4, true);
            await AddAccountIfMissingAsync(db, "51502", "مصروف مساعدات عينية", AccountCategory.Expense, aidExpenses.Id, 4, true);
        }

        private static async Task<FinancialAccount> AccountByCodeAsync(AppDbContext db, string accountCode)
        {
            return await db.Accounts.FirstAsync(x => x.AccountCode == accountCode);
        }

        private static async Task AddAccountIfMissingAsync(
            AppDbContext db,
            string accountCode,
            string accountNameAr,
            AccountCategory category,
            Guid? parentAccountId,
            int level,
            bool isPosting)
        {
            var exists = await db.Accounts.AnyAsync(x => x.AccountCode == accountCode);
            if (exists)
                return;

            await db.Accounts.AddAsync(new FinancialAccount
            {
                Id = Guid.NewGuid(),
                AccountCode = accountCode,
                AccountNameAr = accountNameAr,
                Category = category,
                ParentAccountId = parentAccountId,
                Level = level,
                IsPosting = isPosting,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        private static async Task AddOrUpdateProfileAsync(AppDbContext db, AccountingPostingProfile incoming)
        {
            var existing = await db.AccountingPostingProfiles.FirstOrDefaultAsync(x => x.Code == incoming.Code);
            if (existing == null)
            {
                incoming.Id = Guid.NewGuid();
                incoming.AutoPost = true;
                incoming.IsActive = true;
                incoming.CreatedAtUtc = DateTime.UtcNow;
                await db.AccountingPostingProfiles.AddAsync(incoming);
                return;
            }

            existing.NameAr = incoming.NameAr;
            existing.NameEn = incoming.NameEn;
            existing.Description = incoming.Description;
            existing.ModuleCode = incoming.ModuleCode;
            existing.EventCode = incoming.EventCode;
            existing.DonationType = incoming.DonationType;
            existing.TargetingScopeCode = incoming.TargetingScopeCode;
            existing.PurposeName = incoming.PurposeName;
            existing.DebitAccountId = incoming.DebitAccountId;
            existing.CreditAccountId = incoming.CreditAccountId;
            existing.UseSourceFinancialAccountAsDebit = incoming.UseSourceFinancialAccountAsDebit;
            existing.UseSourceFinancialAccountAsCredit = incoming.UseSourceFinancialAccountAsCredit;
            existing.DefaultCostCenterId = incoming.DefaultCostCenterId;
            existing.Notes = incoming.Notes;
            existing.IsActive = true;
            existing.AutoPost = true;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
