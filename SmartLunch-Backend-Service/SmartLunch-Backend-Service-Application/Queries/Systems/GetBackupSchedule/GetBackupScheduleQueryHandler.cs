using MediatR;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Services;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetBackupSchedule;

public class GetBackupScheduleQueryHandler : IRequestHandler<GetBackupScheduleQuery, BackupScheduleDto>
{
    private readonly ISystemBackupScheduleRepository _scheduleRepository;
    private readonly ISystemBackupRepository _backupRepository;
    private readonly IConfiguration _configuration;

    public GetBackupScheduleQueryHandler(
        ISystemBackupScheduleRepository scheduleRepository,
        ISystemBackupRepository backupRepository,
        IConfiguration configuration)
    {
        _scheduleRepository = scheduleRepository;
        _backupRepository = backupRepository;
        _configuration = configuration;
    }

    public async Task<BackupScheduleDto> Handle(GetBackupScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetOrCreateAsync();
        var latest = await _backupRepository.GetLatestAsync();
        var stats = await _backupRepository.GetStorageStatsAsync();

        var nextRun = schedule.NextRunAt ?? BackupScheduleCalculator.ComputeNextRun(schedule);

        return new BackupScheduleDto
        {
            IsEnabled = schedule.IsEnabled,
            ScheduleMode = schedule.ScheduleMode,
            TimeOfDay = BackupScheduleCalculator.FormatTimeOfDayMinutes(schedule.TimeOfDayMinutes),
            DayOfWeek = schedule.DayOfWeek,
            OnceScheduledAt = schedule.OnceScheduledAt,
            LastRunAt = schedule.LastRunAt,
            NextRunAt = nextRun,
            UpdatedAt = schedule.UpdatedAt,
            LatestBackupAt = latest?.CreatedAtUtc,
            LatestBackupFileName = latest?.FileName,
            TotalBackupBytes = stats.TotalBytes,
            TotalBackupCount = stats.TotalCount,
            StorageProvider = string.IsNullOrWhiteSpace(_configuration["Appwrite:Endpoint"]) ? "Appwrite" : "Appwrite Cloud"
        };
    }
}
