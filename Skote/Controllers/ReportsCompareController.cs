using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class ReportsCompareController : Controller
    {
        private readonly AppDbContext _db;
        public ReportsCompareController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int year = 0, int month = 0, CancellationToken ct = default)
        {
            var today = DateTime.Today;
            if (year  == 0) year  = today.Year;
            if (month == 0) month = today.Month;

            var vm = await BuildAsync(year, month, ct);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Data(int year, int month, CancellationToken ct)
        {
            var vm = await BuildAsync(year, month, ct);
            return Json(vm);
        }

        private async Task<CompareVm> BuildAsync(int year, int month, CancellationToken ct)
        {
            // الشهر الحالي
            var cur  = new PeriodRange(year, month);
            // الشهر الماضي
            var prev = cur.PreviousMonth();
            // نفس الشهر السنة الماضية
            var yago = new PeriodRange(year - 1, month);

            return new CompareVm
            {
                Year = year, Month = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),

                // مستفيدون جدد
                NewBeneficiaries   = await CountAsync<Beneficiary>(x => x.RegistrationDate, cur,  ct),
                NewBeneficiariesPrev = await CountAsync<Beneficiary>(x => x.RegistrationDate, prev, ct),
                NewBeneficiariesYAgo = await CountAsync<Beneficiary>(x => x.RegistrationDate, yago, ct),

                // طلبات مساعدة
                AidRequests      = await CountAsync<BeneficiaryAidRequest>(x => x.CreatedAtUtc, cur,  ct),
                AidRequestsPrev  = await CountAsync<BeneficiaryAidRequest>(x => x.CreatedAtUtc, prev, ct),
                AidRequestsYAgo  = await CountAsync<BeneficiaryAidRequest>(x => x.CreatedAtUtc, yago, ct),

                // صرف المساعدات
                AidDisbursed     = await SumAsync<BeneficiaryAidDisbursement>(x => x.DisbursementDate, x => x.Amount ?? 0, cur,  ct),
                AidDisbursedPrev = await SumAsync<BeneficiaryAidDisbursement>(x => x.DisbursementDate, x => x.Amount ?? 0, prev, ct),
                AidDisbursedYAgo = await SumAsync<BeneficiaryAidDisbursement>(x => x.DisbursementDate, x => x.Amount ?? 0, yago, ct),

                // تبرعات
                Donations     = await SumAsync<Donation>(x => x.DonationDate, x => x.Amount ?? 0, cur,  ct),
                DonationsPrev = await SumAsync<Donation>(x => x.DonationDate, x => x.Amount ?? 0, prev, ct),
                DonationsYAgo = await SumAsync<Donation>(x => x.DonationDate, x => x.Amount ?? 0, yago, ct),

                // كفالات جديدة
                NewKafalas     = await CountAsync<KafalaCase>(x => x.StartDate, cur,  ct),
                NewKafalasPrev = await CountAsync<KafalaCase>(x => x.StartDate, prev, ct),
                NewKafalasYAgo = await CountAsync<KafalaCase>(x => x.StartDate, yago, ct),

                // تحصيل كفالات
                KafalaCollected     = await SumAsync<KafalaPayment>(x => x.PaymentDate, x => x.Amount, cur,  ct),
                KafalaCollectedPrev = await SumAsync<KafalaPayment>(x => x.PaymentDate, x => x.Amount, prev, ct),
                KafalaCollectedYAgo = await SumAsync<KafalaPayment>(x => x.PaymentDate, x => x.Amount, yago, ct),

                // مشروعات جديدة
                NewProjects     = await CountAsync<CharityProject>(x => x.StartDate, cur,  ct),
                NewProjectsPrev = await CountAsync<CharityProject>(x => x.StartDate, prev, ct),
                NewProjectsYAgo = await CountAsync<CharityProject>(x => x.StartDate, yago, ct),
            };
        }

        private Task<int> CountAsync<T>(
            System.Linq.Expressions.Expression<Func<T, DateTime>> dateExpr,
            PeriodRange p, CancellationToken ct) where T : class
        {
            var start = p.Start; var end = p.End;
            var param = dateExpr.Parameters[0];
            var body  = System.Linq.Expressions.Expression.AndAlso(
                System.Linq.Expressions.Expression.GreaterThanOrEqual(dateExpr.Body,
                    System.Linq.Expressions.Expression.Constant(start)),
                System.Linq.Expressions.Expression.LessThan(dateExpr.Body,
                    System.Linq.Expressions.Expression.Constant(end)));
            var pred = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, param);
            return _db.Set<T>().AsNoTracking().CountAsync(pred, ct);
        }

        private Task<decimal> SumAsync<T>(
            System.Linq.Expressions.Expression<Func<T, DateTime>> dateExpr,
            System.Linq.Expressions.Expression<Func<T, decimal>> amountExpr,
            PeriodRange p, CancellationToken ct) where T : class
        {
            var start = p.Start; var end = p.End;
            var param = dateExpr.Parameters[0];
            var body  = System.Linq.Expressions.Expression.AndAlso(
                System.Linq.Expressions.Expression.GreaterThanOrEqual(dateExpr.Body,
                    System.Linq.Expressions.Expression.Constant(start)),
                System.Linq.Expressions.Expression.LessThan(dateExpr.Body,
                    System.Linq.Expressions.Expression.Constant(end)));
            var pred = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, param);
            return _db.Set<T>().AsNoTracking().Where(pred).SumAsync(amountExpr, ct);
        }
    }

    public record PeriodRange(int Year, int Month)
    {
        public DateTime Start => new(Year, Month, 1);
        public DateTime End   => Start.AddMonths(1);
        public PeriodRange PreviousMonth() => Month == 1
            ? new(Year - 1, 12)
            : new(Year, Month - 1);
    }

    public class CompareVm
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = "";

        public int     NewBeneficiaries      { get; set; }
        public int     NewBeneficiariesPrev  { get; set; }
        public int     NewBeneficiariesYAgo  { get; set; }

        public int     AidRequests     { get; set; }
        public int     AidRequestsPrev { get; set; }
        public int     AidRequestsYAgo { get; set; }

        public decimal AidDisbursed     { get; set; }
        public decimal AidDisbursedPrev { get; set; }
        public decimal AidDisbursedYAgo { get; set; }

        public decimal Donations     { get; set; }
        public decimal DonationsPrev { get; set; }
        public decimal DonationsYAgo { get; set; }

        public int     NewKafalas     { get; set; }
        public int     NewKafalasPrev { get; set; }
        public int     NewKafalasYAgo { get; set; }

        public decimal KafalaCollected     { get; set; }
        public decimal KafalaCollectedPrev { get; set; }
        public decimal KafalaCollectedYAgo { get; set; }

        public int     NewProjects     { get; set; }
        public int     NewProjectsPrev { get; set; }
        public int     NewProjectsYAgo { get; set; }
    }
}
