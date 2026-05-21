using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackups;

public class GetSystemBackupsQueryHandler : IRequestHandler<GetSystemBackupsQuery, GetSystemBackupsResponse>
{
    private readonly ISystemBackupRepository _systemBackupRepository;
    private readonly ILogger<GetSystemBackupsQueryHandler> _logger;

    public GetSystemBackupsQueryHandler(ISystemBackupRepository systemBackupRepository, ILogger<GetSystemBackupsQueryHandler> logger)
    {
        _systemBackupRepository = systemBackupRepository;
        _logger = logger;
    }

    public async Task<GetSystemBackupsResponse> Handle(GetSystemBackupsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 10 : request.PageSize;

        var (backups, totalCount) = await _systemBackupRepository.GetBackupsAsync(
            page, pageSize, request.IncludeDeleted, request.From, request.To);

        var dtos = backups.Select(e => new SystemBackupDto
        {
            Id = e.Id,
            FileName = e.FileName,
            StorageBucket = e.StorageBucket,
            StorageObjectName = e.StorageObjectName,
            SizeBytes = e.SizeBytes,
            BackupSource = e.BackupSource,
            CreatedAtUtc = e.CreatedAtUtc,
            RestoredAtUtc = e.RestoredAtUtc,
            IsDeleted = e.IsDeleted
        }).ToList();

        _logger.LogInformation("Retrieved {Count} system backups (Page {Page}, PageSize {PageSize})",
            dtos.Count, page, pageSize);

        return new GetSystemBackupsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}

