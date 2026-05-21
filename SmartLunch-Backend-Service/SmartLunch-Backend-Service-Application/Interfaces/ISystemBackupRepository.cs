using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ISystemBackupRepository
{
    Task<SystemBackup?> GetByIdAsync(int id);
    Task<SystemBackup?> GetLatestAsync();
    Task<(List<SystemBackup> Backups, int TotalCount)> GetBackupsAsync(
        int page,
        int pageSize,
        bool includeDeleted = false,
        DateTime? from = null,
        DateTime? to = null);
    Task<(long TotalBytes, int TotalCount)> GetStorageStatsAsync();
    Task<SystemBackup> CreateAsync(SystemBackup backup);
    Task<SystemBackup> UpdateAsync(SystemBackup backup);
}

