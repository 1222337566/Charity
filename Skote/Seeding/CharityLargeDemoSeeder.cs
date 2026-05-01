using System.Security.Cryptography;
using System.Text;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Skote.Seeding;

public sealed class CharityLargeDemoSeedOptions
{
    public bool Enabled { get; set; }
    public bool ResetExisting { get; set; }
    public int BeneficiaryCount { get; set; } = 8000;
    public int BatchSize { get; set; } = 500;
    public int DonorCount { get; set; } = 800;
    public int CashDonationCount { get; set; } = 1200;
    public int InKindDonationCount { get; set; } = 240;
}

public sealed class CharityLargeDemoSeedResult
{
    public bool Skipped { get; set; }
    public int Beneficiaries { get; set; }
    public int FamilyMembers { get; set; }
    public int ResearchForms { get; set; }
    public int AidRequests { get; set; }
    public int Donations { get; set; }
    public int DonationAllocations { get; set; }
    public int Disbursements { get; set; }
    public int Projects { get; set; }
    public int StoreReceipts { get; set; }
    public int StoreIssues { get; set; }
    public int JournalEntries { get; set; }
}

/// <summary>
/// Large, deterministic, linked demo-data seeder for the charity solution.
/// It is intentionally gated by configuration and never runs unless CharityLargeDemoSeed:Enabled=true.
/// </summary>
public static class CharityLargeDemoSeeder
{
    private const string SeedUserId = "charity-large-demo-seeder";
    private const string BeneficiaryCodePrefix = "D-BEN-";
    private const string ResearchPrefix = "D-HUM-";
    private const string DonorCodePrefix = "D-DON-";
    private const string DonationNumberPrefix = "D-DNT-";
    private const string InKindDonationNumberPrefix = "D-IKD-";
    private const string FunderCodePrefix = "D-FND-";
    private const string ProjectCodePrefix = "D-PRJ-";
    private const string ReceiptPrefix = "D-RCP-";
    private const string IssuePrefix = "D-ISS-";
    private const string AidCyclePrefix = "D-CYC-";
    private const string JournalPrefix = "D-JRN-";
    private const string EmployeeCodePrefix = "D-EMP-";

    private static readonly string[] MaleNames =
    {
        "محمد", "أحمد", "محمود", "مصطفى", "علي", "حسن", "إبراهيم", "طارق", "خالد", "عمر", "عبد الرحمن", "يوسف"
    };

    private static readonly string[] FemaleNames =
    {
        "فاطمة", "أسماء", "مريم", "سعاد", "آية", "هدى", "دعاء", "نجلاء", "أميرة", "منى", "رحمة", "نور"
    };

    private static readonly string[] FamilyNames =
    {
        "أحمد", "محمد", "عبد الله", "السيد", "حسن", "بكر", "محمود", "حامد", "عتمان", "عبد العال", "النجار", "الشاذلي"
    };

    private static readonly string[] Areas =
    {
        "جرجا", "سوهاج", "طهطا", "المنشاة", "أخميم", "البلينا", "دار السلام", "المراغة", "ساقلته", "جهينة"
    };

    public static async Task<CharityLargeDemoSeedResult> SeedAsync(AppDbContext db, CharityLargeDemoSeedOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new CharityLargeDemoSeedOptions();
        options.BeneficiaryCount = Math.Max(1, options.BeneficiaryCount);
        options.BatchSize = Math.Clamp(options.BatchSize <= 0 ? 500 : options.BatchSize, 100, 2000);
        options.DonorCount = Math.Clamp(options.DonorCount <= 0 ? options.BeneficiaryCount / 10 : options.DonorCount, 50, 5000);
        options.CashDonationCount = Math.Clamp(options.CashDonationCount <= 0 ? options.BeneficiaryCount / 5 : options.CashDonationCount, 100, 10000);
        options.InKindDonationCount = Math.Clamp(options.InKindDonationCount <= 0 ? options.BeneficiaryCount / 35 : options.InKindDonationCount, 20, 3000);

        var result = new CharityLargeDemoSeedResult();
        var previousAutoDetect = db.ChangeTracker.AutoDetectChangesEnabled;
        db.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            if (options.ResetExisting)
            {
                await ResetDemoDataAsync(db, cancellationToken);
            }
            else if (await db.Set<Beneficiary>().AsNoTracking().AnyAsync(x => x.Code.StartsWith(BeneficiaryCodePrefix), cancellationToken))
            {
                result.Skipped = true;
                return result;
            }

            await EnsureCoreLookupsAsync(db, cancellationToken);
            var master = await EnsureMasterDataAsync(db, cancellationToken);

            var donors = await SeedDonorsAsync(db, options.DonorCount, master, cancellationToken);
            result.Donations += await SeedCashDonationsAsync(db, options.CashDonationCount, donors, master, cancellationToken);
            result.Donations += await SeedInKindDonationPoolAsync(db, options.InKindDonationCount, donors, master, cancellationToken);

            var cashDonationStates = await LoadCashDonationStatesAsync(db, cancellationToken);
            var inKindStates = await LoadInKindDonationStatesAsync(db, cancellationToken);

            var approvedBeneficiaryIds = new List<Guid>(options.BeneficiaryCount / 2);
            var approvedCommitteeDecisionIds = new Dictionary<Guid, Guid>();
            var createdDisbursements = new List<BeneficiaryAidDisbursement>();

            for (var start = 1; start <= options.BeneficiaryCount; start += options.BatchSize)
            {
                var end = Math.Min(options.BeneficiaryCount, start + options.BatchSize - 1);
                var batch = BuildBeneficiaryBatch(start, end, master, cashDonationStates, inKindStates, approvedBeneficiaryIds, approvedCommitteeDecisionIds, createdDisbursements, result);

                await db.Set<Beneficiary>().AddRangeAsync(batch.Beneficiaries, cancellationToken);
                await db.Set<BeneficiaryFamilyMember>().AddRangeAsync(batch.FamilyMembers, cancellationToken);
                await db.Set<BeneficiaryHumanitarianResearch>().AddRangeAsync(batch.ResearchForms, cancellationToken);
                await db.Set<BeneficiaryHumanitarianResearchFamilyMember>().AddRangeAsync(batch.ResearchFamilyMembers, cancellationToken);
                await db.Set<BeneficiaryHumanitarianResearchIncomeItem>().AddRangeAsync(batch.ResearchIncomeItems, cancellationToken);
                await db.Set<BeneficiaryHumanitarianResearchExpenseItem>().AddRangeAsync(batch.ResearchExpenseItems, cancellationToken);
                await db.Set<BeneficiaryHumanitarianResearchReview>().AddRangeAsync(batch.ResearchReviews, cancellationToken);
                await db.Set<BeneficiaryCommitteeDecision>().AddRangeAsync(batch.CommitteeDecisions, cancellationToken);
                await db.Set<BeneficiaryAidRequest>().AddRangeAsync(batch.AidRequests, cancellationToken);
                await db.Set<DonationAllocation>().AddRangeAsync(batch.DonationAllocations, cancellationToken);
                await db.Set<BeneficiaryAidDisbursement>().AddRangeAsync(batch.Disbursements, cancellationToken);
                await db.Set<BeneficiaryAidDisbursementFundingLine>().AddRangeAsync(batch.FundingLines, cancellationToken);
                await db.Set<CharityStoreIssue>().AddRangeAsync(batch.StoreIssues, cancellationToken);
                await db.Set<CharityStoreIssueLine>().AddRangeAsync(batch.StoreIssueLines, cancellationToken);

                await db.SaveChangesAsync(cancellationToken);
                db.ChangeTracker.Clear();
            }

            await SeedProjectBeneficiariesAsync(db, master.Projects, approvedBeneficiaryIds, cancellationToken);
            await SeedAidCyclesAsync(db, approvedBeneficiaryIds, approvedCommitteeDecisionIds, master, cancellationToken);
            result.JournalEntries = await SeedJournalSamplesAsync(db, master, createdDisbursements, cancellationToken);

            return result;
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect;
        }
    }

    private sealed class MasterData
    {
        public Guid GovernorateId { get; init; }
        public Guid[] CityIds { get; init; } = Array.Empty<Guid>();
        public Guid[] AreaIds { get; init; } = Array.Empty<Guid>();
        public Guid CashPaymentMethodId { get; init; }
        public Guid BankPaymentMethodId { get; init; }
        public Guid CashAccountId { get; init; }
        public Guid BankAccountId { get; init; }
        public Guid GeneralDonationsAccountId { get; init; }
        public Guid RestrictedDonationsAccountId { get; init; }
        public Guid AidExpenseAccountId { get; init; }
        public Guid InventoryAccountId { get; init; }
        public Guid WarehouseId { get; init; }
        public Guid UnitPieceId { get; init; }
        public Guid UnitKgId { get; init; }
        public Guid ItemGroupFoodId { get; init; }
        public Guid ItemGroupClothesId { get; init; }
        public Guid[] ItemIds { get; init; } = Array.Empty<Guid>();
        public Guid[] Projects { get; init; } = Array.Empty<Guid>();
        public Guid FiscalPeriodId { get; init; }
        public Guid[] DepartmentIds { get; init; } = Array.Empty<Guid>();
        public Guid[] JobTitleIds { get; init; } = Array.Empty<Guid>();
    }

    private sealed class DonationState
    {
        public Guid DonationId { get; init; }
        public decimal RemainingAmount { get; set; }
        public Guid? AidTypeId { get; init; }
        public string Scope { get; init; } = "GeneralFund";
    }

    private sealed class InKindState
    {
        public Guid DonationId { get; init; }
        public Guid DonationInKindItemId { get; init; }
        public Guid ItemId { get; init; }
        public Guid WarehouseId { get; init; }
        public decimal UnitCost { get; init; }
        public decimal RemainingQuantity { get; set; }
        public Guid AidTypeId { get; init; }
    }

    private sealed class BatchData
    {
        public List<Beneficiary> Beneficiaries { get; } = new();
        public List<BeneficiaryFamilyMember> FamilyMembers { get; } = new();
        public List<BeneficiaryHumanitarianResearch> ResearchForms { get; } = new();
        public List<BeneficiaryHumanitarianResearchFamilyMember> ResearchFamilyMembers { get; } = new();
        public List<BeneficiaryHumanitarianResearchIncomeItem> ResearchIncomeItems { get; } = new();
        public List<BeneficiaryHumanitarianResearchExpenseItem> ResearchExpenseItems { get; } = new();
        public List<BeneficiaryHumanitarianResearchReview> ResearchReviews { get; } = new();
        public List<BeneficiaryCommitteeDecision> CommitteeDecisions { get; } = new();
        public List<BeneficiaryAidRequest> AidRequests { get; } = new();
        public List<DonationAllocation> DonationAllocations { get; } = new();
        public List<BeneficiaryAidDisbursement> Disbursements { get; } = new();
        public List<BeneficiaryAidDisbursementFundingLine> FundingLines { get; } = new();
        public List<CharityStoreIssue> StoreIssues { get; } = new();
        public List<CharityStoreIssueLine> StoreIssueLines { get; } = new();
    }

    private static async Task EnsureCoreLookupsAsync(AppDbContext db, CancellationToken ct)
    {
        await AddIfMissingAsync(db, new GenderLookup { Id = CharityLookupSeedIds.GenderMale, NameAr = "ذكر", NameEn = "Male", DisplayOrder = 1, IsActive = true }, ct);
        await AddIfMissingAsync(db, new GenderLookup { Id = CharityLookupSeedIds.GenderFemale, NameAr = "أنثى", NameEn = "Female", DisplayOrder = 2, IsActive = true }, ct);

        await AddIfMissingAsync(db, new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalSingle, NameAr = "أعزب", NameEn = "Single", DisplayOrder = 1, IsActive = true }, ct);
        await AddIfMissingAsync(db, new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalMarried, NameAr = "متزوج", NameEn = "Married", DisplayOrder = 2, IsActive = true }, ct);
        await AddIfMissingAsync(db, new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalWidowed, NameAr = "أرمل", NameEn = "Widowed", DisplayOrder = 3, IsActive = true }, ct);
        await AddIfMissingAsync(db, new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalDivorced, NameAr = "مطلق", NameEn = "Divorced", DisplayOrder = 4, IsActive = true }, ct);

        await AddIfMissingAsync(db, new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusNew, NameAr = "جديد", NameEn = "New", DisplayOrder = 1, IsActive = true }, ct);
        await AddIfMissingAsync(db, new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusUnderReview, NameAr = "تحت الدراسة", NameEn = "Under Review", DisplayOrder = 2, IsActive = true }, ct);
        await AddIfMissingAsync(db, new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusApproved, NameAr = "معتمد", NameEn = "Approved", DisplayOrder = 3, IsActive = true }, ct);
        await AddIfMissingAsync(db, new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusSuspended, NameAr = "موقوف", NameEn = "Suspended", DisplayOrder = 4, IsActive = true }, ct);
        await AddIfMissingAsync(db, new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusRejected, NameAr = "مرفوض", NameEn = "Rejected", DisplayOrder = 5, IsActive = true }, ct);

        var aidTypes = new[]
        {
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeCash, NameAr = "مساعدة مالية", NameEn = "Cash Aid", DisplayOrder = 1, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeFood, NameAr = "مساعدة غذائية", NameEn = "Food Aid", DisplayOrder = 2, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeMedical, NameAr = "مساعدة علاجية", NameEn = "Medical Aid", DisplayOrder = 3, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeEducational, NameAr = "مساعدة تعليمية", NameEn = "Educational Aid", DisplayOrder = 4, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeClothes, NameAr = "ملابس", NameEn = "Clothes", DisplayOrder = 5, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeDevices, NameAr = "أجهزة ومستلزمات", NameEn = "Devices", DisplayOrder = 6, IsActive = true },
            new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeSponsorship, NameAr = "كفالة", NameEn = "Sponsorship", DisplayOrder = 7, IsActive = true }
        };

        foreach (var aidType in aidTypes)
        {
            await AddIfMissingAsync(db, aidType, ct);
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task<MasterData> EnsureMasterDataAsync(AppDbContext db, CancellationToken ct)
    {
        var governorateId = DGuid("governorate-sohag");
        await AddIfMissingAsync(db, new Governorate { Id = governorateId, NameAr = "سوهاج", NameEn = "Sohag", IsActive = true }, ct);

        var cityIds = new List<Guid>();
        var areaIds = new List<Guid>();
        for (var i = 0; i < Areas.Length; i++)
        {
            var cityId = DGuid($"city-{i + 1}");
            cityIds.Add(cityId);
            await AddIfMissingAsync(db, new City { Id = cityId, GovernorateId = governorateId, NameAr = Areas[i], NameEn = Areas[i], IsActive = true }, ct);

            for (var j = 1; j <= 3; j++)
            {
                var areaId = DGuid($"area-{i + 1}-{j}");
                areaIds.Add(areaId);
                await AddIfMissingAsync(db, new Area { Id = areaId, CityId = cityId, NameAr = $"{Areas[i]} - منطقة {j}", NameEn = $"{Areas[i]} Area {j}", IsActive = true }, ct);
            }
        }

        var cashPaymentId = await EnsurePaymentMethodAsync(db, "DEMO_CASH", "نقدي - تجريبي", true, true, ct);
        var bankPaymentId = await EnsurePaymentMethodAsync(db, "DEMO_BANK", "تحويل بنكي - تجريبي", false, false, ct);

        var assetsId = await EnsureAccountAsync(db, "D-1", "أصول تجريبية", AccountCategory.Asset, 1, false, null, ct);
        var revenuesId = await EnsureAccountAsync(db, "D-4", "إيرادات تجريبية", AccountCategory.Revenue, 1, false, null, ct);
        var expensesId = await EnsureAccountAsync(db, "D-5", "مصروفات تجريبية", AccountCategory.Expense, 1, false, null, ct);
        var cashAccountId = await EnsureAccountAsync(db, "D-101", "خزنة التبرعات التجريبية", AccountCategory.Asset, 2, true, assetsId, ct);
        var bankAccountId = await EnsureAccountAsync(db, "D-102", "بنك التبرعات التجريبي", AccountCategory.Asset, 2, true, assetsId, ct);
        var inventoryAccountId = await EnsureAccountAsync(db, "D-114", "مخزون التبرعات العينية التجريبي", AccountCategory.Asset, 2, true, assetsId, ct);
        var generalDonationAccountId = await EnsureAccountAsync(db, "D-41301", "إيراد تبرعات عامة تجريبي", AccountCategory.Revenue, 2, true, revenuesId, ct);
        var restrictedDonationAccountId = await EnsureAccountAsync(db, "D-41302", "إيراد تبرعات مقيدة تجريبي", AccountCategory.Revenue, 2, true, revenuesId, ct);
        var aidExpenseAccountId = await EnsureAccountAsync(db, "D-51501", "مصروف مساعدات تجريبي", AccountCategory.Expense, 2, true, expensesId, ct);

        var unitPieceId = await EnsureUnitAsync(db, "DEMO_PCS", "قطعة", "pcs", ct);
        var unitKgId = await EnsureUnitAsync(db, "DEMO_KG", "كيلو", "kg", ct);
        var foodGroupId = await EnsureItemGroupAsync(db, "DEMO_FOOD", "تبرعات غذائية", ct);
        var clothesGroupId = await EnsureItemGroupAsync(db, "DEMO_CLOTHES", "ملابس وبطاطين", ct);
        var warehouseId = await EnsureWarehouseAsync(db, "DEMO_MAIN", "مخزن التبرعات الرئيسي", ct);

        var itemIds = new List<Guid>
        {
            await EnsureItemAsync(db, "DEMO-RICE", "أرز", foodGroupId, unitKgId, 32, 0, ct),
            await EnsureItemAsync(db, "DEMO-SUGAR", "سكر", foodGroupId, unitKgId, 35, 0, ct),
            await EnsureItemAsync(db, "DEMO-OIL", "زيت", foodGroupId, unitPieceId, 85, 0, ct),
            await EnsureItemAsync(db, "DEMO-BAG", "شنطة رمضان", foodGroupId, unitPieceId, 350, 0, ct),
            await EnsureItemAsync(db, "DEMO-BLANKET", "بطانية", clothesGroupId, unitPieceId, 250, 0, ct),
            await EnsureItemAsync(db, "DEMO-CLOTHES", "طقم ملابس", clothesGroupId, unitPieceId, 300, 0, ct)
        };

        var funders = await EnsureFundersAndGrantsAsync(db, ct);
        var projects = await EnsureProjectsAsync(db, funders, ct);
        var (departments, jobTitles) = await EnsureHrAsync(db, ct);
        var fiscalPeriodId = await EnsureFiscalPeriodAsync(db, ct);

        await db.SaveChangesAsync(ct);

        return new MasterData
        {
            GovernorateId = governorateId,
            CityIds = cityIds.ToArray(),
            AreaIds = areaIds.ToArray(),
            CashPaymentMethodId = cashPaymentId,
            BankPaymentMethodId = bankPaymentId,
            CashAccountId = cashAccountId,
            BankAccountId = bankAccountId,
            InventoryAccountId = inventoryAccountId,
            GeneralDonationsAccountId = generalDonationAccountId,
            RestrictedDonationsAccountId = restrictedDonationAccountId,
            AidExpenseAccountId = aidExpenseAccountId,
            UnitPieceId = unitPieceId,
            UnitKgId = unitKgId,
            ItemGroupFoodId = foodGroupId,
            ItemGroupClothesId = clothesGroupId,
            WarehouseId = warehouseId,
            ItemIds = itemIds.ToArray(),
            Projects = projects,
            FiscalPeriodId = fiscalPeriodId,
            DepartmentIds = departments,
            JobTitleIds = jobTitles
        };
    }

    private static async Task<List<Donor>> SeedDonorsAsync(AppDbContext db, int count, MasterData master, CancellationToken ct)
    {
        if (await db.Set<Donor>().AsNoTracking().AnyAsync(x => x.Code.StartsWith(DonorCodePrefix), ct))
        {
            return await db.Set<Donor>().AsNoTracking().Where(x => x.Code.StartsWith(DonorCodePrefix)).OrderBy(x => x.Code).ToListAsync(ct);
        }

        var donors = new List<Donor>(count);
        for (var i = 1; i <= count; i++)
        {
            var isOrg = i % 7 == 0;
            var name = isOrg ? $"مؤسسة الخير التنموية {i:000}" : $"فاعل خير {i:0000}";
            donors.Add(new Donor
            {
                Id = DGuid($"donor-{i}"),
                Code = $"{DonorCodePrefix}{i:0000}",
                DonorType = isOrg ? "مؤسسة" : "فرد",
                FullName = name,
                ContactPerson = isOrg ? $"مسؤول التبرعات {i:000}" : null,
                PhoneNumber = DemoPhone(i, 30),
                Email = isOrg ? $"donor{i:000}@demo-charity.local" : null,
                AddressLine = $"عنوان متبرع تجريبي رقم {i}",
                GovernorateId = master.GovernorateId,
                CityId = master.CityIds[i % master.CityIds.Length],
                AreaId = master.AreaIds[i % master.AreaIds.Length],
                PreferredCommunicationMethod = i % 2 == 0 ? "Phone" : "WhatsApp",
                Notes = "بيانات تجريبية مولدة للاختبار.",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-100 - (i % 300))
            });
        }

        await db.Set<Donor>().AddRangeAsync(donors, ct);
        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
        return donors;
    }

    private static async Task<int> SeedCashDonationsAsync(AppDbContext db, int count, List<Donor> donors, MasterData master, CancellationToken ct)
    {
        if (await db.Set<Donation>().AsNoTracking().AnyAsync(x => x.DonationNumber.StartsWith(DonationNumberPrefix), ct))
        {
            return 0;
        }

        var donations = new List<Donation>(count);
        for (var i = 1; i <= count; i++)
        {
            var scope = i % 12 == 0 ? "GeneralPurpose" : i % 9 == 0 ? "SpecificRequests" : "GeneralFund";
            var purpose = scope == "GeneralPurpose" ? (i % 24 == 0 ? "زكاة الفطر" : "شنطة رمضان") : null;
            var isRestrictedAid = scope == "SpecificRequests";
            donations.Add(new Donation
            {
                Id = DGuid($"cash-donation-{i}"),
                DonationNumber = $"{DonationNumberPrefix}{i:000000}",
                DonorId = donors[(i - 1) % donors.Count].Id,
                DonationDate = DateTime.Today.AddDays(-(i % 360)),
                DonationType = "نقدي",
                AidTypeId = isRestrictedAid ? PickAidType(i) : null,
                Amount = 15000m + ((i % 15) * 2500m),
                PaymentMethodId = i % 4 == 0 ? master.BankPaymentMethodId : master.CashPaymentMethodId,
                FinancialAccountId = i % 4 == 0 ? master.BankAccountId : master.CashAccountId,
                IsRestricted = scope != "GeneralFund",
                CampaignName = purpose,
                TargetingScopeCode = scope,
                GeneralPurposeName = purpose,
                ReceiptNumber = $"REC-D-{i:000000}",
                ReferenceNumber = i % 4 == 0 ? $"BNK-D-{i:000000}" : null,
                Notes = "تبرع نقدي تجريبي مرتبط بدورة التخصيص والصرف.",
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 360))
            });
        }

        await db.Set<Donation>().AddRangeAsync(donations, ct);
        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
        return donations.Count;
    }

    private static async Task<int> SeedInKindDonationPoolAsync(AppDbContext db, int count, List<Donor> donors, MasterData master, CancellationToken ct)
    {
        if (await db.Set<Donation>().AsNoTracking().AnyAsync(x => x.DonationNumber.StartsWith(InKindDonationNumberPrefix), ct))
        {
            return 0;
        }

        var donations = new List<Donation>(count);
        var inKindItems = new List<DonationInKindItem>(count);
        var receipts = new List<CharityStoreReceipt>(count);
        var receiptLines = new List<CharityStoreReceiptLine>(count);
        var balances = new Dictionary<Guid, decimal>();

        for (var i = 1; i <= count; i++)
        {
            var itemId = master.ItemIds[i % master.ItemIds.Length];
            var aidTypeId = i % 3 == 0 ? CharityLookupSeedIds.AidTypeClothes : CharityLookupSeedIds.AidTypeFood;
            var quantity = 120m + (i % 12) * 15m;
            var unitValue = EstimatedUnitValue(itemId, master);
            var donationId = DGuid($"inkind-donation-{i}");
            var donationItemId = DGuid($"inkind-donation-item-{i}");
            var receiptId = DGuid($"store-receipt-{i}");

            donations.Add(new Donation
            {
                Id = donationId,
                DonationNumber = $"{InKindDonationNumberPrefix}{i:000000}",
                DonorId = donors[(i * 3 - 1) % donors.Count].Id,
                DonationDate = DateTime.Today.AddDays(-(i % 240)),
                DonationType = "عيني",
                AidTypeId = aidTypeId,
                Amount = null,
                PaymentMethodId = null,
                FinancialAccountId = null,
                IsRestricted = i % 5 == 0,
                CampaignName = i % 5 == 0 ? "شنطة رمضان" : null,
                TargetingScopeCode = i % 5 == 0 ? "GeneralPurpose" : "GeneralFund",
                GeneralPurposeName = i % 5 == 0 ? "شنطة رمضان" : null,
                ReceiptNumber = $"IK-REC-D-{i:000000}",
                Notes = "تبرع عيني تجريبي مرتبط بالمخزن.",
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 240))
            });

            inKindItems.Add(new DonationInKindItem
            {
                Id = donationItemId,
                DonationId = donationId,
                ItemId = itemId,
                Quantity = quantity,
                EstimatedUnitValue = unitValue,
                EstimatedTotalValue = unitValue * quantity,
                WarehouseId = master.WarehouseId,
                BatchNo = $"BATCH-D-{i:0000}",
                ExpiryDate = aidTypeId == CharityLookupSeedIds.AidTypeFood ? DateTime.Today.AddMonths(6 + (i % 12)) : null,
                Notes = "إضافة مخزنية تلقائية من التبرع العيني التجريبي.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 240))
            });

            receipts.Add(new CharityStoreReceipt
            {
                Id = receiptId,
                ReceiptNumber = $"{ReceiptPrefix}{i:000000}",
                WarehouseId = master.WarehouseId,
                ReceiptDate = DateTime.Today.AddDays(-(i % 240)),
                SourceType = "DonationInKind",
                SourceName = $"تبرع عيني {InKindDonationNumberPrefix}{i:000000}",
                Notes = "إذن إضافة مخزنية تجريبي للتبرع العيني.",
                ApprovalStatus = "Approved",
                ApprovalNotes = "بيانات تجريبية معتمدة.",
                ApprovedAtUtc = DateTime.UtcNow.AddDays(-(i % 240)),
                ApprovedByUserId = SeedUserId,
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 240))
            });

            receiptLines.Add(new CharityStoreReceiptLine
            {
                Id = DGuid($"store-receipt-line-{i}"),
                ReceiptId = receiptId,
                ItemId = itemId,
                Quantity = quantity,
                UnitCost = unitValue,
                ExpiryDate = aidTypeId == CharityLookupSeedIds.AidTypeFood ? DateTime.Today.AddMonths(6 + (i % 12)) : null,
                BatchNo = $"BATCH-D-{i:0000}",
                Notes = "رصيد افتتاحي تجريبي من التبرعات العينية."
            });

            balances[itemId] = balances.TryGetValue(itemId, out var current) ? current + quantity : quantity;
        }

        await db.Set<Donation>().AddRangeAsync(donations, ct);
        await db.Set<DonationInKindItem>().AddRangeAsync(inKindItems, ct);
        await db.Set<CharityStoreReceipt>().AddRangeAsync(receipts, ct);
        await db.Set<CharityStoreReceiptLine>().AddRangeAsync(receiptLines, ct);

        foreach (var balance in balances)
        {
            var balanceId = DGuid($"balance-{master.WarehouseId}-{balance.Key}");
            if (!await db.Set<ItemWarehouseBalance>().AnyAsync(x => x.WarehouseId == master.WarehouseId && x.ItemId == balance.Key, ct))
            {
                await db.Set<ItemWarehouseBalance>().AddAsync(new ItemWarehouseBalance
                {
                    Id = balanceId,
                    WarehouseId = master.WarehouseId,
                    ItemId = balance.Key,
                    QuantityOnHand = balance.Value,
                    AvailableQuantity = balance.Value,
                    ReservedQuantity = 0,
                    LastUpdatedUtc = DateTime.UtcNow
                }, ct);
            }
        }

        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
        return donations.Count;
    }

    private static BatchData BuildBeneficiaryBatch(
        int start,
        int end,
        MasterData master,
        List<DonationState> cashDonationStates,
        List<InKindState> inKindStates,
        List<Guid> approvedBeneficiaryIds,
        Dictionary<Guid, Guid> approvedCommitteeDecisionIds,
        List<BeneficiaryAidDisbursement> createdDisbursements,
        CharityLargeDemoSeedResult result)
    {
        var batch = new BatchData();
        for (var i = start; i <= end; i++)
        {
            var beneficiaryId = DGuid($"beneficiary-{i}");
            var isFemale = i % 3 == 0;
            var name = BuildPersonName(i, isFemale);
            var familyCount = 2 + (i % 6);
            decimal monthlyIncome = (i % 5 == 0) ? 0 : 700 + ((i % 14) * 150);
            var birthDate = DateTime.Today.AddYears(-(22 + (i % 55))).AddDays(-(i % 365));
            var cityId = master.CityIds[i % master.CityIds.Length];
            var areaId = master.AreaIds[i % master.AreaIds.Length];
            var statusId = PickBeneficiaryStatus(i);
            var hasResearch = i % 100 < 78;
            var isApproved = statusId == CharityLookupSeedIds.BeneficiaryStatusApproved;
            var aidTypeId = PickAidType(i);
            var requestedAmount = PickRequestedAmount(aidTypeId, i);

            var beneficiary = new Beneficiary
            {
                Id = beneficiaryId,
                Code = $"{BeneficiaryCodePrefix}{i:000000}",
                FullName = name,
                NationalId = DemoNationalId(i, isFemale),
                BirthDate = birthDate,
                GenderId = isFemale ? CharityLookupSeedIds.GenderFemale : CharityLookupSeedIds.GenderMale,
                MaritalStatusId = PickMaritalStatus(i),
                PhoneNumber = DemoPhone(i, 10),
                AlternatePhoneNumber = i % 4 == 0 ? DemoPhone(i, 11) : null,
                AddressLine = $"منزل رقم {20 + i % 500} - شارع {1 + i % 40} - {Areas[i % Areas.Length]}",
                GovernorateId = master.GovernorateId,
                CityId = cityId,
                AreaId = areaId,
                FamilyMembersCount = familyCount,
                MonthlyIncome = monthlyIncome,
                IncomeSource = monthlyIncome > 0 ? (i % 2 == 0 ? "عمل يومي" : "معاش") : "لا يوجد",
                HealthStatus = i % 9 == 0 ? "حالة مرضية مزمنة" : "مستقر",
                EducationStatus = i % 4 == 0 ? "متوسط" : "أمي/بسيط",
                WorkStatus = monthlyIncome > 0 ? "عمل غير منتظم" : "لا يعمل",
                HousingStatus = i % 5 == 0 ? "إيجار" : "منزل أسرة",
                Notes = "مستفيد تجريبي مولد لاختبار دورة الجمعية.",
                StatusId = statusId,
                RegistrationDate = DateTime.Today.AddDays(-(i % 500)),
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 500)),
                IsActive = statusId != CharityLookupSeedIds.BeneficiaryStatusSuspended,
                Location = Areas[i % Areas.Length]
            };
            batch.Beneficiaries.Add(beneficiary);
            result.Beneficiaries++;

            for (var f = 1; f <= familyCount; f++)
            {
                var memberFemale = (i + f) % 2 == 0;
                batch.FamilyMembers.Add(new BeneficiaryFamilyMember
                {
                    Id = DGuid($"beneficiary-{i}-family-{f}"),
                    BeneficiaryId = beneficiaryId,
                    FullName = BuildFamilyMemberName(i, f, memberFemale),
                    Relationship = f == 1 ? (isFemale ? "زوج" : "زوجة") : f <= 3 ? "ابن/ابنة" : "تابع",
                    NationalId = DemoNationalId(i * 10 + f, memberFemale),
                    BirthDate = DateTime.Today.AddYears(-(5 + ((i + f) % 40))).AddDays(-f * 19),
                    GenderId = memberFemale ? CharityLookupSeedIds.GenderFemale : CharityLookupSeedIds.GenderMale,
                    EducationStatus = f <= 3 ? "طالب" : "غير محدد",
                    WorkStatus = f == 1 && i % 2 == 0 ? "عمل موسمي" : "لا يعمل",
                    MonthlyIncome = f == 1 && i % 2 == 0 ? 500 + (i % 7) * 100 : 0,
                    HealthCondition = (i + f) % 11 == 0 ? "يحتاج متابعة" : "جيد",
                    IsDependent = f != 1 || i % 2 != 0,
                    Notes = "فرد أسرة تجريبي."
                });
                result.FamilyMembers++;
            }

            Guid? committeeDecisionId = null;
            if (hasResearch)
            {
                var researchId = DGuid($"research-{i}");
                var status = isApproved ? "SentToCommittee" : statusId == CharityLookupSeedIds.BeneficiaryStatusRejected ? "ReviewedRejected" : "Submitted";
                var totalExpenses = 1400 + ((i % 10) * 210);
                batch.ResearchForms.Add(new BeneficiaryHumanitarianResearch
                {
                    Id = researchId,
                    BeneficiaryId = beneficiaryId,
                    ResearchNumber = $"{ResearchPrefix}{i:000000}",
                    RequestDate = DateTime.Today.AddDays(-(i % 420)),
                    ResearchDate = DateTime.Today.AddDays(-(i % 390)),
                    AidTypeName = AidTypeName(aidTypeId),
                    ApplicantName = name,
                    SourceOfRequest = i % 3 == 0 ? "مكتب خدمة المستفيدين" : "طلب مباشر",
                    ResearcherCode = $"RSR-{(i % 25) + 1:000}",
                    ResearcherName = $"باحث اجتماعي {(i % 25) + 1:00}",
                    CommitteeCode = $"COM-{(i % 5) + 1:00}",
                    PriorityLevel = i % 8 == 0 ? "High" : i % 3 == 0 ? "Medium" : "Normal",
                    FullName = name,
                    Age = DateTime.Today.Year - birthDate.Year,
                    MaritalStatus = MaritalStatusName(PickMaritalStatus(i)),
                    NationalId = beneficiary.NationalId,
                    AddressLine = beneficiary.AddressLine,
                    Phone1 = beneficiary.PhoneNumber,
                    Phone2 = beneficiary.AlternatePhoneNumber,
                    FamilyMembersCount = familyCount,
                    TotalIncome = monthlyIncome,
                    TotalExpenses = totalExpenses,
                    AverageIncome = familyCount == 0 ? monthlyIncome : Math.Round(monthlyIncome / familyCount, 2),
                    HasExistingProject = i % 14 == 0,
                    ExistingProjectType = i % 14 == 0 ? "مشروع صغير" : null,
                    ExistingProjectSize = i % 14 == 0 ? "متناهي الصغر" : null,
                    RequiredNeedsPrimary = AidTypeName(aidTypeId),
                    RequiredNeedsSecondary = i % 4 == 0 ? "دعم غذائي" : null,
                    HousingDescription = beneficiary.HousingStatus,
                    ResearcherReport = "تقرير بحث اجتماعي تجريبي يوضح احتياج الحالة وفق البيانات المدخلة.",
                    ResearchManagerOpinion = isApproved ? "يوصى بالعرض على اللجنة." : "تحت الاستكمال.",
                    Status = status,
                    SubmittedAtUtc = DateTime.UtcNow.AddDays(-(i % 380)),
                    SubmittedByUserId = SeedUserId,
                    ReviewedAtUtc = isApproved || statusId == CharityLookupSeedIds.BeneficiaryStatusRejected ? DateTime.UtcNow.AddDays(-(i % 360)) : null,
                    ReviewedByUserId = isApproved || statusId == CharityLookupSeedIds.BeneficiaryStatusRejected ? SeedUserId : null,
                    ReviewDecision = isApproved ? "Approve" : statusId == CharityLookupSeedIds.BeneficiaryStatusRejected ? "Reject" : null,
                    ReviewReason = isApproved ? "مراجعة تلقائية تجريبية" : statusId == CharityLookupSeedIds.BeneficiaryStatusRejected ? "بيانات غير مكتملة" : null,
                    SentToCommitteeAtUtc = isApproved ? DateTime.UtcNow.AddDays(-(i % 330)) : null,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 420)),
                });
                result.ResearchForms++;

                batch.ResearchIncomeItems.Add(new BeneficiaryHumanitarianResearchIncomeItem { Id = DGuid($"research-{i}-income-1"), ResearchId = researchId, IncomeType = monthlyIncome > 0 ? "دخل أساسي" : "لا يوجد", Amount = monthlyIncome, Notes = "دخل مسجل بالبحث" });
                if (i % 4 == 0)
                {
                    batch.ResearchIncomeItems.Add(new BeneficiaryHumanitarianResearchIncomeItem { Id = DGuid($"research-{i}-income-2"), ResearchId = researchId, IncomeType = "مساعدة غير منتظمة", Amount = 250 + (i % 5) * 100, Notes = "دخل إضافي متقطع" });
                }

                batch.ResearchExpenseItems.Add(new BeneficiaryHumanitarianResearchExpenseItem { Id = DGuid($"research-{i}-expense-1"), ResearchId = researchId, ExpenseType = "معيشة", Amount = 900 + (i % 8) * 100, Notes = "مصروفات أساسية" });
                batch.ResearchExpenseItems.Add(new BeneficiaryHumanitarianResearchExpenseItem { Id = DGuid($"research-{i}-expense-2"), ResearchId = researchId, ExpenseType = i % 3 == 0 ? "علاج" : "تعليم", Amount = 350 + (i % 9) * 80, Notes = "مصروفات فرعية" });

                for (var rf = 1; rf <= Math.Min(3, familyCount); rf++)
                {
                    batch.ResearchFamilyMembers.Add(new BeneficiaryHumanitarianResearchFamilyMember
                    {
                        Id = DGuid($"research-{i}-family-{rf}"),
                        ResearchId = researchId,
                        FullName = BuildFamilyMemberName(i, rf, (i + rf) % 2 == 0),
                        Relationship = rf == 1 ? "زوج/زوجة" : "ابن/ابنة",
                        Age = 7 + ((i + rf) % 34),
                        EducationLevel = rf == 1 ? "متوسط" : "طالب",
                        WorkType = rf == 1 && i % 2 == 0 ? "يومي" : "لا يعمل",
                        Income = rf == 1 && i % 2 == 0 ? 400 + (i % 4) * 100 : 0,
                        MaritalStatus = rf == 1 ? "متزوج" : "أعزب",
                        NationalId = DemoNationalId(i * 100 + rf, (i + rf) % 2 == 0)
                    });
                }

                if (isApproved || statusId == CharityLookupSeedIds.BeneficiaryStatusRejected)
                {
                    batch.ResearchReviews.Add(new BeneficiaryHumanitarianResearchReview
                    {
                        Id = DGuid($"research-{i}-review-1"),
                        ResearchId = researchId,
                        ReviewerUserId = SeedUserId,
                        ReviewDateUtc = DateTime.UtcNow.AddDays(-(i % 360)),
                        Decision = isApproved ? "Approve" : "Reject",
                        Reason = isApproved ? "تمت المراجعة واعتماد الإحالة للجنة." : "بيانات الحالة غير مكتملة.",
                        Notes = "مراجعة تجريبية مولدة آليًا.",
                        ReviewDate = DateTime.Today.AddDays(-(i % 360)),
                        ReviewerNotes = isApproved ? "صالحة للعرض على اللجنة" : "مطلوب استكمال مستندات",
                        ReviewedByUserId = SeedUserId
                    });
                }
            }

            if (isApproved)
            {
                committeeDecisionId = DGuid($"committee-decision-{i}");
                batch.CommitteeDecisions.Add(new BeneficiaryCommitteeDecision
                {
                    Id = committeeDecisionId.Value,
                    BeneficiaryId = beneficiaryId,
                    DecisionDate = DateTime.Today.AddDays(-(i % 250)),
                    DecisionType = "ApproveAid",
                    ApprovedAidTypeId = aidTypeId,
                    ApprovedAmount = requestedAmount,
                    DurationInMonths = aidTypeId == CharityLookupSeedIds.AidTypeSponsorship ? 12 : null,
                    CommitteeNotes = "قرار لجنة تجريبي بالموافقة على تقديم المساعدة.",
                    ApprovedByUserId = SeedUserId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 250)),
                    ApprovedStatus = true
                });
                approvedBeneficiaryIds.Add(beneficiaryId);
                approvedCommitteeDecisionIds[beneficiaryId] = committeeDecisionId.Value;
            }

            if (isApproved || i % 100 < 65)
            {
                var requestId = DGuid($"aid-request-{i}");
                var requestStatus = isApproved ? "Approved" : i % 5 == 0 ? "Pending" : "Submitted";
                batch.AidRequests.Add(new BeneficiaryAidRequest
                {
                    Id = requestId,
                    BeneficiaryId = beneficiaryId,
                    RequestDate = DateTime.Today.AddDays(-(i % 180)),
                    AidTypeId = aidTypeId,
                    RequestedAmount = requestedAmount,
                    Reason = BuildAidReason(aidTypeId),
                    UrgencyLevel = i % 9 == 0 ? "High" : i % 4 == 0 ? "Medium" : "Normal",
                    Status = requestStatus,
                    ProjectId = master.Projects[i % master.Projects.Length],
                    CreatedByUserId = SeedUserId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 180))
                });
                result.AidRequests++;

                if (requestStatus == "Approved")
                {
                    if (aidTypeId == CharityLookupSeedIds.AidTypeFood || aidTypeId == CharityLookupSeedIds.AidTypeClothes)
                    {
                        TryAllocateInKind(i, beneficiaryId, requestId, aidTypeId, master, inKindStates, batch, result);
                    }
                    else
                    {
                        TryAllocateCash(i, beneficiaryId, requestId, aidTypeId, requestedAmount, master, cashDonationStates, batch, createdDisbursements, result);
                    }
                }
            }
        }

        return batch;
    }

    private static void TryAllocateCash(
        int i,
        Guid beneficiaryId,
        Guid requestId,
        Guid aidTypeId,
        decimal requestedAmount,
        MasterData master,
        List<DonationState> cashDonationStates,
        BatchData batch,
        List<BeneficiaryAidDisbursement> createdDisbursements,
        CharityLargeDemoSeedResult result)
    {
        if (i % 100 >= 82)
        {
            return;
        }

        var amount = Math.Round(requestedAmount * (i % 5 == 0 ? 0.5m : 1m), 2);
        var source = cashDonationStates.FirstOrDefault(x => x.RemainingAmount >= amount && (x.AidTypeId == null || x.AidTypeId == aidTypeId));
        if (source == null)
        {
            return;
        }

        var allocationId = DGuid($"cash-allocation-{i}");
        batch.DonationAllocations.Add(new DonationAllocation
        {
            Id = allocationId,
            DonationId = source.DonationId,
            AllocatedDate = DateTime.Today.AddDays(-(i % 120)),
            AidRequestId = requestId,
            BeneficiaryId = beneficiaryId,
            Amount = amount,
            ApprovalStatus = i % 6 == 0 ? "Pending" : "Approved",
            ApprovedAtUtc = i % 6 == 0 ? null : DateTime.UtcNow.AddDays(-(i % 90)),
            ApprovedByUserId = i % 6 == 0 ? null : SeedUserId,
            ApprovalNotes = "تخصيص نقدي تجريبي.",
            Notes = "تمويل من تبرع نقدي تجريبي.",
            CreatedByUserId = SeedUserId,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 120))
        });
        result.DonationAllocations++;
        source.RemainingAmount -= amount;

        var disbursementId = DGuid($"disbursement-{i}");
        var paid = i % 4 != 0;
        var approvalStatus = i % 6 == 0 ? "Pending" : "Approved";
        var executionStatus = approvalStatus == "Pending" ? "Available" : paid ? "FullyDisbursed" : "Available";
        var disbursement = new BeneficiaryAidDisbursement
        {
            Id = disbursementId,
            BeneficiaryId = beneficiaryId,
            AidRequestId = requestId,
            AidTypeId = aidTypeId,
            DisbursementDate = DateTime.Today.AddDays(-(i % 75)),
            Amount = amount,
            PaymentMethodId = master.CashPaymentMethodId,
            FinancialAccountId = master.CashAccountId,
            DonationId = source.DonationId,
            ApprovalStatus = approvalStatus,
            ApprovedAtUtc = approvalStatus == "Approved" ? DateTime.UtcNow.AddDays(-(i % 75)) : null,
            ApprovedByUserId = approvalStatus == "Approved" ? SeedUserId : null,
            ExecutionStatus = executionStatus,
            ExecutedAmount = executionStatus == "FullyDisbursed" ? amount : 0m,
            ExecutedAtUtc = executionStatus == "FullyDisbursed" ? DateTime.UtcNow.AddDays(-(i % 60)) : null,
            ExecutedByUserId = executionStatus == "FullyDisbursed" ? SeedUserId : null,
            SourceType = "DonationAllocation",
            SourceId = allocationId,
            Notes = approvalStatus == "Pending" ? "سجل صرف معلق ناتج من التخصيص." : "سجل صرف تجريبي مرتبط بتخصيص تبرع.",
            CreatedByUserId = SeedUserId,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 75))
        };
        batch.Disbursements.Add(disbursement);
        createdDisbursements.Add(disbursement);
        result.Disbursements++;

        batch.FundingLines.Add(new BeneficiaryAidDisbursementFundingLine
        {
            Id = DGuid($"funding-line-{i}"),
            DisbursementId = disbursementId,
            DonationAllocationId = allocationId,
            AmountConsumed = amount,
            CreatedByUserId = SeedUserId,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 75))
        });
    }

    private static void TryAllocateInKind(
        int i,
        Guid beneficiaryId,
        Guid requestId,
        Guid aidTypeId,
        MasterData master,
        List<InKindState> inKindStates,
        BatchData batch,
        CharityLargeDemoSeedResult result)
    {
        if (i % 100 >= 70)
        {
            return;
        }

        var qty = aidTypeId == CharityLookupSeedIds.AidTypeFood ? 1m + (i % 3) : 1m;
        var source = inKindStates.FirstOrDefault(x => x.AidTypeId == aidTypeId && x.RemainingQuantity >= qty);
        if (source == null)
        {
            return;
        }

        var allocationId = DGuid($"inkind-allocation-{i}");
        batch.DonationAllocations.Add(new DonationAllocation
        {
            Id = allocationId,
            DonationId = source.DonationId,
            AllocatedDate = DateTime.Today.AddDays(-(i % 120)),
            AidRequestId = requestId,
            BeneficiaryId = beneficiaryId,
            DonationInKindItemId = source.DonationInKindItemId,
            AllocatedQuantity = qty,
            ApprovalStatus = i % 5 == 0 ? "Pending" : "Approved",
            ApprovedAtUtc = i % 5 == 0 ? null : DateTime.UtcNow.AddDays(-(i % 90)),
            ApprovedByUserId = i % 5 == 0 ? null : SeedUserId,
            ApprovalNotes = "تخصيص عيني تجريبي.",
            Notes = "تمويل عيني من تبرع مخزني.",
            CreatedByUserId = SeedUserId,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 120))
        });
        result.DonationAllocations++;
        source.RemainingQuantity -= qty;

        if (i % 5 != 0)
        {
            var issueId = DGuid($"store-issue-{i}");
            batch.StoreIssues.Add(new CharityStoreIssue
            {
                Id = issueId,
                IssueNumber = $"{IssuePrefix}{i:000000}",
                WarehouseId = source.WarehouseId,
                IssueDate = DateTime.Today.AddDays(-(i % 90)),
                IssueType = "BeneficiaryAid",
                BeneficiaryId = beneficiaryId,
                IssuedToName = $"مستفيد {BeneficiaryCodePrefix}{i:000000}",
                Notes = "إذن صرف مخزني تجريبي من تخصيص تبرع عيني.",
                ApprovalStatus = i % 7 == 0 ? "Pending" : "Approved",
                ApprovalNotes = "إذن تجريبي مولد آليًا.",
                ApprovedAtUtc = i % 7 == 0 ? null : DateTime.UtcNow.AddDays(-(i % 70)),
                ApprovedByUserId = i % 7 == 0 ? null : SeedUserId,
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-(i % 90))
            });
            batch.StoreIssueLines.Add(new CharityStoreIssueLine
            {
                Id = DGuid($"store-issue-line-{i}"),
                IssueId = issueId,
                ItemId = source.ItemId,
                Quantity = qty,
                UnitCost = source.UnitCost,
                Notes = "سطر صرف مخزني تجريبي."
            });
            result.StoreIssues++;
        }
    }

    private static async Task<List<DonationState>> LoadCashDonationStatesAsync(AppDbContext db, CancellationToken ct)
    {
        return await db.Set<Donation>()
            .AsNoTracking()
            .Where(x => x.DonationNumber.StartsWith(DonationNumberPrefix) && x.DonationType == "نقدي")
            .OrderBy(x => x.DonationDate)
            .ThenBy(x => x.DonationNumber)
            .Select(x => new DonationState
            {
                DonationId = x.Id,
                RemainingAmount = x.Amount ?? 0m,
                AidTypeId = x.AidTypeId,
                Scope = x.TargetingScopeCode
            })
            .ToListAsync(ct);
    }

    private static async Task<List<InKindState>> LoadInKindDonationStatesAsync(AppDbContext db, CancellationToken ct)
    {
        var rows = await db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Donation)
            .Where(x => x.Donation != null && x.Donation.DonationNumber.StartsWith(InKindDonationNumberPrefix))
            .OrderBy(x => x.Donation!.DonationDate)
            .ThenBy(x => x.Donation!.DonationNumber)
            .Select(x => new InKindState
            {
                DonationId = x.DonationId,
                DonationInKindItemId = x.Id,
                ItemId = x.ItemId,
                WarehouseId = x.WarehouseId ?? Guid.Empty,
                UnitCost = x.EstimatedUnitValue ?? 0m,
                RemainingQuantity = x.Quantity,
                AidTypeId = x.Donation!.AidTypeId ?? CharityLookupSeedIds.AidTypeFood
            })
            .ToListAsync(ct);

        return rows.Where(x => x.WarehouseId != Guid.Empty).ToList();
    }

    private static async Task SeedProjectBeneficiariesAsync(AppDbContext db, Guid[] projectIds, List<Guid> approvedBeneficiaryIds, CancellationToken ct)
    {
        if (await db.Set<ProjectBeneficiary>().AsNoTracking().AnyAsync(x => x.Notes != null && x.Notes.Contains("DEMO-LARGE-SEED"), ct))
        {
            return;
        }

        var links = approvedBeneficiaryIds
            .Take(1200)
            .Select((beneficiaryId, index) => new ProjectBeneficiary
            {
                Id = DGuid($"project-beneficiary-{index + 1}"),
                ProjectId = projectIds[index % projectIds.Length],
                BeneficiaryId = beneficiaryId,
                EnrollmentDate = DateTime.Today.AddDays(-(index % 240)),
                BenefitType = index % 2 == 0 ? "مستفيد مباشر" : "مستفيد غير مباشر",
                Notes = "DEMO-LARGE-SEED - ربط مستفيد بمشروع تجريبي."
            })
            .ToList();

        await db.Set<ProjectBeneficiary>().AddRangeAsync(links, ct);
        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
    }

    private static async Task SeedAidCyclesAsync(AppDbContext db, List<Guid> approvedBeneficiaryIds, Dictionary<Guid, Guid> decisionIds, MasterData master, CancellationToken ct)
    {
        if (await db.Set<AidCycle>().AsNoTracking().AnyAsync(x => x.CycleNumber.StartsWith(AidCyclePrefix), ct))
        {
            return;
        }

        var cycles = new List<AidCycle>();
        var cycleBeneficiaries = new List<AidCycleBeneficiary>();
        var cycleDefs = new[]
        {
            (Type: "Monthly", AidType: CharityLookupSeedIds.AidTypeCash, Title: "دورة مساعدات مالية شهرية"),
            (Type: "Seasonal", AidType: CharityLookupSeedIds.AidTypeFood, Title: "دورة شنطة رمضان"),
            (Type: "Emergency", AidType: CharityLookupSeedIds.AidTypeMedical, Title: "دورة مساعدات علاجية عاجلة")
        };

        for (var c = 0; c < cycleDefs.Length; c++)
        {
            var cycleId = DGuid($"aid-cycle-{c + 1}");
            var members = approvedBeneficiaryIds.Skip(c * 250).Take(250).ToList();
            cycles.Add(new AidCycle
            {
                Id = cycleId,
                CycleNumber = $"{AidCyclePrefix}{c + 1:000}",
                Title = cycleDefs[c].Title,
                CycleType = cycleDefs[c].Type,
                AidTypeId = cycleDefs[c].AidType,
                PeriodYear = DateTime.Today.Year,
                PeriodMonth = DateTime.Today.Month,
                FromDate = DateTime.Today.AddDays(-15),
                ToDate = DateTime.Today.AddDays(15),
                PlannedDisbursementDate = DateTime.Today.AddDays(5 + c),
                Status = c == 0 ? "Approved" : "Generated",
                BeneficiariesCount = members.Count,
                TotalPlannedAmount = members.Count * (c == 1 ? 350m : 1000m),
                TotalDisbursedAmount = c == 0 ? members.Count * 1000m : 0m,
                Notes = "دورة مساعدات تجريبية مولدة للبيانات الكبيرة.",
                CreatedByUserId = SeedUserId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-10),
                ApprovedByUserId = c == 0 ? SeedUserId : null,
                ApprovedAtUtc = c == 0 ? DateTime.UtcNow.AddDays(-5) : null
            });

            foreach (var beneficiaryId in members.Select((value, index) => new { value, index }))
            {
                cycleBeneficiaries.Add(new AidCycleBeneficiary
                {
                    Id = DGuid($"aid-cycle-{c + 1}-beneficiary-{beneficiaryId.index + 1}"),
                    AidCycleId = cycleId,
                    BeneficiaryId = beneficiaryId.value,
                    CommitteeDecisionId = decisionIds.TryGetValue(beneficiaryId.value, out var decisionId) ? decisionId : null,
                    AidTypeId = cycleDefs[c].AidType,
                    ScheduledAmount = c == 1 ? 350m : 1000m,
                    ApprovedAmount = c == 0 ? 1000m : null,
                    DisbursedAmount = c == 0 ? 1000m : 0m,
                    Status = c == 0 ? "Disbursed" : "Eligible",
                    LastDisbursementDate = c == 0 ? DateTime.Today.AddDays(-2) : null,
                    NextDueDate = DateTime.Today.AddMonths(1),
                    Notes = "مستفيد داخل دورة مساعدات تجريبية.",
                    CreatedByUserId = SeedUserId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
                });
            }
        }

        await db.Set<AidCycle>().AddRangeAsync(cycles, ct);
        await db.Set<AidCycleBeneficiary>().AddRangeAsync(cycleBeneficiaries, ct);
        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
    }

    private static async Task<int> SeedJournalSamplesAsync(AppDbContext db, MasterData master, List<BeneficiaryAidDisbursement> disbursements, CancellationToken ct)
    {
        if (await db.Set<JournalEntry>().AsNoTracking().AnyAsync(x => x.EntryNumber.StartsWith(JournalPrefix), ct))
        {
            return 0;
        }

        var entries = new List<JournalEntry>();
        var lines = new List<JournalEntryLine>();
        var donationSamples = await db.Set<Donation>()
            .AsNoTracking()
            .Where(x => x.DonationNumber.StartsWith(DonationNumberPrefix) && x.Amount.HasValue)
            .OrderBy(x => x.DonationNumber)
            .Take(250)
            .ToListAsync(ct);

        var counter = 1;
        foreach (var donation in donationSamples)
        {
            var entryId = DGuid($"journal-donation-{counter}");
            var amount = donation.Amount ?? 0m;
            entries.Add(new JournalEntry
            {
                Id = entryId,
                EntryNumber = $"{JournalPrefix}{counter:000000}",
                EntryDate = donation.DonationDate,
                Description = $"قيد تحصيل تبرع نقدي تجريبي {donation.DonationNumber}",
                FiscalPeriodId = master.FiscalPeriodId,
                Status = JournalEntryStatus.Posted,
                TotalDebit = amount,
                TotalCredit = amount,
                SourceType = "Donation",
                SourceId = donation.Id,
                CreatedAtUtc = DateTime.UtcNow,
                PostedAtUtc = DateTime.UtcNow,
                PostedByUserId = SeedUserId
            });
            lines.Add(new JournalEntryLine { Id = DGuid($"journal-donation-{counter}-dr"), JournalEntryId = entryId, FinancialAccountId = donation.FinancialAccountId ?? master.CashAccountId, Description = "مدين خزنة/بنك", DebitAmount = amount, CreditAmount = 0m, CreatedAtUtc = DateTime.UtcNow });
            lines.Add(new JournalEntryLine { Id = DGuid($"journal-donation-{counter}-cr"), JournalEntryId = entryId, FinancialAccountId = donation.TargetingScopeCode == "GeneralFund" ? master.GeneralDonationsAccountId : master.RestrictedDonationsAccountId, Description = "دائن تبرعات", DebitAmount = 0m, CreditAmount = amount, CreatedAtUtc = DateTime.UtcNow });
            counter++;
        }

        foreach (var disbursement in disbursements.Where(x => x.ExecutionStatus == "FullyDisbursed").Take(250))
        {
            var entryId = DGuid($"journal-disbursement-{counter}");
            var amount = disbursement.ExecutedAmount ?? disbursement.Amount ?? 0m;
            entries.Add(new JournalEntry
            {
                Id = entryId,
                EntryNumber = $"{JournalPrefix}{counter:000000}",
                EntryDate = disbursement.DisbursementDate,
                Description = $"قيد صرف مساعدة تجريبي للمستفيد",
                FiscalPeriodId = master.FiscalPeriodId,
                Status = JournalEntryStatus.Posted,
                TotalDebit = amount,
                TotalCredit = amount,
                SourceType = "BeneficiaryAidDisbursement",
                SourceId = disbursement.Id,
                CreatedAtUtc = DateTime.UtcNow,
                PostedAtUtc = DateTime.UtcNow,
                PostedByUserId = SeedUserId
            });
            lines.Add(new JournalEntryLine { Id = DGuid($"journal-disbursement-{counter}-dr"), JournalEntryId = entryId, FinancialAccountId = master.AidExpenseAccountId, Description = "مدين مصروف مساعدات", DebitAmount = amount, CreditAmount = 0m, CreatedAtUtc = DateTime.UtcNow });
            lines.Add(new JournalEntryLine { Id = DGuid($"journal-disbursement-{counter}-cr"), JournalEntryId = entryId, FinancialAccountId = disbursement.FinancialAccountId ?? master.CashAccountId, Description = "دائن خزنة", DebitAmount = 0m, CreditAmount = amount, CreatedAtUtc = DateTime.UtcNow });
            counter++;
        }

        await db.Set<JournalEntry>().AddRangeAsync(entries, ct);
        await db.Set<JournalEntryLine>().AddRangeAsync(lines, ct);
        await db.SaveChangesAsync(ct);
        db.ChangeTracker.Clear();
        return entries.Count;
    }

    private static async Task ResetDemoDataAsync(AppDbContext db, CancellationToken ct)
    {
        await db.Set<JournalEntryLine>().Where(x => x.JournalEntry != null && x.JournalEntry.EntryNumber.StartsWith(JournalPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<JournalEntry>().Where(x => x.EntryNumber.StartsWith(JournalPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<AidCycleBeneficiary>().Where(x => x.AidCycle != null && x.AidCycle.CycleNumber.StartsWith(AidCyclePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<AidCycle>().Where(x => x.CycleNumber.StartsWith(AidCyclePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryAidDisbursementFundingLine>().Where(x => x.CreatedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryAidDisbursement>().Where(x => x.CreatedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<CharityStoreIssueLine>().Where(x => x.Issue != null && x.Issue.IssueNumber.StartsWith(IssuePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<CharityStoreIssue>().Where(x => x.IssueNumber.StartsWith(IssuePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<DonationAllocation>().Where(x => x.CreatedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryAidRequest>().Where(x => x.CreatedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryCommitteeDecision>().Where(x => x.ApprovedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryHumanitarianResearchReview>().Where(x => x.ReviewerUserId == SeedUserId || x.ReviewedByUserId == SeedUserId).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryHumanitarianResearchExpenseItem>().Where(x => x.Research != null && x.Research.ResearchNumber.StartsWith(ResearchPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryHumanitarianResearchIncomeItem>().Where(x => x.Research != null && x.Research.ResearchNumber.StartsWith(ResearchPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryHumanitarianResearchFamilyMember>().Where(x => x.Research != null && x.Research.ResearchNumber.StartsWith(ResearchPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryHumanitarianResearch>().Where(x => x.ResearchNumber.StartsWith(ResearchPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<BeneficiaryFamilyMember>().Where(x => x.Beneficiary != null && x.Beneficiary.Code.StartsWith(BeneficiaryCodePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<ProjectBeneficiary>().Where(x => x.Notes != null && x.Notes.Contains("DEMO-LARGE-SEED")).ExecuteDeleteAsync(ct);
        await db.Set<Beneficiary>().Where(x => x.Code.StartsWith(BeneficiaryCodePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<CharityStoreReceiptLine>().Where(x => x.Receipt != null && x.Receipt.ReceiptNumber.StartsWith(ReceiptPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<CharityStoreReceipt>().Where(x => x.ReceiptNumber.StartsWith(ReceiptPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<DonationInKindItem>().Where(x => x.Donation != null && x.Donation.DonationNumber.StartsWith(InKindDonationNumberPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<Donation>().Where(x => x.DonationNumber.StartsWith(DonationNumberPrefix) || x.DonationNumber.StartsWith(InKindDonationNumberPrefix)).ExecuteDeleteAsync(ct);
        await db.Set<Donor>().Where(x => x.Code.StartsWith(DonorCodePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<GrantInstallment>().Where(x => x.ReferenceNumber != null && x.ReferenceNumber.StartsWith("D-GI-")).ExecuteDeleteAsync(ct);
        await db.Set<GrantAgreement>().Where(x => x.AgreementNumber.StartsWith("D-GR-")).ExecuteDeleteAsync(ct);
        await db.Set<Funder>().Where(x => x.Code.StartsWith(FunderCodePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<CharityProject>().Where(x => x.Code.StartsWith(ProjectCodePrefix)).ExecuteDeleteAsync(ct);
        await db.Set<HrEmployee>().Where(x => x.Code.StartsWith(EmployeeCodePrefix)).ExecuteDeleteAsync(ct);
    }

    private static async Task AddIfMissingAsync<TEntity>(AppDbContext db, TEntity entity, CancellationToken ct) where TEntity : class
    {
        var id = (Guid?)typeof(TEntity).GetProperty("Id")?.GetValue(entity);
        if (!id.HasValue || id.Value == Guid.Empty)
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} must have a deterministic Id before seeding.");
        }

        var existing = await db.Set<TEntity>().FindAsync(new object?[] { id.Value }, ct);
        if (existing == null)
        {
            await db.Set<TEntity>().AddAsync(entity, ct);
        }
    }

    private static async Task<Guid> EnsurePaymentMethodAsync(AppDbContext db, string code, string nameAr, bool isCash, bool isDefault, CancellationToken ct)
    {
        var existing = await db.Set<PaymentMethod>().AsNoTracking().FirstOrDefaultAsync(x => x.MethodCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"payment-{code}");
        await db.Set<PaymentMethod>().AddAsync(new PaymentMethod { Id = id, MethodCode = code, MethodNameAr = nameAr, MethodNameEn = code, IsCash = isCash, IsDefault = isDefault, IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid> EnsureAccountAsync(AppDbContext db, string code, string nameAr, AccountCategory category, int level, bool isPosting, Guid? parentId, CancellationToken ct)
    {
        var existing = await db.Set<FinancialAccount>().AsNoTracking().FirstOrDefaultAsync(x => x.AccountCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"account-{code}");
        await db.Set<FinancialAccount>().AddAsync(new FinancialAccount { Id = id, AccountCode = code, AccountNameAr = nameAr, AccountNameEn = code, Category = category, Level = level, IsPosting = isPosting, ParentAccountId = parentId, IsActive = true, Notes = "حساب تجريبي لبيانات الجمعية الكبيرة.", CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid> EnsureUnitAsync(AppDbContext db, string code, string nameAr, string symbol, CancellationToken ct)
    {
        var existing = await db.Set<Unit>().AsNoTracking().FirstOrDefaultAsync(x => x.UnitCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"unit-{code}");
        await db.Set<Unit>().AddAsync(new Unit { Id = id, UnitCode = code, UnitNameAr = nameAr, UnitNameEn = code, Symbol = symbol, IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid> EnsureItemGroupAsync(AppDbContext db, string code, string nameAr, CancellationToken ct)
    {
        var existing = await db.Set<ItemGroup>().AsNoTracking().FirstOrDefaultAsync(x => x.GroupCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"item-group-{code}");
        await db.Set<ItemGroup>().AddAsync(new ItemGroup { Id = id, GroupCode = code, GroupNameAr = nameAr, GroupNameEn = code, IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid> EnsureWarehouseAsync(AppDbContext db, string code, string nameAr, CancellationToken ct)
    {
        var existing = await db.Set<Warehouse>().AsNoTracking().FirstOrDefaultAsync(x => x.WarehouseCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"warehouse-{code}");
        await db.Set<Warehouse>().AddAsync(new Warehouse { Id = id, WarehouseCode = code, WarehouseNameAr = nameAr, WarehouseNameEn = code, IsMain = true, IsActive = true, Address = "مخزن تجريبي", CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid> EnsureItemAsync(AppDbContext db, string code, string nameAr, Guid itemGroupId, Guid unitId, decimal purchasePrice, decimal salePrice, CancellationToken ct)
    {
        var existing = await db.Set<Item>().AsNoTracking().FirstOrDefaultAsync(x => x.ItemCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"item-{code}");
        await db.Set<Item>().AddAsync(new Item { Id = id, ItemCode = code, ItemNameAr = nameAr, ItemNameEn = code, ItemGroupId = itemGroupId, UnitId = unitId, IsService = false, IsStockItem = true, IsActive = true, PurchasePrice = purchasePrice, SalePrice = salePrice, MinimumQuantity = 50, ReorderQuantity = 100, IsTaxable = false, TaxRate = 0, Description = "صنف تبرعات تجريبي", CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static async Task<Guid[]> EnsureFundersAndGrantsAsync(AppDbContext db, CancellationToken ct)
    {
        var ids = new List<Guid>();
        for (var i = 1; i <= 5; i++)
        {
            var funderId = DGuid($"funder-{i}");
            ids.Add(funderId);
            if (!await db.Set<Funder>().AnyAsync(x => x.Id == funderId, ct))
            {
                await db.Set<Funder>().AddAsync(new Funder { Id = funderId, Code = $"{FunderCodePrefix}{i:000}", Name = $"جهة مانحة تجريبية {i}", FunderType = i % 2 == 0 ? "مؤسسة" : "شركة", ContactPerson = $"مسؤول تمويل {i}", PhoneNumber = DemoPhone(i, 40), Email = $"funder{i}@demo-charity.local", AddressLine = "عنوان جهة مانحة تجريبية", Notes = "جهة مانحة ضمن بيانات الاختبار", IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
            }

            var agreementId = DGuid($"grant-agreement-{i}");
            if (!await db.Set<GrantAgreement>().AnyAsync(x => x.Id == agreementId, ct))
            {
                await db.Set<GrantAgreement>().AddAsync(new GrantAgreement { Id = agreementId, AgreementNumber = $"D-GR-{i:000}", FunderId = funderId, Title = $"اتفاقية تمويل مشروع خيري {i}", Description = "اتفاقية تجريبية مرتبطة بالمشروعات والمستفيدين", AgreementDate = DateTime.Today.AddMonths(-6 - i), StartDate = DateTime.Today.AddMonths(-6), EndDate = DateTime.Today.AddMonths(12), TotalAmount = 500000 + i * 150000, Currency = "EGP", PaymentTerms = "دفعات ربع سنوية", ReportingRequirements = "تقرير شهري", Status = "Active", Notes = "بيانات تمويل تجريبية", CreatedAtUtc = DateTime.UtcNow }, ct);

                for (var n = 1; n <= 3; n++)
                {
                    await db.Set<GrantInstallment>().AddAsync(new GrantInstallment { Id = DGuid($"grant-installment-{i}-{n}"), GrantAgreementId = agreementId, InstallmentNumber = n, DueDate = DateTime.Today.AddMonths(n * 2), Amount = (500000 + i * 150000) / 3m, ReceivedAmount = n == 1 ? (500000 + i * 150000) / 3m : null, ReceivedDate = n == 1 ? DateTime.Today.AddMonths(-1) : null, Status = n == 1 ? "Received" : "Planned", ReferenceNumber = $"D-GI-{i:000}-{n:00}", Notes = "دفعة تمويل تجريبية", CreatedAtUtc = DateTime.UtcNow }, ct);
                }
            }
        }

        return ids.ToArray();
    }

    private static async Task<Guid[]> EnsureProjectsAsync(AppDbContext db, Guid[] funderIds, CancellationToken ct)
    {
        var projectIds = new List<Guid>();
        for (var i = 1; i <= 6; i++)
        {
            var projectId = DGuid($"charity-project-{i}");
            projectIds.Add(projectId);
            if (!await db.Set<CharityProject>().AnyAsync(x => x.Id == projectId, ct))
            {
                await db.Set<CharityProject>().AddAsync(new CharityProject { Id = projectId, Code = $"{ProjectCodePrefix}{i:000}", Name = ProjectName(i), Description = "مشروع خيري تجريبي مرتبط بالمستفيدين والتمويل", StartDate = DateTime.Today.AddMonths(-4), EndDate = DateTime.Today.AddMonths(8 + i), Budget = 300000 + i * 120000, Status = i % 3 == 0 ? "Planning" : "Active", TargetBeneficiariesCount = 500 + i * 150, Location = Areas[i % Areas.Length], Objectives = "تحسين جودة الحياة ودعم الحالات المستحقة", Kpis = "عدد مستفيدين - قيمة مساعدات - نسبة تنفيذ", Notes = "مشروع تجريبي", IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
            }
        }

        return projectIds.ToArray();
    }

    private static async Task<(Guid[] Departments, Guid[] JobTitles)> EnsureHrAsync(AppDbContext db, CancellationToken ct)
    {
        var departmentNames = new[] { "خدمة المستفيدين", "البحث الاجتماعي", "المخازن", "الحسابات", "المشروعات" };
        var titleNames = new[] { "باحث اجتماعي", "مراجع", "أمين مخزن", "محاسب", "منسق مشروع" };
        var departments = new List<Guid>();
        var titles = new List<Guid>();

        for (var i = 0; i < departmentNames.Length; i++)
        {
            var id = DGuid($"department-{i + 1}");
            departments.Add(id);
            if (!await db.Set<HrDepartment>().AnyAsync(x => x.Id == id, ct))
            {
                await db.Set<HrDepartment>().AddAsync(new HrDepartment { Id = id, Name = departmentNames[i], Description = "قسم تجريبي", IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
            }
        }

        for (var i = 0; i < titleNames.Length; i++)
        {
            var id = DGuid($"job-title-{i + 1}");
            titles.Add(id);
            if (!await db.Set<HrJobTitle>().AnyAsync(x => x.Id == id, ct))
            {
                await db.Set<HrJobTitle>().AddAsync(new HrJobTitle { Id = id, Name = titleNames[i], Description = "وظيفة تجريبية", IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
            }
        }

        if (!await db.Set<HrEmployee>().AnyAsync(x => x.Code.StartsWith(EmployeeCodePrefix), ct))
        {
            for (var i = 1; i <= 45; i++)
            {
                await db.Set<HrEmployee>().AddAsync(new HrEmployee { Id = DGuid($"employee-{i}"), Code = $"{EmployeeCodePrefix}{i:000}", FullName = $"موظف جمعية تجريبي {i:00}", NationalId = DemoNationalId(90000 + i, i % 3 == 0), BirthDate = DateTime.Today.AddYears(-(25 + i % 25)), PhoneNumber = DemoPhone(i, 50), Email = $"employee{i:000}@demo-charity.local", AddressLine = "عنوان موظف تجريبي", DepartmentId = departments[i % departments.Count], JobTitleId = titles[i % titles.Count], HireDate = DateTime.Today.AddMonths(-(i % 60)), EmploymentType = i % 5 == 0 ? "PartTime" : "Permanent", BasicSalary = 4000 + (i % 10) * 450, InsuranceSalary = 3000 + (i % 8) * 300, Status = "Active", Notes = "موظف تجريبي لربط موديول الموارد البشرية", IsActive = true, CreatedAtUtc = DateTime.UtcNow }, ct);
            }
        }

        return (departments.ToArray(), titles.ToArray());
    }

    private static async Task<Guid> EnsureFiscalPeriodAsync(AppDbContext db, CancellationToken ct)
    {
        var code = $"D-FY-{DateTime.Today.Year}";
        var existing = await db.Set<FiscalPeriod>().AsNoTracking().FirstOrDefaultAsync(x => x.PeriodCode == code, ct);
        if (existing != null) return existing.Id;
        var id = DGuid($"fiscal-period-{DateTime.Today.Year}");
        await db.Set<FiscalPeriod>().AddAsync(new FiscalPeriod { Id = id, PeriodCode = code, PeriodNameAr = $"السنة المالية التجريبية {DateTime.Today.Year}", PeriodNameEn = code, StartDate = new DateTime(DateTime.Today.Year, 1, 1), EndDate = new DateTime(DateTime.Today.Year, 12, 31), IsCurrent = true, IsClosed = false, IsActive = true, IsOpen = true, Notes = "فترة مالية للقيود التجريبية", CreatedAtUtc = DateTime.UtcNow }, ct);
        return id;
    }

    private static Guid PickBeneficiaryStatus(int i)
    {
        var bucket = i % 100;
        if (bucket < 15) return CharityLookupSeedIds.BeneficiaryStatusNew;
        if (bucket < 35) return CharityLookupSeedIds.BeneficiaryStatusUnderReview;
        if (bucket < 88) return CharityLookupSeedIds.BeneficiaryStatusApproved;
        if (bucket < 96) return CharityLookupSeedIds.BeneficiaryStatusRejected;
        return CharityLookupSeedIds.BeneficiaryStatusSuspended;
    }

    private static Guid PickMaritalStatus(int i)
    {
        return (i % 4) switch
        {
            0 => CharityLookupSeedIds.MaritalMarried,
            1 => CharityLookupSeedIds.MaritalSingle,
            2 => CharityLookupSeedIds.MaritalWidowed,
            _ => CharityLookupSeedIds.MaritalDivorced
        };
    }

    private static Guid PickAidType(int i)
    {
        return (i % 7) switch
        {
            0 => CharityLookupSeedIds.AidTypeFood,
            1 => CharityLookupSeedIds.AidTypeCash,
            2 => CharityLookupSeedIds.AidTypeMedical,
            3 => CharityLookupSeedIds.AidTypeEducational,
            4 => CharityLookupSeedIds.AidTypeClothes,
            5 => CharityLookupSeedIds.AidTypeDevices,
            _ => CharityLookupSeedIds.AidTypeSponsorship
        };
    }

    private static decimal PickRequestedAmount(Guid aidTypeId, int i)
    {
        if (aidTypeId == CharityLookupSeedIds.AidTypeFood) return 350 + (i % 5) * 50;
        if (aidTypeId == CharityLookupSeedIds.AidTypeClothes) return 500 + (i % 4) * 100;
        if (aidTypeId == CharityLookupSeedIds.AidTypeMedical) return 1500 + (i % 8) * 500;
        if (aidTypeId == CharityLookupSeedIds.AidTypeEducational) return 800 + (i % 6) * 250;
        if (aidTypeId == CharityLookupSeedIds.AidTypeDevices) return 2500 + (i % 8) * 600;
        if (aidTypeId == CharityLookupSeedIds.AidTypeSponsorship) return 1000;
        return 700 + (i % 8) * 200;
    }

    private static string AidTypeName(Guid aidTypeId)
    {
        if (aidTypeId == CharityLookupSeedIds.AidTypeFood) return "مساعدة غذائية";
        if (aidTypeId == CharityLookupSeedIds.AidTypeMedical) return "مساعدة علاجية";
        if (aidTypeId == CharityLookupSeedIds.AidTypeEducational) return "مساعدة تعليمية";
        if (aidTypeId == CharityLookupSeedIds.AidTypeClothes) return "ملابس";
        if (aidTypeId == CharityLookupSeedIds.AidTypeDevices) return "أجهزة ومستلزمات";
        if (aidTypeId == CharityLookupSeedIds.AidTypeSponsorship) return "كفالة";
        return "مساعدة مالية";
    }

    private static string BuildAidReason(Guid aidTypeId)
    {
        if (aidTypeId == CharityLookupSeedIds.AidTypeFood) return "احتياج غذائي للأسرة";
        if (aidTypeId == CharityLookupSeedIds.AidTypeMedical) return "احتياج علاجي وشراء أدوية";
        if (aidTypeId == CharityLookupSeedIds.AidTypeEducational) return "مصاريف تعليم وأدوات مدرسية";
        if (aidTypeId == CharityLookupSeedIds.AidTypeClothes) return "احتياج ملابس وبطاطين";
        if (aidTypeId == CharityLookupSeedIds.AidTypeDevices) return "توفير جهاز أو مستلزم ضروري";
        if (aidTypeId == CharityLookupSeedIds.AidTypeSponsorship) return "كفالة شهرية";
        return "مساعدة مالية عاجلة";
    }

    private static string MaritalStatusName(Guid maritalStatusId)
    {
        if (maritalStatusId == CharityLookupSeedIds.MaritalMarried) return "متزوج";
        if (maritalStatusId == CharityLookupSeedIds.MaritalWidowed) return "أرمل";
        if (maritalStatusId == CharityLookupSeedIds.MaritalDivorced) return "مطلق";
        return "أعزب";
    }

    private static string ProjectName(int i) => i switch
    {
        1 => "مشروع دعم الأسر الأولى بالرعاية",
        2 => "مشروع شنطة رمضان",
        3 => "مشروع دعم العلاج والدواء",
        4 => "مشروع كفالة الأيتام",
        5 => "مشروع التعليم والحقائب المدرسية",
        _ => "مشروع البطاطين والملابس"
    };

    private static decimal EstimatedUnitValue(Guid itemId, MasterData master)
    {
        var index = Array.IndexOf(master.ItemIds, itemId);
        return index switch
        {
            0 => 32m,
            1 => 35m,
            2 => 85m,
            3 => 350m,
            4 => 250m,
            _ => 300m
        };
    }

    private static string BuildPersonName(int i, bool female)
    {
        var first = female ? FemaleNames[i % FemaleNames.Length] : MaleNames[i % MaleNames.Length];
        return $"{first} {FamilyNames[(i / 3) % FamilyNames.Length]} {FamilyNames[(i / 7) % FamilyNames.Length]}";
    }

    private static string BuildFamilyMemberName(int i, int memberIndex, bool female)
    {
        var first = female ? FemaleNames[(i + memberIndex) % FemaleNames.Length] : MaleNames[(i + memberIndex) % MaleNames.Length];
        return $"{first} {FamilyNames[(i / 5) % FamilyNames.Length]}";
    }

    private static string DemoNationalId(int i, bool female)
    {
        var century = "2";
        var year = 70 + (i % 30);
        var month = 1 + (i % 12);
        var day = 1 + (i % 27);
        var governorate = "26";
        var serial = (10000 + i % 89999).ToString("00000");
        var genderDigit = female ? ((i % 5) * 2).ToString() : (((i % 5) * 2) + 1).ToString();
        return $"{century}{year:00}{month:00}{day:00}{governorate}{serial}{genderDigit}".PadRight(14, '0')[..14];
    }

    private static string DemoPhone(int i, int prefixSeed)
        => $"01{prefixSeed % 3}{(10000000 + ((i * 7919) % 89999999)):00000000}"[..11];

    private static Guid DGuid(string key)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes("charity-large-demo-seed:" + key));
        return new Guid(bytes);
    }
}
