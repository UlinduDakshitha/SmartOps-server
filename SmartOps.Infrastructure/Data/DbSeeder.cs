using Microsoft.EntityFrameworkCore;
using SmartOps.Domain.Entities;
using Microsoft.Extensions.Configuration;
using SmartOps.Application.Interfaces;

namespace SmartOps.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        SmartOpsDbContext context,
        IConfiguration configuration,
        IPasswordHasher passwordHasher)
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
        if (!await context.SLAPolicies.AnyAsync())
        {
            var slaPolicies = new[]
            {
                new SLAPolicy(
                    "Low Priority SLA",
                    SmartOps.Domain.Enums.Priority.Low,
                    240,
                    1440
                ),

                new SLAPolicy(
                    "Medium Priority SLA",
                    SmartOps.Domain.Enums.Priority.Medium,
                    120,
                    720
                ),

                new SLAPolicy(
                    "High Priority SLA",
                    SmartOps.Domain.Enums.Priority.High,
                    60,
                    240
                ),

                new SLAPolicy(
                    "Critical Priority SLA",
                    SmartOps.Domain.Enums.Priority.Critical,
                    15,
                    60
                )
            };

            await context.SLAPolicies.AddRangeAsync(slaPolicies);
            await context.SaveChangesAsync();
        }
        // Initial Administrator account
        var adminEmail = configuration["InitialAdmin:Email"]
            ?.Trim()
            .ToLowerInvariant();

        var adminFullName = configuration["InitialAdmin:FullName"]
            ?.Trim();

        var adminPassword = configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminFullName) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(
                "Initial Admin configuration is missing. " +
                "Configure InitialAdmin:Email, InitialAdmin:FullName " +
                "and InitialAdmin:Password.");
        }

        var adminRole = await context.Roles
            .SingleOrDefaultAsync(r => r.Name == "Administrator");

        if (adminRole is null)
        {
            throw new InvalidOperationException(
                "Administrator role was not found.");
        }

        var existingAdmin = await context.Users
            .SingleOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin is null)
        {
            var passwordHash = passwordHasher
                .HashPassword(adminPassword);

            var admin = new User(
                adminFullName,
                adminEmail,
                passwordHash);

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();

            await context.UserRoles.AddAsync(
                new UserRole(admin.Id, adminRole.Id));

            await context.SaveChangesAsync();
        }
        else
        {
            var hasAdminRole = await context.UserRoles
                .AnyAsync(ur =>
                    ur.UserId == existingAdmin.Id &&
                    ur.RoleId == adminRole.Id);

            if (!hasAdminRole)
            {
                throw new InvalidOperationException(
                    "The configured Initial Admin email already belongs " +
                    "to a user without the Administrator role. " +
                    "Review the account before proceeding.");
            }
        }
    }
}