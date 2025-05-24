
using Microsoft.AspNetCore.Identity;

namespace HealthCareApp.Account
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminEmail = "admin@healthcare.com";
            const string adminPassword = "Admin@123456";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    DateOfBirth = new DateOnly(1980, 1, 1),
                    gender = Gender.Male,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
