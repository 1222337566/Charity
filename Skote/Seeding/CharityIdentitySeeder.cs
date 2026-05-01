using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Skote.Helpers;

namespace Skote.Seeding;

public static class CharityIdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in CharityRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var existingClaims = await roleManager.GetClaimsAsync(role);
            var permissionClaims = existingClaims
                .Where(c => c.Type == CharityPermissions.Type)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in CharityDefaultAccess.GetPermissionsForRole(roleName))
            {
                if (!permissionClaims.Contains(permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim(CharityPermissions.Type, permission));
                }
            }
        }
    }

    public static async Task<int> ReseedRoleClaimsAsync(RoleManager<IdentityRole> roleManager)
    {
        var changed = 0;
        foreach (var roleName in CharityRoles.All)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                var createResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createResult.Succeeded)
                {
                    continue;
                }

                role = await roleManager.FindByNameAsync(roleName);
            }

            if (role is null)
            {
                continue;
            }

            var existingClaims = await roleManager.GetClaimsAsync(role);
            var defaultPermissions = CharityDefaultAccess.GetPermissionsForRole(roleName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var extraClaim in existingClaims.Where(c => c.Type == CharityPermissions.Type && !defaultPermissions.Contains(c.Value)).ToList())
            {
                var removeResult = await roleManager.RemoveClaimAsync(role, extraClaim);
                if (removeResult.Succeeded)
                {
                    changed++;
                }
            }

            var currentPermissions = existingClaims
                .Where(c => c.Type == CharityPermissions.Type)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in defaultPermissions)
            {
                if (!currentPermissions.Contains(permission))
                {
                    var addResult = await roleManager.AddClaimAsync(role, new Claim(CharityPermissions.Type, permission));
                    if (addResult.Succeeded)
                    {
                        changed++;
                    }
                }
            }
        }

        return changed;
    }
}
