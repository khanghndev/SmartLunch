using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.DeleteSystemBackup;

public class DeleteSystemBackupCommandHandler : IRequestHandler<DeleteSystemBackupCommand, DeleteSystemBackupResponse>
{
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly IStorageService _storage;
    private readonly ILogger<DeleteSystemBackupCommandHandler> _logger;

    public DeleteSystemBackupCommandHandler(
        ISystemBackupRepository systemBackupRepository,
        IStorageService storage,
        ILogger<DeleteSystemBackupCommandHandler> logger)
    {
        _systemBackupRepository = systemBackupRepository;
        _storage = storage;
        _logger = logger;
    }

    public async Task<DeleteSystemBackupResponse> Handle(DeleteSystemBackupCommand request, CancellationToken cancellationToken)
    {
        var backup = await _systemBackupRepository.GetByIdAsync(request.BackupId)
            ?? throw new KeyNotFoundException($"Backup with ID {request.BackupId} was not found.");

        if (backup.IsDeleted)
            throw new InvalidOperationException("Backup already deleted.");

        var deletedAt = DateTime.UtcNow;
        var physicalDeleted = false;

        if (request.DeletePhysicalFile)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(backup.StorageObjectName))
                    await _storage.DeleteObjectAsync(backup.StorageObjectName);
                physicalDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete backup object {ObjectName}", backup.StorageObjectName);
            }
        }

        backup.IsDeleted = true;
        backup.DeletedAtUtc = deletedAt;
        await _systemBackupRepository.UpdateAsync(backup);

        _logger.LogWarning("Backup deleted. BackupId={BackupId} Actor={ActorUserId} PhysicalFileDeleted={PhysicalFileDeleted}",
            request.BackupId, request.ActorUserId, physicalDeleted);

        return new DeleteSystemBackupResponse
        {
            BackupId = backup.Id,
            IsDeleted = true,
            PhysicalFileDeleted = physicalDeleted,
            DeletedAtUtc = deletedAt,
            FilePath = backup.StorageObjectName
        };
    }
}

