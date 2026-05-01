using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Reports;
using InfrastructureManagmentServices.Reporting;
using InfrastructureManagmentWebFramework.Models.RfpReports;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Skote.Controllers
{
    public class RfpReportsController : Controller
    {
        private readonly ICharityRfpReportRepository _repository;
        private readonly ICharityWordExportService _wordExportService;

        public RfpReportsController(ICharityRfpReportRepository repository, ICharityWordExportService wordExportService)
        {
            _repository = repository;
            _wordExportService = wordExportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExecutiveDashboard(DateTime? fromDate, DateTime? toDate)
        {
            var vm = new ExecutiveDashboardVm
            {
                Filter = new RfpReportFilterVm { FromDate = fromDate, ToDate = toDate },
                Summary = await _repository.GetDashboardSummaryAsync(fromDate, toDate)
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> HrEmployees()
        {
            var vm = new HrEmployeesPageVm
            {
                Rows = await _repository.GetHrEmployeesAsync()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Beneficiaries()
        {
            var vm = new BeneficiariesPageVm
            {
                Rows = await _repository.GetBeneficiariesAsync()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MonthlyAid(DateTime? fromDate, DateTime? toDate)
        {
            var vm = new MonthlyAidPageVm
            {
                Filter = new RfpReportFilterVm { FromDate = fromDate, ToDate = toDate },
                Rows = await _repository.GetMonthlyAidAsync(fromDate, toDate)
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectsActivities(DateTime? fromDate, DateTime? toDate)
        {
            var vm = new ProjectsActivitiesPageVm
            {
                Filter = new RfpReportFilterVm { FromDate = fromDate, ToDate = toDate },
                Rows = await _repository.GetProjectsActivitiesAsync(fromDate, toDate)
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> BoardMeetingsDecisions(DateTime? fromDate, DateTime? toDate)
        {
            var vm = new BoardMeetingsDecisionsPageVm
            {
                Filter = new RfpReportFilterVm { FromDate = fromDate, ToDate = toDate },
                Rows = await _repository.GetBoardMeetingsDecisionsAsync(fromDate, toDate)
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportWord(string reportName, DateTime? fromDate, DateTime? toDate)
        {
            string title;
            string html;

            switch ((reportName ?? string.Empty).ToLowerInvariant())
            {
                case "hremployees":
                    title = "تقرير الموظفين";
                    var employees = await _repository.GetHrEmployeesAsync();
                    html = BuildEmployeesHtml(employees);
                    break;
                case "beneficiaries":
                    title = "تقرير المستفيدين";
                    var beneficiaries = await _repository.GetBeneficiariesAsync();
                    html = BuildBeneficiariesHtml(beneficiaries);
                    break;
                case "monthlyaid":
                    title = "تقرير المساعدات الشهرية";
                    var aid = await _repository.GetMonthlyAidAsync(fromDate, toDate);
                    html = BuildMonthlyAidHtml(aid);
                    break;
                case "projectsactivities":
                    title = "تقرير المشروعات والأنشطة";
                    var activities = await _repository.GetProjectsActivitiesAsync(fromDate, toDate);
                    html = BuildProjectsActivitiesHtml(activities);
                    break;
                case "boardmeetingsdecisions":
                    title = "تقرير المحاضر والقرارات";
                    var decisions = await _repository.GetBoardMeetingsDecisionsAsync(fromDate, toDate);
                    html = BuildBoardDecisionsHtml(decisions);
                    break;
                default:
                    return NotFound();
            }

            var bytes = _wordExportService.BuildWordDocument(title, html);
            return File(bytes, "application/msword", $"{reportName}.doc");
        }

        private static string BuildEmployeesHtml(IEnumerable<dynamic> rows)
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>الكود</th><th>الاسم</th><th>الإدارة</th><th>الوظيفة</th><th>الحالة</th><th>الهاتف</th></tr></thead><tbody>");
            foreach (var x in rows)
                sb.Append($"<tr><td>{x.Code}</td><td>{x.FullName}</td><td>{x.DepartmentName}</td><td>{x.JobTitleName}</td><td>{x.Status}</td><td>{x.PhoneNumber}</td></tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private static string BuildBeneficiariesHtml(IEnumerable<dynamic> rows)
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>الكود</th><th>الاسم</th><th>الرقم القومي</th><th>عدد الأسرة</th><th>الحالة</th><th>تاريخ التسجيل</th></tr></thead><tbody>");
            foreach (var x in rows)
                sb.Append($"<tr><td>{x.Code}</td><td>{x.FullName}</td><td>{x.NationalId}</td><td>{x.FamilyMembersCount}</td><td>{x.StatusName}</td><td>{x.RegistrationDate:yyyy-MM-dd}</td></tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private static string BuildMonthlyAidHtml(IEnumerable<dynamic> rows)
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>الكود</th><th>المستفيد</th><th>نوع المساعدة</th><th>التاريخ</th><th>المبلغ</th><th>طريقة الدفع</th></tr></thead><tbody>");
            foreach (var x in rows)
                sb.Append($"<tr><td>{x.BeneficiaryCode}</td><td>{x.BeneficiaryName}</td><td>{x.AidTypeName}</td><td>{x.DisbursementDate:yyyy-MM-dd}</td><td>{x.Amount:n2}</td><td>{x.PaymentMethodName}</td></tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private static string BuildProjectsActivitiesHtml(IEnumerable<dynamic> rows)
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>كود المشروع</th><th>المشروع</th><th>النشاط</th><th>حالة النشاط</th><th>التاريخ المخطط</th><th>التكلفة الفعلية</th></tr></thead><tbody>");
            foreach (var x in rows)
                sb.Append($"<tr><td>{x.ProjectCode}</td><td>{x.ProjectName}</td><td>{x.ActivityTitle}</td><td>{x.ActivityStatus}</td><td>{x.PlannedDate:yyyy-MM-dd}</td><td>{x.ActualCost:n2}</td></tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private static string BuildBoardDecisionsHtml(IEnumerable<dynamic> rows)
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>رقم الاجتماع</th><th>عنوان الاجتماع</th><th>رقم القرار</th><th>عنوان القرار</th><th>الحالة</th><th>الاستحقاق</th></tr></thead><tbody>");
            foreach (var x in rows)
                sb.Append($"<tr><td>{x.MeetingNumber}</td><td>{x.MeetingTitle}</td><td>{x.DecisionNumber}</td><td>{x.DecisionTitle}</td><td>{x.DecisionStatus}</td><td>{x.DueDate:yyyy-MM-dd}</td></tr>");
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}
