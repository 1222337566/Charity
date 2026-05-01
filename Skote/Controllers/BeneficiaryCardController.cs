using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Workflow;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class BeneficiaryCardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BeneficiaryCardController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db          = db;
            _userManager = userManager;
        }

        // ═══ البطاقة الشاملة ═══
        public async Task<IActionResult> Index(Guid id, CancellationToken ct)
        {
            var ben = await _db.Set<Beneficiary>()
                .AsNoTracking()
                .Include(x => x.Gender)
                .Include(x => x.MaritalStatus)
                .Include(x => x.Governorate)
                .Include(x => x.City)
                .Include(x => x.Status)
                .Include(x => x.FamilyMembers)
                .Include(x => x.Assessments).ThenInclude(a => a.RecommendedAidType)
                .Include(x => x.CommitteeDecisions).ThenInclude(d => d.ApprovedAidType)
                .Include(x => x.AidRequests).ThenInclude(r => r.AidType)
                .Include(x => x.AidDisbursements).ThenInclude(d => d.AidType)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (ben == null) return NotFound();

            // Kafala cases
            var kafalas = await _db.Set<KafalaCase>()
                .AsNoTracking()
                .Include(x => x.Sponsor)
                .Where(x => x.BeneficiaryId == id)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

            // Contact logs
            var logs = await _db.Set<BeneficiaryContactLog>()
                .AsNoTracking()
                .Where(x => x.BeneficiaryId == id)
                .OrderByDescending(x => x.ContactDate)
                .ToListAsync(ct);

            // Workflow steps
            var workflowSteps = await _db.Set<WorkflowStep>()
                .AsNoTracking()
                .Where(x => x.EntityType == "AidRequest"
                         && ben.AidRequests.Select(r => r.Id).Contains(x.EntityId)
                         && x.IsActive)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(10)
                .ToListAsync(ct);

            // Priority score calculation
            var score = CalculatePriorityScore(ben);

            var vm = new BeneficiaryCardVm
            {
                Beneficiary    = ben,
                Kafalas        = kafalas,
                ContactLogs    = logs,
                WorkflowSteps  = workflowSteps,
                PriorityScore  = score,
                TotalDisbursed = ben.AidDisbursements.Sum(x => x.Amount ?? 0),
                LastAidDate    = ben.AidDisbursements.OrderByDescending(x => x.DisbursementDate)
                                    .FirstOrDefault()?.DisbursementDate,
                DaysSinceLastAid = ben.AidDisbursements.Any()
                    ? (int)(DateTime.Today - ben.AidDisbursements
                        .OrderByDescending(x => x.DisbursementDate)
                        .First().DisbursementDate).TotalDays
                    : -1,
                PendingFollowUps = logs.Count(x => x.FollowUpDate.HasValue
                                              && x.FollowUpDate.Value.Date <= DateTime.Today
                                              && x.Outcome != "Reached"),
            };

            return View(vm);
        }

        // ═══ إضافة سجل تواصل ═══
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContact(
            Guid beneficiaryId, string contactType, string outcome,
            string? subject, string? notes,
            DateTime? followUpDate, string? followUpNote,
            CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var log = new BeneficiaryContactLog
            {
                BeneficiaryId  = beneficiaryId,
                ContactType    = contactType,
                ContactDate    = DateTime.Today,
                Outcome        = outcome,
                Subject        = subject,
                Notes          = notes,
                FollowUpDate   = followUpDate,
                FollowUpNote   = followUpNote,
                CreatedByUserId = userId
            };
            _db.Set<BeneficiaryContactLog>().Add(log);
            await _db.SaveChangesAsync(ct);
            TempData["ContactSuccess"] = "تم تسجيل التواصل بنجاح";
            return RedirectToAction(nameof(Index), new { id = beneficiaryId });
        }

        // ═══ لوحة الإنذار المبكر ═══
        public async Task<IActionResult> EarlyWarning(CancellationToken ct)
        {
            var today = DateTime.Today;

            // مستفيدون لم يُصرف لهم منذ 6 أشهر (نشطون)
            var sixMonthsAgo = today.AddMonths(-6);
            var activeBens = await _db.Set<Beneficiary>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new { x.Id, x.FullName, x.Code, x.PhoneNumber,
                    LastDisbDate = x.AidDisbursements
                        .OrderByDescending(d => d.DisbursementDate)
                        .Select(d => (DateTime?)d.DisbursementDate)
                        .FirstOrDefault() })
                .ToListAsync(ct);

            var noDisbursement = activeBens
                .Where(x => !x.LastDisbDate.HasValue || x.LastDisbDate.Value < sixMonthsAgo)
                .Select(x => new EarlyWarningItem
                {
                    BeneficiaryId  = x.Id,
                    BeneficiaryName = x.FullName,
                    Code           = x.Code,
                    Phone          = x.PhoneNumber,
                    WarningType    = "no_aid",
                    Detail         = x.LastDisbDate.HasValue
                        ? $"آخر صرف: {x.LastDisbDate.Value:d/M/yyyy}"
                        : "لم يُصرف له أبداً",
                    DaysSince      = x.LastDisbDate.HasValue
                        ? (int)(today - x.LastDisbDate.Value).TotalDays : 9999
                })
                .OrderByDescending(x => x.DaysSince)
                .Take(30)
                .ToList();

            // طلبات مساعدة معلقة أكثر من 14 يوم
            var oldPending = await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Where(x => x.Status == "Pending"
                         && x.CreatedAtUtc < today.AddDays(-14))
                .OrderBy(x => x.CreatedAtUtc)
                .Take(20)
                .Select(x => new EarlyWarningItem
                {
                    BeneficiaryId   = x.BeneficiaryId,
                    BeneficiaryName = x.Beneficiary!.FullName,
                    Code            = x.Beneficiary.Code,
                    Phone           = x.Beneficiary.PhoneNumber,
                    WarningType     = "pending_request",
                    Detail          = $"طلب {x.AidType!.NameAr} منذ {(int)(today - x.CreatedAtUtc).TotalDays} يوم",
                    DaysSince       = (int)(today - x.CreatedAtUtc).TotalDays
                })
                .ToListAsync(ct);

            // متابعات تواصل مستحقة
            var overdueFollowUps = await _db.Set<BeneficiaryContactLog>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Where(x => x.FollowUpDate.HasValue
                         && x.FollowUpDate.Value.Date <= today
                         && x.Outcome != "Reached")
                .OrderBy(x => x.FollowUpDate)
                .Take(20)
                .Select(x => new EarlyWarningItem
                {
                    BeneficiaryId   = x.BeneficiaryId,
                    BeneficiaryName = x.Beneficiary!.FullName,
                    Code            = x.Beneficiary.Code,
                    Phone           = x.Beneficiary.PhoneNumber,
                    WarningType     = "followup_due",
                    Detail          = $"متابعة مستحقة: {x.FollowUpNote ?? x.Subject}",
                    DaysSince       = (int)(today - x.FollowUpDate!.Value).TotalDays
                })
                .ToListAsync(ct);

            // كفالات متأخرة
            var overdueKafalas = await _db.Set<KafalaCase>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.Sponsor)
                .Where(x => x.Status == "Active"
                         && x.NextDueDate.HasValue
                         && x.NextDueDate.Value.Date < today)
                .OrderBy(x => x.NextDueDate)
                .Take(20)
                .Select(x => new EarlyWarningItem
                {
                    BeneficiaryId   = x.BeneficiaryId,
                    BeneficiaryName = x.Beneficiary!.FullName,
                    Code            = x.Beneficiary.Code,
                    Phone           = x.Beneficiary.PhoneNumber,
                    WarningType     = "kafala_overdue",
                    Detail          = $"كفالة {x.CaseNumber} — الكفيل: {x.Sponsor!.FullName} — متأخرة {(int)(today - x.NextDueDate!.Value).TotalDays} يوم",
                    DaysSince       = (int)(today - x.NextDueDate!.Value).TotalDays
                })
                .ToListAsync(ct);

            return View(new EarlyWarningVm
            {
                NoDisbursement  = noDisbursement,
                OldPendingRequests = oldPending,
                OverdueFollowUps   = overdueFollowUps,
                OverdueKafalas     = overdueKafalas,
            });
        }

        private static int CalculatePriorityScore(Beneficiary ben)
        {
            int score = 0;
            // الدخل (أقل دخل = أعلى أولوية)
            if (ben.MonthlyIncome.HasValue)
            {
                if (ben.MonthlyIncome.Value == 0) score += 30;
                else if (ben.MonthlyIncome.Value < 500)  score += 25;
                else if (ben.MonthlyIncome.Value < 1000) score += 15;
                else if (ben.MonthlyIncome.Value < 2000) score += 5;
            }
            else score += 20;

            // عدد أفراد الأسرة
            if (ben.FamilyMembersCount >= 8) score += 20;
            else if (ben.FamilyMembersCount >= 5) score += 12;
            else if (ben.FamilyMembersCount >= 3) score += 6;

            // الحالة الصحية
            if (ben.HealthStatus?.Contains("مريض") == true || ben.HealthStatus?.Contains("معاق") == true)
                score += 20;

            // الحالة السكنية
            if (ben.HousingStatus?.Contains("إيجار") == true) score += 10;
            if (ben.HousingStatus?.Contains("بدون") == true)  score += 15;

            // حالة العمل
            if (ben.WorkStatus?.Contains("عاطل") == true || ben.WorkStatus?.Contains("بدون") == true)
                score += 10;

            return Math.Min(score, 100);
        }
    }

    // ═══ ViewModels ═══
    public class BeneficiaryCardVm
    {
        public Beneficiary          Beneficiary    { get; set; } = null!;
        public List<KafalaCase>     Kafalas        { get; set; } = new();
        public List<BeneficiaryContactLog> ContactLogs { get; set; } = new();
        public List<WorkflowStep>   WorkflowSteps  { get; set; } = new();
        public int     PriorityScore    { get; set; }
        public decimal TotalDisbursed   { get; set; }
        public DateTime? LastAidDate    { get; set; }
        public int     DaysSinceLastAid { get; set; }
        public int     PendingFollowUps { get; set; }
    }

    public class EarlyWarningVm
    {
        public List<EarlyWarningItem> NoDisbursement     { get; set; } = new();
        public List<EarlyWarningItem> OldPendingRequests { get; set; } = new();
        public List<EarlyWarningItem> OverdueFollowUps   { get; set; } = new();
        public List<EarlyWarningItem> OverdueKafalas     { get; set; } = new();

        public int TotalCount => NoDisbursement.Count + OldPendingRequests.Count
                               + OverdueFollowUps.Count + OverdueKafalas.Count;
    }

    public class EarlyWarningItem
    {
        public Guid    BeneficiaryId   { get; set; }
        public string  BeneficiaryName { get; set; } = "";
        public string  Code            { get; set; } = "";
        public string? Phone           { get; set; }
        public string  WarningType     { get; set; } = "";
        public string  Detail          { get; set; } = "";
        public int     DaysSince       { get; set; }
    }
}
