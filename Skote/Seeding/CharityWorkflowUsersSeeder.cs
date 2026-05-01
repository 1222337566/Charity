using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Skote.Helpers;

namespace Skote.Seeding;

public sealed class CharityWorkflowUsersSeedResult
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Deleted { get; set; }
    public string DefaultPassword { get; set; } = "Demo@12345";
}

public static class CharityWorkflowUsersSeeder
{
    public const string UserNamePrefix = "demo.charity.";
    private const string EmailDomain = "demo.charity.local";
    public const string DefaultPassword = "Demo@12345";

    private sealed record RoleSeed(string RoleName, string UserNameStem, string DisplayPrefix, string Department, int Count);

    private static readonly RoleSeed[] Definitions =
    {
        new(CharityRoles.Admin, "admin", "مسؤول نظام", "الإدارة", 1),
        new(CharityRoles.CharityManager, "manager", "مدير جمعية", "الإدارة", 2),
        new(CharityRoles.Reviewer, "reviewer", "مراجع", "المراجعة", 3),
        new(CharityRoles.BeneficiariesOfficer, "benef", "مسؤول مستفيدين", "خدمة المستفيدين", 6),
        new(CharityRoles.SocialResearcher, "research", "باحث اجتماعي", "البحث الاجتماعي", 10),
        new(CharityRoles.DonorRelations, "donor", "مسؤول متبرعين", "إدارة المتبرعين", 4),
        new(CharityRoles.FunderRelations, "funder", "مسؤول ممولين", "إدارة الممولين", 3),
        new(CharityRoles.ProjectManager, "project", "مدير مشروع", "المشروعات", 4),
        new(CharityRoles.StoreKeeper, "store", "أمين مخزن", "المخازن", 5),
        new(CharityRoles.Accountant, "accountant", "محاسب", "الحسابات", 3),
        new(CharityRoles.FinancialOfficer, "finance", "مسؤول مالي", "المالية", 4),
        new(CharityRoles.HrOfficer, "hr", "مسؤول موارد بشرية", "الموارد البشرية", 2),
        new(CharityRoles.PayrollOfficer, "payroll", "مسؤول رواتب", "الرواتب", 2),
        new(CharityRoles.ReportsViewer, "reports", "متابع تقارير", "المتابعة", 3)
    };

    public static async Task<CharityWorkflowUsersSeedResult> SeedAsync(IServiceProvider services, bool reset = false)
    {
        var result = new CharityWorkflowUsersSeedResult();

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await CharityIdentitySeeder.SeedAsync(services);

        if (reset)
        {
            var existingDemoUsers = userManager.Users
                .Where(x => x.UserName != null && x.UserName.StartsWith(UserNamePrefix))
                .ToList();

            foreach (var user in existingDemoUsers)
            {
                var deleteResult = await userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    result.Deleted++;
                }
            }
        }

        foreach (var def in Definitions)
        {
            if (!await roleManager.RoleExistsAsync(def.RoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(def.RoleName));
            }

            for (var i = 1; i <= def.Count; i++)
            {
                var padded = i.ToString("00");
                var userName = $"{UserNamePrefix}{def.UserNameStem}{padded}";
                var email = $"{def.UserNameStem}{padded}@{EmailDomain}";
                var displayName = $"{def.DisplayPrefix} {padded}";
                var phone = $"0109{(1000000 + ((i * 113) % 8999999)):0000000}";

                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        PhoneNumber = phone,
                        PhoneNumberConfirmed = true,
                        DisplayName = displayName,
                        Department = def.Department,
                        LockoutEnabled = false
                    };

                    var createResult = await userManager.CreateAsync(user, DefaultPassword);
                    if (!createResult.Succeeded)
                    {
                        continue;
                    }

                    result.Created++;
                }
                else
                {
                    user.Email = email;
                    user.EmailConfirmed = true;
                    user.PhoneNumber = phone;
                    user.PhoneNumberConfirmed = true;
                    user.DisplayName = displayName;
                    user.Department = def.Department;
                    user.LockoutEnabled = false;

                    var updateResult = await userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        result.Updated++;
                    }
                }

                var currentRoles = await userManager.GetRolesAsync(user);
                foreach (var extraRole in currentRoles.Where(x => CharityRoles.All.Contains(x) && !string.Equals(x, def.RoleName, StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    await userManager.RemoveFromRoleAsync(user, extraRole);
                }

                if (!await userManager.IsInRoleAsync(user, def.RoleName))
                {
                    await userManager.AddToRoleAsync(user, def.RoleName);
                }
            }
        }

        return result;
    }
}
