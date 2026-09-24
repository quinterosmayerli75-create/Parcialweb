using Microsoft.AspNetCore.Identity;
using ClinicaVeterinaria.Models;

namespace ClinicaVeterinaria.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Administrador", "Cliente" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Crear un usuario Administrador por defecto si no existe
            var adminEmail = "admin@veterinaria.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NombreCompleto = "Administrador Principal",
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Administrador");
                }
            }
        }
    }
}