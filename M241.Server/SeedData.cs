using M241.Server.Data;
using M241.Server.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace M241.Server
{
    public static class SeedData
    {
        private static string[] ROLES = ["Administrator", "Editor", "Viewer"];
        public static async Task SeedDb(this AeroSenseDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager, "admin@example.com", "Asdf123*");
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in ROLES)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager, string adminEmail, string adminPassword)
        {
            if (userManager.Users.All(u => u.UserName != adminEmail))
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    LockoutEnabled = false,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
                else
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}
