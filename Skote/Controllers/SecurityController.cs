using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
using Skote.ViewModels.Security;
using System.Security.Claims;

namespace Skote.Controllers;

[Authorize(Policy = CharityPolicies.SettingsManage)]
public class SecurityController : Controller
{
    public IActionResult PermissionsMatrix()
    {
        var model = new PermissionsMatrixVm
        {
            CurrentUserName = User.Identity?.Name ?? string.Empty,
            CurrentUserRoles = User.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            CurrentUserPermissions = User.Claims
                .Where(x => x.Type == CharityPermissions.Type)
                .Select(x => x.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            Policies = BuildPolicies(),
            ControllerAccesses = CharityControllerAccessMap.GetRepresentativeEntries()
                .Select(x => new ControllerAccessVm
                {
                    Group = x.Group,
                    ControllerName = x.Controller,
                    DisplayName = x.DisplayName,
                    ViewPolicy = x.ViewPolicy ?? string.Empty,
                    ManagePolicy = x.ManagePolicy ?? string.Empty
                })
                .OrderBy(x => x.Group)
                .ThenBy(x => x.DisplayName)
                .ToList()
        };

        return View(model);
    }

    private static List<ModulePolicyVm> BuildPolicies()
    {
        return new List<ModulePolicyVm>
        {
            new() { Group = "Dashboard", PolicyName = CharityPolicies.DashboardView, PermissionName = CharityPermissions.DashboardView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.Reviewer, CharityRoles.Accountant) },

            new() { Group = "Beneficiaries", PolicyName = CharityPolicies.BeneficiariesView, PermissionName = CharityPermissions.BeneficiariesView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.BeneficiariesOfficer, CharityRoles.SocialResearcher, CharityRoles.Reviewer, CharityRoles.Accountant, CharityRoles.FinancialOfficer) },
            new() { Group = "Beneficiaries", PolicyName = CharityPolicies.BeneficiariesManage, PermissionName = CharityPermissions.BeneficiariesManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.BeneficiariesOfficer) },
            new() { Group = "Beneficiaries Workflow", PolicyName = CharityPolicies.ResearchReview, PermissionName = CharityPermissions.ResearchReview, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.Reviewer, CharityRoles.SocialResearcher) },
            new() { Group = "Beneficiaries Workflow", PolicyName = CharityPolicies.CommitteeDecisionManage, PermissionName = CharityPermissions.CommitteeDecisionManage, DefaultRolesCsv = CharityRoles.Admin },
            new() { Group = "Aid Operations", PolicyName = CharityPolicies.AidDisbursementView, PermissionName = CharityPermissions.AidDisbursementView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.Accountant, CharityRoles.FinancialOfficer, CharityRoles.CharityManager) },
            new() { Group = "Aid Operations", PolicyName = CharityPolicies.AidDisbursementManage, PermissionName = CharityPermissions.AidDisbursementManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.Accountant, CharityRoles.FinancialOfficer, CharityRoles.CharityManager) },

            new() { Group = "Donors", PolicyName = CharityPolicies.DonorsView, PermissionName = CharityPermissions.DonorsView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.DonorRelations, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Donors", PolicyName = CharityPolicies.DonorsManage, PermissionName = CharityPermissions.DonorsManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.DonorRelations) },

            new() { Group = "Funders", PolicyName = CharityPolicies.FundersView, PermissionName = CharityPermissions.FundersView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FunderRelations, CharityRoles.ProjectManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Funders", PolicyName = CharityPolicies.FundersManage, PermissionName = CharityPermissions.FundersManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FunderRelations) },

            new() { Group = "Projects", PolicyName = CharityPolicies.ProjectsView, PermissionName = CharityPermissions.ProjectsView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager, CharityRoles.Accountant) },
            new() { Group = "Projects", PolicyName = CharityPolicies.ProjectsManage, PermissionName = CharityPermissions.ProjectsManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager) },

            new() { Group = "Volunteers", PolicyName = CharityPolicies.VolunteersView, PermissionName = CharityPermissions.VolunteersView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager) },
            new() { Group = "Volunteers", PolicyName = CharityPolicies.VolunteersManage, PermissionName = CharityPermissions.VolunteersManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ProjectManager) },

            new() { Group = "Stores", PolicyName = CharityPolicies.StoresView, PermissionName = CharityPermissions.StoresView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Stores", PolicyName = CharityPolicies.StoresManage, PermissionName = CharityPermissions.StoresManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper) },
            new() { Group = "Stores", PolicyName = CharityPolicies.StoresApprove, PermissionName = CharityPermissions.StoresApprove, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },

            new() { Group = "Procurement", PolicyName = CharityPolicies.ProcurementView, PermissionName = CharityPermissions.ProcurementView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Procurement", PolicyName = CharityPolicies.ProcurementManage, PermissionName = CharityPermissions.ProcurementManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.StoreKeeper, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },

            new() { Group = "Sales", PolicyName = CharityPolicies.SalesView, PermissionName = CharityPermissions.SalesView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Sales", PolicyName = CharityPolicies.SalesManage, PermissionName = CharityPermissions.SalesManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },

            new() { Group = "Finance", PolicyName = CharityPolicies.FinanceView, PermissionName = CharityPermissions.FinanceView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Finance", PolicyName = CharityPolicies.FinanceManage, PermissionName = CharityPermissions.FinanceManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },

            new() { Group = "HR", PolicyName = CharityPolicies.HrView, PermissionName = CharityPermissions.HrView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.HrOfficer, CharityRoles.PayrollOfficer) },
            new() { Group = "HR", PolicyName = CharityPolicies.HrManage, PermissionName = CharityPermissions.HrManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.HrOfficer) },

            new() { Group = "Payroll", PolicyName = CharityPolicies.PayrollView, PermissionName = CharityPermissions.PayrollView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.PayrollOfficer, CharityRoles.FinancialOfficer, CharityRoles.Accountant) },
            new() { Group = "Payroll", PolicyName = CharityPolicies.PayrollManage, PermissionName = CharityPermissions.PayrollManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.PayrollOfficer) },

            new() { Group = "Minutes", PolicyName = CharityPolicies.MinutesView, PermissionName = CharityPermissions.MinutesView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.Reviewer, CharityRoles.Accountant) },
            new() { Group = "Minutes", PolicyName = CharityPolicies.MinutesManage, PermissionName = CharityPermissions.MinutesManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager) },

            new() { Group = "Reports", PolicyName = CharityPolicies.ReportsView, PermissionName = CharityPermissions.ReportsView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.FinancialOfficer, CharityRoles.Reviewer, CharityRoles.Accountant) },
            new() { Group = "Audit", PolicyName = CharityPolicies.AuditView, PermissionName = CharityPermissions.AuditView, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager, CharityRoles.ReportsViewer, CharityRoles.FinancialOfficer, CharityRoles.Reviewer, CharityRoles.Accountant) },
            new() { Group = "Settings", PolicyName = CharityPolicies.SettingsManage, PermissionName = CharityPermissions.SettingsManage, DefaultRolesCsv = string.Join(", ", CharityRoles.Admin, CharityRoles.CharityManager) }
        };
    }
}
