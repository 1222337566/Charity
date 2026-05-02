using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
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
    public class DailySummaryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DailySummaryController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db          = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var user   = await _userManager.GetUserAsync(User);
            var roles  = user != null
                ? (await _userManager.GetRolesAsync(user)).ToList()
                : new List<string>();
            var userId = _userManager.GetUserId(User)!;
            var today  = DateTime.Today;
            var todayEnd = today.AddDays(1);

            // ── مهام الـ Workflow المعلقة ──
            var pendingSteps = await _db.Set<WorkflowStep>()
                .AsNoTracking()
                .Where(x => x.Status == "Pending" && x.IsActive
                         && roles.Contains(x.AssignedRole))
                .OrderBy(x => x.CreatedAtUtc)
                .Take(20)
                .ToListAsync(ct);

            // ── طلبات مساعدة جديدة اليوم ──
            var newAidRequests = await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Where(x => x.CreatedAtUtc >= today && x.CreatedAtUtc < todayEnd)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(10)
                .ToListAsync(ct);

            // ── كفالات متأخرة ──
            var overdueKafalas = await _db.Set<KafalaCase>()
                .AsNoTracking()
                .Include(x => x.Sponsor)
                .Where(x => x.Status == "Active"
                         && x.NextDueDate.HasValue
                         && x.NextDueDate.Value.Date < today)
                .OrderBy(x => x.NextDueDate)
                .Take(10)
                .ToListAsync(ct);

            // ── إنجازات تستحق هذا الأسبوع ──
            var weekEnd = today.AddDays(7);
            var dueMilestones = await _db.Set<ProjectPhaseMilestone>()
                .AsNoTracking()
                .Include(x => x.Phase).ThenInclude(p => p!.Project)
                .Where(x => x.Status != "Completed" && x.IsActive
                         && x.DueDate.Date >= today
                         && x.DueDate.Date <= weekEnd)
                .OrderBy(x => x.DueDate)
                .Take(10)
                .ToListAsync(ct);

            // ── مراحل مشروعات متأخرة ──
            var delayedPhases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Where(x => x.Status != "Completed"
                         && x.PlannedEndDate.Date < today
                         && x.ProgressPercent < 100)
                .OrderBy(x => x.PlannedEndDate)
                .Take(8)
                .ToListAsync(ct);

            // ── تبرعات اليوم ──
            var todayDonations = await _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.Donor)
                .Where(x => x.DonationDate == today)
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .ToListAsync(ct);

            // ── إحصاء الأسبوع ──
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekAidReqs   = await _db.Set<BeneficiaryAidRequest>().CountAsync(x => x.CreatedAtUtc >= weekStart, ct);
            var weekDonAmount = await _db.Set<Donation>()
                .Where(x => x.DonationDate >= weekStart && x.Amount.HasValue)
                .SumAsync(x => x.Amount ?? 0, ct);
            var weekNewBens = await _db.Set<Beneficiary>()
                .CountAsync(x => x.RegistrationDate >= weekStart, ct);

            return View(new DailySummaryVm
            {
                Date            = today,
                UserName        = user?.UserName ?? "",
                Roles           = roles,
                PendingSteps    = pendingSteps,
                NewAidRequests  = newAidRequests,
                OverdueKafalas  = overdueKafalas,
                DueMilestones   = dueMilestones,
                DelayedPhases   = delayedPhases,
                TodayDonations  = todayDonations,
                WeekAidRequests = weekAidReqs,
                WeekDonations   = weekDonAmount,
                WeekNewBeneficiaries = weekNewBens,
            });
        }
    }

    public class DailySummaryVm
    {
        public DateTime Today { get; set; }
        public DateTime Date  { get; set; }
        public string   UserName { get; set; } = "";
        public List<string> Roles { get; set; } = new();

        public List<WorkflowStep> PendingSteps   { get; set; } = new();
        public List<BeneficiaryAidRequest> NewAidRequests { get; set; } = new();
        public List<KafalaCase> OverdueKafalas   { get; set; } = new();
        public List<ProjectPhaseMilestone> DueMilestones { get; set; } = new();
        public List<ProjectPhase> DelayedPhases  { get; set; } = new();
        public List<Donation> TodayDonations     { get; set; } = new();

        public int     WeekAidRequests       { get; set; }
        public decimal WeekDonations         { get; set; }
        public int     WeekNewBeneficiaries  { get; set; }

        public bool HasUrgent => OverdueKafalas.Any() || DelayedPhases.Any()
                              || PendingSteps.Any(x =>
                                    (DateTime.UtcNow - x.CreatedAtUtc).TotalDays >= 3);
    }
}
