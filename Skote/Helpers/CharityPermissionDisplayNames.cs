namespace Skote.Helpers;

public static class CharityPermissionDisplayNames
{
    public static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        [CharityPermissions.DashboardView] = "عرض لوحة التحكم",
        [CharityPermissions.BeneficiariesView] = "عرض المستفيدين",
        [CharityPermissions.BeneficiariesManage] = "إدارة المستفيدين",
        [CharityPermissions.ResearchReview] = "مراجعة استمارات البحث الإنساني",
        [CharityPermissions.CommitteeDecisionManage] = "إصدار قرارات اللجنة",
        [CharityPermissions.AidDisbursementView] = "عرض دورات الصرف وسجل الصرف",
        [CharityPermissions.AidDisbursementManage] = "إدارة دورات الصرف وتنفيذ الصرف",
        [CharityPermissions.DonorsView] = "عرض المتبرعين والتبرعات",
        [CharityPermissions.DonorsManage] = "إدارة المتبرعين والتبرعات",
        [CharityPermissions.FundersView] = "عرض الممولين والمنح",
        [CharityPermissions.FundersManage] = "إدارة الممولين والمنح",
        [CharityPermissions.ProjectsView] = "عرض المشروعات",
        [CharityPermissions.ProjectsManage] = "إدارة المشروعات",
        [CharityPermissions.StoresView] = "عرض المخازن",
        [CharityPermissions.StoresManage] = "إدارة المخازن",
        [CharityPermissions.HrView] = "عرض الموظفين والحضور",
        [CharityPermissions.HrManage] = "إدارة الموظفين والحضور",
        [CharityPermissions.PayrollView] = "عرض المرتبات",
        [CharityPermissions.PayrollManage] = "إدارة المرتبات",
        [CharityPermissions.VolunteersView] = "عرض التطوع والمتطوعين",
        [CharityPermissions.VolunteersManage] = "إدارة التطوع والمتطوعين",
        [CharityPermissions.ProcurementView] = "عرض الموردين والمشتريات",
        [CharityPermissions.ProcurementManage] = "إدارة الموردين والمشتريات",
        [CharityPermissions.SalesView] = "عرض الإيرادات الذاتية والمبيعات",
        [CharityPermissions.SalesManage] = "إدارة الإيرادات الذاتية والمبيعات",
        [CharityPermissions.FinanceView] = "عرض المالية والمحاسبة",
        [CharityPermissions.FinanceManage] = "إدارة المالية والمحاسبة",
        [CharityPermissions.MinutesView] = "عرض المحاضر والقرارات",
        [CharityPermissions.MinutesManage] = "إدارة المحاضر والقرارات",
        [CharityPermissions.ReportsView] = "عرض التقارير",
        [CharityPermissions.AuditView] = "عرض سجل الحركات",
        [CharityPermissions.SettingsManage] = "إدارة الإعدادات والصلاحيات"
    };

    public static string Get(string permission)
        => Map.TryGetValue(permission, out var text) ? text : permission;
}
