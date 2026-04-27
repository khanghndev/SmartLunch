using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackupFile;

public class GetSystemBackupFileQueryHandler : IRequestHandler<GetSystemBackupFileQuery, GetSystemBackupFileResponse>
{
    private readonly ISystemBackupRepository _systemBackupRepository;

    public GetSystemBackupFileQueryHandler(ISystemBackupRepository systemBackupRepository)
    {
        _systemBackupRepository = systemBackupRepository;
    }

    public async Task<GetSystemBackupFileResponse> Handle(GetSystemBackupFileQuery request, CancellationToken cancellationToken)
    {
        var backup = await _systemBackupRepository.GetByIdAsync(request.BackupId)
            ?? throw new KeyNotFoundException($"Backup with ID {request.BackupId} was not found.");

        if (backup.IsDeleted)
            throw new InvalidOperationException("Backup has been deleted.");

        if (string.IsNullOrWhiteSpace(backup.FilePath))
            throw new InvalidOperationException("Backup file path is missing.");

        if (!File.Exists(backup.FilePath))
            throw new FileNotFoundException("Backup file not found on disk.", backup.FilePath);

        return new GetSystemBackupFileResponse
        {
            BackupId = backup.Id,
            FileName = backup.FileName,
            FilePath = backup.FilePath
        };
    }
}

