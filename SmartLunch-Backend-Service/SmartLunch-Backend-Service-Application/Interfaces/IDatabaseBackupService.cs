namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDatabaseBackupService
{
    Task<(string FilePath, long SizeBytes, DateTime CreatedAtUtc)> CreateBackupAsync(CancellationToken cancellationToken = default);
    Task<(string FilePath, DateTime RestoredAtUtc)> RestoreLatestAsync(CancellationToken cancellationToken = default);
    Task<(string FilePath, DateTime RestoredAtUtc)> RestoreAsync(string filePath, CancellationToken cancellationToken = default);
    Task<string?> GetLatestBackupFileAsync(CancellationToken cancellationToken = default);
    Task<int> CleanupOldBackupsAsync(CancellationToken cancellationToken = default);
}

