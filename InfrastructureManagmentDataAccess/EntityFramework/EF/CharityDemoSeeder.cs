
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
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;

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
        public object WarehousesCreated { get; set; }
        public object NeedRequestsCreated { get; set; }
        public object ItemsCreated { get; set; }
        public object ReturnVouchersCreated { get; set; }
        public object OpeningEntriesCreated { get; set; }
        public object StockTransactionsCreated { get; set; }
        public object DisposalVouchersCreated { get; set; }
        public object ReceiptVouchersCreated { get; set; }
        public object TransferOperationsCreated { get; set; }
        public object IssueVouchersCreated { get; set; }
    }

    public static class CharityDemoSeeder
    {
        private static readonly string[] MaleNames = { "أحمد", "محمد", "محمود", "خالد", "عبدالله", "مصطفى", "هيثم", "حسام", "طارق", "وائل" };
        private static readonly string[] FemaleNames = { "فاطمة", "أسماء", "مريم", "آية", "رحاب", "هدى", "سارة", "نهى", "إيمان", "دعاء" };
        private static readonly string[] FamilyNames = { "عبدالسلام", "علي", "حسن", "عبدالحميد", "السيد", "مصطفى", "طه", "صالح", "مبارك", "النوبي" };
        private static readonly Random Rng = new(24040);

        public static async Task<CharityDemoSeedResult> SeedAsync(AppDbContext db, string? createdByUserId = null)
        {
            await db.Database.MigrateAsync();

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

            await EnsureOperationalAccountingProfilesAsync(db);
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
            await db.Set<ProjectPhase>().ExecuteDeleteAsync();
            await db.Set<ProjectExpenseLink>().ExecuteDeleteAsync();
            await db.Set<ProjectGrant>().ExecuteDeleteAsync();
            await db.Set<ProjectBeneficiary>().ExecuteDeleteAsync();
            await db.Set<ProjectActivity>().ExecuteDeleteAsync();
            await db.Set<ProjectBudgetLine>().ExecuteDeleteAsync();
            await db.Set<ProjectAccountingProfile>().ExecuteDeleteAsync();
            await db.Set<ProjectPhaseStoreIssueLink>().ExecuteDeleteAsync();
            await db.Set<CharityStoreIssueLine>().ExecuteDeleteAsync();
            await db.Set<CharityStoreIssue>().ExecuteDeleteAsync();
            await db.Set<CharityStoreReceiptLine>().ExecuteDeleteAsync();
            await db.Set<CharityStoreReceipt>().ExecuteDeleteAsync();
            await db.Set<StockNeedRequestLine>().ExecuteDeleteAsync();
            await db.Set<StockNeedRequest>().ExecuteDeleteAsync();
            await db.Set<StockReturnVoucherLine>().ExecuteDeleteAsync();
            await db.Set<StockReturnVoucher>().ExecuteDeleteAsync();
            await db.Set<StockDisposalVoucherLine>().ExecuteDeleteAsync();
            await db.Set<StockDisposalVoucher>().ExecuteDeleteAsync();
            await db.Set<CharityProject>().ExecuteDeleteAsync();

            await db.Set<AccountingIntegrationProfile>().ExecuteDeleteAsync();
            await db.Set<GrantInstallment>().ExecuteDeleteAsync();
            await db.Set<GrantCondition>().ExecuteDeleteAsync();
            await db.Set<GrantAgreement>().ExecuteDeleteAsync();
            await db.Set<Funder>().ExecuteDeleteAsync();

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

        private static async Task EnsureOperationalAccountingProfilesAsync(AppDbContext db)
        {
            var costCenter = await db.Set<CostCenter>().FirstOrDefaultAsync(x => x.CostCenterCode == "CC-DEMO-GENERAL");
            var asset = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.IsActive && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var revenue = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.IsActive && x.Category == AccountCategory.Revenue).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var expense = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.IsActive && x.Category == AccountCategory.Expense).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var liability = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.IsActive && x.Category == AccountCategory.Liability).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            if (asset == null || revenue == null || expense == null || liability == null)
                return;

            async Task EnsureProfileAsync(string sourceType, string profileNameAr, Guid? debitAccountId, Guid? creditAccountId, bool useSourceDebit, bool useSourceCredit)
            {
                if (await db.Set<AccountingIntegrationProfile>().AnyAsync(x => x.SourceType == sourceType))
                    return;

                await db.Set<AccountingIntegrationProfile>().AddAsync(new AccountingIntegrationProfile
                {
                    Id = Guid.NewGuid(),
                    SourceType = sourceType,
                    ProfileNameAr = profileNameAr,
                    DebitAccountId = debitAccountId,
                    CreditAccountId = creditAccountId,
                    UseSourceFinancialAccountAsDebit = useSourceDebit,
                    UseSourceFinancialAccountAsCredit = useSourceCredit,
                    DefaultCostCenterId = costCenter?.Id,
                    AutoPost = true,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await EnsureProfileAsync(AccountingSourceTypes.Donation, "ربط التبرعات النقدية", null, revenue.Id, true, false);
            await EnsureProfileAsync(AccountingSourceTypes.BeneficiaryAidDisbursement, "ربط صرف المساعدات", expense.Id, null, false, true);
            await EnsureProfileAsync(AccountingSourceTypes.GrantInstallment, "ربط دفعات التمويل", null, liability.Id, true, false);
            await EnsureProfileAsync(AccountingSourceTypes.Expense, "ربط المصروفات", expense.Id, asset.Id, false, false);
            await EnsureProfileAsync(AccountingSourceTypes.Payroll, "ربط المرتبات", expense.Id, asset.Id, false, false);
            await EnsureProfileAsync(AccountingSourceTypes.StoreIssue, "ربط الصرف المخزني", expense.Id, asset.Id, false, false);
            await EnsureProfileAsync(AccountingSourceTypes.StoreReceipt, "ربط الإضافة المخزنية", asset.Id, liability.Id, false, false);

            var demoProjects = await db.Set<CharityProject>()
                .Where(x => x.Code.StartsWith("PRJ-DEMO-"))
                .OrderBy(x => x.Code)
                .ToListAsync();

            foreach (var project in demoProjects)
            {
                if (await db.Set<ProjectAccountingProfile>().AnyAsync(x => x.ProjectId == project.Id))
                    continue;

                await db.Set<ProjectAccountingProfile>().AddAsync(new ProjectAccountingProfile
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    DefaultCostCenterId = costCenter?.Id,
                    DefaultRevenueAccountId = revenue.Id,
                    DefaultExpenseAccountId = expense.Id,
                    AutoTagJournalLinesWithProject = true,
                    AutoUseProjectCostCenter = true,
                    IsActive = true,
                    Notes = "Profile ديمو للمشروع"
                });
            }
        }

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

            await EnsureAidTypeAsync(db, "مساعدة مالية", "Financial Aid", 1);
            await EnsureAidTypeAsync(db, "مساعدة غذائية", "Food Aid", 2);
            await EnsureAidTypeAsync(db, "مساعدة صحية", "Health Aid", 3);
            await EnsureAidTypeAsync(db, "مساعدة تعليمية", "Educational Aid", 4);
            await EnsureAidTypeAsync(db, "تدريب", "Training", 5);

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
            var aidFinancial = await db.Set<AidTypeLookup>().FirstAsync(x => x.NameAr == "مساعدة مالية");
            var aidFood = await db.Set<AidTypeLookup>().FirstAsync(x => x.NameAr == "مساعدة غذائية");
            var cities = await db.Set<City>().ToListAsync();
            var areas = await db.Set<Area>().ToListAsync();
            var govs = await db.Set<Governorate>().ToListAsync();
            var cashMethod = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "CASH");
            var cashAccount = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            var beneficiaries = new List<Beneficiary>();
            var families = new List<BeneficiaryFamilyMember>();
            var decisions = new List<BeneficiaryCommitteeDecision>();
            var requests = new List<BeneficiaryAidRequest>();
            var disbursements = new List<BeneficiaryAidDisbursement>();

            for (var i = 1; i <= 18; i++)
            {
                var isMale = i % 3 == 0;
                var fullName = BuildName(isMale);
                var status = i <= 10 ? approvedStatus : reviewStatus;
                var city = cities[(i - 1) % cities.Count];
                var area = areas[(i - 1) % areas.Count];
                var gov = govs.First(x => x.Id == city.GovernorateId);
                var beneficiary = new Beneficiary
                {
                    Id = Guid.NewGuid(),
                    Code = $"BEN-DEMO-{i:000}",
                    FullName = fullName,
                    NationalId = $"2990{i:000000000}",
                    BirthDate = DateTime.Today.AddYears(-(26 + i)),
                    GenderId = isMale ? genderMale.Id : genderFemale.Id,
                    MaritalStatusId = i <= 14 ? married.Id : single.Id,
                    PhoneNumber = $"0105{Rng.Next(1000000, 9999999)}",
                    AlternatePhoneNumber = $"0122{Rng.Next(1000000, 9999999)}",
                    AddressLine = $"{gov.NameAr} - {city.NameAr} - {area.NameAr}",
                    GovernorateId = gov.Id,
                    CityId = city.Id,
                    AreaId = area.Id,
                    FamilyMembersCount = Rng.Next(3, 7),
                    MonthlyIncome = 1200 + (i * 170),
                    IncomeSource = i % 2 == 0 ? "يومية غير ثابتة" : "عمل بسيط",
                    HealthStatus = i % 4 == 0 ? "يحتاج متابعة صحية" : "مستقر",
                    EducationStatus = "تعليم متوسط",
                    WorkStatus = i % 2 == 0 ? "عمل متقطع" : "بدون عمل ثابت",
                    HousingStatus = i % 3 == 0 ? "إيجار" : "ملك أسرة",
                    Notes = "بيانات ديمو للمستفيد",
                    StatusId = status.Id,
                    RegistrationDate = DateTime.Today.AddDays(-Rng.Next(20, 180)),
                    CreatedByUserId = createdByUserId,
                    IsActive = true
                };
                beneficiaries.Add(beneficiary);

                for (var f = 1; f <= Math.Max(2, beneficiary.FamilyMembersCount - 1); f++)
                {
                    families.Add(new BeneficiaryFamilyMember
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        FullName = BuildFamilyMemberName(f % 2 == 0),
                        Relationship = f == 1 ? "زوج/زوجة" : "ابن/ابنة",
                        NationalId = $"300{Rng.Next(100000000, 999999999)}",
                        BirthDate = DateTime.Today.AddYears(-(8 + f * 4)),
                        GenderId = f % 2 == 0 ? genderFemale.Id : genderMale.Id,
                        EducationStatus = f == 1 ? "يقرأ ويكتب" : "طالب",
                        WorkStatus = f == 1 ? "لا يعمل" : "طالب",
                        MonthlyIncome = f == 1 ? 0 : null,
                        HealthCondition = "لا يوجد",
                        IsDependent = true,
                        Notes = "بيانات أسرة ديمو"
                    });
                }

                if (i <= 12)
                {
                    var decision = new BeneficiaryCommitteeDecision
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        DecisionDate = DateTime.Today.AddDays(-Rng.Next(10, 90)),
                        DecisionType = "Approve",
                        ApprovedAidTypeId = (i % 2 == 0 ? aidFinancial.Id : aidFood.Id),
                        ApprovedAmount = i % 2 == 0 ? 700 + (i * 25) : 0,
                        DurationInMonths = i <= 8 ? 6 : 3,
                        CommitteeNotes = "اعتماد مبدئي ضمن بيانات العرض",
                        ApprovedByUserId = createdByUserId
                    };
                    decisions.Add(decision);

                    var request = new BeneficiaryAidRequest
                    {
                        Id = Guid.NewGuid(),
                        BeneficiaryId = beneficiary.Id,
                        RequestDate = DateTime.Today.AddDays(-Rng.Next(5, 40)),
                        AidTypeId = i % 2 == 0 ? aidFinancial.Id : aidFood.Id,
                        RequestedAmount = i % 2 == 0 ? 800 + (i * 30) : 0,
                        Reason = i % 2 == 0 ? "مساعدة شهرية للأسرة" : "دعم غذائي عاجل",
                        UrgencyLevel = i % 3 == 0 ? "High" : "Medium",
                        Status = i <= 8 ? "Approved" : "Pending",
                        CreatedByUserId = createdByUserId
                    };
                    requests.Add(request);

                    if (i <= 8)
                    {
                        disbursements.Add(new BeneficiaryAidDisbursement
                        {
                            Id = Guid.NewGuid(),
                            BeneficiaryId = beneficiary.Id,
                            AidRequestId = request.Id,
                            AidTypeId = request.AidTypeId,
                            DisbursementDate = DateTime.Today.AddDays(-Rng.Next(1, 15)),
                            Amount = request.RequestedAmount,
                            PaymentMethodId = cashMethod.Id,
                            FinancialAccountId = cashAccount?.Id,
                            Notes = "صرف تجريبي لعرض النظام",
                            ApprovedByUserId = createdByUserId,
                            CreatedByUserId = createdByUserId
                        });
                    }
                }
            }

            await db.Set<Beneficiary>().AddRangeAsync(beneficiaries);
            await db.Set<BeneficiaryFamilyMember>().AddRangeAsync(families);
            await db.Set<BeneficiaryCommitteeDecision>().AddRangeAsync(decisions);
            await db.Set<BeneficiaryAidRequest>().AddRangeAsync(requests);
            await db.Set<BeneficiaryAidDisbursement>().AddRangeAsync(disbursements);

            return beneficiaries.Count;
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
                .Take(6)
                .ToListAsync();

            var forms = new List<BeneficiaryHumanitarianResearch>();
            var reviews = new List<BeneficiaryHumanitarianResearchReview>();
            var committee = new List<BeneficiaryHumanitarianResearchCommitteeEvaluation>();

            for (var i = 0; i < beneficiaries.Count; i++)
            {
                var ben = beneficiaries[i];
                var status = i switch
                {
                    0 => "ReviewedApproved",
                    1 => "ReturnedForRevision",
                    2 => "ReviewedRejected",
                    3 => "SentToCommittee",
                    4 => "CommitteeDecided",
                    _ => "SubmittedForReview"
                };

                var form = new BeneficiaryHumanitarianResearch
                {
                    Id = Guid.NewGuid(),
                    BeneficiaryId = ben.Id,
                    ResearchNumber = $"RES-DEMO-{i + 1:000}",
                    RequestDate = DateTime.Today.AddDays(-(30 + i)),
                    ResearchDate = DateTime.Today.AddDays(-(25 + i)),
                    AidTypeName = i % 2 == 0 ? "مساعدة مالية" : "مساعدة غذائية",
                    ApplicantName = ben.FullName,
                    SourceOfRequest = "باحث ميداني",
                    ResearcherCode = $"R-{i + 1:00}",
                    ResearcherName = $"باحث/{(i % 2 == 0 ? "أحمد" : "منى")} ميداني",
                    CommitteeCode = $"COM-{DateTime.Today.Year}-{i+1:00}",
                    PriorityLevel = i % 2 == 0 ? "عاجل" : "عادي",
                    FullName = ben.FullName,
                    NickName = ben.FullName.Split(' ').FirstOrDefault(),
                    Age = ben.BirthDate.HasValue ? DateTime.Today.Year - ben.BirthDate.Value.Year : null,
                    MaritalStatus = i % 2 == 0 ? "متزوج" : "أرمل",
                    NationalId = ben.NationalId,
                    AddressLine = ben.AddressLine,
                    Phone1 = ben.PhoneNumber,
                    Phone2 = ben.AlternatePhoneNumber,
                    FamilyMembersCount = ben.FamilyMembersCount,
                    TotalIncome = ben.MonthlyIncome,
                    TotalExpenses = (ben.MonthlyIncome ?? 0) + 500,
                    AverageIncome = ben.MonthlyIncome,
                    HasExistingProject = i == 5,
                    ExistingProjectType = i == 5 ? "مشروع صغير منزلي" : null,
                    ExistingProjectSize = i == 5 ? "محدود" : null,
                    RequiredNeedsPrimary = i % 2 == 0 ? "مساعدة مالية شهرية" : "دعم غذائي",
                    RequiredNeedsSecondary = "متابعة اجتماعية",
                    HousingDescription = "منزل بسيط مكون من غرفتين وحمام منفصل",
                    ResearcherReport = "تبين من المعاينة أن الأسرة محدودة الدخل وتحتاج دعمًا منتظمًا.",
                    ResearchManagerOpinion = "يوصى بعرض الحالة على اللجنة بعد استكمال المراجعة.",
                    Status = status,
                    SubmittedAtUtc = DateTime.UtcNow.AddDays(-(20 - i)),
                    SubmittedByUserId = createdByUserId,
                    ReviewedAtUtc = status is "ReviewedApproved" or "ReviewedRejected" or "ReturnedForRevision" or "SentToCommittee" or "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(15 - i)) : null,
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
                    ReviewReason = status switch
                    {
                        "ReviewedRejected" => "البيانات غير مكتملة ولا توجد مستندات كافية.",
                        "ReturnedForRevision" => "يرجى توضيح بند الدخل والمصروفات.",
                        _ => "تمت المراجعة والموافقة."
                    },
                    SentToCommitteeAtUtc = status is "SentToCommittee" or "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(10 - i)) : null,
                    CommitteeDecidedAtUtc = status == "CommitteeDecided" ? DateTime.UtcNow.AddDays(-(5 - i)) : null,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-(22 - i)),
                    
                };
                forms.Add(form);

                if (form.ReviewDecision != null)
                {
                    reviews.Add(new BeneficiaryHumanitarianResearchReview
                    {
                        Id = Guid.NewGuid(),
                        ResearchId = form.Id,
                        ReviewerUserId = createdByUserId ?? "demo-reviewer",
                        ReviewDateUtc = form.ReviewedAtUtc ?? DateTime.UtcNow,
                        Decision = form.ReviewDecision,
                        Reason = form.ReviewReason ?? "تمت المراجعة",
                        Notes = "سجل مراجعة تجريبي"
                    });
                }

                if (status == "CommitteeDecided")
                {
                    committee.Add(new BeneficiaryHumanitarianResearchCommitteeEvaluation
                    {
                        Id = Guid.NewGuid(),
                        ResearchId = form.Id,
                        CommitteeMeetingDate = DateTime.Today.AddDays(-3),
                        Decision = "ApproveAid",
                        ApprovedAidType = "مساعدة مالية",
                        ApprovedAmount = 900,
                        DurationMonths = 6,
                        Notes = "اعتماد الحالة ضمن حالات العرض التجريبي",
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
            for (var i = 1; i <= 8; i++)
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

            var donors = await db.Set<Donor>().Where(x => x.Code.StartsWith("DON-DEMO-")).ToListAsync();
            if (!donors.Any()) return 0;

            var pm = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "CASH");
            var account = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            var donations = new List<Donation>();
            for (var i = 1; i <= 15; i++)
            {
                donations.Add(new Donation
                {
                    Id = Guid.NewGuid(),
                    DonationNumber = $"DN-DEMO-{i:000}",
                    DonorId = donors[(i - 1) % donors.Count].Id,
                    DonationDate = DateTime.Today.AddDays(-Rng.Next(2, 100)),
                    DonationType = i % 4 == 0 ? "عيني" : "نقدي",
                    Amount = i % 4 == 0 ? 0 : 1500 + (i * 350),
                    PaymentMethodId = i % 4 == 0 ? null : pm.Id,
                    FinancialAccountId = i % 4 == 0 ? null : account?.Id,
                    IsRestricted = i % 3 == 0,
                    CampaignName = i % 5 == 0 ? "حملة رمضان" : null,
                    ReceiptNumber = $"REC-{i:000}",
                    ReferenceNumber = $"REF-{DateTime.Today:yyyyMM}-{i:000}",
                    Notes = "تبرع تجريبي لعرض النظام",
                    CreatedByUserId = createdByUserId
                });
            }

            await db.Set<Donation>().AddRangeAsync(donations);
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

            for (var i = 1; i <= 4; i++)
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

        private static async Task<int> SeedAidCyclesAsync(AppDbContext db, string? createdByUserId)
        {
            if (await db.Set<AidCycle>().AnyAsync(x => x.CycleNumber.StartsWith("AC-DEMO-")))
                return 0;

            var aidFinancial = await db.Set<AidTypeLookup>().FirstAsync(x => x.NameAr == "مساعدة مالية");
            var approvedDecisions = await db.Set<BeneficiaryCommitteeDecision>()
                .OrderByDescending(x => x.DecisionDate)
                .Take(10)
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

            for (var i = 1; i <= 10; i++)
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

            var payrollMonth = await db.Set<PayrollMonth>().FirstOrDefaultAsync(x => x.Year == DateTime.Today.Year && x.Month == DateTime.Today.Month);
            if (payrollMonth == null)
            {
                payrollMonth = new PayrollMonth
                {
                    Id = Guid.NewGuid(),
                    Year = DateTime.Today.Year,
                    Month = DateTime.Today.Month,
                    Status = "Approved",
                    ApprovedByUserId = createdByUserId,
                    ApprovedAtUtc = DateTime.UtcNow
                };
                await db.Set<PayrollMonth>().AddAsync(payrollMonth);
                result.PayrollMonths = 1;
            }

            if (!await db.Set<PayrollEmployee>().AnyAsync(x => x.PayrollMonthId == payrollMonth.Id))
            {
                var bankMethod = await db.Set<PaymentMethod>().FirstAsync(x => x.MethodCode == "BANK");
                var bankAccount = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
                var bonusRows = new List<HrEmployeeBonus>();
                var payrollEmployees = new List<PayrollEmployee>();
                var payrollItems = new List<PayrollEmployeeItem>();
                var payrollPayments = new List<PayrollPayment>();

                foreach (var e in employees.Take(8))
                {
                    var bonusAmount = Math.Round(e.BasicSalary * 0.08m, 2);
                    var deduction = Math.Round(e.BasicSalary * 0.03m, 2);
                    var gross = e.BasicSalary + bonusAmount;
                    var net = gross - deduction;
                    var pe = new PayrollEmployee
                    {
                        Id = Guid.NewGuid(),
                        PayrollMonthId = payrollMonth.Id,
                        EmployeeId = e.Id,
                        BasicSalary = e.BasicSalary,
                        AttendanceDeduction = deduction,
                        OtherDeductions = 0,
                        Additions = bonusAmount,
                        GrossAmount = gross,
                        TotalDeductions = deduction,
                        NetAmount = net,
                        Notes = "مسير رواتب ديمو"
                    };
                    payrollEmployees.Add(pe);
                    payrollItems.Add(new PayrollEmployeeItem { Id = Guid.NewGuid(), PayrollEmployeeId = pe.Id, ItemType = "Addition", Value = bonusAmount, Notes = "حافز ديمو" });
                    payrollItems.Add(new PayrollEmployeeItem { Id = Guid.NewGuid(), PayrollEmployeeId = pe.Id, ItemType = "Deduction", Value = deduction, Notes = "خصم انضباط ديمو" });
                    payrollPayments.Add(new PayrollPayment { Id = Guid.NewGuid(), PayrollEmployeeId = pe.Id, PaymentDate = DateTime.Today, PaymentMethodId = bankMethod.Id, FinancialAccountId = bankAccount?.Id, Amount = net, ReferenceNumber = $"PAY-DEMO-{e.Code}", Notes = "سداد ديمو" });
                    bonusRows.Add(new HrEmployeeBonus { Id = Guid.NewGuid(), EmployeeId = e.Id, BonusDate = DateTime.Today.AddDays(-2), BonusType = "Performance", Amount = bonusAmount, PayrollMonthId = payrollMonth.Id, IsApproved = true, ApprovedByUserId = createdByUserId, ApprovedAtUtc = DateTime.UtcNow.AddDays(-1), CreatedByUserId = createdByUserId, Reason = "مكافأة أداء ديمو", Notes = "مرتبطة بالمسير" });
                }

                await db.Set<PayrollEmployee>().AddRangeAsync(payrollEmployees);
                await db.Set<PayrollEmployeeItem>().AddRangeAsync(payrollItems);
                await db.Set<PayrollPayment>().AddRangeAsync(payrollPayments);
                if (!await db.Set<HrEmployeeBonus>().AnyAsync())
                {
                    await db.Set<HrEmployeeBonus>().AddRangeAsync(bonusRows);
                }
            }

            var fiscal = await db.Set<FiscalPeriod>().Where(x => x.IsOpen).OrderByDescending(x => x.StartDate).FirstAsync();
            var costCenter = await db.Set<CostCenter>().OrderBy(x => x.CostCenterCode).FirstAsync();
            var asset = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Asset).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var revenue = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Revenue).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var expense = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Expense).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();
            var liability = await db.Set<FinancialAccount>().Where(x => x.IsPosting && x.Category == AccountCategory.Liability).OrderBy(x => x.AccountCode).FirstOrDefaultAsync();

            if (asset != null && revenue != null && expense != null && liability != null && !await db.Set<JournalEntry>().AnyAsync(x => x.EntryNumber.StartsWith("JE-DEMO-")))
            {
                var donation = await db.Set<Donation>().OrderBy(x => x.DonationDate).FirstOrDefaultAsync(x => x.Amount.HasValue && x.Amount.Value > 0);
                var grantInst = await db.Set<GrantInstallment>().OrderBy(x => x.InstallmentNumber).FirstOrDefaultAsync(x => x.ReceivedAmount != null);
                var aid = await db.Set<BeneficiaryAidDisbursement>().OrderBy(x => x.DisbursementDate).FirstOrDefaultAsync(x => x.Amount.HasValue && x.Amount.Value > 0);
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
