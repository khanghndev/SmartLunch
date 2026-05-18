using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Commands.Media.CreateUploadUrl;

public class CreateMediaUploadUrlCommandHandler : IRequestHandler<CreateMediaUploadUrlCommand, CreateMediaUploadUrlResponse>
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

    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateMediaUploadUrlCommandHandler> _logger;

    public CreateMediaUploadUrlCommandHandler(
        IStorageService storage,
        IConfiguration configuration,
        ILogger<CreateMediaUploadUrlCommandHandler> logger)
    {
        _storage = storage;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CreateMediaUploadUrlResponse> Handle(CreateMediaUploadUrlCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (string.IsNullOrWhiteSpace(req.ContentType))
        {
            throw new ArgumentException("ContentType is required");
        }

        if (req.SizeBytes <= 0)
        {
            throw new ArgumentException("SizeBytes must be greater than 0");
        }

        var mediaType = NormalizeMediaType(req.MediaType);
        ValidateContentType(mediaType, req.ContentType);
        ValidateSize(mediaType, req.SizeBytes);

        var ext = GuessExtensionFromContentType(req.ContentType);
        var objectName = BuildObjectName(command.UserId, mediaType, ext);

        var expiresMinutes = int.TryParse(_configuration["Media:SignedUrlExpireMinutes"], out var m) ? m : 10;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Put,
            req.ContentType,
            expiresIn,
            requiredHeaders: null);

        _logger.LogInformation(
            "Issued signed upload URL. UserId={UserId}, MediaType={MediaType}, ContentType={ContentType}, SizeBytes={SizeBytes}, ObjectName={ObjectName}",
            command.UserId, mediaType, req.ContentType, req.SizeBytes, objectName);

        return new CreateMediaUploadUrlResponse
        {
            Bucket = signed.Bucket,
            ObjectName = signed.ObjectName,
            UploadUrl = signed.Url,
            ExpiresAtUtc = signed.ExpiresAtUtc,
            RequiredHeaders = signed.RequiredHeaders.ToDictionary(k => k.Key, v => v.Value)
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

    private static string GuessExtensionFromContentType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "video/mp4" => ".mp4",
            "video/webm" => ".webm",
            "video/quicktime" => ".mov",
            _ => string.Empty
        };
    }

    private static string BuildObjectName(int userId, string mediaType, string ext)
    {
        // Keep objects partitioned by user to simplify authorization checks.
        var now = VietnamTime.Now;
        var id = Guid.NewGuid().ToString("N");
        return $"users/{userId:D}/{mediaType}/{now:yyyy}/{now:MM}/{id}{ext}";
    }
}

