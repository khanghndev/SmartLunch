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
            services.Configure<DatabaseOptions>(options =>
            {
                var section = configuration.GetSection(DatabaseOptions.SectionName);
                if (section.Exists())
                {
                    options.ConnectionString = section[nameof(DatabaseOptions.ConnectionString)] ?? string.Empty;

                    var autoMigrateValue = section[nameof(DatabaseOptions.AutoMigrate)];
                    if (bool.TryParse(autoMigrateValue, out var autoMigrate))
                        options.AutoMigrate = autoMigrate;

                    var enableSensitiveDataLoggingValue = section[nameof(DatabaseOptions.EnableSensitiveDataLogging)];
                    if (bool.TryParse(enableSensitiveDataLoggingValue, out var enableSensitiveDataLogging))
                        options.EnableSensitiveDataLogging = enableSensitiveDataLogging;

                    var commandTimeoutValue = section[nameof(DatabaseOptions.CommandTimeout)];
                    if (int.TryParse(commandTimeoutValue, out var commandTimeout) && commandTimeout > 0)
                        options.CommandTimeout = commandTimeout;

                    var maxRetryCountValue = section[nameof(DatabaseOptions.MaxRetryCount)];
                    if (int.TryParse(maxRetryCountValue, out var maxRetryCount) && maxRetryCount > 0)
                        options.MaxRetryCount = maxRetryCount;

                    var maxRetryDelayValue = section[nameof(DatabaseOptions.MaxRetryDelay)];
                    if (int.TryParse(maxRetryDelayValue, out var maxRetryDelay) && maxRetryDelay > 0)
                        options.MaxRetryDelay = maxRetryDelay;

                    var enableQuerySplittingValue = section[nameof(DatabaseOptions.EnableQuerySplitting)];
                    if (bool.TryParse(enableQuerySplittingValue, out var enableQuerySplitting))
                        options.EnableQuerySplitting = enableQuerySplitting;

                    var poolSizeValue = section[nameof(DatabaseOptions.PoolSize)];
                    if (int.TryParse(poolSizeValue, out var poolSize) && poolSize > 0)
                        options.PoolSize = poolSize;

                    var enableHealthChecksValue = section[nameof(DatabaseOptions.EnableHealthChecks)];
                    if (bool.TryParse(enableHealthChecksValue, out var enableHealthChecks))
                        options.EnableHealthChecks = enableHealthChecks;

                    var healthCheckIntervalValue = section[nameof(DatabaseOptions.HealthCheckInterval)];
                    if (int.TryParse(healthCheckIntervalValue, out var healthCheckInterval) && healthCheckInterval > 0)
                        options.HealthCheckInterval = healthCheckInterval;
                }
            });

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

        /// <summary>
        /// Gets database options from configuration
        /// </summary>
        private static DatabaseOptions GetDatabaseOptions(IConfiguration configuration)
        {
            var options = new DatabaseOptions();
            var section = configuration.GetSection(DatabaseOptions.SectionName);

            if (section.Exists())
            {
                options.ConnectionString = section[nameof(DatabaseOptions.ConnectionString)] ?? string.Empty;

                var autoMigrateValue = section[nameof(DatabaseOptions.AutoMigrate)];
                if (bool.TryParse(autoMigrateValue, out var autoMigrate))
                    options.AutoMigrate = autoMigrate;

                var enableSensitiveDataLoggingValue = section[nameof(DatabaseOptions.EnableSensitiveDataLogging)];
                if (bool.TryParse(enableSensitiveDataLoggingValue, out var enableSensitiveDataLogging))
                    options.EnableSensitiveDataLogging = enableSensitiveDataLogging;

                var commandTimeoutValue = section[nameof(DatabaseOptions.CommandTimeout)];
                if (int.TryParse(commandTimeoutValue, out var commandTimeout) && commandTimeout > 0)
                    options.CommandTimeout = commandTimeout;

                var maxRetryCountValue = section[nameof(DatabaseOptions.MaxRetryCount)];
                if (int.TryParse(maxRetryCountValue, out var maxRetryCount) && maxRetryCount > 0)
                    options.MaxRetryCount = maxRetryCount;

                var maxRetryDelayValue = section[nameof(DatabaseOptions.MaxRetryDelay)];
                if (int.TryParse(maxRetryDelayValue, out var maxRetryDelay) && maxRetryDelay > 0)
                    options.MaxRetryDelay = maxRetryDelay;

                var enableQuerySplittingValue = section[nameof(DatabaseOptions.EnableQuerySplitting)];
                if (bool.TryParse(enableQuerySplittingValue, out var enableQuerySplitting))
                    options.EnableQuerySplitting = enableQuerySplitting;

                var poolSizeValue = section[nameof(DatabaseOptions.PoolSize)];
                if (int.TryParse(poolSizeValue, out var poolSize) && poolSize > 0)
                    options.PoolSize = poolSize;

                var enableHealthChecksValue = section[nameof(DatabaseOptions.EnableHealthChecks)];
                if (bool.TryParse(enableHealthChecksValue, out var enableHealthChecks))
                    options.EnableHealthChecks = enableHealthChecks;

                var healthCheckIntervalValue = section[nameof(DatabaseOptions.HealthCheckInterval)];
                if (int.TryParse(healthCheckIntervalValue, out var healthCheckInterval) && healthCheckInterval > 0)
                    options.HealthCheckInterval = healthCheckInterval;
            }

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
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            optionsBuilder.UseMySql(connectionString, serverVersion, mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: options.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(options.MaxRetryDelay),
                    errorNumbersToAdd: null);

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