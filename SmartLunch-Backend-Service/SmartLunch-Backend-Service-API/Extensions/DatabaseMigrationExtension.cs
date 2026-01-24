using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartLunch.Backend.Service.API.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var config = scope.ServiceProvider
                .GetRequiredService<IOptions<DatabaseOptions>>().Value;

            if (!config.AutoMigrate)
                return;

            var dbContext = scope.ServiceProvider
                .GetRequiredService<SmartLunchDBContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
