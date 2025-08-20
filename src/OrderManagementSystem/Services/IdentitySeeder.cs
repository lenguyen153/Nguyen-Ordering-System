using Microsoft.AspNetCore.Identity;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

public class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleMgr.RoleExistsAsync(role))
                await roleMgr.CreateAsync(new IdentityRole<Guid>(role));
        }

        var adminEmail = "admin@example.com";
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, Role = UserRole.Admin };
            await userMgr.CreateAsync(admin, "Admin@12345");
            await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
}
