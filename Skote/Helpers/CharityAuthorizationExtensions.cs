using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Skote.Helpers;

public static class CharityAuthorizationExtensions
{
    public static IServiceCollection AddCharityModuleAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            AddPolicy(options, CharityPolicies.DashboardView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.Reviewer, CharityRoles.Accountant },
                CharityPermissions.DashboardView);

            AddPolicy(options, CharityPolicies.BeneficiariesView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.BeneficiariesOfficer, CharityRoles.SocialResearcher, CharityRoles.Reviewer, CharityRoles.Accountant, CharityRoles.FinancialOfficer },
                CharityPermissions.BeneficiariesView);
            AddPolicy(options, CharityPolicies.BeneficiariesManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.BeneficiariesOfficer },
                CharityPermissions.BeneficiariesManage);

            AddPolicy(options, CharityPolicies.ResearchReview,
                new[] { CharityRoles.Admin, CharityRoles.Reviewer, CharityRoles.SocialResearcher },
                CharityPermissions.ResearchReview);
            AddPolicy(options, CharityPolicies.CommitteeDecisionManage,
                new[] { CharityRoles.Admin },
                CharityPermissions.CommitteeDecisionManage);
            AddPolicy(options, CharityPolicies.AidDisbursementView,
                new[] { CharityRoles.Admin, CharityRoles.Accountant, CharityRoles.FinancialOfficer, CharityRoles.CharityManager },
                CharityPermissions.AidDisbursementView);
            AddPolicy(options, CharityPolicies.AidDisbursementManage,
                new[] { CharityRoles.Admin, CharityRoles.Accountant, CharityRoles.FinancialOfficer, CharityRoles.CharityManager },
                CharityPermissions.AidDisbursementManage);

            AddPolicy(options, CharityPolicies.DonorsView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.DonorRelations, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.DonorsView);
            AddPolicy(options, CharityPolicies.DonorsManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.DonorRelations },
                CharityPermissions.DonorsManage);

            AddPolicy(options, CharityPolicies.FundersView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FunderRelations, CharityRoles.ProjectManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.FundersView);
            AddPolicy(options, CharityPolicies.FundersManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FunderRelations },
                CharityPermissions.FundersManage);

            AddPolicy(options, CharityPolicies.ProjectsView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager, CharityRoles.Accountant },
                CharityPermissions.ProjectsView);
            AddPolicy(options, CharityPolicies.ProjectsManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager },
                CharityPermissions.ProjectsManage);

            AddPolicy(options, CharityPolicies.VolunteersView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager },
                CharityPermissions.VolunteersView);
            AddPolicy(options, CharityPolicies.VolunteersManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager },
                CharityPermissions.VolunteersManage);

            AddPolicy(options, CharityPolicies.StoresView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.StoresView);
            AddPolicy(options, CharityPolicies.StoresManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper },
                CharityPermissions.StoresManage);

            AddPolicy(options, CharityPolicies.StoresApprove,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.StoresApprove);

            AddPolicy(options, CharityPolicies.ProcurementView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.ProcurementView);
            AddPolicy(options, CharityPolicies.ProcurementManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.ProcurementManage);

            AddPolicy(options, CharityPolicies.SalesView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.SalesView);
            AddPolicy(options, CharityPolicies.SalesManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.SalesManage);

            AddPolicy(options, CharityPolicies.FinanceView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.FinanceView);
            AddPolicy(options, CharityPolicies.FinanceManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.FinanceManage);

            AddPolicy(options, CharityPolicies.HrView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.HrOfficer, CharityRoles.PayrollOfficer },
                CharityPermissions.HrView);
            AddPolicy(options, CharityPolicies.HrManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.HrOfficer },
                CharityPermissions.HrManage);

            AddPolicy(options, CharityPolicies.PayrollView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.PayrollOfficer, CharityRoles.FinancialOfficer, CharityRoles.Accountant },
                CharityPermissions.PayrollView);
            AddPolicy(options, CharityPolicies.PayrollManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.PayrollOfficer },
                CharityPermissions.PayrollManage);

            AddPolicy(options, CharityPolicies.MinutesView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.Reviewer, CharityRoles.Accountant },
                CharityPermissions.MinutesView);
            AddPolicy(options, CharityPolicies.MinutesManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager },
                CharityPermissions.MinutesManage);

            AddPolicy(options, CharityPolicies.ReportsView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.FinancialOfficer, CharityRoles.Reviewer, CharityRoles.Accountant },
                CharityPermissions.ReportsView);
            AddPolicy(options, CharityPolicies.AuditView,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.FinancialOfficer, CharityRoles.Reviewer, CharityRoles.Accountant },
                CharityPermissions.AuditView);

            AddPolicy(options, CharityPolicies.SettingsManage,
                new[] { CharityRoles.Admin, CharityRoles.CharityManager },
                CharityPermissions.SettingsManage);
        });

        return services;
    }

    public static bool HasCharityPermission(this ClaimsPrincipal user, string permission)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        if (user.IsInRole(CharityRoles.Admin))
        {
            return true;
        }

        return user.Claims.Any(c => c.Type == CharityPermissions.Type && string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddPolicy(AuthorizationOptions options, string policyName, string[] roles, string permission)
    {
        options.AddPolicy(policyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                roles.Any(context.User.IsInRole) ||
                context.User.Claims.Any(c => c.Type == CharityPermissions.Type && c.Value == permission));
        });
    }
}
