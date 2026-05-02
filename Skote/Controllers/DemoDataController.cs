using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.EntityFramework.EF;
using InfrastructureManagmentWeb.Seeding;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastructureManagmentDataAccess.EntityFramework.EF2;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skote.Seeding;
using Skote.ViewModels.DemoData;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class DemoDataController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IServiceProvider _services;

        public DemoDataController(AppDbContext db, IServiceProvider services)
        {
            _db = db;
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new DemoDataIndexVm
            {
                EmployeesCount = await _db.Set<HrEmployee>().CountAsync(),
                BeneficiariesCount = await _db.Set<Beneficiary>().CountAsync(),
                HumanResearchCount = await _db.Set<BeneficiaryHumanitarianResearch>().CountAsync(),
                DonorsCount = await _db.Set<Donor>().CountAsync(),
                DonationsCount = await _db.Set<Donation>().CountAsync(),
                ProjectsCount = await _db.Set<CharityProject>().CountAsync(),
                AidCyclesCount = await _db.Set<AidCycle>().CountAsync(),
                VolunteersCount = await _db.Set<Volunteer>().CountAsync(),
                BoardMeetingsCount = await _db.Set<BoardMeeting>().CountAsync(),
                ProjectProposalsCount = await _db.Set<ProjectProposal>().CountAsync(),
                FundersCount = await _db.Set<Funder>().CountAsync(),
                GrantAgreementsCount = await _db.Set<GrantAgreement>().CountAsync(),
                GrantInstallmentsCount = await _db.Set<GrantInstallment>().CountAsync(),
                PayrollMonthsCount = await _db.Set<PayrollMonth>().CountAsync(),
                JournalEntriesCount = await _db.Set<JournalEntry>().CountAsync(),
                EmployeeContractsCount = await _db.Set<HrEmployeeContract>().CountAsync(),
                EmployeeFundingAssignmentsCount = await _db.Set<HrEmployeeFundingAssignment>().CountAsync(),
                EmployeeTaskAssignmentsCount = await _db.Set<HrEmployeeTaskAssignment>().CountAsync(),
                EmployeeBonusesCount = await _db.Set<HrEmployeeBonus>().CountAsync(),
                WarehousesCount = await _db.Set<Warehouse>().CountAsync(),
                StockItemsCount = await _db.Set<Item>().CountAsync(x => x.IsStockItem && !x.IsService),
                StoreReceiptsCount = await _db.Set<CharityStoreReceipt>().CountAsync(),
                StoreIssuesCount = await _db.Set<CharityStoreIssue>().CountAsync(),
                StockNeedRequestsCount = await _db.Set<StockNeedRequest>().CountAsync(),
                StockReturnVouchersCount = await _db.Set<StockReturnVoucher>().CountAsync(),
                StockDisposalVouchersCount = await _db.Set<StockDisposalVoucher>().CountAsync(),
                StockTransactionsCount = await _db.Set<StockTransaction>().CountAsync(),
                StockBalancesCount = await _db.Set<ItemWarehouseBalance>().CountAsync(),
                WorkflowUsersCount = await _db.Set<ApplicationUser>().CountAsync(x => x.UserName != null && x.UserName.StartsWith(CharityWorkflowUsersSeeder.UserNamePrefix)),
                LeaveRequestsCount = await _db.Set<HrLeaveRequest>().CountAsync(),
                LeaveBalancesCount  = await _db.Set<HrLeaveBalance>().CountAsync(),
                KafaCasesCount      = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Kafala.KafalaCase>().CountAsync(),
                LastResultMessage = TempData["DemoSeedMessage"] as string
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await CharityDemoSeeder.SeedAsync(_db, userId);

            TempData["DemoSeedMessage"] =
                $"تم إدخال بيانات تجريبية مترابطة: موظفين {result.EmployeesCreated}، عقود {result.EmployeeContractsCreated}، ربط تمويل موظفين {result.EmployeeFundingAssignmentsCreated}، " +
                $"مستفيدين {result.BeneficiariesCreated}، بحوث {result.HumanResearchFormsCreated}، متبرعين {result.DonorsCreated}، تبرعات {result.DonationsCreated}، " +
                $"ممولين {result.FundersCreated}، اتفاقيات {result.GrantAgreementsCreated}، دفعات منح {result.GrantInstallmentsCreated}، مشروعات {result.ProjectsCreated}، " +
                $"مخازن {result.WarehousesCreated}، أصناف مخزنية {result.ItemsCreated}، أرصدة افتتاحية {result.OpeningEntriesCreated}، أذون إضافة {result.ReceiptVouchersCreated}، أذون صرف {result.IssueVouchersCreated}، " +
                $"طلبات احتياج {result.NeedRequestsCreated}، مرتجعات {result.ReturnVouchersCreated}، إعدام {result.DisposalVouchersCreated}، تحويلات {result.TransferOperationsCreated}، " +
                $"دورات صرف {result.AidCyclesCreated}، متطوعين {result.VolunteersCreated}، اجتماعات {result.BoardMeetingsCreated}، مقترحات {result.ProjectProposalsCreated}، " +
                $"مسيرات رواتب {result.PayrollMonthsCreated}، قيود يومية {result.JournalEntriesCreated}، حركات مخزنية {result.StockTransactionsCreated}.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAndSeedNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await CharityDemoSeeder.ResetAndSeedAsync(_db, userId);
            TempData["DemoSeedMessage"] =
                $"تم مسح بيانات الموديولات التشغيلية ثم إعادة تعبئتها ببيانات ديمو مترابطة: موظفين {result.EmployeesCreated}، مستفيدين {result.BeneficiariesCreated}، متبرعين {result.DonorsCreated}، ممولين {result.FundersCreated}، مشروعات {result.ProjectsCreated}، مخازن {result.WarehousesCreated}، أصناف {result.ItemsCreated}، أذون إضافة {result.ReceiptVouchersCreated}، أذون صرف {result.IssueVouchersCreated}، وتحويلات {result.TransferOperationsCreated}، محاسبة/قيود {result.JournalEntriesCreated}.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedLargeDemo(
            int beneficiaryCount = 8000,
            int batchSize = 500,
            int donorCount = 800,
            int cashDonationCount = 1200,
            int inKindDonationCount = 240)
        {
            var options = BuildLargeDemoSeedOptions(
                resetExisting: false,
                beneficiaryCount,
                batchSize,
                donorCount,
                cashDonationCount,
                inKindDonationCount);

            var result = await CharityLargeDemoSeeder.SeedAsync(_db, options);
            TempData["DemoSeedMessage"] = FormatLargeDemoSeedMessage(result, options, "تم تشغيل Seeder البيانات الكبيرة");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAndSeedLargeDemo(
            int beneficiaryCount = 8000,
            int batchSize = 500,
            int donorCount = 800,
            int cashDonationCount = 1200,
            int inKindDonationCount = 240)
        {
            var options = BuildLargeDemoSeedOptions(
                resetExisting: true,
                beneficiaryCount,
                batchSize,
                donorCount,
                cashDonationCount,
                inKindDonationCount);

            var result = await CharityLargeDemoSeeder.SeedAsync(_db, options);
            TempData["DemoSeedMessage"] = FormatLargeDemoSeedMessage(result, options, "تم مسح بيانات الديمو الكبيرة ثم إعادة توليدها");
            return RedirectToAction(nameof(Index));
        }

        private static CharityLargeDemoSeedOptions BuildLargeDemoSeedOptions(
            bool resetExisting,
            int beneficiaryCount,
            int batchSize,
            int donorCount,
            int cashDonationCount,
            int inKindDonationCount)
        {
            return new CharityLargeDemoSeedOptions
            {
                Enabled = true,
                ResetExisting = resetExisting,
                BeneficiaryCount = Math.Clamp(beneficiaryCount <= 0 ? 8000 : beneficiaryCount, 1, 50000),
                BatchSize = Math.Clamp(batchSize <= 0 ? 500 : batchSize, 100, 2000),
                DonorCount = Math.Clamp(donorCount <= 0 ? 800 : donorCount, 50, 5000),
                CashDonationCount = Math.Clamp(cashDonationCount <= 0 ? 1200 : cashDonationCount, 100, 10000),
                InKindDonationCount = Math.Clamp(inKindDonationCount <= 0 ? 240 : inKindDonationCount, 20, 3000)
            };
        }

        private static string FormatLargeDemoSeedMessage(CharityLargeDemoSeedResult result, CharityLargeDemoSeedOptions options, string title)
        {
            if (result.Skipped)
            {
                return "لم يتم تشغيل Seeder البيانات الكبيرة لأن بيانات الديمو الكبيرة موجودة بالفعل. استخدم زر المسح وإعادة التوليد إذا أردت إعادة إنشائها.";
            }

            return $"{title}: مستفيدين {result.Beneficiaries} من أصل {options.BeneficiaryCount}، أفراد أسرة {result.FamilyMembers}، بحوث إنسانية {result.ResearchForms}، " +
                   $"طلبات مساعدة {result.AidRequests}، تبرعات {result.Donations}، تخصيصات {result.DonationAllocations}، سجلات صرف {result.Disbursements}، " +
                   $"مشروعات {result.Projects}، أذون إضافة {result.StoreReceipts}، أذون صرف {result.StoreIssues}، قيود يومية {result.JournalEntries}.";
        }

        // ══════════════════════════════════════════════════
        //  إصلاح حسابات التبرعات — يربط التبرعات النقدية بحساب مالي
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> FixDonationAccounts()
        {
            var cashAccountId = await InfrastructureManagmentDataAccess.EntityFramework.EF2
                .LargeScaleSeeder.GetCashAccountAsync(_db);

            if (cashAccountId == null)
            {
                TempData["DemoSeedMessage"] = "❌ لا توجد حسابات مالية. شغّل AccountSeeder أولاً.";
                return RedirectToAction(nameof(Index));
            }

            // 1. ربط التبرعات النقدية بلا حساب
            var updated = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation>()
                .Where(x => x.DonationType == "نقدي" && x.FinancialAccountId == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.FinancialAccountId, cashAccountId)
                );

            // 2. ضبط TargetingScopeCode للتبرعات النقدية بلا scope
            var scopeFixed = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Donors.Donation>()
                .Where(x => x.DonationType == "نقدي"
                          && (x.TargetingScopeCode == null || x.TargetingScopeCode == ""))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.TargetingScopeCode, "GeneralFund")
                );

            TempData["DemoSeedMessage"] =
                $"✅ تم إصلاح حسابات التبرعات — " +
                $"ربط بحساب مالي: {updated} تبرع · ضبط نطاق: {scopeFixed} تبرع";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        //  Large Scale Seeder — 10,000 مستفيد + موظفين + كل الموديولات
        // ══════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedLargeScale()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await LargeScaleSeeder.SeedAsync(_db, userId);
                var benCount  = await _db.Set<Beneficiary>().CountAsync();
                var empCount  = await _db.Set<HrEmployee>().CountAsync();
                var donCount  = await _db.Set<Donation>().CountAsync();
                var kafCount  = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Kafala.KafalaCase>().CountAsync();
                var lvCount   = await _db.Set<HrLeaveRequest>().CountAsync();
                TempData["DemoSeedMessage"] =
                    $"✅ اكتمل LargeScaleSeeder — " +
                    $"المستفيدون: {benCount:N0} · الموظفون: {empCount} · " +
                    $"التبرعات: {donCount:N0} · الكفالات: {kafCount:N0} · الإجازات: {lvCount}";
            }
            catch (Exception ex)
            {
                TempData["DemoSeedMessage"] = $"❌ خطأ: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedWorkflowUsers()
        {
            var users = await CharityWorkflowUsersSeeder.SeedAsync(_services);
            TempData["DemoSeedMessage"] =
                $"تم إنشاء/تحديث مستخدمي المحاكاة. جديد: {users.Created}، محدث: {users.Updated}. كلمة المرور الافتراضية: {users.DefaultPassword}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetWorkflowUsers()
        {
            var users = await CharityWorkflowUsersSeeder.SeedAsync(_services, reset: true);
            TempData["DemoSeedMessage"] =
                $"تم إعادة إنشاء مستخدمي المحاكاة. محذوف: {users.Deleted}، جديد: {users.Created}، محدث: {users.Updated}. كلمة المرور الافتراضية: {users.DefaultPassword}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
