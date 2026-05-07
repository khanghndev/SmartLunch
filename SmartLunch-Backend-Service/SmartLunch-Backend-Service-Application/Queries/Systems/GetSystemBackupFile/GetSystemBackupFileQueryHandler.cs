using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackupFile;

public class GetSystemBackupFileQueryHandler : IRequestHandler<GetSystemBackupFileQuery, GetSystemBackupFileResponse>
{
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly IStorageService _storage;

    public GetSystemBackupFileQueryHandler(ISystemBackupRepository systemBackupRepository, IStorageService storage)
    {
        _systemBackupRepository = systemBackupRepository;
        _storage = storage;
    }

    public async Task<GetSystemBackupFileResponse> Handle(GetSystemBackupFileQuery request, CancellationToken cancellationToken)
    {
        var backup = await _systemBackupRepository.GetByIdAsync(request.BackupId)
            ?? throw new KeyNotFoundException($"Backup with ID {request.BackupId} was not found.");

        if (backup.IsDeleted)
            throw new InvalidOperationException("Backup has been deleted.");

        if (string.IsNullOrWhiteSpace(backup.StorageObjectName))
            throw new InvalidOperationException("Backup storage object name is missing.");

        var signed = await _storage.CreateSignedUrlAsync(
            backup.StorageObjectName,
            System.Net.Http.HttpMethod.Get,
            contentType: null,
            expiresIn: TimeSpan.FromMinutes(60));

        return new GetSystemBackupFileResponse
        {
            BackupId = backup.Id,
            FileName = backup.FileName,
            StorageBucket = backup.StorageBucket,
            StorageObjectName = backup.StorageObjectName,
            DownloadUrl = signed.Url,
            ExpiresAtUtc = signed.ExpiresAtUtc
        };
    }
}

