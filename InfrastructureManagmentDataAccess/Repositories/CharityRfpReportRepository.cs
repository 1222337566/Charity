using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Reports;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Reports;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class CharityRfpReportRepository : ICharityRfpReportRepository
    {
        private readonly AppDbContext _db;

        public CharityRfpReportRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RfpDashboardSummary> GetDashboardSummaryAsync(DateTime? fromDate, DateTime? toDate)
        {
            var aidQuery = _db.Set<BeneficiaryAidDisbursement>().AsNoTracking();
            var projectQuery = _db.Set<CharityProject>().AsNoTracking();
            var meetingsQuery = _db.Set<BoardMeeting>().AsNoTracking();
            var decisionsQuery = _db.Set<BoardDecision>().AsNoTracking();

            if (fromDate.HasValue)
            {
                aidQuery = aidQuery.Where(x => x.DisbursementDate.Date >= fromDate.Value.Date);
                meetingsQuery = meetingsQuery.Where(x => x.MeetingDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                aidQuery = aidQuery.Where(x => x.DisbursementDate.Date <= toDate.Value.Date);
                meetingsQuery = meetingsQuery.Where(x => x.MeetingDate.Date <= toDate.Value.Date);
            }

            return new RfpDashboardSummary
            {
                EmployeesCount = await _db.Set<HrEmployee>().CountAsync(),
                ActiveEmployeesCount = await _db.Set<HrEmployee>().CountAsync(x => x.Status == "Active" || x.Status == "نشط"),
                BeneficiariesCount = await _db.Set<Beneficiary>().CountAsync(),
                ApprovedBeneficiariesCount = await _db.Set<Beneficiary>().CountAsync(x => x.Status != null && x.Status.NameAr == "معتمد"),
                MonthlyAidDisbursementsCount = await aidQuery.CountAsync(),
                MonthlyAidTotalAmount = await aidQuery.SumAsync(x => (decimal?)x.Amount) ?? 0m,
                ProjectsCount = await projectQuery.CountAsync(),
                ActiveProjectsCount = await projectQuery.CountAsync(x => x.Status != null && (x.Status == "Active" || x.Status == "جاري")),
                BoardMeetingsCount = await meetingsQuery.CountAsync(),
                BoardDecisionsCount = await decisionsQuery.CountAsync()
            };
        }

        public async Task<List<HrEmployeeReportRow>> GetHrEmployeesAsync()
        {
            return await _db.Set<HrEmployee>()
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.JobTitle)
                .OrderBy(x => x.Code)
                .Select(x => new HrEmployeeReportRow
                {
                    Id = x.Id,
                    Code = x.Code,
                    FullName = x.FullName,
                    DepartmentName = x.Department != null ? x.Department.Name : null,
                    JobTitleName = x.JobTitle != null ? x.JobTitle.Name : null,
                    HireDate = x.HireDate,
                    EmploymentType = x.EmploymentType,
                    BasicSalary = x.BasicSalary,
                    Status = x.Status,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<List<BeneficiaryReportRow>> GetBeneficiariesAsync(Guid? statusId = null)
        {
            var query = _db.Set<Beneficiary>()
                .AsNoTracking()
                .Include(x => x.Status)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(x => x.StatusId == statusId.Value);

            return await query
                .OrderByDescending(x => x.RegistrationDate)
                .Select(x => new BeneficiaryReportRow
                {
                    Id = x.Id,
                    Code = x.Code,
                    FullName = x.FullName,
                    NationalId = x.NationalId,
                    FamilyMembersCount = x.FamilyMembersCount,
                    MonthlyIncome = (decimal)x.MonthlyIncome,
                    StatusName = x.Status != null ? x.Status.NameAr : null,
                    RegistrationDate = x.RegistrationDate,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<List<MonthlyAidReportRow>> GetMonthlyAidAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.DisbursementDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(x => x.DisbursementDate.Date <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.DisbursementDate)
                .Select(x => new MonthlyAidReportRow
                {
                    BeneficiaryId = x.BeneficiaryId,
                    BeneficiaryCode = x.Beneficiary != null ? x.Beneficiary.Code : string.Empty,
                    BeneficiaryName = x.Beneficiary != null ? x.Beneficiary.FullName : string.Empty,
                    AidTypeName = x.AidType != null ? x.AidType.NameAr : null,
                    DisbursementDate = x.DisbursementDate,
                    Amount = (decimal)x.Amount,
                    PaymentMethodName = x.PaymentMethod != null ? x.PaymentMethod.MethodNameAr : null,
                    CycleName = x.Notes
                })
                .ToListAsync();
        }

        public async Task<List<ProjectActivityReportRow>> GetProjectsActivitiesAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Set<ProjectActivity>()
                .AsNoTracking()
                .Include(x => x.Project)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => !x.PlannedDate.Equals(null) || x.PlannedDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(x => !x.PlannedDate.Equals(null) || x.PlannedDate.Date <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.PlannedDate)
                .Select(x => new ProjectActivityReportRow
                {
                    ProjectId = x.ProjectId,
                    ProjectCode = x.Project != null ? x.Project.Code : string.Empty,
                    ProjectName = x.Project != null ? x.Project.Name : string.Empty,
                    ProjectStatus = x.Project != null && x.Project.Status != null ? x.Project.Status : null,
                    ActivityTitle = x.Title,
                    ActivityStatus = x.Status,
                    PlannedDate = x.PlannedDate,
                    ActualDate = x.ActualDate,
                    PlannedCost = x.PlannedCost,
                    ActualCost = x.ActualCost
                })
                .ToListAsync();
        }

        public async Task<List<BoardDecisionReportRow>> GetBoardMeetingsDecisionsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Set<BoardDecision>()
                .AsNoTracking()
                .Include(x => x.BoardMeeting)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.BoardMeeting != null && x.BoardMeeting.MeetingDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(x => x.BoardMeeting != null && x.BoardMeeting.MeetingDate.Date <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.BoardMeeting!.MeetingDate)
                .Select(x => new BoardDecisionReportRow
                {
                    MeetingId = x.BoardMeetingId,
                    MeetingNumber = x.BoardMeeting != null ? x.BoardMeeting.MeetingNumber : string.Empty,
                    MeetingTitle = x.BoardMeeting != null ? x.BoardMeeting.Title : string.Empty,
                    MeetingDate = x.BoardMeeting != null ? x.BoardMeeting.MeetingDate : DateTime.MinValue,
                    DecisionNumber = x.DecisionNumber,
                    DecisionTitle = x.Title,
                    DecisionStatus = x.Status,
                    ResponsibleParty = x.ResponsibleParty,
                    DueDate = x.DueDate
                })
                .ToListAsync();
        }
    }
}
