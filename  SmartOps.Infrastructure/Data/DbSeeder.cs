using Microsoft.EntityFrameworkCore;
using SmartOps.Domain.Entities;

namespace SmartOps.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(SmartOpsDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role(
                    "Administrator",
                    "Full system access and administration."
                ),

                new Role(
                    "Support Agent",
                    "Handles and manages support incidents."
                ),

                new Role(
                    "Team Lead",
                    "Manages teams and oversees incident assignments."
                ),

                new Role(
                    "Engineer",
                    "Works on assigned incidents and technical resolution."
                ),

                new Role(
                    "Manager",
                    "Views operational performance and analytics."
                )
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}