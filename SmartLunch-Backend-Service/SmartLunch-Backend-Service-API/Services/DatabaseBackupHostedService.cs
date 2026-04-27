using Cronos;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.API.Services;

public class DatabaseBackupHostedService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseBackupHostedService> _logger;

    public DatabaseBackupHostedService(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<DatabaseBackupHostedService> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var section = _configuration.GetSection("DatabaseBackup");
        var enabled = section.GetValue<bool?>("Enabled") ?? false;
        var cronExpr = section.GetValue<string>("Cron");

        if (!enabled)
        {
            _logger.LogInformation("DatabaseBackupHostedService disabled.");
            return;
        }

        if (string.IsNullOrWhiteSpace(cronExpr))
        {
            _logger.LogWarning("DatabaseBackupHostedService enabled but Cron not configured.");
            return;
        }

        CronExpression cron;
        try
        {
            cron = CronExpression.Parse(cronExpr, CronFormat.IncludeSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid DatabaseBackup:Cron expression: {Cron}", cronExpr);
            return;
        }

        _logger.LogInformation("DatabaseBackupHostedService started. Cron={Cron}", cronExpr);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.Now;
            var next = cron.GetNextOccurrence(now, TimeZoneInfo.Local);
            if (next == null)
            {
                _logger.LogWarning("Cron has no next occurrence. Stopping backup scheduler.");
                return;
            }

            var delay = next.Value - now;
            if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
                await backupService.CreateBackupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled database backup failed.");
            }
        }
    }
}

