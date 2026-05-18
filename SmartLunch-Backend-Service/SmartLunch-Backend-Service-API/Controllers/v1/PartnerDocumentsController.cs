using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Partner document upload (business license, certificates, etc.) - private by default.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/Partner/{partnerId:int}/documents")]
[Authorize(Policy = "roles:Admin,Manager")]
public class PartnerDocumentsController : ControllerBase
{
    private readonly ILogger<PartnerDocumentsController> _logger;
    private readonly SmartLunchDBContext _db;
    private readonly IStorageService _storage;
    private readonly IMediaFileRepository _mediaFiles;
    private readonly IConfiguration _configuration;

    public PartnerDocumentsController(
        ILogger<PartnerDocumentsController> logger,
        SmartLunchDBContext db,
        IStorageService storage,
        IMediaFileRepository mediaFiles,
        IConfiguration configuration)
    {
        _logger = logger;
        _db = db;
        _storage = storage;
        _mediaFiles = mediaFiles;
        _configuration = configuration;
    }

    [HttpPost]
    [EnableRateLimiting("media-upload")]
    [Authorize(Policy = "permission:partners.update")]
    [RequestSizeLimit(210 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<UploadPartnerDocumentResponse>>> Upload(int partnerId, [FromForm] IFormFile file, [FromForm] string documentType = "business_license")
    {
        try
        {
            var userId = GetUserIdOrThrow();

            var partnerExists = await _db.Partners.AnyAsync(p => p.Id == partnerId);
            if (!partnerExists)
                return NotFound(BaseApiResponse<UploadPartnerDocumentResponse>.ErrorResult("Partner not found", new[] { "Partner not found" }));

            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<UploadPartnerDocumentResponse>.ErrorResult("File is required", new[] { "Missing file." }));

            var ct = file.ContentType ?? "application/octet-stream";
            var ext = ct.ToLowerInvariant() switch
            {
                "application/pdf" => ".pdf",
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ""
            };

            var now = VietnamTime.Now;
            var safeType = (documentType ?? "other").Trim().ToLowerInvariant();
            var objectName = $"users/{userId:D}/partner/{partnerId:D}/documents/{safeType}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

            await using var stream = file.OpenReadStream();
            await _storage.UploadObjectAsync(objectName, stream, ct, HttpContext.RequestAborted);

            var bucketId = _configuration["Appwrite:BucketId"] ?? "";
            var media = new MediaFile
            {
                OwnerUserId = userId,
                Bucket = bucketId,
                ObjectName = objectName,
                OriginalFileName = file.FileName,
                ContentType = ct,
                SizeBytes = file.Length,
                MediaType = "document",
                IsPublic = false
            };
            var createdMedia = await _mediaFiles.CreateAsync(media);

            var doc = new PartnerDocument
            {
                PartnerId = partnerId,
                MediaFileId = createdMedia.Id,
                DocumentType = safeType,
                IsVerified = false,
                CreatedAt = VietnamTime.Now
            };
            _db.PartnerDocuments.Add(doc);
            await _db.SaveChangesAsync();

            var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
            var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
            var signed = await _storage.CreateSignedUrlAsync(objectName, HttpMethod.Get, contentType: null, expiresIn: expiresIn);

            var response = new UploadPartnerDocumentResponse
            {
                PartnerDocumentId = doc.Id,
                MediaFileId = createdMedia.Id,
                DocumentType = doc.DocumentType,
                Url = signed.Url,
                ExpiresAtUtc = signed.ExpiresAtUtc
            };

            return Ok(BaseApiResponse<UploadPartnerDocumentResponse>.SuccessResult(response, "Uploaded partner document successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<UploadPartnerDocumentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading partner document. PartnerId={PartnerId}", partnerId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<UploadPartnerDocumentResponse>.ErrorResult("An error occurred while uploading partner document", new[] { ex.Message }));
        }
    }

    private int GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context");
        return userId;
    }
}

public sealed class UploadPartnerDocumentResponse
{
    public int PartnerDocumentId { get; set; }
    public int MediaFileId { get; set; }
    public string DocumentType { get; set; } = "other";
    public string Url { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

