using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.RestoreSystem;

public class RestoreSystemCommandHandler : IRequestHandler<RestoreSystemCommand, RestoreSystemResponse>
{
    private readonly IDatabaseBackupService _databaseBackupService;
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly ILogger<RestoreSystemCommandHandler> _logger;

    public RestoreSystemCommandHandler(
        IDatabaseBackupService databaseBackupService,
        ISystemBackupRepository systemBackupRepository,
        ILogger<RestoreSystemCommandHandler> logger)
    {
        _databaseBackupService = databaseBackupService;
        _systemBackupRepository = systemBackupRepository;
        _logger = logger;
    }

    public async Task<RestoreSystemResponse> Handle(RestoreSystemCommand request, CancellationToken cancellationToken)
    {
        // Restore whole database from file, or from latest backup when FilePath is omitted.
        (string filePath, DateTime restoredAtUtc) result;
        if (request.Request.BackupId.HasValue)
        {
            var backup = await _systemBackupRepository.GetByIdAsync(request.Request.BackupId.Value)
                ?? throw new KeyNotFoundException($"Backup with ID {request.Request.BackupId.Value} was not found.");

            if (backup.IsDeleted)
                throw new InvalidOperationException("Selected backup has been deleted.");

            // Restore is performed from storage by resolving a signed URL internally.
            // We pass the object name via a pseudo-path: "storage://<objectName>"
            result = await _databaseBackupService.RestoreAsync($"storage://{backup.StorageObjectName}", cancellationToken);
        }
        else if (string.IsNullOrWhiteSpace(request.Request.FilePath))
        {
            result = await _databaseBackupService.RestoreLatestAsync(cancellationToken);
        }
        else
        {
            result = await _databaseBackupService.RestoreAsync(request.Request.FilePath, cancellationToken);
        }

        try
        {
            // Update restored time in DB record if possible.
            var record = request.Request.BackupId.HasValue
                ? await _systemBackupRepository.GetByIdAsync(request.Request.BackupId.Value)
                : null;
            if (record != null)
            {
                record.RestoredAtUtc = result.restoredAtUtc;
                await _systemBackupRepository.UpdateAsync(record);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update system_backups restore metadata.");
        }

        return new RestoreSystemResponse
        {
            InsertedCount = 0,
            SkippedCount = 0,
            RestoredAt = result.restoredAtUtc,
            ActorUserId = request.ActorUserId,
            RestoredFromFilePath = result.filePath
        };
    }
}

