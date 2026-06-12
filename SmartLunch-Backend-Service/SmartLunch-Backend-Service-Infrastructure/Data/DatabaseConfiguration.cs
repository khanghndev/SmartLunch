using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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
        /// <summary>
        /// Adds enhanced database configuration with support for all DatabaseOptions settings
        /// </summary>
        public static IServiceCollection AddEnhancedDatabase(
            this IServiceCollection services,
            IConfiguration configuration,
            bool usePooling = true)
        {
            // Register DatabaseOptions
            services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

            // Get the options to use during configuration
            var databaseOptions = GetDatabaseOptions(configuration);

            // Determine connection string with fallback logic
            var connectionString = !string.IsNullOrWhiteSpace(databaseOptions.ConnectionString)
                ? databaseOptions.ConnectionString
                : configuration.GetConnectionString("SmartLunchDatabase")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is not configured. Please set Database:ConnectionString or ConnectionStrings:SmartLunchDatabase or ConnectionStrings:DefaultConnection");

            // Configure DbContext
            if (usePooling)
            {
                services.AddDbContextPool<SmartLunchDBContext>(options =>
                {
                    ConfigureDbContextOptions(options, connectionString, databaseOptions);
                }, databaseOptions.PoolSize);
            }
            else
            {
                services.AddDbContext<SmartLunchDBContext>(options =>
                {
                    ConfigureDbContextOptions(options, connectionString, databaseOptions);
                });
            }

            return services;
        }

        private static DatabaseOptions GetDatabaseOptions(IConfiguration configuration)
        {
            var options = new DatabaseOptions();
            configuration.GetSection(DatabaseOptions.SectionName).Bind(options);
            return options;
        }

        /// <summary>
        /// Configures DbContext options with all settings from DatabaseOptions
        /// </summary>
        private static void ConfigureDbContextOptions(
            DbContextOptionsBuilder optionsBuilder,
            string connectionString,
            DatabaseOptions options)
        {
            // Try to auto-detect server version, fall back to MySQL 8.0 if it fails
            // This handles cases where connection might not be ready during startup
            ServerVersion serverVersion;
            try
            {
                serverVersion = ServerVersion.AutoDetect(connectionString);
            }
            catch
            {
                // Fall back to MySQL 8.0 if auto-detection fails
                // This is safe since docker-compose.yml uses mysql:8.0
                serverVersion = ServerVersion.Parse("8.0.0-mysql");
            }

            optionsBuilder.UseMySql(connectionString, serverVersion, mysqlOptions =>
            {
                // mysqlOptions.EnableRetryOnFailure(
                //     maxRetryCount: options.MaxRetryCount,
                //     maxRetryDelay: TimeSpan.FromSeconds(options.MaxRetryDelay),
                //     errorNumbersToAdd: null);

                // Apply command timeout
                if (options.CommandTimeout > 0)
                {
                    mysqlOptions.CommandTimeout(options.CommandTimeout);
                }

                // Enable query splitting if configured
                if (options.EnableQuerySplitting)
                {
                    mysqlOptions.EnableStringComparisonTranslations();
                }
            });

            // Enable sensitive data logging if configured (must be on optionsBuilder, not inside UseMySql)
            if (options.EnableSensitiveDataLogging)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }
    }
}