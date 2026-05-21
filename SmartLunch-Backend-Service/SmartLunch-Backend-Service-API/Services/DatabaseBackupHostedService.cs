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
        _logger.LogInformation("DatabaseBackupHostedService started (daily schedule, VN time).");

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

                var nextRun = schedule.NextRunAt ?? BackupScheduleCalculator.ComputeNextRun(schedule);
                if (!nextRun.HasValue)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                if (schedule.NextRunAt != nextRun)
                {
                    schedule.NextRunAt = nextRun;
                    await scheduleRepo.UpdateAsync(schedule);
                }

                var now = VietnamTime.Now;
                var delay = nextRun.Value - now;

                // Chờ đến đúng giờ hẹn (không chạy sớm, không polling 30 phút).
                if (delay > TimeSpan.FromSeconds(30))
                {
                    var wait = delay > TimeSpan.FromHours(24) ? TimeSpan.FromHours(24) : delay;
                    _logger.LogDebug("Next scheduled backup at {NextRun} (sleep {Minutes:F0} min)", nextRun, wait.TotalMinutes);
                    await Task.Delay(wait, stoppingToken);
                    continue;
                }

                // Tránh chạy lặp khi upload lỗi: đã chạy trong 2 giờ gần đây thì bỏ qua slot này.
                if (schedule.LastRunAt.HasValue && (now - schedule.LastRunAt.Value) < TimeSpan.FromHours(2))
                {
                    schedule.NextRunAt = BackupScheduleCalculator.ComputeNextRun(schedule);
                    await scheduleRepo.UpdateAsync(schedule);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Running scheduled database backup at {Now} (planned {NextRun})", now, nextRun);

                var backupService = scope.ServiceProvider.GetRequiredService<IDatabaseBackupService>();
                try
                {
                    await backupService.CreateBackupAsync("Scheduled", stoppingToken);
                    schedule = await scheduleRepo.GetOrCreateAsync();
                    schedule.LastRunAt = VietnamTime.Now;
                    schedule.NextRunAt = BackupScheduleCalculator.ComputeNextRun(schedule);
                    await scheduleRepo.UpdateAsync(schedule);
                    _logger.LogInformation("Scheduled backup OK. NextRun={NextRun}", schedule.NextRunAt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled database backup failed.");
                    schedule = await scheduleRepo.GetOrCreateAsync();
                    schedule.NextRunAt = BackupScheduleCalculator.ComputeNextRun(schedule);
                    await scheduleRepo.UpdateAsync(schedule);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled database backup loop failed.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
