using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SmartLunch.Backend.Service.Infrastructure.Data
{
    /// <summary>
    /// Database configuration options
    /// </summary>
    public class DatabaseOptions
    {
        public const string SectionName = "Database";

        /// <summary>
        /// Connection string for the main database (falls back to ConnectionStrings:DefaultConnection if not set)
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Enable automatic migrations on startup
        /// </summary>
        public bool AutoMigrate { get; set; } = false;

        /// <summary>
        /// Enable detailed logging for EF Core
        /// </summary>
        public bool EnableSensitiveDataLogging { get; set; } = false;

        /// <summary>
        /// Command timeout in seconds
        /// </summary>
        public int CommandTimeout { get; set; } = 30;

        /// <summary>
        /// Maximum retry count for transient failures
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// Maximum delay between retries in seconds
        /// </summary>
        public int MaxRetryDelay { get; set; } = 30;

        /// <summary>
        /// Enable query splitting for complex queries
        /// </summary>
        public bool EnableQuerySplitting { get; set; } = true;

        /// <summary>
        /// Pool size for DbContext pooling
        /// </summary>
        public int PoolSize { get; set; } = 128;

        /// <summary>
        /// Enable database health checks
        /// </summary>
        public bool EnableHealthChecks { get; set; } = true;

        /// <summary>
        /// Health check interval in minutes
        /// </summary>
        public int HealthCheckInterval { get; set; } = 5;
    }

    /// <summary>
    /// Database configuration extensions
    /// </summary>
    public static class DatabaseConfigurationExtensions
    {
        public static IServiceCollection AddEnhancedDatabase(this IServiceCollection services, IConfiguration configuration, bool usePooling = true)
        {
            var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

            // Fallback to ConnectionStrings:DefaultConnection if Database:ConnectionString is not set
            if (string.IsNullOrEmpty(databaseOptions.ConnectionString))
            {
                databaseOptions.ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            }

            services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

            // Configure Npgsql options
            Action<DbContextOptionsBuilder> configureOptions = options =>
            {
                options.UseNpgsql(databaseOptions.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseVector(); // Enable pgvector support
                    npgsqlOptions.CommandTimeout(databaseOptions.CommandTimeout);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: databaseOptions.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(databaseOptions.MaxRetryDelay),
                        errorCodesToAdd: null);
                });

                // Apply snake case naming convention for PostgreSQL
                options.UseSnakeCaseNamingConvention();

                if (databaseOptions.EnableSensitiveDataLogging)
                {
                    options.EnableSensitiveDataLogging();
                }

                options.EnableDetailedErrors();
                options.EnableServiceProviderCaching();
            };

            // Use pooling for better performance (recommended for production)
            if (usePooling)
            {
                services.AddPooledDbContextFactory<RhetorAIServiceDBContext>(configureOptions, poolSize: databaseOptions.PoolSize);
                // Also register regular DbContext for services that need it
                services.AddDbContext<RhetorAIServiceDBContext>(configureOptions, ServiceLifetime.Scoped);
            }
            else
            {
                // Use factory pattern without pooling
                services.AddDbContextFactory<RhetorAIServiceDBContext>(configureOptions);
                services.AddDbContext<RhetorAIServiceDBContext>(configureOptions, ServiceLifetime.Scoped);
            }

            //// Add health checks if enabled
            //if (databaseOptions.EnableHealthChecks)
            //{
            //    services.AddHealthChecks()
            //        .AddDbContextCheck<RhetorAIServiceDBContext>("database", tags: new[] { "ready", "live" });
            //}

            return services;
        }

        public static async Task<IHost> MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<RhetorAIServiceDBContext>>();
            var databaseOptions = services.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            if (databaseOptions.AutoMigrate)
            {
                try
                {
                    // Try to use DbContextFactory first, fallback to regular DbContext
                    RhetorAIServiceDBContext context;
                    var contextFactory = services.GetService<IDbContextFactory<RhetorAIServiceDBContext>>();

                    if (contextFactory != null)
                    {
                        context = contextFactory.CreateDbContext();
                    }
                    else
                    {
                        context = services.GetRequiredService<RhetorAIServiceDBContext>();
                    }

                    using (context)
                    {
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                            await context.Database.MigrateAsync();
                            logger.LogInformation("Database migrations applied successfully");
                        }
                        else
                        {
                            logger.LogInformation("Database is up to date");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database");
                    throw;
                }
            }

            return host;
        }
    }
}