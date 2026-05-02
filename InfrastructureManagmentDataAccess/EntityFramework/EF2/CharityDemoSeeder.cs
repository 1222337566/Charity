
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.EntityFramework.EF;
using Microsoft.EntityFrameworkCore;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace InfrastructureManagmentWeb.Seeding
{
    public class CharityDemoSeedResult
    {
        public bool ResetApplied { get; set; }
        public int EmployeesCreated { get; set; }
        public int EmployeeContractsCreated { get; set; }
        public int EmployeeFundingAssignmentsCreated { get; set; }
        public int EmployeeTaskAssignmentsCreated { get; set; }
        public int EmployeeBonusesCreated { get; set; }
        public int BeneficiariesCreated { get; set; }
        public int HumanResearchFormsCreated { get; set; }
        public int DonorsCreated { get; set; }
        public int DonationsCreated { get; set; }
        public int FundersCreated { get; set; }
        public int GrantAgreementsCreated { get; set; }
        public int GrantInstallmentsCreated { get; set; }
        public int ProjectsCreated { get; set; }
        public int AidCyclesCreated { get; set; }
        public int VolunteersCreated { get; set; }
        public int BoardMeetingsCreated { get; set; }
        public int ProjectProposalsCreated { get; set; }
        public int PayrollMonthsCreated { get; set; }
        public int JournalEntriesCreated { get; set; }
        public int WarehousesCreated { get; set; }
        public int StockItemsCreated { get; set; }
        public int StoreReceiptsCreated { get; set; }
        public int StoreIssuesCreated { get; set; }
        public int StockNeedRequestsCreated { get; set; }
        public int StockReturnVouchersCreated { get; set; }
        public int StockDisposalVouchersCreated { get; set; }
        public int StockTransferOperationsCreated { get; set; }
        public object NeedRequestsCreated { get; set; }
        public object ItemsCreated { get; set; }
        public object ReturnVouchersCreated { get; set; }
        public object OpeningEntriesCreated { get; set; }
        public object ReceiptVouchersCreated { get; set; }
        public object IssueVouchersCreated { get; set; }
        public object TransferOperationsCreated { get; set; }
        public object DisposalVouchersCreated { get; set; }
        public object StockTransactionsCreated { get; set; }
    }

    public static class CharityDemoSeeder
    {
        private static readonly string[] MaleNames = { "أحمد", "محمد", "محمود", "خالد", "عبدالله", "مصطفى", "هيثم", "حسام", "طارق", "وائل" };
        private static readonly string[] FemaleNames = { "فاطمة", "أسماء", "مريم", "آية", "رحاب", "هدى", "سارة", "نهى", "إيمان", "دعاء" };
        private static readonly string[] FamilyNames = { "عبدالسلام", "علي", "حسن", "عبدالحميد", "السيد", "مصطفى", "طه", "صالح", "مبارك", "النوبي" };
        private static readonly Random Rng = new(24040);

        private const int DemoBeneficiaryCount = 7000;
        private const int DemoHumanResearchCount = 480;
        private const int DemoKafalaBeneficiaryCount = 600;
        private const int DemoDonorCount = 240;
        private const int DemoDonationMonths = 30;

        private static async Task<AidTypeLookup> ResolveAidTypeAsync(AppDbContext db, params string[] names)
        {
            foreach (var name in names.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var match = await db.Set<AidTypeLookup>().FirstOrDefaultAsync(x => x.NameAr == name);
                if (match != null)
                    return match;
            }

            var fallback = await db.Set<AidTypeLookup>().OrderBy(x => x.DisplayOrder).FirstAsync();
            return fallback;
        }

        private static string GetAidTypeDisplayName(AidTypeLookup? aidType)
            => aidType?.NameAr ?? string.Empty;

        public static async Task<CharityDemoSeedResult> SeedAsync(AppDbContext db, string? createdByUserId = null)
        {
           // await db.Database.MigrateAsync();

            await PosBasicSeeder.SeedUnitsAndGroupsAsync(db);
            await AccountSeeder.SeedAsync(db);
            await EnsurePaymentMethodsAsync(db);
            await EnsureLookupsAsync(db);
            await EnsureAccountingBaseAsync(db);

            var result = new CharityDemoSeedResult();

            await db.SaveChangesAsync();

            result.EmployeesCreated = await SeedHrAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            var hrRfp = await SeedHrRfpAsync(db, createdByUserId);
            result.EmployeeContractsCreated = hrRfp.Contracts;
            result.EmployeeFundingAssignmentsCreated = hrRfp.FundingAssignments;
            result.EmployeeTaskAssignmentsCreated = hrRfp.TaskAssignments;
            result.EmployeeBonusesCreated = hrRfp.Bonuses;
            await db.SaveChangesAsync();

            result.BeneficiariesCreated = await SeedBeneficiariesAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.HumanResearchFormsCreated = await SeedHumanResearchAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.DonorsCreated = await SeedDonorsAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.DonationsCreated = await SeedDonationsAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            var grants = await SeedFundersAndGrantsAsync(db, createdByUserId);
            result.FundersCreated = grants.Funders;
            result.GrantAgreementsCreated = grants.GrantAgreements;
            result.GrantInstallmentsCreated = grants.GrantInstallments;
            await db.SaveChangesAsync();

            result.ProjectsCreated = await SeedProjectsAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            await SeedKafalaAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            var stock = await SeedStockAsync(db, createdByUserId);
            result.WarehousesCreated = stock.Warehouses;
            result.StockItemsCreated = stock.Items;
            result.StoreReceiptsCreated = stock.Receipts;
            result.StoreIssuesCreated = stock.Issues;
            result.StockNeedRequestsCreated = stock.NeedRequests;
            result.StockReturnVouchersCreated = stock.ReturnVouchers;
            result.StockDisposalVouchersCreated = stock.Disposals;
            result.StockTransferOperationsCreated = stock.Transfers;
            await db.SaveChangesAsync();

            result.AidCyclesCreated = await SeedAidCyclesAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.VolunteersCreated = await SeedVolunteersAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.BoardMeetingsCreated = await SeedBoardMeetingsAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            result.ProjectProposalsCreated = await SeedProjectProposalsAsync(db, createdByUserId);
            await db.SaveChangesAsync();

            var accounting = await SeedPayrollAndAccountingAsync(db, createdByUserId);
            result.PayrollMonthsCreated = accounting.PayrollMonths;
            result.JournalEntriesCreated = accounting.JournalEntries;
            await db.SaveChangesAsync();

            return result;
        }

        public static async Task<CharityDemoSeedResult> ResetAndSeedAsync(AppDbContext db, string? createdByUserId = null)
        {
            await db.Database.MigrateAsync();
            await ClearExistingCharityDataAsync(db);
            var result = await SeedAsync(db, createdByUserId);
            result.ResetApplied = true;
            return result;
        }


        private static async Task ClearExistingCharityDataAsync(AppDbContext db)
        {
            await using var tx = await db.Database.BeginTransactionAsync();

            await db.Set<JournalEntryLine>().ExecuteDeleteAsync();
            await db.Set<JournalEntry>().ExecuteDeleteAsync();
            await db.Set<PayrollPayment>().ExecuteDeleteAsync();
            await db.Set<PayrollEmployeeItem>().ExecuteDeleteAsync();
            await db.Set<PayrollEmployee>().ExecuteDeleteAsync();
            await db.Set<PayrollMonth>().ExecuteDeleteAsync();

            await db.Set<ProjectProposalAttachment>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalTeamMember>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalMonitoringIndicator>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalWorkPlan>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalActivity>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalObjective>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalTargetGroup>().ExecuteDeleteAsync();
            await db.Set<ProjectProposalPastExperience>().ExecuteDeleteAsync();
            await db.Set<ProjectProposal>().ExecuteDeleteAsync();

            await db.Set<BoardDecisionAttachment>().ExecuteDeleteAsync();
            await db.Set<BoardDecisionFollowUp>().ExecuteDeleteAsync();
            await db.Set<BoardDecision>().ExecuteDeleteAsync();
            await db.Set<BoardMeetingAttachment>().ExecuteDeleteAsync();
            await db.Set<BoardMeetingMinute>().ExecuteDeleteAsync();
            await db.Set<BoardMeetingAttendee>().ExecuteDeleteAsync();
            await db.Set<BoardMeeting>().ExecuteDeleteAsync();

            await db.Set<ProjectTaskDailyUpdate>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseTask>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseActivity>().ExecuteDeleteAsync();
            await db.Set<ProjectTrackingLog>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseMilestone>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseExpenseLink>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseStoreIssueLink>().ExecuteDeleteAsync();

            await db.Set<StockDisposalVoucherLine>().ExecuteDeleteAsync();
            await db.Set<StockDisposalVoucher>().ExecuteDeleteAsync();
            await db.Set<StockReturnVoucherLine>().ExecuteDeleteAsync();
            await db.Set<StockReturnVoucher>().ExecuteDeleteAsync();
            await db.Set<StockNeedRequestLine>().ExecuteDeleteAsync();
            await db.Set<StockNeedRequest>().ExecuteDeleteAsync();
            await db.Set<CharityStoreIssueLine>().ExecuteDeleteAsync();
            await db.Set<CharityStoreIssue>().ExecuteDeleteAsync();
            await db.Set<CharityStoreReceiptLine>().ExecuteDeleteAsync();
            await db.Set<CharityStoreReceipt>().ExecuteDeleteAsync();
            await db.Set<ItemWarehouseBalance>().ExecuteDeleteAsync();
            await db.Set<StockTransaction>().ExecuteDeleteAsync();

            await db.Set<ProjectPhase>().ExecuteDeleteAsync();
            await db.Set<ProjectExpenseLink>().ExecuteDeleteAsync();
            await db.Set<ProjectGrant>().ExecuteDeleteAsync();
            await db.Set<ProjectBeneficiary>().ExecuteDeleteAsync();
            await db.Set<ProjectActivity>().ExecuteDeleteAsync();
            await db.Set<ProjectBudgetLine>().ExecuteDeleteAsync();
            await db.Set<ProjectAccountingProfile>().ExecuteDeleteAsync();
            await db.Set<CharityProject>().ExecuteDeleteAsync();

            await db.Set<AccountingIntegrationProfile>().ExecuteDeleteAsync();
            await db.Set<GrantInstallment>().ExecuteDeleteAsync();
            await db.Set<GrantCondition>().ExecuteDeleteAsync();
            await db.Set<GrantAgreement>().ExecuteDeleteAsync();
            await db.Set<Funder>().ExecuteDeleteAsync();

            await db.Set<BeneficiaryAidDisbursementFundingLine>().ExecuteDeleteAsync();

            await db.Set<DonationAllocation>().ExecuteDeleteAsync();
            await db.Set<DonationInKindItem>().ExecuteDeleteAsync();
            await db.Set<Donation>().ExecuteDeleteAsync();
            await db.Set<Donor>().ExecuteDeleteAsync();

            await db.Set<KafalaPayment>().ExecuteDeleteAsync();
            await db.Set<KafalaCase>().ExecuteDeleteAsync();
            await db.Set<KafalaSponsor>().ExecuteDeleteAsync();

            await db.Set<AidCycleBeneficiary>().ExecuteDeleteAsync();
            await db.Set<AidCycle>().ExecuteDeleteAsync();

            await db.Set<BeneficiaryHumanitarianResearchCommitteeEvaluation>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchReview>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchHouseAsset>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchDebt>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchExpenseItem>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchIncomeItem>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearchFamilyMember>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryHumanitarianResearch>().ExecuteDeleteAsync();

            await db.Set<BeneficiaryAidDisbursementFundingLine>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryAidDisbursement>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryAidRequest>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryCommitteeDecision>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryAssessment>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryDocument>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryFamilyMember>().ExecuteDeleteAsync();
            await db.Set<BeneficiaryOldRecord>().ExecuteDeleteAsync();
            await db.Set<Beneficiary>().ExecuteDeleteAsync();

            await db.Set<VolunteerHourLog>().ExecuteDeleteAsync();
            await db.Set<VolunteerProjectAssignment>().ExecuteDeleteAsync();
            await db.Set<VolunteerAvailabilitySlot>().ExecuteDeleteAsync();
            await db.Set<VolunteerSkill>().ExecuteDeleteAsync();
            await db.Set<Volunteer>().ExecuteDeleteAsync();
            await db.Set<VolunteerSkillDefinition>().ExecuteDeleteAsync();

            await db.Set<HrAttendanceRecord>().ExecuteDeleteAsync();
            await db.Set<HrPerformanceEvaluation>().ExecuteDeleteAsync();
            await db.Set<HrOutRequest>().ExecuteDeleteAsync();
            await db.Set<HrSanctionRecord>().ExecuteDeleteAsync();
            await db.Set<HrEmployeeMovement>().ExecuteDeleteAsync();
            await db.Set<HrEmployeeBonus>().ExecuteDeleteAsync();
            await db.Set<HrEmployeeTaskAssignment>().ExecuteDeleteAsync();
            await db.Set<HrEmployeeFundingAssignment>().ExecuteDeleteAsync();
            await db.Set<HrEmployeeContract>().ExecuteDeleteAsync();
            await db.Set<HrEmployee>().ExecuteDeleteAsync();
            await db.Set<HrShift>().ExecuteDeleteAsync();
            await db.Set<HrJobTitle>().ExecuteDeleteAsync();
            await db.Set<HrDepartment>().ExecuteDeleteAsync();

            await db.Set<Item>()
                .Where(x => x.ItemCode.StartsWith("ITM-DEMO-"))
                .ExecuteDeleteAsync();
            await db.Set<Warehouse>()
                .Where(x => x.WarehouseCode.StartsWith("WH-DEMO-"))
                .ExecuteDeleteAsync();

            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        private static async Task EnsureAccountingBaseAsync(AppDbContext db)
        {
            if (!await db.Set<FiscalPeriod>().AnyAsync())
            {
                await db.Set<FiscalPeriod>().AddAsync(new FiscalPeriod
                {
                    Id = Guid.NewGuid(),
                    PeriodCode = $"FY-{DateTime.Today.Year}",
                    PeriodNameAr = $"الفترة المالية {DateTime.Today.Year}",
                    StartDate = new DateTime(DateTime.Today.Year, 1, 1),
                    EndDate = new DateTime(DateTime.Today.Year, 12, 31),
                    IsCurrent = true,
                    IsClosed = false,
                    IsActive = true,
                    IsOpen = true,
                    Notes = "فترة ديمو"
                });
            }

            if (!await db.Set<CostCenter>().AnyAsync(x => x.CostCenterCode == "CC-DEMO-GENERAL"))
            {
                await db.Set<CostCenter>().AddAsync(new CostCenter
                {
                    Id = Guid.NewGuid(),
                    CostCenterCode = "CC-DEMO-GENERAL",
                    NameAr = "مركز تكلفة عام",
                    Level = 1,
                    IsActive = true,
                    IsProjectRelated = false,
                    Notes = "مركز تكلفة ديمو"
                });
            }
        }

        private sealed class HrRfpSeedResult { public int Contracts { get; set; } public int FundingAssignments { get; set; } public int TaskAssignments { get; set; } public int Bonuses { get; set; } }
        private sealed class GrantSeedResult { public int Funders { get; set; } public int GrantAgreements { get; set; } public int GrantInstallments { get; set; } }
        private sealed class AccountingSeedResult { public int PayrollMonths { get; set; } public int JournalEntries { get; set; } }
        private sealed class StockSeedResult { public int Warehouses { get; set; } public int Items { get; set; } public int Receipts { get; set; } public int Issues { get; set; } public int NeedRequests { get; set; } public int ReturnVouchers { get; set; } public int Disposals { get; set; } public int Transfers { get; set; } }

        private static async Task EnsureLookupsAsync(AppDbContext db)
        {
            await EnsureGenderAsync(db, "ذكر", "Male", 1);
            await EnsureGenderAsync(db, "أنثى", "Female", 2);

            await EnsureMaritalStatusAsync(db, "أعزب", "Single", 1);
            await EnsureMaritalStatusAsync(db, "متزوج", "Married", 2);
            await EnsureMaritalStatusAsync(db, "أرمل", "Widowed", 3);
            await EnsureMaritalStatusAsync(db, "مطلق", "Divorced", 4);

            await EnsureBeneficiaryStatusAsync(db, "جديد", "New", 1);
            await EnsureBeneficiaryStatusAsync(db, "تحت الدراسة", "Under Review", 2);
            await EnsureBeneficiaryStatusAsync(db, "معتمد", "Approved", 3);
            await EnsureBeneficiaryStatusAsync(db, "موقوف", "Stopped", 4);

            await EnsureAidTypeAsync(db, "نقدي", "Cash", 1);
            await EnsureAidTypeAsync(db, "غذائي", "Food", 2);
            await EnsureAidTypeAsync(db, "علاجي", "Medical", 3);
            await EnsureAidTypeAsync(db, "تعليمي", "Educational", 4);
            await EnsureAidTypeAsync(db, "ملابس", "Clothes", 5);
            await EnsureAidTypeAsync(db, "أجهزة", "Devices", 6);
            await EnsureAidTypeAsync(db, "كفالة", "Sponsorship", 7);

            var governorates = db.Set<Governorate>();
            var cities = db.Set<City>();
            var areas = db.Set<Area>();

            var sohag = await governorates.FirstOrDefaultAsync(x => x.NameAr == "سوهاج");
            if (sohag == null)
            {
                sohag = new Governorate { Id = Guid.NewGuid(), NameAr = "سوهاج", NameEn = "Sohag", IsActive = true };
                await governorates.AddAsync(sohag);
            }

            var assiut = await governorates.FirstOrDefaultAsync(x => x.NameAr == "أسيوط");
            if (assiut == null)
            {
                assiut = new Governorate { Id = Guid.NewGuid(), NameAr = "أسيوط", NameEn = "Assiut", IsActive = true };
                await governorates.AddAsync(assiut);
            }

            var city1 = await cities.FirstOrDefaultAsync(x => x.NameAr == "سوهاج");
            if (city1 == null)
            {
                city1 = new City { Id = Guid.NewGuid(), GovernorateId = sohag.Id, NameAr = "سوهاج", NameEn = "Sohag", IsActive = true };
                await cities.AddAsync(city1);
            }

            var city2 = await cities.FirstOrDefaultAsync(x => x.NameAr == "أخميم");
            if (city2 == null)
            {
                city2 = new City { Id = Guid.NewGuid(), GovernorateId = sohag.Id, NameAr = "أخميم", NameEn = "Akhmim", IsActive = true };
                await cities.AddAsync(city2);
            }

            var city3 = await cities.FirstOrDefaultAsync(x => x.NameAr == "أبوتيج");
            if (city3 == null)
            {
                city3 = new City { Id = Guid.NewGuid(), GovernorateId = assiut.Id, NameAr = "أبوتيج", NameEn = "Abutig", IsActive = true };
                await cities.AddAsync(city3);
            }

            if (!await areas.AnyAsync(x => x.NameAr == "الحويتي"))
                await areas.AddAsync(new Area { Id = Guid.NewGuid(), CityId = city1.Id, NameAr = "الحويتي", NameEn = "El Hewity", IsActive = true });

            if (!await areas.AnyAsync(x => x.NameAr == "السلاموني"))
                await areas.AddAsync(new Area { Id = Guid.NewGuid(), CityId = city2.Id, NameAr = "السلاموني", NameEn = "Elsalamony", IsActive = true });

            if (!await areas.AnyAsync(x => x.NameAr == "باقور"))
                await areas.AddAsync(new Area { Id = Guid.NewGuid(), CityId = city3.Id, NameAr = "باقور", NameEn = "Baqour", IsActive = true });
        }

        private static async Task EnsurePaymentMethodsAsync(AppDbContext db)
        {
            await EnsurePaymentMethodAsync(db, "CASH", "نقدي", "Cash", true, true);
            await EnsurePaymentMethodAsync(db, "BANK", "تحويل بنكي", "Bank Transfer", false, false);
            await EnsurePaymentMethodAsync(db, "VISA", "بطاقة", "Card", false, false);
            await EnsurePaymentMethodAsync(db, "WALLET", "محفظة", "Wallet", false, false);
        }

        private static async Task EnsureGenderAsync(AppDbContext db, string nameAr, string nameEn, int displayOrder)
        {
            if (await db.Set<GenderLookup>().AnyAsync(x => x.NameAr == nameAr)) return;
            await db.Set<GenderLookup>().AddAsync(new GenderLookup { Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn, DisplayOrder = displayOrder, IsActive = true });
        }

        private static async Task EnsureMaritalStatusAsync(AppDbContext db, string nameAr, string nameEn, int displayOrder)
        {
            if (await db.Set<MaritalStatusLookup>().AnyAsync(x => x.NameAr == nameAr)) return;
            await db.Set<MaritalStatusLookup>().AddAsync(new MaritalStatusLookup { Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn, DisplayOrder = displayOrder, IsActive = true });
        }

        private static async Task EnsureBeneficiaryStatusAsync(AppDbContext db, string nameAr, string nameEn, int displayOrder)
        {
            if (await db.Set<BeneficiaryStatusLookup>().AnyAsync(x => x.NameAr == nameAr)) return;
            await db.Set<BeneficiaryStatusLookup>().AddAsync(new BeneficiaryStatusLookup { Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn, DisplayOrder = displayOrder, IsActive = true });
        }

        private static async Task EnsureAidTypeAsync(AppDbContext db, string nameAr, string nameEn, int displayOrder)
        {
            if (await db.Set<AidTypeLookup>().AnyAsync(x => x.NameAr == nameAr)) return;
            await db.Set<AidTypeLookup>().AddAsync(new AidTypeLookup { Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn, DisplayOrder = displayOrder, IsActive = true });
        }

        private static async Task EnsurePaymentMethodAsync(AppDbContext db, string methodCode, string methodNameAr, string methodNameEn, bool isCash, bool isDefault)
        {
            if (await db.Set<PaymentMethod>().AnyAsync(x => x.MethodCode == methodCode)) return;
            await db.Set<PaymentMethod>().AddAsync(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                MethodCode = methodCode,
                MethodNameAr = methodNameAr,
                MethodNameEn = methodNameEn,
                IsCash = isCash,
                IsDefault = isDefault,
                IsActive = true
            });
        }

        private static async Task<int> SeedHrAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<HrEmployee>().AnyAsync(x => x.Code.StartsWith("EMP-DEMO-")))
                return 0;

            var departments = await EnsureHrDepartmentsAsync(db);
            var jobs = await EnsureHrJobTitlesAsync(db);
            var shift = await EnsureDefaultShiftAsync(db);

            var employees = new List<HrEmployee>();
            for (var i = 1; i <= 12; i++)
            {
                var isMale = i % 2 == 1;
                var name = BuildName(isMale);
                var dept = departments[(i - 1) % departments.Count];
                var job = jobs[(i - 1) % jobs.Count];

                employees.Add(new HrEmployee
                {
                    Id = Guid.NewGuid(),
                    Code = $"EMP-DEMO-{i:000}",
                    FullName = name,
                    NationalId = $"2980{i:000000000}",
                    BirthDate = DateTime.Today.AddYears(-(24 + i)),
                    PhoneNumber = $"0100{Rng.Next(1000000, 9999999)}",
                    Email = $"employee{i}@demo.charity.local",
                    AddressLine = "سوهاج - شارع التحرير",
                    DepartmentId = dept.Id,
                    JobTitleId = job.Id,
                    HireDate = DateTime.Today.AddMonths(-Rng.Next(6, 48)),
                    EmploymentType = i <= 8 ? "Permanent" : "Contract",
                    BasicSalary = 4500 + (i * 450),
                    InsuranceSalary = 3500 + (i * 250),
                    BankName = "البنك الأهلي المصري",
                    BankAccountNumber = $"10020030{i:000}",
                    Status = "Active",
                    Notes = "بيانات ديمو لعرض النظام",
                    UserId = createdByUserId,
                    IsActive = true
                });
            }

            await db.Set<HrEmployee>().AddRangeAsync(employees);

            var attendance = new List<HrAttendanceRecord>();
            foreach (var emp in employees.Take(8))
            {
                for (var d = 1; d <= 7; d++)
                {
                    var date = DateTime.Today.AddDays(-d);
                    attendance.Add(new HrAttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = emp.Id,
                        AttendanceDate = date,
                        ShiftId = shift.Id,
                        CheckInTime = new TimeSpan(8, Rng.Next(0, 35), 0),
                        CheckOutTime = new TimeSpan(15, Rng.Next(0, 35), 0),
                        WorkedHours = 7.5m + (decimal)(Rng.NextDouble() * 1.5),
                        LateMinutes = Rng.Next(0, 20),
                        EarlyLeaveMinutes = Rng.Next(0, 10),
                        OvertimeMinutes = Rng.Next(0, 45),
                        Status = "Present",
                        Source = "Manual",
                        Notes = "سجل حضور ديمو"
                    });
                }
            }

            await db.Set<HrAttendanceRecord>().AddRangeAsync(attendance);
            return employees.Count;
        }

        private static async Task<int> SeedBeneficiariesAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<Beneficiary>().AnyAsync(x => x.Code.StartsWith("BEN-DEMO-")))
                return 0;

            var genderMale = await db.Set<GenderLookup>().OrderBy(x => x.DisplayOrder).FirstAsync();
            var genderFemale = await db.Set<GenderLookup>().OrderBy(x => x.DisplayOrder).Skip(1).FirstAsync();
            var married = await db.Set<MaritalStatusLookup>().FirstAsync(x => x.NameAr == "متزوج");
            var single = await db.Set<MaritalStatusLookup>().FirstAsync(x => x.NameAr == "أعزب");
            var approvedStatus = await db.Set<BeneficiaryStatusLookup>().FirstAsync(x => x.NameAr == "معتمد");
            var reviewStatus = await db.Set<BeneficiaryStatusLookup>().FirstAsync(x => x.NameAr == "تحت الدراسة");
            var newStatus = await db.Set<BeneficiaryStatusLookup>().FirstAsync(x => x.NameAr == "جديد");
            var stoppedStatus = await db.Set<BeneficiaryStatusLookup>().FirstAsync(x => x.NameAr == "موقوف");

            var aidFinancial = await ResolveAidTypeAsync(db, "نقدي");
            var aidFood = await ResolveAidTypeAsync(db, "غذائي");
            var aidMedical = await ResolveAidTypeAsync(db, "علاجي");
            var aidEducational = await ResolveAidTypeAsync(db, "تعليمي");
            var aidTypes = new[] { aidFinancial, aidFood, aidMedical, aidEducational };

            var cities = await db.Set<City>().ToListAsync();
            var areas = await db.Set<Area>().ToListAsync();
            var govs = await db.Set<Governorate>().ToListAsync();

            var beneficiaries = new List<Beneficiary>(DemoBeneficiaryCount);
            var families = new List<BeneficiaryFamilyMember>(DemoBeneficiaryCount * 3);
            var decisions = new List<BeneficiaryCommitteeDecision>(2600);
            var requests = new List<BeneficiaryAidRequest>(3600);

            for (var i = 1; i <= DemoBeneficiaryCount; i++)
            {
                var isMale = i % 3 == 0;
                var fullName = BuildName(isMale);
                var status = i <= 3600
                    ? approvedStatus
                    : i <= 5000
                        ? reviewStatus
                        : i <= 6300
                            ? newStatus
                            : stoppedStatus;
                var city = cities[(i - 1) % cities.Count];
                var area = areas[(i - 1) % areas.Count];
                var gov = govs.First(x => x.Id == city.GovernorateId);
                var familyMembersCount = 2 + (i % 6);
                var monthlyIncome = 900m + ((i % 40) * 110m);
                var beneficiary = new Beneficiary
                {
                    Id = Guid.NewGuid(),
                    Code = $"BEN-DEMO-{i:00000}",
                    FullName = fullName,
                    NationalId = $"299{i:011}",
                    BirthDate = DateTime.Today.AddYears(-(22 + (i % 38))).AddDays(-(i % 300)),
                    GenderId = isMale ? genderMale.Id : genderFemale.Id,
                    MaritalStatusId = i % 4 == 0 ? married.Id : single.Id,
                    PhoneNumber = $"0105{(1000000 + i % 8999999):0000000}",
                    AlternatePhoneNumber = $"0122{(1000000 + ((i * 7) % 8999999)):0000000}",
                    AddressLine = $"{gov.NameAr} - {city.NameAr} - {area.NameAr}",
                    GovernorateId = gov.Id,
                    CityId = city.Id,
                    AreaId = area.Id,
                    FamilyMembersCount = familyMembersCount,
                    MonthlyIncome = monthlyIncome,
                    IncomeSource = (i % 5) switch
                    {
                        0 => "يومية غير ثابتة",
                        1 => "عمل بسيط",
                        2 => "معاش محدود",
                        3 => "مساعدة أقارب",
                        _ => "عمل موسمي"
                    },
                    HealthStatus = i % 6 == 0 ? "يحتاج متابعة صحية" : i % 11 == 0 ? "إعاقة جزئية" : "مستقر",
                    EducationStatus = i % 7 == 0 ? "أمي" : i % 3 == 0 ? "تعليم أساسي" : "تعليم متوسط",
                    WorkStatus = i % 5 == 0 ? "بدون عمل ثابت" : i % 2 == 0 ? "عمل متقطع" : "عامل يومية",
                    HousingStatus = i % 4 == 0 ? "إيجار" : i % 6 == 0 ? "سكن مشترك" : "ملك أسرة",
                    Notes = i % 17 == 0 ? "حالة متابعة دورية ضمن داتا الديمو" : "بيانات ديمو موسعة للمستفيد",
                    StatusId = status.Id,
                    RegistrationDate = DateTime.Today.AddDays(-(10 + (i % 720))),
                    CreatedByUserId = createdByUserId,
                    IsActive = i % 29 != 0
                };
                beneficiaries.Add(beneficiary);

                for (var f = 1; f <= Math.Max(1, familyMembersCount - 1); f++)
                {
                    families.Add(new BeneficiaryFamilyMember
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        FullName = BuildFamilyMemberName(f % 2 == 0),
                        Relationship = f == 1 ? "زوج/زوجة" : f <= 3 ? "ابن/ابنة" : "تابع أسري",
                        NationalId = $"300{Rng.Next(100000000, 999999999)}",
                        BirthDate = DateTime.Today.AddYears(-(6 + (f * 5) + (i % 9))),
                        GenderId = f % 2 == 0 ? genderFemale.Id : genderMale.Id,
                        EducationStatus = f == 1 ? "يقرأ ويكتب" : f <= 3 ? "طالب" : "غير محدد",
                        WorkStatus = f == 1 ? "لا يعمل" : f <= 2 ? "طالب" : "لا يعمل",
                        MonthlyIncome = f == 1 ? 0 : null,
                        HealthCondition = f % 5 == 0 ? "حالة صحية مزمنة" : "لا يوجد",
                        IsDependent = true,
                        Notes = "بيانات أسرة ديمو"
                    });
                }

                if (i <= 2600)
                {
                    var aidType = aidTypes[(i - 1) % aidTypes.Length];
                    var approvedAmount = aidType.Id == aidFinancial.Id || aidType.Id == aidMedical.Id || aidType.Id == aidEducational.Id
                        ? 900m + ((i % 12) * 125m)
                        : 0m;

                    decisions.Add(new BeneficiaryCommitteeDecision
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        DecisionDate = DateTime.Today.AddDays(-(5 + (i % 180))),
                        DecisionType = i % 9 == 0 ? "Reconsider" : "Approve",
                        ApprovedAidTypeId = aidType.Id,
                        ApprovedAmount = approvedAmount,
                        DurationInMonths = i % 8 == 0 ? 12 : i % 3 == 0 ? 6 : 3,
                        CommitteeNotes = i % 10 == 0 ? "اعتماد مشروط باستكمال المراجعة الميدانية" : "اعتماد ضمن بيانات التشغيل الموسعة",
                        ApprovedByUserId = createdByUserId
                    });
                }

                if (i <= 3600)
                {
                    var aidType = aidTypes[(i - 1) % aidTypes.Length];
                    var requestAmount = aidType.Id == aidFinancial.Id || aidType.Id == aidMedical.Id || aidType.Id == aidEducational.Id
                        ? 1000m + ((i % 10) * 150m)
                        : 0m;
                    var requestStatus = i <= 1800 ? "Approved" : i <= 2700 ? "UnderReview" : "Pending";

                    requests.Add(new BeneficiaryAidRequest
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        RequestDate = DateTime.Today.AddDays(-(3 + (i % 150))),
                        AidTypeId = aidType.Id,
                        RequestedAmount = requestAmount,
                        Reason = aidType.Id == aidFinancial.Id
                            ? "دعم أسري ومعيشي"
                            : aidType.Id == aidFood.Id
                                ? "مساعدة غذائية عاجلة"
                                : aidType.Id == aidMedical.Id
                                    ? "علاج وفحوصات"
                                    : "مصروفات تعليمية",
                        UrgencyLevel = i % 7 == 0 ? "High" : i % 3 == 0 ? "Medium" : "Low",
                        Status = requestStatus,
                        CreatedByUserId = createdByUserId,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-(2 + (i % 120)))
                    });
                }
            }

            await db.Set<Beneficiary>().AddRangeAsync(beneficiaries);
            await db.Set<BeneficiaryFamilyMember>().AddRangeAsync(families);
            await db.Set<BeneficiaryCommitteeDecision>().AddRangeAsync(decisions);
            await db.Set<BeneficiaryAidRequest>().AddRangeAsync(requests);

            return beneficiaries.Count;
        }

        private static async Task SeedKafalaAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<KafalaCase>().AnyAsync(x => x.CaseNumber.StartsWith("KAF-DEMO-")))
                return;

            var beneficiaries = await db.Set<Beneficiary>()
                .Where(x => x.Code.StartsWith("BEN-DEMO-"))
                .OrderBy(x => x.Code)
                .Take(DemoKafalaBeneficiaryCount)
                .ToListAsync();

            if (beneficiaries.Count == 0)
                return;

            var paymentMethods = await db.Set<PaymentMethod>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.MethodCode)
                .ToListAsync();

            var financialAccounts = await db.Set<FinancialAccount>()
                .Where(x => x.IsActive && x.IsPosting)
                .OrderBy(x => x.AccountCode)
                .Take(12)
                .ToListAsync();

            var sponsors = new List<KafalaSponsor>();
            var cases = new List<KafalaCase>();
            var payments = new List<KafalaPayment>();

            var sponsorCount = 180;
            for (var i = 1; i <= sponsorCount; i++)
            {
                var sponsorType = i % 9 == 0 ? "Company" : i % 13 == 0 ? "Institution" : "Individual";
                var sponsorName = sponsorType switch
                {
                    "Company" => $"شركة الخير المستدام {i:00}",
                    "Institution" => $"مؤسسة الرحمة والتنمية {i:00}",
                    _ => BuildName(i % 2 == 1)
                };

                sponsors.Add(new KafalaSponsor
                {
                    Id = Guid.NewGuid(),
                    SponsorCode = $"KSP-DEMO-{i:000}",
                    FullName = sponsorName,
                    SponsorType = sponsorType,
                    NationalIdOrTaxNo = sponsorType == "Individual" ? $"3010{i:000000000}" : $"TAX-{i:000000}",
                    PhoneNumber = $"0107{Rng.Next(1000000, 9999999)}",
                    Email = $"kafala.sponsor{i:000}@demo.charity.local",
                    AddressLine = i % 2 == 0 ? "سوهاج - مدينة ناصر" : "أسيوط - الأربعين",
                    Notes = "بيانات تجريبية للكفلاء لعرض لوحة الكفالة",
                    CreatedByUserId = createdByUserId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-(30 + i)),
                    IsActive = i % 17 != 0
                });
            }

            var today = DateTime.Today;
            var historyStart = new DateTime(today.Year, today.Month, 1).AddMonths(-14);

            for (var i = 0; i < beneficiaries.Count; i++)
            {
                var beneficiary = beneficiaries[i];
                var sponsor = sponsors[i % sponsors.Count];
                var sponsorshipType = (i % 5) switch
                {
                    0 => "Orphan",
                    1 => "Family",
                    2 => "Education",
                    3 => "Health",
                    _ => "Monthly"
                };
                var frequency = (i % 10) switch
                {
                    0 => "Annual",
                    1 => "SemiAnnual",
                    2 => "SemiAnnual",
                    3 => "Quarterly",
                    4 => "Quarterly",
                    5 => "Quarterly",
                    _ => "Monthly"
                };
                var frequencyMonths = GetFrequencyMonths(frequency);
                var startDate = new DateTime(today.Year, today.Month, Math.Min(5 + (i % 20), 28)).AddMonths(-(6 + (i % 18)));
                var status = i < 150 ? "Active" : i < 175 ? "Suspended" : "Closed";
                var installmentAmount = GetKafalaInstallmentAmount(sponsorshipType, frequencyMonths, i);
                var monthlyAmount = Math.Round(installmentAmount / frequencyMonths, 2);
                DateTime? endDate = null;
                if (status == "Closed")
                {
                    endDate = startDate.AddMonths(frequencyMonths * (3 + (i % 5)));
                    if (endDate.Value >= today)
                        endDate = today.AddMonths(-1 - (i % 3));
                }

                var caseEntity = new KafalaCase
                {
                    Id = Guid.NewGuid(),
                    CaseNumber = $"KAF-DEMO-{i + 1:000}",
                    SponsorId = sponsor.Id,
                    BeneficiaryId = beneficiary.Id,
                    SponsorshipType = sponsorshipType,
                    Frequency = frequency,
                    MonthlyAmount = monthlyAmount,
                    StartDate = startDate,
                    EndDate = endDate,
                    PaymentMethodId = paymentMethods.Count == 0 ? null : paymentMethods[i % paymentMethods.Count].Id,
                    FinancialAccountId = financialAccounts.Count == 0 ? null : financialAccounts[i % financialAccounts.Count].Id,
                    Status = status,
                    AutoIncludeInAidCycles = status == "Active" && i % 5 != 0,
                    Notes = i % 6 == 0 ? "كفالة منتظمة مع متابعة شهرية" : i % 6 == 1 ? "كفالة تعليمية مع مصروفات دورية" : "ملف كفالة ضمن بيانات الديمو",
                    CreatedAtUtc = startDate.AddDays(-7),
                    UpdatedAtUtc = startDate.AddDays(i % 9)
                };

                var cycleDate = new DateTime(startDate.Year, startDate.Month, Math.Min(startDate.Day, 28));
                var lastEligibleDate = endDate.HasValue && endDate.Value < today ? endDate.Value : today.AddDays(-(i % 6));
                DateTime? lastReceived = null;
                DateTime? lastDisbursed = null;
                var sequence = 1;

                while (cycleDate <= lastEligibleDate)
                {
                    if (cycleDate >= historyStart)
                    {
                        var periodLabel = frequencyMonths == 1
                            ? cycleDate.ToString("yyyy/MM")
                            : $"{cycleDate:yyyy/MM} - {(cycleDate.AddMonths(frequencyMonths - 1)):yyyy/MM}";

                        var receivedDate = cycleDate.AddDays(-((i + sequence) % 3));
                        if (receivedDate > today)
                            receivedDate = cycleDate;

                        payments.Add(new KafalaPayment
                        {
                            Id = Guid.NewGuid(),
                            KafalaCaseId = caseEntity.Id,
                            SponsorId = sponsor.Id,
                            PaymentDate = receivedDate,
                            Amount = installmentAmount,
                            PeriodLabel = periodLabel,
                            Direction = "Received",
                            PaymentMethodId = caseEntity.PaymentMethodId,
                            FinancialAccountId = caseEntity.FinancialAccountId,
                            ReferenceNumber = $"KRP-REC-{i + 1:000}-{receivedDate:yyyyMM}-{sequence:00}",
                            Status = sequence % 11 == 0 ? "Draft" : "Confirmed",
                            Notes = "تحصيل دوري من الكفيل",
                            CreatedByUserId = createdByUserId,
                            CreatedAtUtc = receivedDate.AddHours(10)
                        });
                        lastReceived = receivedDate;

                        var shouldDisburse = sequence % 7 != 0 || status == "Closed";
                        if (shouldDisburse)
                        {
                            var disbursementDate = cycleDate.AddDays(2 + ((i + sequence) % 4));
                            if (disbursementDate > today)
                                disbursementDate = cycleDate;

                            payments.Add(new KafalaPayment
                            {
                                Id = Guid.NewGuid(),
                                KafalaCaseId = caseEntity.Id,
                                SponsorId = sponsor.Id,
                                PaymentDate = disbursementDate,
                                Amount = installmentAmount,
                                PeriodLabel = periodLabel,
                                Direction = "Disbursed",
                                PaymentMethodId = caseEntity.PaymentMethodId,
                                FinancialAccountId = caseEntity.FinancialAccountId,
                                ReferenceNumber = $"KRP-DIS-{i + 1:000}-{disbursementDate:yyyyMM}-{sequence:00}",
                                Status = sequence % 9 == 0 ? "Draft" : "Confirmed",
                                Notes = "صرف دوري للمكفول",
                                CreatedByUserId = createdByUserId,
                                CreatedAtUtc = disbursementDate.AddHours(13)
                            });
                            lastDisbursed = disbursementDate;
                        }

                        if (sequence % 8 == 0)
                        {
                            var adjustmentDate = cycleDate.AddDays(5 + (sequence % 3));
                            if (adjustmentDate > today)
                                adjustmentDate = today;

                            payments.Add(new KafalaPayment
                            {
                                Id = Guid.NewGuid(),
                                KafalaCaseId = caseEntity.Id,
                                SponsorId = sponsor.Id,
                                PaymentDate = adjustmentDate,
                                Amount = Math.Round(installmentAmount * 0.10m, 2),
                                PeriodLabel = periodLabel,
                                Direction = "Adjustment",
                                PaymentMethodId = caseEntity.PaymentMethodId,
                                FinancialAccountId = caseEntity.FinancialAccountId,
                                ReferenceNumber = $"KRP-ADJ-{i + 1:000}-{adjustmentDate:yyyyMM}-{sequence:00}",
                                Status = "Confirmed",
                                Notes = sequence % 16 == 0 ? "تسوية فرق تحويل" : "تسوية مصروف إداري بسيط",
                                CreatedByUserId = createdByUserId,
                                CreatedAtUtc = adjustmentDate.AddHours(15)
                            });
                        }
                    }

                    cycleDate = cycleDate.AddMonths(frequencyMonths);
                    sequence++;
                }

                caseEntity.LastCollectionDate = lastReceived;
                caseEntity.LastDisbursementDate = lastDisbursed;
                caseEntity.NextDueDate = status == "Closed"
                    ? null
                    : (lastReceived ?? startDate).AddMonths(frequencyMonths);

                cases.Add(caseEntity);
            }

            await db.Set<KafalaSponsor>().AddRangeAsync(sponsors);
            await db.Set<KafalaCase>().AddRangeAsync(cases);
            await db.Set<KafalaPayment>().AddRangeAsync(payments);
        }

        private static int GetFrequencyMonths(string frequency) => frequency switch
        {
            "Quarterly" => 3,
            "SemiAnnual" => 6,
            "Annual" => 12,
            _ => 1
        };

        private static decimal GetKafalaInstallmentAmount(string sponsorshipType, int frequencyMonths, int index)
        {
            var monthlyBase = sponsorshipType switch
            {
                "Orphan" => 650m + ((index % 6) * 75m),
                "Family" => 900m + ((index % 5) * 100m),
                "Education" => 500m + ((index % 4) * 60m),
                "Health" => 700m + ((index % 5) * 90m),
                _ => 550m + ((index % 6) * 70m)
            };

            return Math.Round(monthlyBase * frequencyMonths, 2);
        }

        private static async Task<int> SeedHumanResearchAsync(AppDbContext db, string? createdByUserId)
        {
            if (!await db.Set<Beneficiary>().AnyAsync(x => x.Code.StartsWith("BEN-DEMO-")))
                return 0;

            if (await db.Set<BeneficiaryHumanitarianResearch>().AnyAsync(x => x.ResearchNumber.StartsWith("RES-DEMO-")))
                return 0;

            var beneficiaries = await db.Set<Beneficiary>()
                .Where(x => x.Code.StartsWith("BEN-DEMO-"))
                .OrderBy(x => x.Code)
                .Take(DemoHumanResearchCount)
                .ToListAsync();

            var cashAid = await ResolveAidTypeAsync(db, "نقدي");
            var foodAid = await ResolveAidTypeAsync(db, "غذائي");
            var medicalAid = await ResolveAidTypeAsync(db, "علاجي");
            var educationalAid = await ResolveAidTypeAsync(db, "تعليمي");
            var aidTypes = new[] { cashAid, foodAid, medicalAid, educationalAid };

            var forms = new List<BeneficiaryHumanitarianResearch>();
            var reviews = new List<BeneficiaryHumanitarianResearchReview>();
            var committee = new List<BeneficiaryHumanitarianResearchCommitteeEvaluation>();
            var statuses = new[] { "SubmittedForReview", "ReviewedApproved", "ReturnedForRevision", "ReviewedRejected", "SentToCommittee", "CommitteeDecided" };

            for (var i = 0; i < beneficiaries.Count; i++)
            {
                var ben = beneficiaries[i];
                var aidType = aidTypes[i % aidTypes.Length];
                var status = statuses[i % statuses.Length];

                var form = new BeneficiaryHumanitarianResearch
                {
                    Id = Guid.NewGuid(),
                    BeneficiaryId = ben.Id,
                    ResearchNumber = $"RES-DEMO-{i + 1:0000}",
                    RequestDate = DateTime.Today.AddDays(-(50 + (i % 90))),
                    ResearchDate = DateTime.Today.AddDays(-(45 + (i % 80))),
                    AidTypeName = GetAidTypeDisplayName(aidType),
                    ApplicantName = ben.FullName,
                    SourceOfRequest = i % 4 == 0 ? "باحث ميداني" : i % 4 == 1 ? "لجنة المساعدات" : i % 4 == 2 ? "تحويل داخلي" : "منصة إلكترونية",
                    ResearcherCode = $"R-{(i % 35) + 1:000}",
                    ResearcherName = $"باحث/{(i % 2 == 0 ? "أحمد" : "منى")} {FamilyNames[i % FamilyNames.Length]}",
                    CommitteeCode = $"COM-{DateTime.Today.Year}-{(i % 20) + 1:00}",
                    PriorityLevel = i % 5 == 0 ? "عاجل" : i % 2 == 0 ? "متوسط" : "عادي",
                    FullName = ben.FullName,
                    NickName = ben.FullName.Split(' ').FirstOrDefault(),
                    Age = ben.BirthDate.HasValue ? DateTime.Today.Year - ben.BirthDate.Value.Year : null,
                    MaritalStatus = i % 3 == 0 ? "متزوج" : i % 4 == 0 ? "أرمل" : "أعزب",
                    NationalId = ben.NationalId,
                    AddressLine = ben.AddressLine,
                    Phone1 = ben.PhoneNumber,
                    Phone2 = ben.AlternatePhoneNumber,
                    FamilyMembersCount = ben.FamilyMembersCount,
                    TotalIncome = ben.MonthlyIncome,
                    TotalExpenses = (ben.MonthlyIncome ?? 0) + 750,
                    AverageIncome = ben.MonthlyIncome,
                    HasExistingProject = i % 19 == 0,
                    ExistingProjectType = i % 19 == 0 ? "مشروع منزلي" : null,
                    ExistingProjectSize = i % 19 == 0 ? "صغير" : null,
                    RequiredNeedsPrimary = aidType.Id == cashAid.Id ? "مساعدة مالية شهرية" : aidType.Id == foodAid.Id ? "دعم غذائي" : aidType.Id == medicalAid.Id ? "علاج وأدوية" : "مصروفات تعليمية",
                    RequiredNeedsSecondary = i % 4 == 0 ? "متابعة اجتماعية" : "زيارة دورية",
                    HousingDescription = "سكن بسيط ضمن بيانات الديمو الموسعة",
                    ResearcherReport = "تبين من المعاينة أن الحالة تستحق العرض على اللجنة وفق المعايير المعتمدة.",
                    ResearchManagerOpinion = "يوصى باستكمال المسار وفق نتيجة المراجعة.",
                    Status = status,
                    SubmittedAtUtc = DateTime.UtcNow.AddDays(-(35 - (i % 15))),
                    SubmittedByUserId = createdByUserId,
                    ReviewedAtUtc = status is "ReviewedApproved" or "ReviewedRejected" or "ReturnedForRevision" or "SentToCommittee" or "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(18 - (i % 7))) : null,
                    ReviewedByUserId = createdByUserId,
                    ReviewDecision = status switch
                    {
                        "ReviewedApproved" => "Approve",
                        "ReviewedRejected" => "Reject",
                        "ReturnedForRevision" => "ReturnForRevision",
                        "SentToCommittee" => "Approve",
                        "CommitteeDecided" => "Approve",
                        _ => null
                    },
                    ReviewReason = status == "ReviewedRejected" ? "مرفقات غير كافية" : status == "ReturnedForRevision" ? "يلزم استكمال بيانات الأسرة" : null,
                    CommitteeSentAtUtc = status is "SentToCommittee" or "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(8 - (i % 5))) : null,
                    CommitteeSentByUserId = status is "SentToCommittee" or "CommitteeDecided" ? createdByUserId : null,
                    CommitteeDecisionAtUtc = status == "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(3 - (i % 2))) : null,
                    CommitteeDecisionByUserId = status == "CommitteeDecided" ? createdByUserId : null,
                    CommitteeDecision = status == "CommitteeDecided" ? "ApproveAid" : null,
                    CommitteeDecisionNotes = status == "CommitteeDecided" ? "اعتماد نهائي ضمن داتا المحاكاة" : null
                };
                forms.Add(form);

                if (form.ReviewedAtUtc.HasValue)
                {
                    reviews.Add(new BeneficiaryHumanitarianResearchReview
                    {
                        Id = Guid.NewGuid(),
                        ResearchId = form.Id,
                        ReviewDate = form.ReviewedAtUtc.Value,
                        Decision = form.ReviewDecision ?? "Approve",
                        ReviewerNotes = form.ReviewReason ?? "تمت المراجعة بنجاح",
                        ReviewedByUserId = createdByUserId
                    });
                }

                if (status == "CommitteeDecided")
                {
                    committee.Add(new BeneficiaryHumanitarianResearchCommitteeEvaluation
                    {
                        Id = Guid.NewGuid(),
                        ResearchId = form.Id,
                        CommitteeMeetingDate = DateTime.Today.AddDays(-(2 + (i % 6))),
                        Decision = "ApproveAid",
                        ApprovedAidType = GetAidTypeDisplayName(aidType),
                        ApprovedAmount = aidType.Id == foodAid.Id ? 0 : 1200 + ((i % 8) * 175),
                        DurationMonths = i % 4 == 0 ? 6 : 3,
                        Notes = "اعتماد الحالة ضمن بيانات العرض الموسعة",
                        ApprovedByUserId = createdByUserId
                    });
                }
            }

            await db.Set<BeneficiaryHumanitarianResearch>().AddRangeAsync(forms);
            await db.Set<BeneficiaryHumanitarianResearchReview>().AddRangeAsync(reviews);
            await db.Set<BeneficiaryHumanitarianResearchCommitteeEvaluation>().AddRangeAsync(committee);
            return forms.Count;
        }

        private static async Task<int> SeedDonorsAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<Donor>().AnyAsync(x => x.Code.StartsWith("DON-DEMO-")))
                return 0;

            var gov = await db.Set<Governorate>().FirstAsync();
            var city = await db.Set<City>().FirstAsync();
            var area = await db.Set<Area>().FirstAsync();

            var donors = new List<Donor>();
            for (var i = 1; i <= DemoDonorCount; i++)
            {
                donors.Add(new Donor
                {
                    Id = Guid.NewGuid(),
                    Code = $"DON-DEMO-{i:000}",
                    DonorType = i <= 4 ? "Individual" : "Organization",
                    FullName = i <= 4 ? BuildName(true) : $"مؤسسة الخير {i}",
                    ContactPerson = i <= 4 ? null : $"أ/ {BuildName(true)}",
                    NationalIdOrTaxNo = $"TAX-{i:0000}",
                    PhoneNumber = $"0111{Rng.Next(1000000, 9999999)}",
                    Email = $"donor{i}@demo.local",
                    AddressLine = $"{gov.NameAr} - {city.NameAr}",
                    GovernorateId = gov.Id,
                    CityId = city.Id,
                    AreaId = area.Id,
                    PreferredCommunicationMethod = "Phone",
                    Notes = "بيانات متبرع ديمو",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-Rng.Next(10, 120)),
                    UpdatedAtUtc = null
                });
            }

            await db.Set<Donor>().AddRangeAsync(donors);
            return donors.Count;
        }

        private static async Task<int> SeedDonationsAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<Donation>().AnyAsync(x => x.DonationNumber.StartsWith("DN-DEMO-")))
                return 0;

            var donors = await db.Set<Donor>().Where(x => x.Code.StartsWith("DON-DEMO-")).OrderBy(x => x.Code).ToListAsync();
            if (!donors.Any()) return 0;

            var pm = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "CASH");
            var account = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            var aidCash = await ResolveAidTypeAsync(db, "نقدي");
            var aidMedical = await ResolveAidTypeAsync(db, "علاجي");
            var aidEducational = await ResolveAidTypeAsync(db, "تعليمي");
            var aidSponsorship = await ResolveAidTypeAsync(db, "كفالة");
            var aidFood = await ResolveAidTypeAsync(db, "غذائي");
            var aidClothes = await ResolveAidTypeAsync(db, "ملابس");
            var aidDevices = await ResolveAidTypeAsync(db, "أجهزة");

            var cashAidTypes = new[] { aidCash, aidMedical, aidEducational, aidSponsorship };
            var inKindAidTypes = new[] { aidFood, aidClothes, aidDevices };

            var startMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(DemoDonationMonths - 1));
            var donations = new List<Donation>();
            var seq = 1;

            for (var monthOffset = 0; monthOffset < DemoDonationMonths; monthOffset++)
            {
                var monthDate = startMonth.AddMonths(monthOffset);
                var cashCount = 6 + (monthOffset % 4);

                for (var slot = 1; slot <= cashCount; slot++)
                {
                    var day = Math.Min(3 + (slot * 3) + (monthOffset % 5), DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
                    var donationDate = new DateTime(monthDate.Year, monthDate.Month, day);
                    var amount = 2200m + (monthOffset * 160m) + (slot * 320m) + ((monthOffset % 6) * 110m);
                    var aidType = cashAidTypes[(seq - 1) % cashAidTypes.Length];

                    donations.Add(new Donation
                    {
                        Id = Guid.NewGuid(),
                        DonationNumber = $"DN-DEMO-{seq:0000}",
                        DonorId = donors[(seq - 1) % donors.Count].Id,
                        DonationDate = donationDate,
                        DonationType = "نقدي",
                        AidTypeId = aidType.Id,
                        Amount = Math.Round(amount, 2),
                        PaymentMethodId = pm.Id,
                        FinancialAccountId = account?.Id,
                        IsRestricted = (monthOffset + slot) % 5 == 0,
                        CampaignName = monthDate.Month switch
                        {
                            3 => "حملة رمضان",
                            4 => "زكاة الفطر",
                            9 => "العودة للدراسة",
                            12 => "الشتاء الدافئ",
                            _ => slot == cashCount ? "صدقة جارية" : null
                        },
                        ReceiptNumber = $"REC-{seq:0000}",
                        ReferenceNumber = $"REF-{donationDate:yyyyMM}-{seq:0000}",
                        Notes = "تبرع نقدي تجريبي متوافق مع منطق التخصيص على طلب المساعدة",
                        CreatedByUserId = createdByUserId,
                        CreatedAtUtc = donationDate.AddHours(10)
                    });
                    seq++;
                }

                var inKindCount = 1 + (monthOffset % 2);
                for (var slot = 1; slot <= inKindCount; slot++)
                {
                    var inKindDay = Math.Min(18 + (slot * 4), DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
                    var inKindDate = new DateTime(monthDate.Year, monthDate.Month, inKindDay);
                    var aidType = inKindAidTypes[(seq - 1) % inKindAidTypes.Length];
                    donations.Add(new Donation
                    {
                        Id = Guid.NewGuid(),
                        DonationNumber = $"DN-DEMO-{seq:0000}",
                        DonorId = donors[(seq - 1) % donors.Count].Id,
                        DonationDate = inKindDate,
                        DonationType = "عيني",
                        AidTypeId = aidType.Id,
                        Amount = 0,
                        PaymentMethodId = null,
                        FinancialAccountId = null,
                        IsRestricted = slot % 3 == 0,
                        CampaignName = aidType.Id == aidFood.Id ? "سلال غذائية" : aidType.Id == aidClothes.Id ? "كسوة موسمية" : "أجهزة ومستلزمات",
                        ReceiptNumber = $"REC-{seq:0000}",
                        ReferenceNumber = $"REF-{inKindDate:yyyyMM}-{seq:0000}",
                        Notes = "تبرع عيني تجريبي تمهيدًا لإضافة الأصناف وإذن الإضافة",
                        CreatedByUserId = createdByUserId,
                        CreatedAtUtc = inKindDate.AddHours(12)
                    });
                    seq++;
                }
            }

            await db.Set<Donation>().AddRangeAsync(donations);

            var approvedRequests = await db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Where(x => x.Status == "Approved")
                .OrderBy(x => x.RequestDate)
                .ToListAsync();

            if (approvedRequests.Any())
            {
                var requestFunding = approvedRequests.ToDictionary(x => x.Id, _ => 0m);
                var allocations = new List<DonationAllocation>();
                var disbursements = new List<BeneficiaryAidDisbursement>();
                var fundingLines = new List<BeneficiaryAidDisbursementFundingLine>();
                var approvedCashDonations = donations.Where(x => x.DonationType == "نقدي" && (x.Amount ?? 0) > 0).ToList();
                var allocSequence = 0;
                var disbSequence = 0;

                foreach (var donation in approvedCashDonations)
                {
                    var donationRemaining = donation.Amount ?? 0m;
                    var matchingRequests = approvedRequests
                        .Where(x => x.AidTypeId == donation.AidTypeId)
                        .Where(x => (x.RequestedAmount ?? 0m) > requestFunding[x.Id])
                        .Take(6)
                        .ToList();

                    foreach (var request in matchingRequests)
                    {
                        if (donationRemaining <= 0m)
                            break;

                        var requestRemaining = Math.Max(0m, (request.RequestedAmount ?? 0m) - requestFunding[request.Id]);
                        if (requestRemaining <= 0m)
                            continue;

                        var proposalAmount = Math.Min(donationRemaining, Math.Min(requestRemaining, 1200m + ((allocSequence % 5) * 250m)));
                        if (proposalAmount <= 0m)
                            continue;

                        allocSequence++;
                        var approvalStatus = allocSequence % 9 == 0 ? "Pending" : allocSequence % 17 == 0 ? "Rejected" : "Approved";
                        var allocation = new DonationAllocation
                        {
                            Id = Guid.NewGuid(),
                            DonationId = donation.Id,
                            AidRequestId = request.Id,
                            BeneficiaryId = request.BeneficiaryId,
                            AllocatedDate = donation.DonationDate.AddDays(1 + (allocSequence % 5)),
                            Amount = Math.Round(proposalAmount, 2),
                            ApprovalStatus = approvalStatus,
                            ApprovedAtUtc = approvalStatus == "Approved" ? donation.DonationDate.AddDays(2 + (allocSequence % 3)).AddHours(11) : null,
                            ApprovedByUserId = approvalStatus == "Approved" ? createdByUserId : null,
                            RejectedAtUtc = approvalStatus == "Rejected" ? donation.DonationDate.AddDays(2).AddHours(15) : null,
                            RejectedByUserId = approvalStatus == "Rejected" ? createdByUserId : null,
                            ApprovalNotes = approvalStatus == "Pending" ? "بانتظار اعتماد التخصيص ضمن الداتا التجريبية" : approvalStatus == "Rejected" ? "تم رفض هذا التخصيص لأغراض المحاكاة" : "تخصيص معتمد تجريبي",
                            Notes = "تخصيص ديمو على طلب مساعدة",
                            CreatedByUserId = createdByUserId,
                            CreatedAtUtc = donation.DonationDate.AddHours(14)
                        };
                        allocations.Add(allocation);

                        if (approvalStatus != "Rejected")
                        {
                            donationRemaining -= proposalAmount;
                            requestFunding[request.Id] += proposalAmount;
                        }

                        if (approvalStatus == "Approved" && disbSequence < 900)
                        {
                            var disbursedAmount = Math.Round(Math.Min(proposalAmount, proposalAmount * (allocSequence % 4 == 0 ? 1m : 0.65m)), 2);
                            if (disbursedAmount > 0)
                            {
                                disbSequence++;
                                var disbursement = new BeneficiaryAidDisbursement
                                {
                                    Id = Guid.NewGuid(),
                                    BeneficiaryId = request.BeneficiaryId,
                                    AidRequestId = request.Id,
                                    AidTypeId = request.AidTypeId,
                                    DisbursementDate = allocation.AllocatedDate.AddDays(1 + (disbSequence % 4)),
                                    Amount = disbursedAmount,
                                    PaymentMethodId = pm.Id,
                                    FinancialAccountId = account?.Id,
                                    DonationId = donation.Id,
                                    Notes = "صرف تجريبي مبني على تخصيص معتمد",
                                    ApprovedByUserId = createdByUserId,
                                    CreatedByUserId = createdByUserId
                                };
                                disbursements.Add(disbursement);
                                fundingLines.Add(new BeneficiaryAidDisbursementFundingLine
                                {
                                    Id = Guid.NewGuid(),
                                    DisbursementId = disbursement.Id,
                                    DonationAllocationId = allocation.Id,
                                    AmountConsumed = disbursedAmount,
                                    CreatedByUserId = createdByUserId,
                                    CreatedAtUtc = disbursement.DisbursementDate.AddHours(13)
                                });
                            }
                        }
                    }
                }

                await db.Set<DonationAllocation>().AddRangeAsync(allocations);
                await db.Set<BeneficiaryAidDisbursement>().AddRangeAsync(disbursements);
                await db.Set<BeneficiaryAidDisbursementFundingLine>().AddRangeAsync(fundingLines);
            }

            return donations.Count;
        }

        private static async Task<int> SeedProjectsAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<CharityProject>().AnyAsync(x => x.Code.StartsWith("PRJ-DEMO-")))
                return 0;

            var beneficiaries = await db.Set<Beneficiary>().Where(x => x.Code.StartsWith("BEN-DEMO-")).ToListAsync();
            var funder = await EnsureDemoFunderAsync(db);
            var grant = await EnsureDemoGrantAgreementAsync(db, funder.Id);

            var projects = new List<CharityProject>();
            var budgetLines = new List<ProjectBudgetLine>();
            var activities = new List<ProjectActivity>();
            var projBenefs = new List<ProjectBeneficiary>();
            var projGrants = new List<ProjectGrant>();
            var phases = new List<ProjectPhase>();
            var milestones = new List<ProjectPhaseMilestone>();
            var trackingLogs = new List<ProjectTrackingLog>();

            for (var i = 1; i <= 6; i++)
            {
                var project = new CharityProject
                {
                    Id = Guid.NewGuid(),
                    Code = $"PRJ-DEMO-{i:000}",
                    Name = i switch
                    {
                        1 => "مشروع دعم الأسر الأكثر احتياجًا",
                        2 => "مشروع الرعاية الصحية المجتمعية",
                        3 => "مشروع التدريب والتأهيل للشباب",
                        _ => "مشروع تنمية القرى المستهدفة"
                    },
                    Description = "مشروع تجريبي لعرض إمكانيات النظام في إدارة المشروعات والمتابعة.",
                    StartDate = DateTime.Today.AddMonths(-Rng.Next(1, 5)),
                    EndDate = DateTime.Today.AddMonths(Rng.Next(4, 10)),
                    Budget = 150000 + (i * 50000),
                    Status = i == 4 ? "Planned" : "Active",
                    TargetBeneficiariesCount = 50 * i,
                    Location = i <= 2 ? "سوهاج" : "أسيوط",
                    Objectives = "تحسين الوصول للخدمات الاجتماعية ورفع كفاءة المتابعة.",
                    Kpis = "عدد المستفيدين - نسبة الإنجاز - الالتزام الزمني",
                    Notes = "بيانات مشروع ديمو",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-Rng.Next(20, 120))
                };
                projects.Add(project);

                budgetLines.Add(new ProjectBudgetLine { Id = Guid.NewGuid(), ProjectId = project.Id, LineName = "أنشطة ميدانية", LineType = "Operations", PlannedAmount = project.Budget * 0.40m, ActualAmount = project.Budget * 0.18m, Notes = "بند تجريبي" });
                budgetLines.Add(new ProjectBudgetLine { Id = Guid.NewGuid(), ProjectId = project.Id, LineName = "دعم مباشر", LineType = "Aid", PlannedAmount = project.Budget * 0.45m, ActualAmount = project.Budget * 0.22m, Notes = "بند تجريبي" });
                budgetLines.Add(new ProjectBudgetLine { Id = Guid.NewGuid(), ProjectId = project.Id, LineName = "متابعة وتقييم", LineType = "M&E", PlannedAmount = project.Budget * 0.15m, ActualAmount = project.Budget * 0.06m, Notes = "بند تجريبي" });

                activities.Add(new ProjectActivity
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    Title = "حصر المستفيدين وتحديث البيانات",
                    Description = "نشاط ميداني لتحديث قواعد البيانات.",
                    PlannedDate = project.StartDate.AddDays(10),
                    ActualDate = i == 4 ? null : project.StartDate.AddDays(12),
                    Status = i == 4 ? "Planned" : "Completed",
                    PlannedCost = 10000,
                    ActualCost = i == 4 ? 0 : 9500,
                    Notes = "نشاط تجريبي"
                });
                activities.Add(new ProjectActivity
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    Title = "تنفيذ جلسات التوعية والمتابعة",
                    Description = "جلسات توعوية وتقارير ميدانية.",
                    PlannedDate = project.StartDate.AddDays(35),
                    ActualDate = i == 4 ? null : project.StartDate.AddDays(38),
                    Status = i == 4 ? "Planned" : "InProgress",
                    PlannedCost = 18000,
                    ActualCost = i == 4 ? 0 : 12000,
                    Notes = "نشاط تجريبي"
                });

                foreach (var ben in beneficiaries.Skip((i - 1) * 3).Take(4))
                {
                    projBenefs.Add(new ProjectBeneficiary
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        BeneficiaryId = ben.Id,
                        EnrollmentDate = project.StartDate.AddDays(Rng.Next(1, 25)),
                        BenefitType = i % 2 == 0 ? "تدريب" : "دعم مباشر",
                        Notes = "ربط مستفيد تجريبي"
                    });
                }

                projGrants.Add(new ProjectGrant
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    GrantAgreementId = grant.Id,
                    AllocatedAmount = project.Budget * 0.70m,
                    AllocatedDate = project.StartDate,
                    Notes = "ربط منحة تجريبي"
                });

                for (var p = 1; p <= 2; p++)
                {
                    var phase = new ProjectPhase
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        Code = $"PH-{i}-{p}",
                        Name = p == 1 ? "مرحلة التجهيز" : "مرحلة التنفيذ",
                        Description = "مرحلة تجريبية لعرض التقسيم المرحلي.",
                        SortOrder = p,
                        PlannedStartDate = project.StartDate.AddDays((p - 1) * 45),
                        PlannedEndDate = project.StartDate.AddDays((p * 45) - 1),
                        ActualStartDate = p == 1 ? project.StartDate.AddDays(1) : null,
                        ActualEndDate = p == 1 && i <= 2 ? project.StartDate.AddDays(40) : null,
                        Status = p == 1 ? "Completed" : "InProgress",
                        ProgressPercent = p == 1 ? 100 : 55,
                        PlannedCost = project.Budget * 0.5m,
                        ActualCost = project.Budget * (p == 1 ? 0.42m : 0.17m),
                        ResponsiblePersonName = "مدير مشروع",
                        Notes = "مرحلة تجريبية",
                        IsActive = true
                    };
                    phases.Add(phase);

                    milestones.Add(new ProjectPhaseMilestone
                    {
                        Id = Guid.NewGuid(),
                        ProjectPhaseId = phase.Id,
                        Title = p == 1 ? "اعتماد خطة العمل" : "بدء التشغيل الميداني",
                        Description = "Milestone تجريبية",
                        DueDate = phase.PlannedStartDate.AddDays(15),
                        CompletedDate = p == 1 ? phase.PlannedStartDate.AddDays(16) : null,
                        Status = p == 1 ? "Completed" : "Pending",
                        ProgressPercent = p == 1 ? 100 : 40,
                        Notes = "بيانات عرض"
                    });

                    trackingLogs.Add(new ProjectTrackingLog
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        ProjectPhaseId = phase.Id,
                        EntryDate = DateTime.Today.AddDays(-Rng.Next(2, 20)),
                        EntryType = "Update",
                        Title = $"متابعة {phase.Name}",
                        Details = "تم تحديث تقدم المرحلة ضمن بيانات العرض.",
                        Status = phase.Status,
                        ProgressPercent = phase.ProgressPercent,
                        RequiresAttention = phase.ProgressPercent < 60,
                        CreatedByUserId = createdByUserId
                    });
                }
            }

            await db.Set<CharityProject>().AddRangeAsync(projects);
            await db.Set<ProjectBudgetLine>().AddRangeAsync(budgetLines);
            await db.Set<ProjectActivity>().AddRangeAsync(activities);
            await db.Set<ProjectBeneficiary>().AddRangeAsync(projBenefs);
            await db.Set<ProjectGrant>().AddRangeAsync(projGrants);
            await db.Set<ProjectPhase>().AddRangeAsync(phases);
            await db.Set<ProjectPhaseMilestone>().AddRangeAsync(milestones);
            await db.Set<ProjectTrackingLog>().AddRangeAsync(trackingLogs);
            return projects.Count;
        }

        private static async Task<StockSeedResult> SeedStockAsync(AppDbContext db, string? createdByUserId)
        {
            var result = new StockSeedResult();

            var warehouses = await EnsureDemoWarehousesAsync(db);
            result.Warehouses = warehouses.Count(x => db.Entry(x).State == EntityState.Added);

            var items = await EnsureDemoStockItemsAsync(db);
            result.Items = items.Count(x => db.Entry(x).State == EntityState.Added);

            if (!warehouses.Any() || !items.Any())
                return result;

            if (await db.Set<CharityStoreReceipt>().AnyAsync(x => x.ReceiptNumber.StartsWith("SR-DEMO-"))
                || await db.Set<CharityStoreIssue>().AnyAsync(x => x.IssueNumber.StartsWith("SI-DEMO-"))
                || await db.Set<StockNeedRequest>().AnyAsync(x => x.RequestNumber.StartsWith("NR-DEMO-"))
                || await db.Set<StockReturnVoucher>().AnyAsync(x => x.VoucherNumber.StartsWith("RT-DEMO-"))
                || await db.Set<StockDisposalVoucher>().AnyAsync(x => x.VoucherNumber.StartsWith("DS-DEMO-")))
            {
                return result;
            }

            var beneficiaries = await db.Set<Beneficiary>()
                .Where(x => x.Code.StartsWith("BEN-DEMO-"))
                .OrderBy(x => x.Code)
                .Take(12)
                .ToListAsync();
            var projects = await db.Set<CharityProject>()
                .Where(x => x.Code.StartsWith("PRJ-DEMO-"))
                .OrderBy(x => x.Code)
                .Take(6)
                .ToListAsync();

            var warehouseByCode = warehouses.ToDictionary(x => x.WarehouseCode, x => x);
            var itemByCode = items.ToDictionary(x => x.ItemCode, x => x);

            var mainWarehouse = warehouseByCode["WH-DEMO-001"];
            var projectWarehouse = warehouseByCode["WH-DEMO-002"];
            var campaignWarehouse = warehouseByCode["WH-DEMO-003"];

            var balances = new Dictionary<(Guid ItemId, Guid WarehouseId), decimal>();
            var touchedKeys = new HashSet<(Guid ItemId, Guid WarehouseId)>();
            var transactions = new List<StockTransaction>();
            var receipts = new List<CharityStoreReceipt>();
            var issues = new List<CharityStoreIssue>();
            var needRequests = new List<StockNeedRequest>();
            var returnVouchers = new List<StockReturnVoucher>();
            var disposalVouchers = new List<StockDisposalVoucher>();

            decimal GetBalance(Guid itemId, Guid warehouseId)
                => balances.TryGetValue((itemId, warehouseId), out var qty) ? qty : 0m;

            void Increase(Guid itemId, Guid warehouseId, decimal quantity, decimal unitCost, DateTime trxDate, StockTransactionType type, string referenceType, string referenceNumber, string? notes, Guid? relatedWarehouseId = null)
            {
                var key = (itemId, warehouseId);
                balances[key] = GetBalance(itemId, warehouseId) + quantity;
                touchedKeys.Add(key);
                transactions.Add(new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    WarehouseId = warehouseId,
                    RelatedWarehouseId = relatedWarehouseId,
                    TransactionType = type,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TransactionDateUtc = trxDate,
                    ReferenceType = referenceType,
                    ReferenceNumber = referenceNumber,
                    Notes = notes,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            void Decrease(Guid itemId, Guid warehouseId, decimal quantity, decimal unitCost, DateTime trxDate, StockTransactionType type, string referenceType, string referenceNumber, string? notes, Guid? relatedWarehouseId = null)
            {
                var key = (itemId, warehouseId);
                var current = GetBalance(itemId, warehouseId);
                if (current < quantity)
                    throw new InvalidOperationException($"الرصيد غير كافٍ للصنف {itemId} في المخزن {warehouseId} أثناء Seed الديمو.");

                balances[key] = current - quantity;
                touchedKeys.Add(key);
                transactions.Add(new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ItemId = itemId,
                    WarehouseId = warehouseId,
                    RelatedWarehouseId = relatedWarehouseId,
                    TransactionType = type,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TransactionDateUtc = trxDate,
                    ReferenceType = referenceType,
                    ReferenceNumber = referenceNumber,
                    Notes = notes,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            void Transfer(Guid itemId, Guid fromWarehouseId, Guid toWarehouseId, decimal quantity, decimal unitCost, DateTime trxDate, string referenceNumber, string? notes)
            {
                Decrease(itemId, fromWarehouseId, quantity, unitCost, trxDate, StockTransactionType.TransferOut, "StockTransfer", referenceNumber, notes, toWarehouseId);
                Increase(itemId, toWarehouseId, quantity, unitCost, trxDate, StockTransactionType.TransferIn, "StockTransfer", referenceNumber, notes, fromWarehouseId);
            }

            CharityStoreReceipt AddReceipt(string number, Guid warehouseId, DateTime receiptDate, string sourceType, string sourceName, params (string ItemCode, decimal Quantity, decimal UnitCost, string BatchNo)[] lines)
            {
                var receipt = new CharityStoreReceipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptNumber = number,
                    WarehouseId = warehouseId,
                    ReceiptDate = receiptDate,
                    SourceType = sourceType,
                    SourceName = sourceName,
                    Notes = "إذن إضافة مخزني تجريبي",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-5),
                    CreatedByUserId = createdByUserId
                };

                foreach (var line in lines)
                {
                    var item = itemByCode[line.ItemCode];
                    receipt.Lines.Add(new CharityStoreReceiptLine
                    {
                        Id = Guid.NewGuid(),
                        ReceiptId = receipt.Id,
                        ItemId = item.Id,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        BatchNo = line.BatchNo,
                        Notes = "إضافة ضمن بيانات الديمو"
                    });

                    Increase(item.Id, warehouseId, line.Quantity, line.UnitCost, receiptDate, StockTransactionType.Purchase, "CharityStoreReceipt", number, receipt.Notes);
                }

                receipts.Add(receipt);
                return receipt;
            }

            CharityStoreIssue AddIssue(string number, Guid warehouseId, DateTime issueDate, string issueType, Guid? beneficiaryId, Guid? projectId, string issuedToName, params (string ItemCode, decimal Quantity, decimal UnitCost)[] lines)
            {
                var issue = new CharityStoreIssue
                {
                    Id = Guid.NewGuid(),
                    IssueNumber = number,
                    WarehouseId = warehouseId,
                    IssueDate = issueDate,
                    IssueType = issueType,
                    BeneficiaryId = beneficiaryId,
                    ProjectId = projectId,
                    IssuedToName = issuedToName,
                    Notes = "إذن صرف مخزني تجريبي",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-4),
                    CreatedByUserId = createdByUserId
                };

                foreach (var line in lines)
                {
                    var item = itemByCode[line.ItemCode];
                    issue.Lines.Add(new CharityStoreIssueLine
                    {
                        Id = Guid.NewGuid(),
                        IssueId = issue.Id,
                        ItemId = item.Id,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        Notes = "صرف ضمن بيانات الديمو"
                    });

                    Decrease(item.Id, warehouseId, line.Quantity, line.UnitCost, issueDate, StockTransactionType.Sale, "CharityStoreIssue", number, issue.Notes);
                }

                issues.Add(issue);
                return issue;
            }

            StockNeedRequest AddNeedRequest(string number, DateTime requestDate, string requestType, Guid? projectId, Guid? beneficiaryId, string requestedByName, string status, params (string ItemCode, decimal Requested, decimal Approved, decimal Fulfilled)[] lines)
            {
                var request = new StockNeedRequest
                {
                    Id = Guid.NewGuid(),
                    RequestNumber = number,
                    RequestDate = requestDate,
                    RequestType = requestType,
                    ProjectId = projectId,
                    BeneficiaryId = beneficiaryId,
                    RequestedByName = requestedByName,
                    Status = status,
                    Notes = "طلب احتياج تجريبي",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
                };

                foreach (var line in lines)
                {
                    var item = itemByCode[line.ItemCode];
                    request.Lines.Add(new StockNeedRequestLine
                    {
                        Id = Guid.NewGuid(),
                        StockNeedRequestId = request.Id,
                        ItemId = item.Id,
                        RequestedQuantity = line.Requested,
                        ApprovedQuantity = line.Approved,
                        FulfilledQuantity = line.Fulfilled,
                        Notes = "احتياج تشغيلي ضمن الديمو"
                    });
                }

                needRequests.Add(request);
                return request;
            }

            StockReturnVoucher AddReturnVoucher(string number, Guid warehouseId, DateTime voucherDate, string returnType, Guid? projectId, Guid? beneficiaryId, string reason, params (string ItemCode, decimal Quantity, decimal UnitCost)[] lines)
            {
                var voucher = new StockReturnVoucher
                {
                    Id = Guid.NewGuid(),
                    VoucherNumber = number,
                    WarehouseId = warehouseId,
                    VoucherDate = voucherDate,
                    ReturnType = returnType,
                    ProjectId = projectId,
                    BeneficiaryId = beneficiaryId,
                    Reason = reason,
                    Notes = "مرتجع مخزني تجريبي",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                };

                foreach (var line in lines)
                {
                    var item = itemByCode[line.ItemCode];
                    voucher.Lines.Add(new StockReturnVoucherLine
                    {
                        Id = Guid.NewGuid(),
                        StockReturnVoucherId = voucher.Id,
                        ItemId = item.Id,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        Notes = "مرتجع ضمن الديمو"
                    });

                    Increase(item.Id, warehouseId, line.Quantity, line.UnitCost, voucherDate, StockTransactionType.SaleReturn, "StockReturnVoucher", number, reason);
                }

                returnVouchers.Add(voucher);
                return voucher;
            }

            StockDisposalVoucher AddDisposalVoucher(string number, Guid warehouseId, DateTime voucherDate, string disposalType, string reason, params (string ItemCode, decimal Quantity, decimal UnitCost)[] lines)
            {
                var voucher = new StockDisposalVoucher
                {
                    Id = Guid.NewGuid(),
                    VoucherNumber = number,
                    WarehouseId = warehouseId,
                    VoucherDate = voucherDate,
                    DisposalType = disposalType,
                    Reason = reason,
                    Notes = "إعدام/هالك تجريبي",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
                };

                foreach (var line in lines)
                {
                    var item = itemByCode[line.ItemCode];
                    voucher.Lines.Add(new StockDisposalVoucherLine
                    {
                        Id = Guid.NewGuid(),
                        StockDisposalVoucherId = voucher.Id,
                        ItemId = item.Id,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        Notes = "هالك ضمن الديمو"
                    });

                    Decrease(item.Id, warehouseId, line.Quantity, line.UnitCost, voucherDate, StockTransactionType.AdjustmentDecrease, "StockDisposalVoucher", number, reason);
                }

                disposalVouchers.Add(voucher);
                return voucher;
            }

            AddReceipt("SR-DEMO-001", mainWarehouse.Id, DateTime.Today.AddDays(-35), "Purchase", "توريد مشتريات بداية الفترة",
                ("ITM-DEMO-001", 120, 120, "B-001"),
                ("ITM-DEMO-002", 80, 250, "B-002"),
                ("ITM-DEMO-003", 150, 110, "B-003"));

            AddReceipt("SR-DEMO-002", mainWarehouse.Id, DateTime.Today.AddDays(-28), "Donation", "تبرع عيني من مؤسسة الخير",
                ("ITM-DEMO-004", 100, 80, "B-004"),
                ("ITM-DEMO-005", 90, 45, "B-005"),
                ("ITM-DEMO-006", 75, 90, "B-006"));

            AddReceipt("SR-DEMO-003", projectWarehouse.Id, DateTime.Today.AddDays(-21), "TransferIn", "إضافة تشغيلية لمخزن المشروعات",
                ("ITM-DEMO-007", 60, 35, "B-007"),
                ("ITM-DEMO-008", 70, 20, "B-008"),
                ("ITM-DEMO-009", 55, 70, "B-009"));

            AddReceipt("SR-DEMO-004", campaignWarehouse.Id, DateTime.Today.AddDays(-15), "Purchase", "توريد حملة موسمية",
                ("ITM-DEMO-010", 25, 180, "B-010"),
                ("ITM-DEMO-011", 18, 150, "B-011"),
                ("ITM-DEMO-012", 40, 200, "B-012"));

            AddReceipt("SR-DEMO-005", mainWarehouse.Id, DateTime.Today.AddDays(-10), "Purchase", "توريد تكميلي",
                ("ITM-DEMO-001", 30, 118, "B-013"),
                ("ITM-DEMO-003", 40, 109, "B-014"),
                ("ITM-DEMO-005", 25, 44, "B-015"));

            AddIssue("SI-DEMO-001", mainWarehouse.Id, DateTime.Today.AddDays(-12), "Beneficiary", beneficiaries.ElementAtOrDefault(0)?.Id, null, beneficiaries.ElementAtOrDefault(0)?.FullName ?? "مستفيد",
                ("ITM-DEMO-001", 12, 120),
                ("ITM-DEMO-003", 8, 110));

            AddIssue("SI-DEMO-002", mainWarehouse.Id, DateTime.Today.AddDays(-11), "Project", null, projects.ElementAtOrDefault(0)?.Id, projects.ElementAtOrDefault(0)?.Name ?? "مشروع",
                ("ITM-DEMO-002", 10, 250),
                ("ITM-DEMO-005", 15, 45));

            AddIssue("SI-DEMO-003", projectWarehouse.Id, DateTime.Today.AddDays(-9), "Beneficiary", beneficiaries.ElementAtOrDefault(1)?.Id, null, beneficiaries.ElementAtOrDefault(1)?.FullName ?? "مستفيد",
                ("ITM-DEMO-007", 6, 35),
                ("ITM-DEMO-008", 10, 20));

            AddIssue("SI-DEMO-004", campaignWarehouse.Id, DateTime.Today.AddDays(-7), "Project", null, projects.ElementAtOrDefault(1)?.Id, projects.ElementAtOrDefault(1)?.Name ?? "مشروع",
                ("ITM-DEMO-010", 3, 180),
                ("ITM-DEMO-012", 5, 200));

            AddIssue("SI-DEMO-005", mainWarehouse.Id, DateTime.Today.AddDays(-6), "Internal", null, null, "المخزن الفرعي",
                ("ITM-DEMO-004", 12, 80),
                ("ITM-DEMO-006", 6, 90));

            AddIssue("SI-DEMO-006", mainWarehouse.Id, DateTime.Today.AddDays(-5), "Project", null, projects.ElementAtOrDefault(2)?.Id, projects.ElementAtOrDefault(2)?.Name ?? "مشروع",
                ("ITM-DEMO-001", 10, 120),
                ("ITM-DEMO-005", 8, 45));

            AddNeedRequest("NR-DEMO-001", DateTime.Today.AddDays(-14), "Project", projects.ElementAtOrDefault(0)?.Id, null, "منسق المشروع الأول", "Approved",
                ("ITM-DEMO-001", 20, 15, 10),
                ("ITM-DEMO-005", 12, 10, 8));

            AddNeedRequest("NR-DEMO-002", DateTime.Today.AddDays(-12), "Beneficiary", null, beneficiaries.ElementAtOrDefault(2)?.Id, beneficiaries.ElementAtOrDefault(2)?.FullName ?? "مستفيد", "Submitted",
                ("ITM-DEMO-003", 6, 4, 0),
                ("ITM-DEMO-004", 5, 5, 0));

            AddNeedRequest("NR-DEMO-003", DateTime.Today.AddDays(-9), "Project", projects.ElementAtOrDefault(1)?.Id, null, "فريق حملة رمضان", "PartiallyFulfilled",
                ("ITM-DEMO-010", 4, 4, 3),
                ("ITM-DEMO-012", 7, 5, 5));

            AddNeedRequest("NR-DEMO-004", DateTime.Today.AddDays(-7), "Project", projects.ElementAtOrDefault(2)?.Id, null, "وحدة المشروعات", "Approved",
                ("ITM-DEMO-007", 10, 8, 6),
                ("ITM-DEMO-008", 15, 10, 10));

            AddNeedRequest("NR-DEMO-005", DateTime.Today.AddDays(-4), "Beneficiary", null, beneficiaries.ElementAtOrDefault(3)?.Id, beneficiaries.ElementAtOrDefault(3)?.FullName ?? "مستفيد", "Draft",
                ("ITM-DEMO-006", 3, 0, 0));

            AddReturnVoucher("RT-DEMO-001", mainWarehouse.Id, DateTime.Today.AddDays(-4), "Project", projects.ElementAtOrDefault(0)?.Id, null, "مرتجع من مشروع بعد انتهاء النشاط",
                ("ITM-DEMO-002", 2, 250),
                ("ITM-DEMO-005", 3, 45));

            AddReturnVoucher("RT-DEMO-002", projectWarehouse.Id, DateTime.Today.AddDays(-3), "Beneficiary", null, beneficiaries.ElementAtOrDefault(1)?.Id, "رد جزء غير مستخدم",
                ("ITM-DEMO-007", 1, 35),
                ("ITM-DEMO-008", 2, 20));

            AddReturnVoucher("RT-DEMO-003", campaignWarehouse.Id, DateTime.Today.AddDays(-2), "Project", projects.ElementAtOrDefault(1)?.Id, null, "مرتجع من فعالية ميدانية",
                ("ITM-DEMO-012", 1, 200));

            AddDisposalVoucher("DS-DEMO-001", mainWarehouse.Id, DateTime.Today.AddDays(-2), "Damage", "تالف نتيجة تخزين غير مناسب",
                ("ITM-DEMO-003", 5, 110),
                ("ITM-DEMO-004", 2, 80));

            AddDisposalVoucher("DS-DEMO-002", projectWarehouse.Id, DateTime.Today.AddDays(-1), "Expiry", "منتهي الصلاحية",
                ("ITM-DEMO-008", 3, 20));

            AddDisposalVoucher("DS-DEMO-003", campaignWarehouse.Id, DateTime.Today.AddDays(-1), "Damage", "عبوات غير صالحة للتسليم",
                ("ITM-DEMO-010", 1, 180));

            Transfer(itemByCode["ITM-DEMO-001"].Id, mainWarehouse.Id, projectWarehouse.Id, 20, 120, DateTime.Today.AddDays(-8), "TR-DEMO-001", "تحويل لخدمة المشروع");
            Transfer(itemByCode["ITM-DEMO-004"].Id, mainWarehouse.Id, campaignWarehouse.Id, 15, 80, DateTime.Today.AddDays(-6), "TR-DEMO-002", "تغذية حملة موسمية");
            Transfer(itemByCode["ITM-DEMO-005"].Id, mainWarehouse.Id, projectWarehouse.Id, 10, 45, DateTime.Today.AddDays(-5), "TR-DEMO-003", "تحويل احتياج مشروع");
            Transfer(itemByCode["ITM-DEMO-012"].Id, campaignWarehouse.Id, mainWarehouse.Id, 4, 200, DateTime.Today.AddDays(-3), "TR-DEMO-004", "إرجاع فائض للمخزن الرئيسي");

            var finalBalances = touchedKeys
                .Select(key => new ItemWarehouseBalance
                {
                    Id = Guid.NewGuid(),
                    ItemId = key.ItemId,
                    WarehouseId = key.WarehouseId,
                    QuantityOnHand = balances[key],
                    ReservedQuantity = 0,
                    AvailableQuantity = balances[key],
                    LastUpdatedUtc = DateTime.UtcNow
                })
                .ToList();

            await db.Set<CharityStoreReceipt>().AddRangeAsync(receipts);
            await db.Set<CharityStoreIssue>().AddRangeAsync(issues);
            await db.Set<StockNeedRequest>().AddRangeAsync(needRequests);
            await db.Set<StockReturnVoucher>().AddRangeAsync(returnVouchers);
            await db.Set<StockDisposalVoucher>().AddRangeAsync(disposalVouchers);
            await db.Set<StockTransaction>().AddRangeAsync(transactions);
            await db.Set<ItemWarehouseBalance>().AddRangeAsync(finalBalances);

            result.Receipts = receipts.Count;
            result.Issues = issues.Count;
            result.NeedRequests = needRequests.Count;
            result.ReturnVouchers = returnVouchers.Count;
            result.Disposals = disposalVouchers.Count;
            result.Transfers = 4;

            return result;
        }

        private static async Task<List<Warehouse>> EnsureDemoWarehousesAsync(AppDbContext db)
        {
            var list = new List<Warehouse>();
            async Task<Warehouse> EnsureAsync(string code, string nameAr, string? address, bool isMain)
            {
                var existing = await db.Set<Warehouse>().FirstOrDefaultAsync(x => x.WarehouseCode == code);
                if (existing != null)
                {
                    list.Add(existing);
                    return existing;
                }

                var warehouse = new Warehouse
                {
                    Id = Guid.NewGuid(),
                    WarehouseCode = code,
                    WarehouseNameAr = nameAr,
                    WarehouseNameEn = nameAr,
                    Address = address,
                    Notes = "مخزن تجريبي",
                    IsMain = isMain,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await db.Set<Warehouse>().AddAsync(warehouse);
                list.Add(warehouse);
                return warehouse;
            }

            await EnsureAsync("WH-DEMO-001", "المخزن الرئيسي للجمعية", "سوهاج - المركز الرئيسي", true);
            await EnsureAsync("WH-DEMO-002", "مخزن المشروعات", "سوهاج - إدارة المشروعات", false);
            await EnsureAsync("WH-DEMO-003", "مخزن الحملات الموسمية", "سوهاج - المخزن الموسمي", false);
            return list;
        }

        private static async Task<List<Item>> EnsureDemoStockItemsAsync(AppDbContext db)
        {
            var units = await db.Set<Unit>().OrderBy(x => x.UnitCode).ToListAsync();
            var groups = await db.Set<ItemGroup>().OrderBy(x => x.GroupCode).ToListAsync();
            if (!units.Any() || !groups.Any())
                return new List<Item>();

            var piece = units.First();
            var box = units.FirstOrDefault(x => x.UnitCode == "BOX") ?? piece;
            var defaultGroup = groups.First();

            var definitions = new[]
            {
                new { Code = "ITM-DEMO-001", Name = "بطاطين شتوية", GroupIndex = 0, UnitId = piece.Id, Purchase = 120m, Sale = 145m },
                new { Code = "ITM-DEMO-002", Name = "كراتين مواد غذائية", GroupIndex = 0, UnitId = box.Id, Purchase = 250m, Sale = 290m },
                new { Code = "ITM-DEMO-003", Name = "أكياس أرز 5 كجم", GroupIndex = 0, UnitId = piece.Id, Purchase = 110m, Sale = 130m },
                new { Code = "ITM-DEMO-004", Name = "زجاجات زيت", GroupIndex = 0, UnitId = piece.Id, Purchase = 80m, Sale = 96m },
                new { Code = "ITM-DEMO-005", Name = "أدوات مدرسية", GroupIndex = 1, UnitId = piece.Id, Purchase = 45m, Sale = 55m },
                new { Code = "ITM-DEMO-006", Name = "شنط مدرسية", GroupIndex = 1, UnitId = piece.Id, Purchase = 90m, Sale = 110m },
                new { Code = "ITM-DEMO-007", Name = "أدوية أساسية", GroupIndex = 2, UnitId = piece.Id, Purchase = 35m, Sale = 42m },
                new { Code = "ITM-DEMO-008", Name = "عبوات مطهرات", GroupIndex = 2, UnitId = piece.Id, Purchase = 20m, Sale = 25m },
                new { Code = "ITM-DEMO-009", Name = "ملابس أطفال", GroupIndex = 1, UnitId = piece.Id, Purchase = 70m, Sale = 85m },
                new { Code = "ITM-DEMO-010", Name = "أجهزة قياس سكر", GroupIndex = 1, UnitId = piece.Id, Purchase = 180m, Sale = 220m },
                new { Code = "ITM-DEMO-011", Name = "سماعات طبية", GroupIndex = 2, UnitId = piece.Id, Purchase = 150m, Sale = 180m },
                new { Code = "ITM-DEMO-012", Name = "فرشات نوم", GroupIndex = 0, UnitId = piece.Id, Purchase = 200m, Sale = 240m }
            };

            var list = new List<Item>();
            foreach (var def in definitions)
            {
                var existing = await db.Set<Item>().FirstOrDefaultAsync(x => x.ItemCode == def.Code);
                if (existing != null)
                {
                    list.Add(existing);
                    continue;
                }

                var group = groups.ElementAtOrDefault(def.GroupIndex) ?? defaultGroup;
                var item = new Item
                {
                    Id = Guid.NewGuid(),
                    ItemCode = def.Code,
                    ItemNameAr = def.Name,
                    ItemNameEn = def.Name,
                    ItemGroupId = group.Id,
                    UnitId = def.UnitId,
                    IsService = false,
                    IsStockItem = true,
                    IsActive = true,
                    PurchasePrice = def.Purchase,
                    SalePrice = def.Sale,
                    MinimumQuantity = 5,
                    ReorderQuantity = 10,
                    IsTaxable = false,
                    TaxRate = 0,
                    Description = "صنف مخزني تجريبي لبيانات الجمعية",
                    CreatedAtUtc = DateTime.UtcNow
                };

                await db.Set<Item>().AddAsync(item);
                list.Add(item);
            }

            return list;
        }

        private static async Task<int> SeedAidCyclesAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<AidCycle>().AnyAsync(x => x.CycleNumber.StartsWith("AC-DEMO-")))
                return 0;

            var aidFinancial = await ResolveAidTypeAsync(db, "نقدي");
            var approvedDecisions = await db.Set<BeneficiaryCommitteeDecision>()
                .OrderByDescending(x => x.DecisionDate)
                .Take(24)
                .ToListAsync();

            if (!approvedDecisions.Any())
                return 0;

            var cycles = new List<AidCycle>();
            var lines = new List<AidCycleBeneficiary>();

            var cycle1 = new AidCycle
            {
                Id = Guid.NewGuid(),
                CycleNumber = $"AC-DEMO-{DateTime.Today:yyyyMM}-01",
                Title = $"دورة صرف شهرية {DateTime.Today:MMMM yyyy}",
                CycleType = "Monthly",
                AidTypeId = aidFinancial.Id,
                PeriodYear = DateTime.Today.Year,
                PeriodMonth = DateTime.Today.Month,
                FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)),
                PlannedDisbursementDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, Math.Min(25, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))),
                Status = "Generated",
                BeneficiariesCount = approvedDecisions.Count,
                TotalPlannedAmount = approvedDecisions.Sum(x => x.ApprovedAmount ?? 0),
                TotalDisbursedAmount = approvedDecisions.Take(4).Sum(x => x.ApprovedAmount ?? 0),
                Notes = "دورة تجريبية",
                CreatedByUserId = createdByUserId
            };
            cycles.Add(cycle1);

            var idx = 0;
            foreach (var decision in approvedDecisions)
            {
                idx++;
                lines.Add(new AidCycleBeneficiary
                {
                    Id = Guid.NewGuid(),
                    AidCycleId = cycle1.Id,
                    BeneficiaryId = decision.BeneficiaryId,
                    CommitteeDecisionId = decision.Id,
                    AidTypeId = decision.ApprovedAidTypeId ?? aidFinancial.Id,
                    ScheduledAmount = decision.ApprovedAmount,
                    ApprovedAmount = decision.ApprovedAmount,
                    DisbursedAmount = idx <= 4 ? decision.ApprovedAmount : 0,
                    Status = idx <= 4 ? "Disbursed" : "Eligible",
                    LastDisbursementDate = idx <= 4 ? DateTime.Today.AddDays(-2) : null,
                    NextDueDate = DateTime.Today.AddMonths(1),
                    Notes = "سطر دورة صرف تجريبي",
                    StopReason = null
                });
            }

            await db.Set<AidCycle>().AddRangeAsync(cycles);
            await db.Set<AidCycleBeneficiary>().AddRangeAsync(lines);

            return cycles.Count;
        }

        private static async Task<int> SeedVolunteersAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<Volunteer>().AnyAsync(x => x.VolunteerCode.StartsWith("VOL-DEMO-")))
                return 0;

            var projects = await db.Set<CharityProject>().Where(x => x.Code.StartsWith("PRJ-DEMO-")).ToListAsync();

            var volunteers = new List<Volunteer>();
            var assignments = new List<VolunteerProjectAssignment>();
            var hours = new List<VolunteerHourLog>();

            for (var i = 1; i <= 18; i++)
            {
                var v = new Volunteer
                {
                    Id = Guid.NewGuid(),
                    VolunteerCode = $"VOL-DEMO-{i:000}",
                    FullName = BuildName(i % 2 == 0),
                    Qualification = i % 2 == 0 ? "مؤهل متوسط" : "بكالوريوس خدمة اجتماعية",
                    AddressLine = "سوهاج - مركز أخميم",
                    Nationality = "مصري",
                    NationalId = $"3020{i:000000000}",
                    PhoneNumber = $"0155{Rng.Next(1000000, 9999999)}",
                    Email = $"volunteer{i}@demo.local",
                    BirthDate = DateTime.Today.AddYears(-(20 + i)),
                    Gender = i % 2 == 0 ? "Female" : "Male",
                    MaritalStatus = i % 3 == 0 ? "Married" : "Single",
                    PreferredArea = i <= 5 ? "المستفيدين" : "المشروعات",
                    AvailabilityNotes = "متاح 3 أيام أسبوعيًا",
                    IsActive = true,
                    Notes = "بيانات متطوع تجريبية",
                    CreatedByUserId = createdByUserId
                };
                volunteers.Add(v);

                if (projects.Any())
                {
                    var project = projects[(i - 1) % projects.Count];
                    assignments.Add(new VolunteerProjectAssignment
                    {
                        Id = Guid.NewGuid(),
                        VolunteerId = v.Id,
                        ProjectId = project.Id,
                        RoleTitle = "دعم ميداني",
                        CreatedAtUtc = DateTime.Today.AddDays(-Rng.Next(5, 50)),
                        Notes = "إسناد تجريبي"
                    });

                    hours.Add(new VolunteerHourLog
                    {
                        Id = Guid.NewGuid(),
                        VolunteerId = v.Id,
                        ProjectId = project.Id,
                        WorkDate = DateTime.Today.AddDays(-Rng.Next(1, 30)),
                        Hours = 3 + (i % 4),
                        ActivityTitle = "زيارة ميدانية / تنظيم فعالية",
                        Notes = "ساعات تطوع تجريبية"
                    });
                }
            }

            await db.Set<Volunteer>().AddRangeAsync(volunteers);
            await db.Set<VolunteerProjectAssignment>().AddRangeAsync(assignments);
            await db.Set<VolunteerHourLog>().AddRangeAsync(hours);
            return volunteers.Count;
        }

        private static async Task<int> SeedBoardMeetingsAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<BoardMeeting>().AnyAsync(x => x.MeetingNumber.StartsWith("BM-DEMO-")))
                return 0;

            var meetings = new List<BoardMeeting>();
            var attendees = new List<BoardMeetingAttendee>();
            var minutes = new List<BoardMeetingMinute>();
            var decisions = new List<BoardDecision>();
            var followUps = new List<BoardDecisionFollowUp>();

            for (var i = 1; i <= 3; i++)
            {
                var meeting = new BoardMeeting
                {
                    Id = Guid.NewGuid(),
                    MeetingNumber = $"BM-DEMO-{i:000}",
                    Title = i == 1 ? "اجتماع مجلس الإدارة الشهري" : $"اجتماع متابعة رقم {i}",
                    MeetingDate = DateTime.Today.AddDays(-(i * 14)),
                    StartTime = new TimeSpan(11, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    Location = "مقر الجمعية",
                    MeetingType = i == 1 ? "Board" : "Committee",
                    Status = "Approved",
                    Agenda = "مراجعة الموقف التنفيذي والمالي واعتماد التوصيات.",
                    Notes = "بيانات اجتماع تجريبية",
                    PreparedByUserId = createdByUserId,
                    ApprovedByUserId = createdByUserId,
                    ApprovedAtUtc = DateTime.UtcNow.AddDays(-i)
                };
                meetings.Add(meeting);

                minutes.Add(new BoardMeetingMinute
                {
                    Id = Guid.NewGuid(),
                    BoardMeetingId = meeting.Id,
                    LegalOpeningText = "افتتحت الجلسة بعد اكتمال النصاب القانوني.",
                    DiscussionSummary = "تمت مناقشة البنود المدرجة بجدول الأعمال واستعراض مؤشرات الأداء.",
                    RecommendationsSummary = "التوصية باستكمال إجراءات الصرف والمتابعة الميدانية.",
                    LegalClosingText = "انتهى الاجتماع في الموعد المحدد وتم تحرير هذا المحضر.",
                    FullMinuteText = "محضر اجتماع تجريبي لعرض النظام."
                });

                attendees.Add(new BoardMeetingAttendee { Id = Guid.NewGuid(), BoardMeetingId = meeting.Id, FullName = "أ/ رئيس مجلس الإدارة", PositionTitle = "رئيس مجلس الإدارة", AttendanceStatus = "Present" });
                attendees.Add(new BoardMeetingAttendee { Id = Guid.NewGuid(), BoardMeetingId = meeting.Id, FullName = "أ/ المدير التنفيذي", PositionTitle = "مدير تنفيذي", AttendanceStatus = "Present" });
                attendees.Add(new BoardMeetingAttendee { Id = Guid.NewGuid(), BoardMeetingId = meeting.Id, FullName = "أ/ المحاسب", PositionTitle = "محاسب", AttendanceStatus = "Present" });

                for (var d = 1; d <= 2; d++)
                {
                    var decision = new BoardDecision
                    {
                        Id = Guid.NewGuid(),
                        BoardMeetingId = meeting.Id,
                        DecisionNumber = $"DEC-DEMO-{i}{d}",
                        Title = d == 1 ? "اعتماد تقرير الصرف الشهري" : "متابعة مشروع دعم الأسر",
                        DecisionKind = "Decision",
                        Description = "قرار تجريبي ضمن بيانات عرض النظام.",
                        ResponsibleParty = d == 1 ? "الإدارة المالية" : "إدارة المشروعات",
                        DueDate = DateTime.Today.AddDays(10 + (d * 5)),
                        Status = d == 1 ? "InProgress" : "Open",
                        Priority = d == 1 ? "High" : "Medium",
                        Notes = "بيانات قرار تجريبية"
                    };
                    decisions.Add(decision);

                    followUps.Add(new BoardDecisionFollowUp
                    {
                        Id = Guid.NewGuid(),
                        BoardDecisionId = decision.Id,
                        FollowUpDate = DateTime.Today.AddDays(-Rng.Next(1, 10)),
                        Status = decision.Status,
                        ProgressPercent = d == 1 ? 60 : 30,
                        Details = "متابعة مبدئية للتنفيذ.",
                        NextAction = "استكمال المتابعة الأسبوع القادم.",
                        CreatedByUserId = createdByUserId
                    });
                }
            }

            await db.Set<BoardMeeting>().AddRangeAsync(meetings);
            await db.Set<BoardMeetingAttendee>().AddRangeAsync(attendees);
            await db.Set<BoardMeetingMinute>().AddRangeAsync(minutes);
            await db.Set<BoardDecision>().AddRangeAsync(decisions);
            await db.Set<BoardDecisionFollowUp>().AddRangeAsync(followUps);

            return meetings.Count;
        }

        private static async Task<int> SeedProjectProposalsAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<ProjectProposal>().AnyAsync(x => x.ProposalNumber.StartsWith("PP-DEMO-")))
                return 0;

            var proposal = new ProjectProposal
            {
                Id = Guid.NewGuid(),
                ProposalNumber = $"PP-DEMO-{DateTime.Today:yyyyMM}-001",
                Title = "مقترح مشروع التمكين الاقتصادي للأسر",
                DonorName = "جهة مانحة افتراضية",
                OrganizationName = "جمعية نموذجية للتنمية",
                ProjectLocation = "سوهاج - أخميم - القرى المستهدفة",
                SubmissionDate = DateTime.Today.AddDays(-10),
                DurationMonths = 12,
                RequestedBudget = 850000,
                Currency = "EGP",
                Status = "Submitted",
                RegistrationNumber = "1357/2014",
                RegistrationYear = 2014,
                Vision = "مجتمع قادر ومنتج وشامل.",
                Mission = "تقديم تدخلات تنموية متكاملة قائمة على الاحتياج والمتابعة.",
                ExpertiseSummary = "للجمعية خبرة في التدخلات الاجتماعية والاقتصادية والتدريبية.",
                EmployeesCount = 13,
                VolunteersCount = 73,
                YearsOfExperience = 8,
                ContactPerson = "م/ مدير البرامج",
                ContactPhone = "01001234567",
                ContactEmail = "programs@demo.local",
                Address = "سوهاج - مصر",
                ProblemBackground = "ارتفاع نسب الاحتياج الاقتصادي وضعف الدخل بين الأسر المستهدفة.",
                ProblemAnalysis = "الحاجة إلى تدخلات دعم مباشر وتمكين اقتصادي وتدريب.",
                NationalAlignment = "يتسق مع رؤية مصر 2030 وبرامج الحماية الاجتماعية.",
                ProposedApproach = "مزيج من الدعم المباشر والتدريب والمتابعة.",
                ProposedSolution = "تقديم دعم نقدي وتدريبي وربط بالمشروعات الصغيرة.",
                RisksAndExternalFactors = "تذبذب الأسعار - صعوبة الوصول لبعض المناطق - ضعف الاستدامة.",
                ExecutiveSummary = "مقترح تجريبي لعرض موديول مقترحات المشروعات.",
                GeneralGoal = "تعزيز التمكين الاقتصادي والاجتماعي للفئات الأكثر احتياجًا.",
                ExpectedResults = "تحسن الدخل، زيادة الوصول للخدمات، ورفع كفاءة المتابعة.",
                PreparatoryRequirements = "الحصول على الموافقات اللازمة وتجهيز فريق التنفيذ.",
                ImplementationTeamSummary = "مدير مشروع - منسق - محاسب - مسؤول متابعة.",
                SustainabilityPlan = "استدامة عبر الشراكات المحلية والتمويل المرحلي.",
                CreatedByUserId = createdByUserId
            };

            var objectives = new List<ProjectProposalObjective>
            {
                new ProjectProposalObjective { Id = Guid.NewGuid(), ProjectProposalId = proposal.Id, ObjectiveType = "General", Title = "تحسين الوضع الاقتصادي للأسر", Description = "رفع كفاءة التدخلات الداعمة للأسر" },
                new ProjectProposalObjective { Id = Guid.NewGuid(), ProjectProposalId = proposal.Id, ObjectiveType = "Specific", Title = "دعم 120 أسرة", Description = "تقديم دعم نقدي ومتابعة اجتماعية" },
                new ProjectProposalObjective { Id = Guid.NewGuid(), ProjectProposalId = proposal.Id, ObjectiveType = "Result", Title = "تحسن الاستقرار المعيشي", Description = "قياس أثر التدخل على الأسر المستهدفة" }
            };

            var activities = new List<ProjectProposalActivity>
            {
                new ProjectProposalActivity { Id = Guid.NewGuid(), ProjectProposalId = proposal.Id, Title = "حصر وتقييم الأسر", Description = "استكمال البحث الاجتماعي وتحديد الأولويات", ResponsibleRole = "الباحث الاجتماعي", NeededResources = "استمارات - انتقالات", PlannedStartMonth = 1, PlannedEndMonth = 2 },
                new ProjectProposalActivity { Id = Guid.NewGuid(), ProjectProposalId = proposal.Id, Title = "تنفيذ الدعم والتدريب", Description = "صرف المساعدات وتنفيذ جلسات تدريبية", ResponsibleRole = "منسق المشروع", NeededResources = "موازنة - مواد تدريبية", PlannedStartMonth = 3, PlannedEndMonth = 10 }
            };

            await db.Set<ProjectProposal>().AddAsync(proposal);
            await db.Set<ProjectProposalObjective>().AddRangeAsync(objectives);
            await db.Set<ProjectProposalActivity>().AddRangeAsync(activities);

            return 1;
        }


        private static async Task<HrRfpSeedResult> SeedHrRfpAsync(AppDbContext db, string? createdByUserId)
        {
            var result = new HrRfpSeedResult();
            var employees = await db.Set<HrEmployee>().Where(x => x.Code.StartsWith("EMP-DEMO-")).OrderBy(x => x.Code).ToListAsync();
            if (!employees.Any()) return result;

            var project = await db.Set<CharityProject>().OrderBy(x => x.Code).FirstOrDefaultAsync();
            var funder = await EnsureDemoFunderAsync(db);

            if (!await db.Set<HrEmployeeContract>().AnyAsync())
            {
                var contracts = employees.Select((e, i) => new HrEmployeeContract
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = e.Id,
                    ContractType = e.EmploymentType,
                    ContractNumber = $"CTR-DEMO-{i + 1:000}",
                    StartDate = e.HireDate,
                    EndDate = e.EmploymentType == "Contract" ? e.HireDate.AddYears(1) : null,
                    BasicSalary = e.BasicSalary,
                    GrossSalary = e.BasicSalary + 500,
                    Status = "Active",
                    CreatedByUserId = createdByUserId,
                    Notes = "عقد ديمو"
                }).ToList();
                await db.Set<HrEmployeeContract>().AddRangeAsync(contracts);
                result.Contracts = contracts.Count;
            }

            if (!await db.Set<HrEmployeeFundingAssignment>().AnyAsync())
            {
                var funding = employees.Take(6).Select(e => new HrEmployeeFundingAssignment
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = e.Id,
                    FunderId = funder.Id,
                    FundingSourceName = funder.Name,
                    CharityProjectId = project?.Id,
                    GrantOrBudgetLine = "موازنة تشغيلية",
                    AllocationPercentage = 100,
                    FromDate = DateTime.Today.AddMonths(-2),
                    IsPrimary = true,
                    CreatedByUserId = createdByUserId,
                    Notes = "ربط تمويل ديمو"
                }).ToList();
                await db.Set<HrEmployeeFundingAssignment>().AddRangeAsync(funding);
                result.FundingAssignments = funding.Count;
            }

            if (!await db.Set<HrEmployeeTaskAssignment>().AnyAsync())
            {
                var tasks = employees.Take(6).Select((e, i) => new HrEmployeeTaskAssignment
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = e.Id,
                    CharityProjectId = project?.Id,
                    TaskTitle = i % 2 == 0 ? "متابعة ميدانية" : "إدخال بيانات المستفيدين",
                    TaskDescription = "تكليف تجريبي مرتبط بالمشروع",
                    AssignedDate = DateTime.Today.AddDays(-10),
                    DueDate = DateTime.Today.AddDays(15 + i),
                    Priority = i % 2 == 0 ? "High" : "Medium",
                    Status = i % 3 == 0 ? "InProgress" : "Assigned",
                    IsPrimaryDuty = i % 2 == 0,
                    CreatedByUserId = createdByUserId,
                    Notes = "تكليف ديمو"
                }).ToList();
                await db.Set<HrEmployeeTaskAssignment>().AddRangeAsync(tasks);
                result.TaskAssignments = tasks.Count;
            }

            return result;
        }

        private static async Task<GrantSeedResult> SeedFundersAndGrantsAsync(AppDbContext db, string? createdByUserId)
        {
            var result = new GrantSeedResult();
            var funder = await EnsureDemoFunderAsync(db);
            if (db.Entry(funder).State == EntityState.Added) result.Funders = 1;
            var grant = await EnsureDemoGrantAgreementAsync(db, funder.Id);
            if (db.Entry(grant).State == EntityState.Added) result.GrantAgreements = 1;

            if (!await db.Set<GrantInstallment>().AnyAsync(x => x.GrantAgreementId == grant.Id))
            {
                var bankMethod = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "BANK");
                var bankAccount = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
                var list = new List<GrantInstallment>();
                for (var i = 1; i <= 3; i++)
                {
                    list.Add(new GrantInstallment
                    {
                        Id = Guid.NewGuid(),
                        GrantAgreementId = grant.Id,
                        InstallmentNumber = i,
                        DueDate = grant.StartDate!.Value.AddMonths((i - 1) * 3),
                        Amount = Math.Round(grant.TotalAmount / 3m, 2),
                        ReceivedAmount = i <= 2 ? Math.Round(grant.TotalAmount / 3m, 2) : null,
                        ReceivedDate = i <= 2 ? grant.StartDate!.Value.AddMonths((i - 1) * 3).AddDays(7) : null,
                        PaymentMethodId = i <= 2 ? bankMethod.Id : null,
                        FinancialAccountId = i <= 2 ? bankAccount?.Id : null,
                        Status = i <= 2 ? "Received" : "Planned",
                        ReferenceNumber = $"GI-DEMO-{i:000}",
                        Notes = "دفعة منحة ديمو"
                    });
                }
                await db.Set<GrantInstallment>().AddRangeAsync(list);
                result.GrantInstallments = list.Count;
            }

            return result;
        }

        private static async Task<AccountingSeedResult> SeedPayrollAndAccountingAsync(AppDbContext db, string? createdByUserId)
        {
            var result = new AccountingSeedResult();
            var employees = await db.Set<HrEmployee>().Where(x => x.Code.StartsWith("EMP-DEMO-")).OrderBy(x => x.Code).ToListAsync();
            if (!employees.Any()) return result;

            var payrollMonths = new List<PayrollMonth>();
            var historyStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-23);

            for (var monthOffset = 0; monthOffset < 24; monthOffset++)
            {
                var currentMonth = historyStart.AddMonths(monthOffset);
                var existingMonth = await db.Set<PayrollMonth>()
                    .FirstOrDefaultAsync(x => x.Year == currentMonth.Year && x.Month == currentMonth.Month);

                if (existingMonth == null)
                {
                    existingMonth = new PayrollMonth
                    {
                        Id = Guid.NewGuid(),
                        Year = currentMonth.Year,
                        Month = currentMonth.Month,
                        Status = monthOffset >= 22 ? "Approved" : "Posted",
                        CreatedAtUtc = currentMonth.AddDays(1),
                        ApprovedByUserId = createdByUserId,
                        ApprovedAtUtc = currentMonth.AddDays(26)
                    };
                    await db.Set<PayrollMonth>().AddAsync(existingMonth);
                    result.PayrollMonths++;
                }

                payrollMonths.Add(existingMonth);
            }

            var bankMethod = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "BANK");
            var bankAccount = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var bonusRows = new List<HrEmployeeBonus>();
            var payrollEmployees = new List<PayrollEmployee>();
            var payrollItems = new List<PayrollEmployeeItem>();
            var payrollPayments = new List<PayrollPayment>();

            foreach (var payrollMonth in payrollMonths)
            {
                var alreadySeeded = await db.Set<PayrollEmployee>().AnyAsync(x => x.PayrollMonthId == payrollMonth.Id);
                if (alreadySeeded)
                    continue;

                var monthDate = new DateTime(payrollMonth.Year, payrollMonth.Month, 1);
                var monthIndex = ((payrollMonth.Year - historyStart.Year) * 12) + (payrollMonth.Month - historyStart.Month);

                foreach (var e in employees.Take(8))
                {
                    var annualFactor = monthIndex / 12;
                    var seasonalFactor = (monthIndex % 6) * 0.005m;
                    var indexedBasic = Math.Round(e.BasicSalary * (1m + (annualFactor * 0.06m) + seasonalFactor), 2);
                    var bonusRate = 0.06m + ((monthIndex + employees.IndexOf(e)) % 4) * 0.01m;
                    var bonusAmount = Math.Round(indexedBasic * bonusRate, 2);
                    var deduction = Math.Round(indexedBasic * (0.025m + ((monthIndex + 1) % 3) * 0.003m), 2);
                    var gross = indexedBasic + bonusAmount;
                    var net = gross - deduction;

                    var payrollEmployee = new PayrollEmployee
                    {
                        Id = Guid.NewGuid(),
                        PayrollMonthId = payrollMonth.Id,
                        EmployeeId = e.Id,
                        BasicSalary = indexedBasic,
                        AttendanceDeduction = deduction,
                        OtherDeductions = 0,
                        Additions = bonusAmount,
                        GrossAmount = gross,
                        TotalDeductions = deduction,
                        NetAmount = net,
                        Notes = "مسير رواتب تاريخي تجريبي"
                    };
                    payrollEmployees.Add(payrollEmployee);

                    payrollItems.Add(new PayrollEmployeeItem
                    {
                        Id = Guid.NewGuid(),
                        PayrollEmployeeId = payrollEmployee.Id,
                        ItemType = "Addition",
                        Value = bonusAmount,
                        Notes = monthIndex % 12 == 8 ? "بدل مدارس" : "حافز أداء شهري"
                    });
                    payrollItems.Add(new PayrollEmployeeItem
                    {
                        Id = Guid.NewGuid(),
                        PayrollEmployeeId = payrollEmployee.Id,
                        ItemType = "Deduction",
                        Value = deduction,
                        Notes = "خصم انضباط/تأخير"
                    });

                    var paymentDate = new DateTime(payrollMonth.Year, payrollMonth.Month, Math.Min(27, DateTime.DaysInMonth(payrollMonth.Year, payrollMonth.Month)));
                    payrollPayments.Add(new PayrollPayment
                    {
                        Id = Guid.NewGuid(),
                        PayrollEmployeeId = payrollEmployee.Id,
                        PaymentDate = paymentDate,
                        PaymentMethodId = bankMethod.Id,
                        FinancialAccountId = bankAccount?.Id,
                        Amount = net,
                        ReferenceNumber = $"PAY-DEMO-{payrollMonth.Year}{payrollMonth.Month:00}-{e.Code}",
                        Notes = "سداد تاريخي تجريبي"
                    });

                    if (!await db.Set<HrEmployeeBonus>().AnyAsync(x => x.PayrollMonthId == payrollMonth.Id && x.EmployeeId == e.Id))
                    {
                        bonusRows.Add(new HrEmployeeBonus
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = e.Id,
                            BonusDate = paymentDate.AddDays(-2),
                            BonusType = monthIndex % 12 == 11 ? "YearEnd" : "Performance",
                            Amount = bonusAmount,
                            PayrollMonthId = payrollMonth.Id,
                            IsApproved = true,
                            ApprovedByUserId = createdByUserId,
                            ApprovedAtUtc = paymentDate.AddDays(-1),
                            CreatedByUserId = createdByUserId,
                            CreatedAtUtc = paymentDate.AddDays(-3),
                            Reason = monthIndex % 12 == 11 ? "حافز نهاية عام" : "مكافأة أداء ديمو",
                            Notes = "مرتبطة بمسير تاريخي"
                        });
                    }
                }
            }

            if (payrollEmployees.Any())
            {
                await db.Set<PayrollEmployee>().AddRangeAsync(payrollEmployees);
                await db.Set<PayrollEmployeeItem>().AddRangeAsync(payrollItems);
                await db.Set<PayrollPayment>().AddRangeAsync(payrollPayments);
            }

            if (bonusRows.Any())
            {
                await db.Set<HrEmployeeBonus>().AddRangeAsync(bonusRows);
            }

            var fiscal = await db.Set<FiscalPeriod>().Where(x => x.IsOpen).OrderByDescending(x => x.StartDate).FirstAsync();
            var costCenter = await db.Set<CostCenter>().OrderBy(x => x.CostCenterCode).FirstAsync();
            var asset = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var revenue = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Revenue).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var expense = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Expense).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var liability = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Liability).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            if (asset != null && revenue != null && expense != null && liability != null && !await db.Set<JournalEntry>().AnyAsync(x => x.EntryNumber.StartsWith("JE-DEMO-")))
            {
                var donation = await db.Set<Donation>().OrderByDescending(x => x.DonationDate).FirstOrDefaultAsync(x => x.Amount.HasValue && x.Amount.Value > 0);
                var grantInst = await db.Set<GrantInstallment>().OrderByDescending(x => x.InstallmentNumber).FirstOrDefaultAsync(x => x.ReceivedAmount != null);
                var aid = await db.Set<BeneficiaryAidDisbursement>().OrderByDescending(x => x.DisbursementDate).FirstOrDefaultAsync(x => x.Amount.HasValue && x.Amount.Value > 0);
                var payroll = await db.Set<PayrollMonth>().OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefaultAsync();
                var entries = new List<JournalEntry>();

                void AddEntry(string number, string desc, Guid? sourceId, string? sourceType, decimal amount, Guid debitAccountId, Guid creditAccountId, Guid? projectId = null)
                {
                    var entry = new JournalEntry
                    {
                        Id = Guid.NewGuid(),
                        EntryNumber = number,
                        EntryDate = DateTime.Today,
                        Description = desc,
                        FiscalPeriodId = fiscal.Id,
                        Status = JournalEntryStatus.Posted,
                        TotalDebit = amount,
                        TotalCredit = amount,
                        SourceId = sourceId,
                        SourceType = sourceType,
                        PostedAtUtc = DateTime.UtcNow,
                        PostedByUserId = createdByUserId
                    };
                    entry.Lines.Add(new JournalEntryLine { Id = Guid.NewGuid(), JournalEntryId = entry.Id, FinancialAccountId = debitAccountId, CostCenterId = costCenter.Id, ProjectId = projectId, Description = desc, DebitAmount = amount, CreditAmount = 0 });
                    entry.Lines.Add(new JournalEntryLine { Id = Guid.NewGuid(), JournalEntryId = entry.Id, FinancialAccountId = creditAccountId, CostCenterId = costCenter.Id, ProjectId = projectId, Description = desc, DebitAmount = 0, CreditAmount = amount });
                    entries.Add(entry);
                }

                if (donation != null) AddEntry("JE-DEMO-001", "قيد تبرع نقدي", donation.Id, "Donation", donation.Amount!.Value, asset.Id, revenue.Id);
                if (grantInst != null) AddEntry("JE-DEMO-002", "قيد استلام دفعة منحة", grantInst.Id, "GrantInstallment", grantInst.ReceivedAmount!.Value, asset.Id, liability.Id);
                if (aid != null) AddEntry("JE-DEMO-003", "قيد صرف مساعدة", aid.Id, "BeneficiaryAidDisbursement", aid.Amount!.Value, expense.Id, asset.Id);
                if (payroll != null)
                {
                    var payrollAmount = await db.Set<PayrollEmployee>().Where(x => x.PayrollMonthId == payroll.Id).SumAsync(x => x.NetAmount);
                    if (payrollAmount > 0) AddEntry("JE-DEMO-004", "قيد رواتب الشهر", payroll.Id, "PayrollMonth", payrollAmount, expense.Id, asset.Id);
                }

                if (entries.Any())
                {
                    await db.Set<JournalEntry>().AddRangeAsync(entries);
                    result.JournalEntries = entries.Count;
                }
            }

            return result;
        }

        private static async Task<Funder> EnsureDemoFunderAsync(AppDbContext db)
        {
            var funder = await db.Set<Funder>().FirstOrDefaultAsync(x => x.Code == "FUN-DEMO-001");
            if (funder != null) return funder;

            funder = new Funder
            {
                Id = Guid.NewGuid(),
                Code = "FUN-DEMO-001",
                Name = "مؤسسة دعم المجتمع",
                FunderType = "Foundation",
                ContactPerson = "أ/ مسؤول المنح",
                PhoneNumber = "01002223344",
                Email = "grants@demo.local",
                AddressLine = "القاهرة",
                Notes = "جهة ممولة تجريبية",
                IsActive = true
            };
            await db.Set<Funder>().AddAsync(funder);
            return funder;
        }

        private static async Task<GrantAgreement> EnsureDemoGrantAgreementAsync(AppDbContext db, Guid funderId)
        {
            var grant = await db.Set<GrantAgreement>().FirstOrDefaultAsync(x => x.AgreementNumber == "GA-DEMO-001");
            if (grant != null) return grant;

            grant = new GrantAgreement
            {
                Id = Guid.NewGuid(),
                AgreementNumber = "GA-DEMO-001",
                FunderId = funderId,
                Title = "اتفاقية دعم البرامج الاجتماعية",
                Description = "اتفاقية تجريبية لعرض النظام",
                AgreementDate = DateTime.Today.AddMonths(-2),
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today.AddMonths(11),
                TotalAmount = 1200000,
                Currency = "EGP",
                PaymentTerms = "دفعات ربع سنوية",
                ReportingRequirements = "تقارير شهرية وربعية",
                Status = "Active",
                Notes = "منحة تجريبية"
            };
            await db.Set<GrantAgreement>().AddAsync(grant);
            return grant;
        }

        private static async Task<List<HrDepartment>> EnsureHrDepartmentsAsync(AppDbContext db)
        {
            var existing = await db.Set<HrDepartment>().ToListAsync();
            if (existing.Any()) return existing;

            var list = new List<HrDepartment>
            {
                new HrDepartment { Id = Guid.NewGuid(), Name = "الموارد البشرية", Description = "HR", IsActive = true },
                new HrDepartment { Id = Guid.NewGuid(), Name = "المستفيدين", Description = "Beneficiaries", IsActive = true },
                new HrDepartment { Id = Guid.NewGuid(), Name = "المشروعات", Description = "Projects", IsActive = true },
                new HrDepartment { Id = Guid.NewGuid(), Name = "المالية", Description = "Finance", IsActive = true }
            };
            await db.Set<HrDepartment>().AddRangeAsync(list);
            return list;
        }

        private static async Task<List<HrJobTitle>> EnsureHrJobTitlesAsync(AppDbContext db)
        {
            var existing = await db.Set<HrJobTitle>().ToListAsync();
            if (existing.Any()) return existing;

            var list = new List<HrJobTitle>
            {
                new HrJobTitle { Id = Guid.NewGuid(), Name = "باحث اجتماعي", Description = "Social Researcher", IsActive = true },
                new HrJobTitle { Id = Guid.NewGuid(), Name = "مدخل بيانات", Description = "Data Entry", IsActive = true },
                new HrJobTitle { Id = Guid.NewGuid(), Name = "محاسب", Description = "Accountant", IsActive = true },
                new HrJobTitle { Id = Guid.NewGuid(), Name = "منسق مشروع", Description = "Project Coordinator", IsActive = true },
                new HrJobTitle { Id = Guid.NewGuid(), Name = "مدير برنامج", Description = "Program Manager", IsActive = true }
            };
            await db.Set<HrJobTitle>().AddRangeAsync(list);
            return list;
        }

        private static async Task<HrShift> EnsureDefaultShiftAsync(AppDbContext db)
        {
            var shift = await db.Set<HrShift>().FirstOrDefaultAsync();
            if (shift != null) return shift;

            shift = new HrShift
            {
                Id = Guid.NewGuid(),
                Name = "الورديه الصباحية",
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(15, 0, 0),
                GraceMinutes = 15,
                Notes = "ورديه افتراضية"
            };
            await db.Set<HrShift>().AddAsync(shift);
            return shift;
        }

        private static string BuildName(bool male)
        {
            var first = male ? MaleNames[Rng.Next(MaleNames.Length)] : FemaleNames[Rng.Next(FemaleNames.Length)];
            var middle = MaleNames[Rng.Next(MaleNames.Length)];
            var family = FamilyNames[Rng.Next(FamilyNames.Length)];
            return $"{first} {middle} {family}";
        }

        private static string BuildFamilyMemberName(bool female)
        {
            var first = female ? FemaleNames[Rng.Next(FemaleNames.Length)] : MaleNames[Rng.Next(MaleNames.Length)];
            var family = FamilyNames[Rng.Next(FamilyNames.Length)];
            return $"{first} {family}";
        }
    }
}
