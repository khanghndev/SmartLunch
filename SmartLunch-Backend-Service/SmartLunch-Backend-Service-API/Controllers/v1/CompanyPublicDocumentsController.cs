using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLunch.Backend.Service.Application.CompanyPublicDocuments;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.CompanyPublicDocuments;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using System.Net;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/company-public-documents")]
public class CompanyPublicDocumentsController : ControllerBase
{
    private const long MaxBytes = 10 * 1024 * 1024;

    private readonly ILogger<CompanyPublicDocumentsController> _logger;
    private readonly ICompanyPublicDocumentRepository _repository;
    private readonly IMediaFileRepository _mediaFiles;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public CompanyPublicDocumentsController(
        ILogger<CompanyPublicDocumentsController> logger,
        ICompanyPublicDocumentRepository repository,
        IMediaFileRepository mediaFiles,
        IStorageService storage,
        IConfiguration configuration)
    {
        _logger = logger;
        _repository = repository;
        _mediaFiles = mediaFiles;
        _storage = storage;
        _configuration = configuration;
    }

    /// <summary>Danh sách hồ sơ công khai cho trang Giới thiệu (khách).</summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseApiResponse<CompanyPublicDocumentListResponse>>> GetPublic(CancellationToken cancellationToken)
    {
        try
        {
            var docs = await _repository.GetPublishedAsync(cancellationToken);
            var lifetime = TimeSpan.FromDays(7);
            var dtos = new List<CompanyPublicDocumentDto>();
            foreach (var d in docs)
                dtos.Add(await CompanyPublicDocumentMapper.ToDtoAsync(d, _storage, lifetime, cancellationToken));

            return Ok(BaseApiResponse<CompanyPublicDocumentListResponse>.SuccessResult(
                new CompanyPublicDocumentListResponse { Documents = dtos },
                "Public company documents retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading public company documents");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CompanyPublicDocumentListResponse>.ErrorResult("Failed to load documents", new[] { ex.Message }));
        }
    }

    [HttpGet]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<CompanyPublicDocumentListResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var docs = await _repository.GetAllAsync(cancellationToken);
            var lifetime = TimeSpan.FromMinutes(30);
            var dtos = new List<CompanyPublicDocumentDto>();
            foreach (var d in docs)
                dtos.Add(await CompanyPublicDocumentMapper.ToDtoAsync(d, _storage, lifetime, cancellationToken));

            return Ok(BaseApiResponse<CompanyPublicDocumentListResponse>.SuccessResult(
                new CompanyPublicDocumentListResponse { Documents = dtos },
                "Company documents retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading company documents for manager");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CompanyPublicDocumentListResponse>.ErrorResult("Failed to load documents", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "roles:Admin,Manager")]
    [EnableRateLimiting("media-upload")]
    [RequestSizeLimit(MaxBytes + 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<CompanyPublicDocumentDto>>> Upload(
        [FromForm] IFormFile file,
        [FromForm] string title,
        [FromForm] string documentType = "food_safety",
        [FromForm] string? description = null,
        [FromForm] DateTime? issuedDate = null,
        [FromForm] DateTime? expiryDate = null,
        [FromForm] bool isPublished = true,
        [FromForm] int sortOrder = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("File is required", new[] { "Missing file." }));
            if (file.Length > MaxBytes)
                return BadRequest(BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("File too large", new[] { "Max 10MB." }));

            var ct = file.ContentType ?? "application/octet-stream";
            if (!IsAllowedContentType(ct))
                return BadRequest(BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("Invalid file type", new[] { "Only PDF and images allowed." }));

            var ext = ct.ToLowerInvariant() switch
            {
                "application/pdf" => ".pdf",
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => Path.GetExtension(file.FileName)
            };

            var safeType = NormalizeType(documentType);
            var displayTitle = string.IsNullOrWhiteSpace(title)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : title.Trim();

            var now = VietnamTime.Now;
            var objectName = $"company/public-documents/{safeType}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

            await using var stream = file.OpenReadStream();
            await _storage.UploadObjectAsync(objectName, stream, ct, cancellationToken);

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
                IsPublic = true,
            };
            var createdMedia = await _mediaFiles.CreateAsync(media);

            var entity = new CompanyPublicDocument
            {
                Title = displayTitle,
                DocumentType = safeType,
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                MediaFileId = createdMedia.Id,
                SortOrder = sortOrder,
                IsPublished = isPublished,
                IssuedDate = issuedDate?.Date,
                ExpiryDate = expiryDate?.Date,
                CreatedByUserId = userId,
                CreatedAt = VietnamTime.Now,
            };
            var created = await _repository.CreateAsync(entity, cancellationToken);
            created.MediaFile = createdMedia;

            var dto = await CompanyPublicDocumentMapper.ToDtoAsync(
                created, _storage, TimeSpan.FromMinutes(30), cancellationToken);

            return Ok(BaseApiResponse<CompanyPublicDocumentDto>.SuccessResult(dto, "Document uploaded"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading company public document");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("Upload failed", new[] { ex.Message }));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<CompanyPublicDocumentDto>>> Update(
        int id,
        [FromBody] UpdateCompanyPublicDocumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return NotFound(BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("Not found", new[] { "Document not found" }));

            if (!string.IsNullOrWhiteSpace(request.Title))
                entity.Title = request.Title.Trim();
            if (!string.IsNullOrWhiteSpace(request.DocumentType))
                entity.DocumentType = NormalizeType(request.DocumentType);
            if (request.Description != null)
                entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            if (request.SortOrder.HasValue)
                entity.SortOrder = request.SortOrder.Value;
            if (request.IsPublished.HasValue)
                entity.IsPublished = request.IsPublished.Value;
            if (request.IssuedDate.HasValue)
                entity.IssuedDate = request.IssuedDate.Value.Date;
            if (request.ExpiryDate.HasValue)
                entity.ExpiryDate = request.ExpiryDate.Value.Date;

            entity.UpdatedAt = VietnamTime.Now;
            await _repository.UpdateAsync(entity, cancellationToken);

            var dto = await CompanyPublicDocumentMapper.ToDtoAsync(
                entity, _storage, TimeSpan.FromMinutes(30), cancellationToken);
            return Ok(BaseApiResponse<CompanyPublicDocumentDto>.SuccessResult(dto, "Updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company document {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CompanyPublicDocumentDto>.ErrorResult("Update failed", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return NotFound(BaseApiResponse<object>.ErrorResult("Not found", new[] { "Document not found" }));

            await _repository.DeleteAsync(id, cancellationToken);
            return Ok(BaseApiResponse<object>.SuccessResult(new { id }, "Deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company document {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("Delete failed", new[] { ex.Message }));
        }
    }

    private static bool IsAllowedContentType(string ct) =>
        ct.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ||
        ct.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeType(string? raw)
    {
        var t = (raw ?? "other").Trim().ToLowerInvariant();
        return t switch
        {
            "food_safety" or "iso" or "business_license" or "inspection" or "contract" => t,
            _ => "other"
        };
    }

    private int GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context");
        return userId;
    }
}

public sealed class UpdateCompanyPublicDocumentRequest
{
    public string? Title { get; set; }
    public string? DocumentType { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
