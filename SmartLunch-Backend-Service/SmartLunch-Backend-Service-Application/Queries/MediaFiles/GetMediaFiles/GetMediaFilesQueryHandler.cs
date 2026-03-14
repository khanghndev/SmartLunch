using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFiles;

public class GetMediaFilesQueryHandler : IRequestHandler<GetMediaFilesQuery, GetMediaFilesResponse>
{
    private readonly IMediaFileRepository _mediaFileRepository;
    private readonly ILogger<GetMediaFilesQueryHandler> _logger;

    public GetMediaFilesQueryHandler(IMediaFileRepository mediaFileRepository, ILogger<GetMediaFilesQueryHandler> logger)
    {
        _mediaFileRepository = mediaFileRepository;
        _logger = logger;
    }

    public async Task<GetMediaFilesResponse> Handle(GetMediaFilesQuery request, CancellationToken cancellationToken)
    {
        var (mediaFiles, totalCount) = await _mediaFileRepository.GetMediaFilesAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var mediaFileDtos = mediaFiles.Select(mediaFile => new MediaFileDto
        {
                Id = mediaFile.Id,
                OwnerUserId = mediaFile.OwnerUserId,
                Bucket = mediaFile.Bucket,
                ObjectName = mediaFile.ObjectName,
                OriginalFileName = mediaFile.OriginalFileName,
                ContentType = mediaFile.ContentType,
                SizeBytes = mediaFile.SizeBytes,
                Md5HashBase64 = mediaFile.Md5HashBase64,
                MediaType = mediaFile.MediaType,
                IsPublic = mediaFile.IsPublic,
                CreatedAt = mediaFile.CreatedAt,
                UpdatedAt = mediaFile.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} mediafiles (Page {Page}, PageSize {PageSize})",
            mediaFileDtos.Count, request.Page, request.PageSize);

        return new GetMediaFilesResponse
        {
            Data = mediaFileDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
