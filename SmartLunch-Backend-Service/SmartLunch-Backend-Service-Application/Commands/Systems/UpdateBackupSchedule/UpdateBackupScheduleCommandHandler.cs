using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Services;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.UpdateBackupSchedule;

public class UpdateBackupScheduleCommandHandler : IRequestHandler<UpdateBackupScheduleCommand, BackupScheduleDto>
{
    private readonly ISystemBackupScheduleRepository _scheduleRepository;
    private readonly ISystemBackupRepository _backupRepository;

    public UpdateBackupScheduleCommandHandler(
        ISystemBackupScheduleRepository scheduleRepository,
        ISystemBackupRepository backupRepository)
    {
        _scheduleRepository = scheduleRepository;
        _backupRepository = backupRepository;
    }

    public async Task<BackupScheduleDto> Handle(UpdateBackupScheduleCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        var mode = NormalizeMode(req.ScheduleMode);

        if (mode == "Once" && !req.OnceScheduledAt.HasValue)
            throw new ArgumentException("Vui lòng chọn ngày giờ cho lịch sao lưu một lần.");

        if (mode == "Once" && req.OnceScheduledAt!.Value <= VietnamTime.Now)
            throw new ArgumentException("Thời điểm sao lưu phải ở tương lai.");

        if (mode == "Weekly" && !req.DayOfWeek.HasValue)
            throw new ArgumentException("Vui lòng chọn thứ trong tuần cho lịch hàng tuần.");

        var schedule = await _scheduleRepository.GetOrCreateAsync();
        schedule.IsEnabled = req.IsEnabled;
        schedule.ScheduleMode = mode;
        schedule.TimeOfDayMinutes = BackupScheduleCalculator.ParseTimeOfDayMinutes(req.TimeOfDay);
        schedule.DayOfWeek = mode == "Weekly" ? req.DayOfWeek : null;
        schedule.OnceScheduledAt = mode == "Once" ? req.OnceScheduledAt : null;
        schedule.UpdatedAt = VietnamTime.Now;
        schedule.UpdatedByUserId = command.ActorUserId;
        schedule.NextRunAt = req.IsEnabled ? BackupScheduleCalculator.ComputeNextRun(schedule) : null;

        await _scheduleRepository.UpdateAsync(schedule);

        var latest = await _backupRepository.GetLatestAsync();
        var stats = await _backupRepository.GetStorageStatsAsync();

        return new BackupScheduleDto
        {
            IsEnabled = schedule.IsEnabled,
            ScheduleMode = schedule.ScheduleMode,
            TimeOfDay = BackupScheduleCalculator.FormatTimeOfDayMinutes(schedule.TimeOfDayMinutes),
            DayOfWeek = schedule.DayOfWeek,
            OnceScheduledAt = schedule.OnceScheduledAt,
            LastRunAt = schedule.LastRunAt,
            NextRunAt = schedule.NextRunAt,
            UpdatedAt = schedule.UpdatedAt,
            LatestBackupAt = latest?.CreatedAtUtc,
            LatestBackupFileName = latest?.FileName,
            TotalBackupBytes = stats.TotalBytes,
            TotalBackupCount = stats.TotalCount,
            StorageProvider = "Appwrite"
        };
    }

    private static string NormalizeMode(string? mode)
    {
        var m = (mode ?? "Daily").Trim();
        if (m.Equals("weekly", StringComparison.OrdinalIgnoreCase)) return "Weekly";
        if (m.Equals("once", StringComparison.OrdinalIgnoreCase)) return "Once";
        return "Daily";
    }
}
