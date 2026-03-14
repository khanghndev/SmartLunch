using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFile;

public class GetMediaFileQueryHandler : IRequestHandler<GetMediaFileQuery, GetMediaFileResponse>
{
    private readonly IMediaFileRepository _mediaFileRepository;
    private readonly ILogger<GetMediaFileQueryHandler> _logger;

    public GetMediaFileQueryHandler(IMediaFileRepository mediaFileRepository, ILogger<GetMediaFileQueryHandler> logger)
    {
        _mediaFileRepository = mediaFileRepository;
        _logger = logger;
    }

    public async Task<GetMediaFileResponse> Handle(GetMediaFileQuery request, CancellationToken cancellationToken)
    {
        var mediaFile = await _mediaFileRepository.GetByIdAsync(request.MediaFileId);

        if (mediaFile == null)
        {
            _logger.LogWarning("MediaFile not found with ID: {MediaFileId}", request.MediaFileId);
            return new GetMediaFileResponse { MediaFile = new MediaFileDto() };
        }

        return new GetMediaFileResponse
        {
            MediaFile = new MediaFileDto
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
            }
        };
    }
}
