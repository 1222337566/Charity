using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Skote.Helpers;

public sealed class CharityControllerAccessEntry
{
    public string Group { get; init; } = string.Empty;
    public string Controller { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? ViewPolicy { get; init; }
    public string? ManagePolicy { get; init; }
    public IReadOnlyDictionary<string, string> ActionPolicies { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

public static class CharityControllerAccessMap
{
    private static readonly HashSet<string> ManageActionNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Create", "Edit", "Delete", "Remove", "BatchDisburse", "Disburse", "Approve", "Reject", "SubmitForReview",
        "Review", "Committee", "GenerateCycle", "Generate", "Attach", "Upload", "Import", "Post", "Close", "Open",
        "Assign", "Unassign", "ResetAndSeedNow", "SeedDefaults"
    };

    public static CharityControllerAccessEntry? GetEntry(string? controllerName)
    {
        if (string.IsNullOrWhiteSpace(controllerName))
        {
            return null;
        }

        var controller = controllerName.Trim();

        return controller switch
        {
            "Dashboard3" => Entry("الرئيسية", controller, "لوحة التحكم", CharityPolicies.DashboardView),
            "DailySummary" => Entry("الرئيسية", controller, "ملخص اليوم", CharityPolicies.DashboardView),
            "Search" => Entry("الرئيسية", controller, "البحث الموحد", CharityPolicies.DashboardView),
            "CharityWorkspace" => Entry("الرئيسية", controller, "مساحة العمل اليومية", CharityPolicies.DashboardView),
            "CharityDashboard" => Entry("الرئيسية", controller, "لوحة مؤشرات الجمعية", CharityPolicies.DashboardView),

            "Beneficiaries" => Entry("المستفيدون", controller, "المستفيدون", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryAidRequests" => Entry("المستفيدون", controller, "طلبات المساعدة", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryAssessments" => Entry("المستفيدون", controller, "التقييمات", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryDocuments" => Entry("المستفيدون", controller, "مستندات المستفيد", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryFamilyMembers" => Entry("المستفيدون", controller, "أفراد الأسرة", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryOldRecords" => Entry("المستفيدون", controller, "السجلات القديمة", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage),
            "BeneficiaryHumanitarianResearchs" => Entry(
                "المستفيدون", controller, "استمارات البحث الإنساني", CharityPolicies.BeneficiariesView, CharityPolicies.BeneficiariesManage,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Review"] = CharityPolicies.ResearchReview,
                    ["SendToCommittee"] = CharityPolicies.ResearchReview,
                    ["Committee"] = CharityPolicies.CommitteeDecisionManage
                }),
            "BeneficiaryCommitteeDecisions" => Entry("المستفيدون", controller, "قرارات اللجنة", CharityPolicies.BeneficiariesView, CharityPolicies.CommitteeDecisionManage),
            "BeneficiaryAidDisbursements" => Entry("المستفيدون", controller, "سجل الصرف", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),
            "AidCycles" => Entry("المستفيدون", controller, "دورات الصرف", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),
            "AidCycleBeneficiaries" => Entry("المستفيدون", controller, "مستحقو دورة الصرف", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),
            "AidCycleAlerts" => Entry("المستفيدون", controller, "تنبيهات الصرف", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),

            "Donors" => Entry("التبرعات", controller, "المتبرعون", CharityPolicies.DonorsView, CharityPolicies.DonorsManage),
            "Donations" => Entry("التبرعات", controller, "التبرعات", CharityPolicies.DonorsView, CharityPolicies.DonorsManage),
            "DonationAllocations" => Entry("التبرعات", controller, "تخصيص التبرعات", CharityPolicies.DonorsView, CharityPolicies.DonorsManage),
            "DonationInKindItems" => Entry("التبرعات", controller, "التبرعات العينية", CharityPolicies.DonorsView, CharityPolicies.DonorsManage),

            "Funders" => Entry("التمويل", controller, "الممولون", CharityPolicies.FundersView, CharityPolicies.FundersManage),
            "GrantAgreements" => Entry("التمويل", controller, "اتفاقيات التمويل", CharityPolicies.FundersView, CharityPolicies.FundersManage),
            "GrantInstallments" => Entry("التمويل", controller, "دفعات التمويل", CharityPolicies.FundersView, CharityPolicies.FundersManage),
            "GrantConditions" => Entry("التمويل", controller, "شروط المنح", CharityPolicies.FundersView, CharityPolicies.FundersManage),

            "KafalaSponsors" => Entry("الكفالات", controller, "الكفلاء", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),
            "KafalaCases" => Entry("الكفالات", controller, "حالات الكفالة", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),
            "KafalaPayments" => Entry("الكفالات", controller, "حركات الكفالة", CharityPolicies.AidDisbursementView, CharityPolicies.AidDisbursementManage),

            "CharityProjects" => Entry("المشروعات", controller, "المشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "Projects" => Entry("المشروعات", controller, "المشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectProposals" => Entry("المشروعات", controller, "مقترحات المشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPlanning" => Entry("المشروعات", controller, "التخطيط", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectTaskTracking" => Entry("المشروعات", controller, "لوحة المهام", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectTaskDailyUpdates" => Entry("المشروعات", controller, "المتابعة اليومية", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectTrackingLogs" => Entry("المشروعات", controller, "سجل المتابعة", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectExpenseLinks" => Entry("المشروعات", controller, "ربط المصروفات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectAccountingProfiles" => Entry("المشروعات", controller, "الربط المحاسبي للمشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectAccountingReports" => Entry("المشروعات", controller, "تقارير المشروعات المحاسبية", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhases" => Entry("المشروعات", controller, "مراحل المشروع", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectActivities" => Entry("المشروعات", controller, "أنشطة المشروع", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseActivities" => Entry("المشروعات", controller, "أنشطة المراحل", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseTasks" => Entry("المشروعات", controller, "مهام المراحل", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseExpenseLinks" => Entry("المشروعات", controller, "ربط مصروفات المراحل", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseStoreIssueLinks" => Entry("المشروعات", controller, "ربط الصرف المخزني بالمراحل", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseMilestones" => Entry("المشروعات", controller, "المعالم الرئيسية", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectPhaseAccountingReports" => Entry("المشروعات", controller, "تقارير محاسبية للمراحل", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectBudgetLines" => Entry("المشروعات", controller, "بنود الميزانية", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectGrants" => Entry("المشروعات", controller, "ربط المنح بالمشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),
            "ProjectBeneficiaries" => Entry("المشروعات", controller, "مستفيدو المشروعات", CharityPolicies.ProjectsView, CharityPolicies.ProjectsManage),

            "Volunteers" => Entry("التطوع", controller, "المتطوعون", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerDirectory" => Entry("التطوع", controller, "دليل المتطوعين", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerSkillDefinitions" => Entry("التطوع", controller, "تعريف المهارات", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerSkills" => Entry("التطوع", controller, "مهارات المتطوعين", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerProjectAssignments" => Entry("التطوع", controller, "إسناد المتطوعين", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerHourLogs" => Entry("التطوع", controller, "ساعات التطوع", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),
            "VolunteerAvailabilitySlots" => Entry("التطوع", controller, "أوقات التوفر", CharityPolicies.VolunteersView, CharityPolicies.VolunteersManage),

            "Items" => Entry("المخازن", controller, "الأصناف", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "ItemGroups" => Entry("المخازن", controller, "مجموعات الأصناف", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "Units" => Entry("المخازن", controller, "الوحدات", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "Warehouses" => Entry("المخازن", controller, "المخازن", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "CharityStoreReceipts" => Entry("المخازن", controller, "أذون الإضافة", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "CharityStoreIssues" => Entry("المخازن", controller, "أذون الصرف", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockNeedRequests" => Entry("المخازن", controller, "طلبات الاحتياج", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockReturnVouchers" => Entry("المخازن", controller, "المرتجعات", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockDisposalVouchers" => Entry("المخازن", controller, "الإعدام والهالك", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockTransfers" => Entry("المخازن", controller, "التحويلات المخزنية", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockItemCard" => Entry("المخازن", controller, "كارت الصنف", CharityPolicies.StoresView, CharityPolicies.StoresManage),
            "StockBalances" => Entry("المخازن", controller, "أرصدة المخازن", CharityPolicies.StoresView, CharityPolicies.StoresManage),

            "Suppliers" => Entry("المشتريات", controller, "الموردون", CharityPolicies.ProcurementView, CharityPolicies.ProcurementManage),
            "PurchaseInvoices" => Entry("المشتريات", controller, "فواتير الشراء", CharityPolicies.ProcurementView, CharityPolicies.ProcurementManage),
            "FindPurchases" => Entry("المشتريات", controller, "بحث المشتريات", CharityPolicies.ProcurementView, CharityPolicies.ProcurementManage),

            "SalesInvoices" => Entry("الإيرادات الذاتية", controller, "فواتير البيع", CharityPolicies.SalesView, CharityPolicies.SalesManage),
            "FindSales" => Entry("الإيرادات الذاتية", controller, "بحث المبيعات", CharityPolicies.SalesView, CharityPolicies.SalesManage),
            "SalesReports" => Entry("الإيرادات الذاتية", controller, "تقارير الإيرادات", CharityPolicies.SalesView, CharityPolicies.SalesManage),
            "SalesReturns" => Entry("الإيرادات الذاتية", controller, "مرتجعات المبيعات", CharityPolicies.SalesView, CharityPolicies.SalesManage),

            "HrEmployees" => Entry("الموارد البشرية", controller, "الموظفون", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrAttendanceRecords" => Entry("الموارد البشرية", controller, "الحضور والانصراف", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrEmployeeMovements" => Entry("الموارد البشرية", controller, "الحركات الوظيفية", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrSanctionRecords" => Entry("الموارد البشرية", controller, "الجزاءات", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrOutRequests" => Entry("الموارد البشرية", controller, "طلبات الخروج", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrPerformanceEvaluations" => Entry("الموارد البشرية", controller, "تقييم الأداء", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrEmployeeContracts" => Entry("الموارد البشرية", controller, "عقود الموظفين", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrEmployeeFundingAssignments" => Entry("الموارد البشرية", controller, "تمويل الموظفين", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrEmployeeTaskAssignments" => Entry("الموارد البشرية", controller, "تكليفات الموظفين", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrEmployeeBonuses" => Entry("الموارد البشرية", controller, "مكافآت الموظفين", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrDepartments" => Entry("الموارد البشرية", controller, "الأقسام", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrShifts" => Entry("الموارد البشرية", controller, "الورديات", CharityPolicies.HrView, CharityPolicies.HrManage),
            "HrJobTitles" => Entry("الموارد البشرية", controller, "المسميات الوظيفية", CharityPolicies.HrView, CharityPolicies.HrManage),
            "PayrollMonths" => Entry("المرتبات", controller, "المرتبات", CharityPolicies.PayrollView, CharityPolicies.PayrollManage),
            "PayrollEmployees" => Entry("المرتبات", controller, "موظفو المرتبات", CharityPolicies.PayrollView, CharityPolicies.PayrollManage),
            "SalaryItemDefinitions" => Entry("المرتبات", controller, "عناصر المرتب", CharityPolicies.PayrollView, CharityPolicies.PayrollManage),
            "EmployeeSalaryStructures" => Entry("المرتبات", controller, "هياكل الرواتب", CharityPolicies.PayrollView, CharityPolicies.PayrollManage),

            "Accounting" => Entry("المالية", controller, "الصفحة المحاسبية", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "FinancialAccount" => Entry("المالية", controller, "الدليل المحاسبي", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "TreasuryBank" => Entry("المالية", controller, "الخزينة والبنك", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "CostCenters" => Entry("المالية", controller, "مراكز التكلفة", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "FiscalPeriods" => Entry("المالية", controller, "الفترات المالية", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "JournalEntries" => Entry("المالية", controller, "القيود اليومية", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "AccountingReports" => Entry("المالية", controller, "التقارير المحاسبية", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "AccountingIntegrationProfiles" => Entry("المالية", controller, "ربط الحركات محاسبيًا", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),
            "OperationalJournalEntries" => Entry("المالية", controller, "ربط الحركات بالقيود", CharityPolicies.FinanceView, CharityPolicies.FinanceManage),

            "BoardMeetings" => Entry("المحاضر والقرارات", controller, "الاجتماعات", CharityPolicies.MinutesView, CharityPolicies.MinutesManage),
            "BoardDecisions" => Entry("المحاضر والقرارات", controller, "القرارات والتوصيات", CharityPolicies.MinutesView, CharityPolicies.MinutesManage),
            "BoardDecisionFollowUps" => Entry("المحاضر والقرارات", controller, "متابعة تنفيذ القرارات", CharityPolicies.MinutesView, CharityPolicies.MinutesManage),
            "BoardDecisionAttachments" => Entry("المحاضر والقرارات", controller, "مرفقات القرارات", CharityPolicies.MinutesView, CharityPolicies.MinutesManage),

            "CharityReports" => Entry("التقارير", controller, "التقارير العامة", CharityPolicies.ReportsView),
            "ReportsCompare" => Entry("التقارير", controller, "المقارنة الزمنية", CharityPolicies.ReportsView),
            "NotificationCenter" => Entry("التقارير", controller, "مركز التنبيهات", CharityPolicies.ReportsView),
            "WorkflowBoard" => Entry("التقارير", controller, "لوحة سير العمل", CharityPolicies.ReportsView),
            "OperationalNotifications" => Entry("التقارير", controller, "الإشعارات التشغيلية", CharityPolicies.ReportsView),
            "UserPreferences" => Entry("التقارير", controller, "تفضيلات المستخدم", CharityPolicies.ReportsView),
            "RfpReports" => Entry("التقارير", controller, "تقارير RFP", CharityPolicies.ReportsView),

            "SecurityAdmin" => Entry("الإدارة", controller, "إدارة الصلاحيات", CharityPolicies.SettingsManage, CharityPolicies.SettingsManage),
            "Security" => Entry("الإدارة", controller, "مصفوفة الصلاحيات", CharityPolicies.SettingsManage, CharityPolicies.SettingsManage),
            "DemoData" => Entry("الإدارة", controller, "البيانات التجريبية", CharityPolicies.SettingsManage, CharityPolicies.SettingsManage),
            "QaAudit" => Entry("الإدارة", controller, "فحص النظام", CharityPolicies.SettingsManage, CharityPolicies.SettingsManage),

            _ => null
        };
    }

    public static string? GetPolicy(string? controllerName, string? actionName = null)
    {
        var entry = GetEntry(controllerName);
        if (entry is null)
        {
            return null;
        }

        var action = actionName?.Trim();
        if (!string.IsNullOrWhiteSpace(action))
        {
            if (entry.ActionPolicies.TryGetValue(action, out var actionPolicy))
            {
                return actionPolicy;
            }

            if (!string.IsNullOrWhiteSpace(entry.ManagePolicy) && ManageActionNames.Contains(action))
            {
                return entry.ManagePolicy;
            }
        }

        return entry.ViewPolicy;
    }

    public static IReadOnlyList<CharityControllerAccessEntry> GetRepresentativeEntries()
        => RepresentativeControllers
            .Select(GetEntry)
            .Where(x => x is not null)
            .Cast<CharityControllerAccessEntry>()
            .ToList();

    private static CharityControllerAccessEntry Entry(string group, string controller, string displayName, string? viewPolicy, string? managePolicy = null, IReadOnlyDictionary<string, string>? actionPolicies = null)
        => new()
        {
            Group = group,
            Controller = controller,
            DisplayName = displayName,
            ViewPolicy = viewPolicy,
            ManagePolicy = managePolicy,
            ActionPolicies = actionPolicies ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        };

    private static readonly string[] RepresentativeControllers =
    {
        "Dashboard3", "Beneficiaries", "BeneficiaryAidRequests", "BeneficiaryHumanitarianResearchs", "BeneficiaryCommitteeDecisions",
        "BeneficiaryAidDisbursements", "AidCycles", "Donors", "Donations", "Funders", "GrantAgreements", "GrantInstallments",
        "KafalaCases", "KafalaPayments", "CharityProjects", "ProjectPhases", "ProjectPhaseActivities", "ProjectPhaseTasks",
        "ProjectTrackingLogs", "Volunteers", "VolunteerProjectAssignments", "Items", "Warehouses", "CharityStoreReceipts",
        "CharityStoreIssues", "StockNeedRequests", "StockReturnVouchers", "StockDisposalVouchers", "Suppliers", "PurchaseInvoices",
        "SalesInvoices", "SalesReports", "HrEmployees", "PayrollMonths", "Accounting", "FinancialAccount", "TreasuryBank", "AccountingReports",
        "BoardMeetings", "BoardDecisions", "CharityReports", "SecurityAdmin", "DemoData"
    };
}

public static class CharityNavigationAuthorizationExtensions
{
    public static async Task<bool> CanAccessCharityAsync(this IAuthorizationService authorizationService, ClaimsPrincipal user, string controller, string action = "Index")
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var policy = CharityControllerAccessMap.GetPolicy(controller, action);
        if (string.IsNullOrWhiteSpace(policy))
        {
            return true;
        }

        var result = await authorizationService.AuthorizeAsync(user, policy);
        return result.Succeeded;
    }
}
