using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.BackupSystem;

public class BackupSystemQueryHandler : IRequestHandler<BackupSystemQuery, BackupSystemResponse>
{
    private readonly IDatabaseBackupService _databaseBackupService;
    private readonly IStorageService _storage;
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly ILogger<BackupSystemQueryHandler> _logger;

    public BackupSystemQueryHandler(
        IDatabaseBackupService databaseBackupService,
        IStorageService storage,
        ISystemBackupRepository systemBackupRepository,
        ILogger<BackupSystemQueryHandler> logger)
    {
        _databaseBackupService = databaseBackupService;
        _storage = storage;
        _systemBackupRepository = systemBackupRepository;
        _logger = logger;
    }

    public async Task<BackupSystemResponse> Handle(BackupSystemQuery request, CancellationToken cancellationToken)
    {
        // NOTE: This backup exports the whole MySQL database using mysqldump (file extension .bak by convention).
        // The `id` parameter is reserved for future extension (multi-tenant/system-id).
        var (filePath, sizeBytes, createdAtUtc) = await _databaseBackupService.CreateBackupAsync(cancellationToken);
        var fileName = Path.GetFileName(filePath);

        _logger.LogInformation("Created database backup. File={FileName} SizeBytes={SizeBytes}", fileName, sizeBytes);

        // Read latest metadata (created by DatabaseBackupService) to return storage info
        var latest = await _systemBackupRepository.GetLatestAsync();
        if (latest == null)
            throw new InvalidOperationException("Backup metadata not found after creation.");

        var signed = await _storage.CreateSignedUrlAsync(
            latest.StorageObjectName,
            System.Net.Http.HttpMethod.Get,
            contentType: null,
            expiresIn: TimeSpan.FromMinutes(60));

        return new BackupSystemResponse
        {
            FileName = fileName,
            StorageBucket = latest.StorageBucket,
            StorageObjectName = latest.StorageObjectName,
            DownloadUrl = signed.Url,
            DownloadUrlExpiresAtUtc = signed.ExpiresAtUtc,
            SizeBytes = sizeBytes,
            CreatedAtUtc = createdAtUtc
        };
    }
}

