using Microsoft.AspNetCore.Identity;
using SplendoreMVP.Models;

namespace SplendoreMVP.Data
{
    public class IdentityDataSeeder
    {
        public async Task SeedAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                // Define roles
                string[] roles = { "Admin", "Customer", "Supervisor" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = role
                        });
                    }
                }

                // Seed Admin User
                string adminEmail = "admin@shop.com";
                string adminPassword = "Admin@123";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}
