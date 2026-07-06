using Microsoft.AspNetCore.Identity;

namespace IoTDeviceManager.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Admin", "Technician", "Viewer" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@iotmanager.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // Skips the email verification step
            };
            
            // Create the user with a default password (must have uppercase, lowercase, number, and special character)
            var createPowerUser = await userManager.CreateAsync(newAdmin, "AdminPass123!");
            if (createPowerUser.Succeeded)
            {
                // Tie the new account to the "Admin" role
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
        
        var techEmail = "tech@iotmanager.com";
        if (await userManager.FindByEmailAsync(techEmail) == null)
        {
            var newTech = new IdentityUser { UserName = techEmail, Email = techEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(newTech, "TechPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newTech, "Technician");
            }
        }
        
        var viewerEmail = "viewer@iotmanager.com";
        if (await userManager.FindByEmailAsync(viewerEmail) == null)
        {
            var newViewer = new IdentityUser { UserName = viewerEmail, Email = viewerEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(newViewer, "ViewerPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newViewer, "Viewer");
            }
        }
    }
}