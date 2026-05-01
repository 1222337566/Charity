namespace Skote.Helpers;

public static class CharityDefaultAccess
{
    public static readonly Dictionary<string, string[]> RolePermissions = new(StringComparer.OrdinalIgnoreCase)
    {
        [CharityRoles.Admin] = CharityPermissions.All,
        [CharityRoles.CharityManager] = CharityPermissions.All,

        [CharityRoles.Reviewer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.BeneficiariesView,
            CharityPermissions.ResearchReview,
            CharityPermissions.MinutesView,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.Accountant] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.BeneficiariesView,
            CharityPermissions.AidDisbursementView,
            CharityPermissions.AidDisbursementManage,
            CharityPermissions.DonorsView,
            CharityPermissions.FundersView,
            CharityPermissions.ProjectsView,
            CharityPermissions.StoresView,
            CharityPermissions.PayrollView,
            CharityPermissions.ProcurementView,
            CharityPermissions.ProcurementManage,
            CharityPermissions.SalesView,
            CharityPermissions.SalesManage,
            CharityPermissions.FinanceView,
            CharityPermissions.FinanceManage,
            CharityPermissions.MinutesView,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.BeneficiariesOfficer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.BeneficiariesView,
            CharityPermissions.BeneficiariesManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.SocialResearcher] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.BeneficiariesView,
            CharityPermissions.ResearchReview,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.DonorRelations] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.DonorsView,
            CharityPermissions.DonorsManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.FunderRelations] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.FundersView,
            CharityPermissions.FundersManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.ProjectManager] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.ProjectsView,
            CharityPermissions.ProjectsManage,
            CharityPermissions.FundersView,
            CharityPermissions.VolunteersView,
            CharityPermissions.VolunteersManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.StoreKeeper] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.StoresView,
            CharityPermissions.StoresManage,
            CharityPermissions.ProcurementView,
            CharityPermissions.ProcurementManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.HrOfficer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.HrView,
            CharityPermissions.HrManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.PayrollOfficer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.HrView,
            CharityPermissions.PayrollView,
            CharityPermissions.PayrollManage,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.FinancialOfficer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.BeneficiariesView,
            CharityPermissions.AidDisbursementView,
            CharityPermissions.AidDisbursementManage,
            CharityPermissions.DonorsView,
            CharityPermissions.FundersView,
            CharityPermissions.StoresView,
            CharityPermissions.PayrollView,
            CharityPermissions.ProcurementView,
            CharityPermissions.ProcurementManage,
            CharityPermissions.SalesView,
            CharityPermissions.SalesManage,
            CharityPermissions.FinanceView,
            CharityPermissions.FinanceManage,
            CharityPermissions.MinutesView,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        },

        [CharityRoles.ReportsViewer] = new[]
        {
            CharityPermissions.DashboardView,
            CharityPermissions.MinutesView,
            CharityPermissions.ReportsView,
            CharityPermissions.AuditView
        }
    };

    public static IEnumerable<string> GetPermissionsForRole(string roleName)
        => RolePermissions.TryGetValue(roleName, out var permissions)
            ? permissions
            : Enumerable.Empty<string>();
}
