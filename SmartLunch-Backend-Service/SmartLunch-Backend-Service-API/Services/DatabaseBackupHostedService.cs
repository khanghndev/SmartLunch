using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Services;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Services;

public class DatabaseBackupHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseBackupHostedService> _logger;

    public DatabaseBackupHostedService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseBackupHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DatabaseBackupHostedService started (DB schedule).");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scheduleRepo = scope.ServiceProvider.GetRequiredService<ISystemBackupScheduleRepository>();
                var schedule = await scheduleRepo.GetOrCreateAsync();

                if (!schedule.IsEnabled)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                var next = schedule.NextRunAt ?? BackupScheduleCalculator.ComputeNextRun(schedule);
                if (!next.HasValue)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                if (schedule.NextRunAt != next)
                {
                    schedule.NextRunAt = next;
                    await scheduleRepo.UpdateAsync(schedule);
                }

                var delay = next.Value - VietnamTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    var wait = delay > TimeSpan.FromMinutes(30) ? TimeSpan.FromMinutes(30) : delay;
                    await Task.Delay(wait, stoppingToken);
                    continue;
                }

                var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
                await backupService.CreateBackupAsync("Scheduled", stoppingToken);

                schedule = await scheduleRepo.GetOrCreateAsync();
                schedule.LastRunAt = VietnamTime.Now;

                if (string.Equals(schedule.ScheduleMode, "Once", StringComparison.OrdinalIgnoreCase))
                {
                    schedule.IsEnabled = false;
                    schedule.NextRunAt = null;
                }
                else
                {
                    schedule.NextRunAt = BackupScheduleCalculator.ComputeNextRun(schedule);
                }

                await scheduleRepo.UpdateAsync(schedule);
                _logger.LogInformation("Scheduled database backup completed. NextRun={NextRun}", schedule.NextRunAt);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled database backup loop failed.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
