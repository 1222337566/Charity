using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1) أدوار
        var roles = new[] { "Admin", "User" };
        foreach (var r in roles)
           if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole(r));

        // 2) أدمن
        var adminUserName = "admin";   // غيّر حسب رغبتك
        var admin = await userManager.FindByNameAsync(adminUserName);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = adminUserName };
            var created = await userManager.CreateAsync(admin, "Admin@12345"); // غيّر الباسورد
            if (created.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                // مثال: إضافة Claim صلاحيات
                await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("permission", "logs.read"));
            }
        }
    }
}
