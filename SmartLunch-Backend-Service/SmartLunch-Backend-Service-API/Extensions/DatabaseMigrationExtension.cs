using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SmartLunch.Backend.Service.API.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var config = scope.ServiceProvider
                .GetRequiredService<IOptions<DatabaseOptions>>().Value;

            var dbContext = scope.ServiceProvider
                .GetRequiredService<SmartLunchDBContext>();

            // Check if database can connect
            if (!await dbContext.Database.CanConnectAsync())
            {
                // Database doesn't exist or can't connect, create it with schema
                await dbContext.Database.EnsureCreatedAsync();
                return;
            }

            // If AutoMigrate is enabled, run migrations
            if (config.AutoMigrate)
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await dbContext.Database.MigrateAsync();
                }
            }
            else
            {
                // If AutoMigrate is disabled, ensure tables exist (for development)
                // This creates tables if they don't exist, but doesn't use migrations
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }
}
