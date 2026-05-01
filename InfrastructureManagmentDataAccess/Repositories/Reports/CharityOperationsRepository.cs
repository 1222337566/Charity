using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastrfuctureManagmentCore.Queries.Reports;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

namespace InfrastructureManagmentDataAccess.Repositories.Reports
{
    public class CharityOperationsRepository : ICharityOperationsRepository
    {
        private readonly AppDbContext _db;
        public CharityOperationsRepository(AppDbContext db) => _db = db;

        public async Task<CharityWorkspaceSnapshotDto> GetWorkspaceSnapshotAsync(DateTime? today = null)
        {
            var now = (today ?? DateTime.Today).Date;
            var next14 = now.AddDays(14);
            var next30 = now.AddDays(30);
            var last7 = now.AddDays(-7);

            var activeEmployees = await _db.Set<HrEmployee>()
                .AsNoTracking()
                .CountAsync(x => x.IsActive && x.Status == "Active");

            var attendedToday = await _db.Set<HrAttendanceRecord>()
                .AsNoTracking()
                .Where(x => x.AttendanceDate == now)
                .Select(x => x.EmployeeId)
                .Distinct()
                .CountAsync();

            var snapshot = new CharityWorkspaceSnapshotDto
            {
                NewBeneficiariesCount = await _db.Set<Beneficiary>()
                    .AsNoTracking()
                    .CountAsync(x => x.RegistrationDate >= last7),

                PendingAidRequestsCount = await _db.Set<BeneficiaryAidRequest>()
                    .AsNoTracking()
                    .CountAsync(x => x.Status == "Pending"),

                DueGrantInstallmentsCount = await _db.Set<GrantInstallment>()
                    .AsNoTracking()
                    .CountAsync(x => !x.ReceivedDate.HasValue && x.DueDate <= next14),

                ProjectsEndingSoonCount = await _db.Set<CharityProject>()
                    .AsNoTracking()
                    .CountAsync(x => x.IsActive && x.EndDate.HasValue && x.EndDate.Value >= now && x.EndDate.Value <= next30 && x.Status != "Completed" && x.Status != "Cancelled"),

                MissingAttendanceCount = Math.Max(0, activeEmployees - attendedToday)
            };

            snapshot.QuickActions = new List<QuickActionDto>
            {
                new() { Title = "إضافة مستفيد", Description = "تسجيل حالة جديدة", Controller = "Beneficiaries", Action = "Create", IconClass = "bx bx-user-plus", ColorClass = "primary", Policy = CharityPolicies.BeneficiariesManage },
                new() { Title = "طلب مساعدة", Description = "تسجيل طلب جديد", Controller = "BeneficiaryAidRequests", Action = "Create", IconClass = "bx bx-file", ColorClass = "warning", Policy = CharityPolicies.BeneficiariesManage, BadgeText = snapshot.PendingAidRequestsCount > 0 ? $"{snapshot.PendingAidRequestsCount} pending" : null },
                new() { Title = "تسجيل تبرع", Description = "نقدي أو عيني", Controller = "Donations", Action = "Create", IconClass = "bx bx-donate-heart", ColorClass = "success", Policy = CharityPolicies.DonorsManage },
                new() { Title = "إذن صرف مخزني", Description = "صرف للمستفيد أو المشروع", Controller = "CharityStoreIssues", Action = "Create", IconClass = "bx bx-package", ColorClass = "info", Policy = CharityPolicies.StoresManage },
                new() { Title = "حضور اليوم", Description = "تسجيل حضور وانصراف", Controller = "HrAttendanceRecords", Action = "Create", IconClass = "bx bx-time-five", ColorClass = "secondary", Policy = CharityPolicies.HrManage, BadgeText = snapshot.MissingAttendanceCount > 0 ? $"{snapshot.MissingAttendanceCount} missing" : null },
                new() { Title = "شهر مرتبات", Description = "فتح أو مراجعة شهر المرتبات", Controller = "PayrollMonths", Action = "Index", IconClass = "bx bx-wallet-alt", ColorClass = "dark", Policy = CharityPolicies.PayrollView }
            };

            var alerts = new List<OperationalAlertDto>();
            if (snapshot.PendingAidRequestsCount > 0)
                alerts.Add(new OperationalAlertDto
                {
                    Title = "طلبات مساعدة معلقة",
                    Details = "يوجد طلبات ما زالت في حالة Pending وتحتاج مراجعة أو اعتماد.",
                    Severity = snapshot.PendingAidRequestsCount >= 10 ? "danger" : "warning",
                    Count = snapshot.PendingAidRequestsCount,
                    Controller = "BeneficiaryAidRequests",
                    Action = "Index",
                    BadgeText = "Pending"
                });

            var overdueInstallmentsCount = await _db.Set<GrantInstallment>()
                .AsNoTracking()
                .CountAsync(x => !x.ReceivedDate.HasValue && x.DueDate < now);

            if (overdueInstallmentsCount > 0)
                alerts.Add(new OperationalAlertDto
                {
                    Title = "دفعات تمويل متأخرة",
                    Details = "هناك دفعات تمويل تجاوزت تاريخ الاستحقاق ولم يتم تسجيل استلامها بعد.",
                    Severity = "danger",
                    Count = overdueInstallmentsCount,
                    Controller = "GrantInstallments",
                    Action = "Index",
                    BadgeText = "Overdue"
                });

            if (snapshot.ProjectsEndingSoonCount > 0)
                alerts.Add(new OperationalAlertDto
                {
                    Title = "مشروعات تقترب من الانتهاء",
                    Details = "يوجد مشروعات ستنتهي خلال 30 يومًا وتحتاج متابعة التنفيذ أو الإغلاق.",
                    Severity = "warning",
                    Count = snapshot.ProjectsEndingSoonCount,
                    Controller = "CharityProjects",
                    Action = "Index",
                    BadgeText = "30 days"
                });

            if (snapshot.MissingAttendanceCount > 0)
                alerts.Add(new OperationalAlertDto
                {
                    Title = "حضور اليوم غير مكتمل",
                    Details = "عدد من الموظفين النشطين لم يتم تسجيل حضورهم اليوم حتى الآن.",
                    Severity = snapshot.MissingAttendanceCount >= 5 ? "warning" : "info",
                    Count = snapshot.MissingAttendanceCount,
                    Controller = "HrAttendanceRecords",
                    Action = "Index",
                    BadgeText = "Today"
                });

            snapshot.Alerts = alerts;

            var deadlines = new List<DeadlineReminderDto>();

            var upcomingInstallments = await _db.Set<GrantInstallment>()
                .AsNoTracking()
                .Include(x => x.GrantAgreement)
                .Where(x => !x.ReceivedDate.HasValue && x.DueDate >= now && x.DueDate <= next14)
                .OrderBy(x => x.DueDate)
                .Take(6)
                .ToListAsync();

            deadlines.AddRange(upcomingInstallments.Select(x => new DeadlineReminderDto
            {
                Title = $"دفعة تمويل #{x.InstallmentNumber}",
                Category = "المنح والتمويل",
                DueDate = x.DueDate,
                DaysRemaining = (x.DueDate.Date - now).Days,
                Notes = x.GrantAgreement != null ? x.GrantAgreement.Title : null,
                Controller = "GrantInstallments",
                Action = "Index",
                RouteId = x.GrantAgreementId,
                RouteKey = "grantAgreementId"
            }));

            var endingProjects = await _db.Set<CharityProject>()
                .AsNoTracking()
                .Where(x => x.IsActive && x.EndDate.HasValue && x.EndDate.Value >= now && x.EndDate.Value <= next30 && x.Status != "Completed" && x.Status != "Cancelled")
                .OrderBy(x => x.EndDate)
                .Take(6)
                .ToListAsync();

            deadlines.AddRange(endingProjects.Select(x => new DeadlineReminderDto
            {
                Title = x.Name,
                Category = "المشروعات",
                DueDate = x.EndDate!.Value,
                DaysRemaining = (x.EndDate!.Value.Date - now).Days,
                Notes = x.Code,
                Controller = "CharityProjects",
                Action = "Details",
                RouteId = x.Id,
                RouteKey = "id"
            }));

            var endingAgreements = await _db.Set<GrantAgreement>()
                .AsNoTracking()
                .Where(x => x.EndDate.HasValue && x.EndDate.Value >= now && x.EndDate.Value <= next30 && x.Status != "Closed" && x.Status != "Cancelled")
                .OrderBy(x => x.EndDate)
                .Take(4)
                .ToListAsync();

            deadlines.AddRange(endingAgreements.Select(x => new DeadlineReminderDto
            {
                Title = x.Title,
                Category = "اتفاقيات التمويل",
                DueDate = x.EndDate!.Value,
                DaysRemaining = (x.EndDate!.Value.Date - now).Days,
                Notes = x.AgreementNumber,
                Controller = "GrantAgreements",
                Action = "Edit",
                RouteId = x.Id,
                RouteKey = "id"
            }));

            snapshot.Deadlines = deadlines
                .OrderBy(x => x.DueDate)
                .Take(10)
                .ToList();

            var recentActivities = new List<RecentActivityDto>();

            var recentDonations = await _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.Donor)
                .OrderByDescending(x => x.DonationDate)
                .Take(5)
                .ToListAsync();
            recentActivities.AddRange(recentDonations.Select(x => new RecentActivityDto
            {
                Title = $"تبرع {x.DonationNumber}",
                Subtitle = x.Donor != null ? x.Donor.FullName : x.DonationType,
                EventDate = x.DonationDate,
                Source = "التبرعات",
                Controller = "Donations",
                Action = "Index"
            }));

            var recentDisbursements = await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .OrderByDescending(x => x.DisbursementDate)
                .Take(5)
                .ToListAsync();
            recentActivities.AddRange(recentDisbursements.Select(x => new RecentActivityDto
            {
                Title = "صرف مساعدة",
                Subtitle = x.Beneficiary != null ? x.Beneficiary.FullName : string.Empty,
                EventDate = x.DisbursementDate,
                Source = "المستفيدون",
                Controller = "BeneficiaryAidDisbursements",
                Action = "Index",
                RouteId = x.BeneficiaryId,
                RouteKey = "beneficiaryId"
            }));

            var recentIssues = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .OrderByDescending(x => x.IssueDate)
                .Take(5)
                .ToListAsync();
            recentActivities.AddRange(recentIssues.Select(x => new RecentActivityDto
            {
                Title = $"إذن صرف {x.IssueNumber}",
                Subtitle = x.Warehouse != null ? x.Warehouse.WarehouseNameAr : x.IssueType,
                EventDate = x.IssueDate,
                Source = "المخازن",
                Controller = "CharityStoreIssues",
                Action = "Details",
                RouteId = x.Id,
                RouteKey = "id"
            }));

            var recentPayrollMonths = await _db.Set<PayrollMonth>()
                .AsNoTracking()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Take(3)
                .ToListAsync();
            recentActivities.AddRange(recentPayrollMonths.Select(x => new RecentActivityDto
            {
                Title = $"مرتبات {x.Month:00}/{x.Year}",
                Subtitle = x.Status,
                EventDate = x.CreatedAtUtc,
                Source = "المرتبات",
                Controller = "PayrollEmployees",
                Action = "Index",
                RouteId = x.Id,
                RouteKey = "payrollMonthId"
            }));

            snapshot.RecentActivities = recentActivities
                .OrderByDescending(x => x.EventDate)
                .Take(12)
                .ToList();

            return snapshot;
        }
    }
}
