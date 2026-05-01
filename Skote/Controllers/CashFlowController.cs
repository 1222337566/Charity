using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class CashFlowController : Controller
    {
        private readonly AppDbContext _db;
        public CashFlowController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int months = 6, CancellationToken ct = default)
        {
            var today     = DateTime.Today;
            var vm        = new CashFlowVm { Months = months };

            // ── الرصيد الحالي (تبرعات - صرف) ──
            var totalIn = await _db.Set<Donation>()
                .Where(x => x.Amount.HasValue).SumAsync(x => x.Amount!.Value, ct);
            var kafalaIn = await _db.Set<KafalaPayment>()
                .Where(x => x.Direction == "Received" && x.Status == "Confirmed")
                .SumAsync(x => x.Amount, ct);
            var grantIn = await _db.Set<GrantInstallment>()
                .Where(x => x.ReceivedAmount.HasValue).SumAsync(x => x.ReceivedAmount!.Value, ct);
            var totalOut = await _db.Set<BeneficiaryAidDisbursement>()
                .Where(x => x.Amount.HasValue).SumAsync(x => x.Amount!.Value, ct);
            var kafalaOut = await _db.Set<KafalaPayment>()
                .Where(x => x.Direction == "Disbursed" && x.Status == "Confirmed")
                .SumAsync(x => x.Amount, ct);
            var payrollOut = await _db.Set<PayrollEmployee>()
                .SumAsync(x => x.NetAmount, ct);

            vm.CurrentBalance = totalIn + kafalaIn + grantIn - totalOut - kafalaOut - payrollOut;

            // ── تقدير الرواتب الشهرية ──
            // ── تقدير الرواتب الشهرية — من PayrollMonth المعتمدة ──
            var recentPayroll = await _db.Set<PayrollEmployee>()
                .Include(x => x.PayrollMonth)
                .Where(x => x.PayrollMonth != null
                         && x.PayrollMonth.Status == "Approved"
                         && x.CreatedAtUtc >= today.AddMonths(-3))
                .Select(x => new {
                    Year      = x.PayrollMonth!.Year,
                    Month     = x.PayrollMonth!.Month,
                    NetAmount = x.NetAmount
                })
                .ToListAsync(ct);

            var lastMonthPayroll = recentPayroll
                .GroupBy(x => new { x.Year, x.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Select(g => g.Sum(x => x.NetAmount))
                .FirstOrDefault();

            var monthlyPayroll = lastMonthPayroll > 0 ? lastMonthPayroll : 0m;

            // ── الكفالات النشطة — دخل متوقع شهري ──
            var activeKafalas = await _db.Set<KafalaCase>()
                .Where(x => x.Status == "Active").ToListAsync(ct);
            var monthlyKafalaIncome = activeKafalas.Sum(k => k.Frequency switch {
                "Monthly"    => k.MonthlyAmount,
                "Quarterly"  => k.MonthlyAmount / 3,
                "SemiAnnual" => k.MonthlyAmount / 6,
                "Annual"     => k.MonthlyAmount / 12,
                _ => k.MonthlyAmount
            });

            // ── دورات الصرف المخططة ──
            var upcomingCycles = await _db.Set<AidCycle>()
                .Where(x => x.Status != "Disbursed" && x.Status != "Closed" && x.Status != "Cancelled"
                         && x.PlannedDisbursementDate >= today)
                .OrderBy(x => x.PlannedDisbursementDate)
                .Take(20)
                .ToListAsync(ct);

            // ── أقساط منح مستحقة ──
            var upcomingGrants = await _db.Set<GrantInstallment>()
                .Include(x => x.GrantAgreement)
                .Where(x => !x.ReceivedDate.HasValue && x.DueDate >= today)
                .OrderBy(x => x.DueDate)
                .Take(20)
                .ToListAsync(ct);

            // ── بناء الـ 6 أشهر القادمة ──
            for (int i = 0; i < months; i++)
            {
                var mStart = new DateTime(today.Year, today.Month, 1).AddMonths(i);
                var mEnd   = mStart.AddMonths(1);
                var label  = mStart.ToString("MMMM yyyy");

                // دخل — كفالات
                decimal inflow = monthlyKafalaIncome;

                // دخل — أقساط منح هذا الشهر
                inflow += upcomingGrants
                    .Where(g => g.DueDate >= mStart && g.DueDate < mEnd)
                    .Sum(g => g.Amount);

                // خروج — رواتب
                decimal outflow = monthlyPayroll;

                // خروج — دورات صرف هذا الشهر
                outflow += upcomingCycles
                    .Where(c => c.PlannedDisbursementDate >= mStart && c.PlannedDisbursementDate < mEnd)
                    .Sum(c => c.TotalPlannedAmount ?? 0);

                // خروج — كفالات هذا الشهر
                outflow += activeKafalas
                    .Where(k => k.NextDueDate.HasValue
                             && k.NextDueDate.Value >= mStart
                             && k.NextDueDate.Value < mEnd)
                    .Sum(k => k.MonthlyAmount);

                vm.Months_List.Add(new CashFlowMonth {
                    Label    = label,
                    Inflow   = inflow,
                    Outflow  = outflow,
                    NetFlow  = inflow - outflow,
                    Notes    = BuildNotes(upcomingCycles, upcomingGrants, mStart, mEnd)
                });
            }

            // ── المتبرعون الصامتون ──
            vm.SilentDonors = await _db.Set<Donor>().AsNoTracking()
                .Where(x => !x.Donations.Any()
                         || x.Donations.Max(d => d.DonationDate) < today.AddMonths(-6))
                .CountAsync(ct);

            // ── تنبيهات الميزانية ──
            vm.BudgetAlerts = await GetBudgetAlertsAsync(ct);

            return View(vm);
        }

        private async Task<List<BudgetAlert>> GetBudgetAlertsAsync(CancellationToken ct)
        {
            var alerts = new List<BudgetAlert>();
            var projects = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Projects.CharityProject>()
                .AsNoTracking()
                .Include(x => x.BudgetLines)
                .Where(x => x.IsActive && x.BudgetLines.Any())
                .ToListAsync(ct);

            foreach (var p in projects)
            {
                var planned = p.BudgetLines.Sum(b => b.PlannedAmount);
                var actual  = p.BudgetLines.Sum(b => b.ActualAmount);
                if (planned > 0)
                {
                    var pct = actual / planned * 100;
                    if (pct >= 80)
                        alerts.Add(new BudgetAlert {
                            Name    = p.Name,
                            Type    = "Project",
                            Percent = Math.Round(pct, 0),
                            Planned = planned,
                            Actual  = actual,
                            Url     = $"/ProjectTracking/Index?projectId={p.Id}"
                        });
                }
            }
            return alerts.OrderByDescending(x => x.Percent).ToList();
        }

        private static List<string> BuildNotes(
            List<AidCycle> cycles, List<GrantInstallment> grants,
            DateTime start, DateTime end)
        {
            var notes = new List<string>();
            var cyc = cycles.Where(c => c.PlannedDisbursementDate >= start && c.PlannedDisbursementDate < end).ToList();
            if (cyc.Any()) notes.Add($"{cyc.Count} دورة صرف مخططة");
            var gr  = grants.Where(g => g.DueDate >= start && g.DueDate < end).ToList();
            if (gr.Any()) notes.Add($"قسط منحة: {gr.Sum(g => g.Amount):N0}");
            return notes;
        }
    }

    public class CashFlowVm
    {
        public int     Months          { get; set; } = 6;
        public decimal CurrentBalance  { get; set; }
        public decimal SilentDonorsCount => SilentDonors;
        public int     SilentDonors    { get; set; }
        public List<CashFlowMonth> Months_List  { get; set; } = new();
        public List<BudgetAlert>   BudgetAlerts { get; set; } = new();
        public decimal TotalExpected   => Months_List.Sum(x => x.Inflow);
        public decimal TotalPlannedOut => Months_List.Sum(x => x.Outflow);
        public bool    IsFinanciallyHealthy => Months_List.All(x => x.CumulativeBalance >= 0);
    }

    public class CashFlowMonth
    {
        public string  Label   { get; set; } = "";
        public decimal Inflow  { get; set; }
        public decimal Outflow { get; set; }
        public decimal NetFlow { get; set; }
        public decimal CumulativeBalance { get; set; }
        public List<string> Notes { get; set; } = new();
        public bool IsNegative => NetFlow < 0;
    }

    public class BudgetAlert
    {
        public string  Name    { get; set; } = "";
        public string  Type    { get; set; } = "";
        public decimal Percent { get; set; }
        public decimal Planned { get; set; }
        public decimal Actual  { get; set; }
        public string? Url     { get; set; }
        public bool    IsCritical => Percent >= 95;
    }
}
