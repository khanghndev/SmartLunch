using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.BackupSystem;

public class BackupSystemQueryHandler : IRequestHandler<BackupSystemQuery, BackupSystemResponse>
{
    private readonly IDatabaseBackupService _databaseBackupService;
    private readonly ILogger<BackupSystemQueryHandler> _logger;

    public BackupSystemQueryHandler(IDatabaseBackupService databaseBackupService, ILogger<BackupSystemQueryHandler> logger)
    {
        _databaseBackupService = databaseBackupService;
        _logger = logger;
    }

    public async Task<BackupSystemResponse> Handle(BackupSystemQuery request, CancellationToken cancellationToken)
    {
        // NOTE: This backup exports the whole MySQL database using mysqldump (file extension .bak by convention).
        // The `id` parameter is reserved for future extension (multi-tenant/system-id).
        var (filePath, sizeBytes, createdAtUtc) = await _databaseBackupService.CreateBackupAsync(cancellationToken);
        var fileName = Path.GetFileName(filePath);

        _logger.LogInformation("Created database backup. File={FileName} SizeBytes={SizeBytes}", fileName, sizeBytes);

        return new BackupSystemResponse
        {
            FileName = fileName,
            FilePath = filePath,
            SizeBytes = sizeBytes,
            CreatedAtUtc = createdAtUtc
        };
    }
}

