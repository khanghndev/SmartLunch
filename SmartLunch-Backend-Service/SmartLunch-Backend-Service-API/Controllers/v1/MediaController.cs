using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLunch.Backend.Service.Application.Commands.Media.ConfirmUpload;
using SmartLunch.Backend.Service.Application.Commands.Media.CreateUploadUrl;
using SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Media;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Media upload/download controller (Appwrite Storage bucket)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly ILogger<MediaController> _logger;
    private readonly IMediator _mediator;
    private readonly IStorageService _storage;
    private readonly IMediaFileRepository _mediaFileRepository;
    private readonly IConfiguration _configuration;

    public MediaController(
        ILogger<MediaController> logger,
        IMediator mediator,
        IStorageService storage,
        IMediaFileRepository mediaFileRepository,
        IConfiguration configuration)
    {
        _logger = logger;
        _mediator = mediator;
        _storage = storage;
        _mediaFileRepository = mediaFileRepository;
        _configuration = configuration;
    }

    [HttpPost("upload-url")]
    public async Task<ActionResult<BaseApiResponse<CreateMediaUploadUrlResponse>>> CreateUploadUrl([FromBody] CreateMediaUploadUrlRequest request)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new CreateMediaUploadUrlCommand(userId, request));
            response.UploadUrl = BuildProxyUploadUrl(response.ObjectName);
            response.RequiredHeaders["Content-Type"] = request.ContentType;
            return Ok(BaseApiResponse<CreateMediaUploadUrlResponse>.SuccessResult(response, "Upload URL created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating upload URL");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateMediaUploadUrlResponse>.ErrorResult("An error occurred while creating upload URL", new[] { ex.Message }));
        }
    }

    [HttpPost("confirm-upload")]
    public async Task<ActionResult<BaseApiResponse<ConfirmMediaUploadResponse>>> ConfirmUpload([FromBody] ConfirmMediaUploadRequest request)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new ConfirmMediaUploadCommand(userId, request));
            return Ok(BaseApiResponse<ConfirmMediaUploadResponse>.SuccessResult(response, "Upload confirmed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ConfirmMediaUploadResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming upload");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ConfirmMediaUploadResponse>.ErrorResult("An error occurred while confirming upload", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Direct upload via backend (multipart). Stores metadata in DB (media_files).
    /// Frontend never needs Appwrite keys.
    /// </summary>
    [HttpPost("upload")]
    [EnableRateLimiting("media-upload")]
    [RequestSizeLimit(210 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<MediaUploadResponse>>> Upload(
        [FromForm] IFormFile file,
        [FromForm] string mediaType = "image",
        [FromForm] string? purpose = null,
        [FromForm] bool isPublic = false)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<MediaUploadResponse>.ErrorResult("File is required", new[] { "Missing file." }));

            var ct = file.ContentType ?? string.Empty;
            var mt = NormalizeMediaType(mediaType);
            ValidateContentType(mt, ct);

            // Size limits from config
            var maxImageBytes = long.TryParse(_configuration["Media:MaxImageBytes"], out var img) ? img : 10L * 1024 * 1024;
            var maxVideoBytes = long.TryParse(_configuration["Media:MaxVideoBytes"], out var vid) ? vid : 200L * 1024 * 1024;
            var max = mt == "image" ? maxImageBytes : maxVideoBytes;
            if (file.Length > max)
                return BadRequest(BaseApiResponse<MediaUploadResponse>.ErrorResult("File is too large", new[] { $"Max for {mt} is {max} bytes" }));

            var ext = GuessExtensionFromContentType(ct);
            var safePurpose = string.IsNullOrWhiteSpace(purpose) ? "general" : Slugify(purpose);
            var now = VietnamTime.Now;
            var objectName = $"users/{userId:D}/{safePurpose}/{mt}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

            await using var stream = file.OpenReadStream();
            await _storage.UploadObjectAsync(objectName, stream, ct, HttpContext.RequestAborted);

            // Persist metadata row
            var bucketId = _configuration["Appwrite:BucketId"] ?? "";
            var entity = new SmartLunch.Backend.Service.Domain.Entities.MediaFile
            {
                OwnerUserId = userId,
                Bucket = bucketId,
                ObjectName = objectName,
                OriginalFileName = file.FileName,
                ContentType = ct,
                SizeBytes = file.Length,
                MediaType = mt,
                IsPublic = isPublic
            };
            var created = await _mediaFileRepository.CreateAsync(entity);

            var url = isPublic ? BuildPublicViewUrl(objectName) : (await CreateSignedViewUrlAsync(objectName));
            var resp = new MediaUploadResponse
            {
                MediaFileId = created.Id,
                ObjectName = objectName,
                ContentType = ct,
                SizeBytes = created.SizeBytes,
                IsPublic = isPublic,
                Url = url
            };

            return Ok(BaseApiResponse<MediaUploadResponse>.SuccessResult(resp, "Uploaded successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<MediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<MediaUploadResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading media");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<MediaUploadResponse>.ErrorResult("An error occurred while uploading media", new[] { ex.Message }));
        }
    }

    [HttpGet("{id:int}/download-url")]
    public async Task<ActionResult<BaseApiResponse<GetMediaDownloadUrlResponse>>> GetDownloadUrl(int id, [FromQuery] int? expiresMinutes = null)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var response = await _mediator.Send(new GetMediaDownloadUrlCommand(userId, id, expiresMinutes));
            return Ok(BaseApiResponse<GetMediaDownloadUrlResponse>.SuccessResult(response, "Download URL created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetMediaDownloadUrlResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating download URL");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMediaDownloadUrlResponse>.ErrorResult("An error occurred while creating download URL", new[] { ex.Message }));
        }
    }

    [HttpPut("upload-proxy")]
    [EnableRateLimiting("media-upload")]
    public async Task<ActionResult<BaseApiResponse<object>>> UploadProxy(
        [FromQuery] string objectName,
        [FromHeader(Name = "Content-Type")] string? contentType = null)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var expectedPrefix = $"users/{userId:D}/";
            if (string.IsNullOrWhiteSpace(objectName) || !objectName.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid objectName", new[] { "objectName must be under current user prefix." }));

            if (string.IsNullOrWhiteSpace(contentType))
                return BadRequest(BaseApiResponse<object>.ErrorResult("Content-Type is required", new[] { "Missing Content-Type header." }));

            if (Request.ContentLength is null or <= 0)
                return BadRequest(BaseApiResponse<object>.ErrorResult("Body is required", new[] { "Upload body is empty." }));

            await _storage.UploadObjectAsync(objectName, Request.Body, contentType, HttpContext.RequestAborted);
            return Ok(BaseApiResponse<object>.SuccessResult(new { ObjectName = objectName }, "Uploaded to Appwrite successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<object>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxy uploading media");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while uploading media", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Upload an image via backend (server keeps Appwrite API key).
    /// Returns a time-limited view URL.
    /// </summary>
    [HttpPost("upload-image")]
    [EnableRateLimiting("media-upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<UploadImageResponse>>> UploadImage([FromForm] IFormFile file)
    {
        try
        {
            var userId = GetUserIdOrThrow();

            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<UploadImageResponse>.ErrorResult("File is required", new[] { "Missing file." }));

            var contentType = file.ContentType ?? string.Empty;
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(contentType))
                return BadRequest(BaseApiResponse<UploadImageResponse>.ErrorResult("Unsupported image ContentType", new[] { $"ContentType: {contentType}" }));

            var ext = contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ""
            };

            var now = VietnamTime.Now;
            var objectName = $"users/{userId:D}/image/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

            await using var stream = file.OpenReadStream();
            await _storage.UploadObjectAsync(objectName, stream, contentType, HttpContext.RequestAborted);

            var cfg = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expiresMinutes = int.TryParse(cfg["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
            var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
            var signed = await _storage.CreateSignedUrlAsync(objectName, HttpMethod.Get, contentType: null, expiresIn: expiresIn);

            var payload = new UploadImageResponse
            {
                ObjectName = objectName,
                Url = signed.Url,
                ExpiresAtUtc = signed.ExpiresAtUtc
            };

            return Ok(BaseApiResponse<UploadImageResponse>.SuccessResult(payload, "Uploaded image successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<UploadImageResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<UploadImageResponse>.ErrorResult("An error occurred while uploading image", new[] { ex.Message }));
        }
    }

    private int GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user context");
        }

        return userId;
    }

    private string BuildProxyUploadUrl(string objectName)
    {
        var version = RouteData.Values.TryGetValue("version", out var raw) ? raw?.ToString() : "1";
        var encoded = Uri.EscapeDataString(objectName);
        return $"{Request.Scheme}://{Request.Host}/api/v{version}/Media/upload-proxy?objectName={encoded}";
    }

    private async Task<string> CreateSignedViewUrlAsync(string objectName)
    {
        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(objectName, System.Net.Http.HttpMethod.Get, contentType: null, expiresIn: expiresIn);
        return signed.Url;
    }

    private string BuildPublicViewUrl(string objectName)
    {
        // For public images, return Appwrite view URL without token.
        // Requires bucket/file permissions configured in Appwrite as public-read.
        var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
        var bucketId = _configuration["Appwrite:BucketId"] ?? "";
        var projectId = _configuration["Appwrite:ProjectId"] ?? "";
        var fileId = ToFileId(objectName);
        return $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }

    private static string NormalizeMediaType(string? mediaType)
    {
        var mt = (mediaType ?? string.Empty).Trim().ToLowerInvariant();
        return mt switch
        {
            "image" => "image",
            "video" => "video",
            "document" => "document",
            _ => throw new ArgumentException("MediaType must be 'image', 'video', or 'document'")
        };
    }

    private static void ValidateContentType(string mediaType, string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("ContentType is required");

        if (mediaType == "image")
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(contentType))
                throw new ArgumentException($"Unsupported image ContentType: {contentType}");
            return;
        }

        if (mediaType == "video")
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "video/mp4", "video/webm", "video/quicktime" };
            if (!allowed.Contains(contentType))
                throw new ArgumentException($"Unsupported video ContentType: {contentType}");
            return;
        }

        // document
        var allowedDoc = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp"
        };
        if (!allowedDoc.Contains(contentType))
            throw new ArgumentException($"Unsupported document ContentType: {contentType}");
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
            "application/pdf" => ".pdf",
            _ => string.Empty
        };
    }

    private static string Slugify(string input)
    {
        var s = input.Trim().ToLowerInvariant();
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (char.IsLetterOrDigit(ch)) sb.Append(ch);
            else if (ch == '-' || ch == '_' || ch == ' ') sb.Append('-');
        }
        var result = sb.ToString();
        while (result.Contains("--")) result = result.Replace("--", "-");
        return result.Trim('-');
    }
}

public sealed class UploadImageResponse
{
    public string ObjectName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed class MediaUploadResponse
{
    public int MediaFileId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsPublic { get; set; }
    public string Url { get; set; } = string.Empty;
}

