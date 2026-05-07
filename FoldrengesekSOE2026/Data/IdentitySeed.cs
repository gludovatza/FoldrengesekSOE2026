using Microsoft.AspNetCore.Identity;

namespace FoldrengesekSOE2026.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = ["User", "Admin"]; // definiált szerepkörök

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Opcionális: legyen egy fix admin felhasználó
            var adminEmail = "admin@demo.local";
            var adminPassword = "Admin123!"; // demo jelszó, élesben NEM így!

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var create = await userManager.CreateAsync(admin, adminPassword);
                if (!create.Succeeded)
                    throw new Exception(string.Join("; ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");
        }

    }
}
