using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Media.ConfirmUpload;

public class ConfirmMediaUploadCommandHandler : IRequestHandler<ConfirmMediaUploadCommand, ConfirmMediaUploadResponse>
{
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private static readonly HashSet<string> AllowedVideoContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4",
        "video/webm",
        "video/quicktime"
    };

    private readonly IFirebaseStorageService _storage;
    private readonly IMediaFileRepository _mediaFileRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfirmMediaUploadCommandHandler> _logger;

    public ConfirmMediaUploadCommandHandler(
        IFirebaseStorageService storage,
        IMediaFileRepository mediaFileRepository,
        IConfiguration configuration,
        ILogger<ConfirmMediaUploadCommandHandler> logger)
    {
        _storage = storage;
        _mediaFileRepository = mediaFileRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ConfirmMediaUploadResponse> Handle(ConfirmMediaUploadCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (string.IsNullOrWhiteSpace(req.ObjectName))
        {
            throw new ArgumentException("ObjectName is required");
        }

        // Authorization: user can only confirm uploads under their own prefix.
        var expectedPrefix = $"users/{command.UserId:D}/";
        if (!req.ObjectName.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("You are not allowed to confirm this object");
        }

        var metadata = await _storage.GetObjectMetadataAsync(req.ObjectName);
        if (metadata == null)
        {
            throw new KeyNotFoundException("Uploaded object not found");
        }

        if (string.IsNullOrWhiteSpace(metadata.ContentType))
        {
            throw new InvalidOperationException("Uploaded object missing ContentType");
        }

        var actualMediaType = InferMediaType(metadata.ContentType);
        ValidateContentType(actualMediaType, metadata.ContentType);
        var requestedMediaType = NormalizeMediaType(req.MediaType);

        if (!string.Equals(actualMediaType, requestedMediaType, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"MediaType mismatch. Requested={requestedMediaType}, Actual={actualMediaType}");
        }

        ValidateSize(actualMediaType, metadata.SizeBytes);

        var entity = new MediaFile
        {

            OwnerUserId = command.UserId,
            Bucket = metadata.Bucket,
            ObjectName = metadata.ObjectName,
            OriginalFileName = string.IsNullOrWhiteSpace(req.OriginalFileName) ? null : req.OriginalFileName.Trim(),
            ContentType = metadata.ContentType,
            SizeBytes = metadata.SizeBytes,
            Md5HashBase64 = metadata.Md5HashBase64,
            MediaType = actualMediaType,
            IsPublic = req.IsPublic,
            CreatedAt = DateTime.UtcNow
        };

        await _mediaFileRepository.CreateAsync(entity);

        _logger.LogInformation(
            "Confirmed media upload. MediaFileId={MediaFileId}, UserId={UserId}, ObjectName={ObjectName}, ContentType={ContentType}, SizeBytes={SizeBytes}",
            entity.Id, command.UserId, entity.ObjectName, entity.ContentType, entity.SizeBytes);

        return new ConfirmMediaUploadResponse
        {
            MediaFile = new MediaFileDto
            {
                Id = entity.Id,
                Bucket = entity.Bucket,
                ObjectName = entity.ObjectName,
                OriginalFileName = entity.OriginalFileName,
                ContentType = entity.ContentType,
                SizeBytes = entity.SizeBytes,
                Md5HashBase64 = entity.Md5HashBase64,
                MediaType = entity.MediaType,
                IsPublic = entity.IsPublic,
                CreatedAt = entity.CreatedAt
            }
        };
    }

    private static string NormalizeMediaType(string? mediaType)
    {
        var mt = (mediaType ?? string.Empty).Trim().ToLowerInvariant();
        return mt switch
        {
            "image" => "image",
            "video" => "video",
            _ => throw new ArgumentException("MediaType must be 'image' or 'video'")
        };
    }

    private static string InferMediaType(string contentType)
    {
        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return "image";
        }

        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            return "video";
        }

        throw new ArgumentException($"Unsupported ContentType: {contentType}");
    }

    private static void ValidateContentType(string mediaType, string contentType)
    {
        if (mediaType == "image" && !AllowedImageContentTypes.Contains(contentType))
        {
            throw new ArgumentException($"Unsupported image ContentType: {contentType}");
        }

        if (mediaType == "video" && !AllowedVideoContentTypes.Contains(contentType))
        {
            throw new ArgumentException($"Unsupported video ContentType: {contentType}");
        }
    }

    private void ValidateSize(string mediaType, long sizeBytes)
    {
        var maxImageBytes = long.TryParse(_configuration["Media:MaxImageBytes"], out var img) ? img : 10L * 1024 * 1024; // 10MB
        var maxVideoBytes = long.TryParse(_configuration["Media:MaxVideoBytes"], out var vid) ? vid : 200L * 1024 * 1024; // 200MB

        var max = mediaType == "image" ? maxImageBytes : maxVideoBytes;
        if (sizeBytes > max)
        {
            throw new ArgumentException($"File is too large. Max allowed for {mediaType} is {max} bytes");
        }
    }
}

