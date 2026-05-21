using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ISystemBackupScheduleRepository
{
    Task<SystemBackupSchedule> GetOrCreateAsync();
    Task<SystemBackupSchedule> UpdateAsync(SystemBackupSchedule schedule);
}
