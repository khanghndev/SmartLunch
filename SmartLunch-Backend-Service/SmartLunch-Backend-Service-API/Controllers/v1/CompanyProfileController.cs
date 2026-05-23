using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.CompanyProfile;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationProfile;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using System.Net;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Khách hàng doanh nghiệp tự quản lý hồ sơ đơn vị và tài liệu pháp lý.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/company/profile")]
[Authorize(Policy = "roles:Company,Organization,Khách hàng doanh nghiệp")]
public class CompanyProfileController : ControllerBase
{
    private const long MaxBytes = 10 * 1024 * 1024;

    private readonly ILogger<CompanyProfileController> _logger;
    private readonly IUserOrganizationRepository _userOrganizations;
    private readonly IOrganizationRepository _organizations;
    private readonly IOrganizationLegalDocumentRepository _documents;
    private readonly IMediaFileRepository _mediaFiles;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public CompanyProfileController(
        ILogger<CompanyProfileController> logger,
        IUserOrganizationRepository userOrganizations,
        IOrganizationRepository organizations,
        IOrganizationLegalDocumentRepository documents,
        IMediaFileRepository mediaFiles,
        IStorageService storage,
        IConfiguration configuration)
    {
        _logger = logger;
        _userOrganizations = userOrganizations;
        _organizations = organizations;
        _documents = documents;
        _mediaFiles = mediaFiles;
        _storage = storage;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<OrganizationProfileResponse>>> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var org = await RequireOrganizationAsync(userId, cancellationToken);
            var docs = await _documents.GetByOrganizationIdAsync(org.Id, cancellationToken);
            var lifetime = TimeSpan.FromMinutes(30);
            var docDtos = new List<OrganizationLegalDocumentDto>();
            foreach (var d in docs)
                docDtos.Add(await OrganizationLegalDocumentMapper.ToDtoAsync(d, _storage, lifetime, cancellationToken));

            return Ok(BaseApiResponse<OrganizationProfileResponse>.SuccessResult(
                await MapOrgAsync(org, docDtos, cancellationToken), "Profile loaded"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<OrganizationProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading company profile");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationProfileResponse>.ErrorResult("Failed to load profile", new[] { ex.Message }));
        }
    }

    [HttpPut]
    public async Task<ActionResult<BaseApiResponse<OrganizationProfileResponse>>> UpdateProfile(
        [FromBody] UpdateOrganizationProfileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var org = await RequireOrganizationAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(BaseApiResponse<OrganizationProfileResponse>.ErrorResult("Name is required", new[] { "Tên đơn vị không được để trống." }));

            org.Name = request.Name.Trim();
            org.TaxCode = TrimOrNull(request.TaxCode);
            org.LegalRepresentative = TrimOrNull(request.LegalRepresentative);
            org.ContactPerson = TrimOrNull(request.ContactPerson);
            org.Address = TrimOrNull(request.Address);
            org.Phone = TrimOrNull(request.Phone);
            org.ContactEmail = TrimOrNull(request.ContactEmail);
            org.Website = TrimOrNull(request.Website);
            org.UpdatedAt = VietnamTime.Now;
            org.UpdatedBy = userId;

            await _organizations.UpdateAsync(org, cancellationToken);

            var docs = await _documents.GetByOrganizationIdAsync(org.Id, cancellationToken);
            var lifetime = TimeSpan.FromMinutes(30);
            var docDtos = new List<OrganizationLegalDocumentDto>();
            foreach (var d in docs)
                docDtos.Add(await OrganizationLegalDocumentMapper.ToDtoAsync(d, _storage, lifetime, cancellationToken));

            return Ok(BaseApiResponse<OrganizationProfileResponse>.SuccessResult(
                await MapOrgAsync(org, docDtos, cancellationToken), "Profile updated"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<OrganizationProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company profile");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationProfileResponse>.ErrorResult("Update failed", new[] { ex.Message }));
        }
    }

    [HttpPost("logo")]
    [EnableRateLimiting("media-upload")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<UploadOrganizationLogoResponse>>> UploadLogo(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var org = await RequireOrganizationAsync(userId, cancellationToken);

            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<UploadOrganizationLogoResponse>.ErrorResult("File is required", new[] { "Vui lòng chọn ảnh." }));
            if (file.Length > 3 * 1024 * 1024)
                return BadRequest(BaseApiResponse<UploadOrganizationLogoResponse>.ErrorResult("File too large", new[] { "Tối đa 3MB." }));

            var ct = file.ContentType ?? "application/octet-stream";
            if (!IsImageContentType(ct))
                return BadRequest(BaseApiResponse<UploadOrganizationLogoResponse>.ErrorResult("Invalid file type", new[] { "Chỉ chấp nhận JPG, PNG, WEBP." }));

            var ext = ct.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => Path.GetExtension(file.FileName)
            };

            var now = VietnamTime.Now;
            var objectName = $"organizations/{org.Id:D}/logo/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

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
                MediaType = "image",
                IsPublic = true,
            };
            await _mediaFiles.CreateAsync(media);

            org.LogoUrl = objectName;
            org.UpdatedAt = VietnamTime.Now;
            org.UpdatedBy = userId;
            await _organizations.UpdateAsync(org, cancellationToken);

            var logoUrl = await ResolveLogoUrlAsync(org.LogoUrl, cancellationToken)
                ?? throw new InvalidOperationException("Could not resolve logo URL");

            return Ok(BaseApiResponse<UploadOrganizationLogoResponse>.SuccessResult(
                new UploadOrganizationLogoResponse { LogoUrl = logoUrl }, "Logo uploaded"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<UploadOrganizationLogoResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading organization logo");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<UploadOrganizationLogoResponse>.ErrorResult("Upload failed", new[] { ex.Message }));
        }
    }

    [HttpPost("documents")]
    [EnableRateLimiting("media-upload")]
    [RequestSizeLimit(MaxBytes + 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<OrganizationLegalDocumentDto>>> UploadDocument(
        [FromForm] IFormFile file,
        [FromForm] string title,
        [FromForm] string documentType = "other",
        [FromForm] string? description = null,
        [FromForm] DateTime? issuedDate = null,
        [FromForm] DateTime? expiryDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var org = await RequireOrganizationAsync(userId, cancellationToken);

            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<OrganizationLegalDocumentDto>.ErrorResult("File is required", new[] { "Vui lòng chọn file." }));
            if (file.Length > MaxBytes)
                return BadRequest(BaseApiResponse<OrganizationLegalDocumentDto>.ErrorResult("File too large", new[] { "Tối đa 10MB." }));

            var ct = file.ContentType ?? "application/octet-stream";
            if (!IsAllowedContentType(ct))
                return BadRequest(BaseApiResponse<OrganizationLegalDocumentDto>.ErrorResult("Invalid file type", new[] { "Chỉ chấp nhận PDF hoặc ảnh." }));

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
            var objectName = $"organizations/{org.Id:D}/legal/{safeType}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

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
                IsPublic = false,
            };
            var createdMedia = await _mediaFiles.CreateAsync(media);

            var entity = new OrganizationLegalDocument
            {
                OrganizationId = org.Id,
                Title = displayTitle,
                DocumentType = safeType,
                Description = TrimOrNull(description),
                MediaFileId = createdMedia.Id,
                IssuedDate = issuedDate?.Date,
                ExpiryDate = expiryDate?.Date,
                UploadedByUserId = userId,
                CreatedAt = VietnamTime.Now,
            };
            var created = await _documents.CreateAsync(entity, cancellationToken);
            created.MediaFile = createdMedia;

            var dto = await OrganizationLegalDocumentMapper.ToDtoAsync(
                created, _storage, TimeSpan.FromMinutes(30), cancellationToken);

            return Ok(BaseApiResponse<OrganizationLegalDocumentDto>.SuccessResult(dto, "Document uploaded"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<OrganizationLegalDocumentDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading organization legal document");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationLegalDocumentDto>.ErrorResult("Upload failed", new[] { ex.Message }));
        }
    }

    [HttpDelete("documents/{id:int}")]
    public async Task<ActionResult<BaseApiResponse<object>>> DeleteDocument(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdOrThrow();
            var org = await RequireOrganizationAsync(userId, cancellationToken);
            var doc = await _documents.GetByIdAsync(id, cancellationToken);
            if (doc == null || doc.OrganizationId != org.Id)
                return NotFound(BaseApiResponse<object>.ErrorResult("Not found", new[] { "Không tìm thấy tài liệu." }));

            await _documents.DeleteAsync(id, cancellationToken);
            return Ok(BaseApiResponse<object>.SuccessResult(new { }, "Deleted"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<object>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting organization document {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("Delete failed", new[] { ex.Message }));
        }
    }

    private async Task<Organization> RequireOrganizationAsync(int userId, CancellationToken cancellationToken)
    {
        var memberships = await _userOrganizations.GetActiveByUserIdAsync(userId);
        var membership = memberships.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Tài khoản chưa được gán đơn vị doanh nghiệp.");

        var org = await _organizations.GetByIdAsync(membership.OrganizationId);
        if (org == null)
            throw new KeyNotFoundException("Organization not found");

        return org;
    }

    private async Task<OrganizationProfileResponse> MapOrgAsync(
        Organization org,
        List<OrganizationLegalDocumentDto> documents,
        CancellationToken cancellationToken) =>
        new()
        {
            OrganizationId = org.Id,
            Name = org.Name,
            TaxCode = org.TaxCode,
            LegalRepresentative = org.LegalRepresentative,
            ContactPerson = org.ContactPerson,
            Address = org.Address,
            Phone = org.Phone,
            ContactEmail = org.ContactEmail,
            Website = org.Website,
            UnitType = org.Type,
            IsActive = org.IsActive,
            LogoUrl = await ResolveLogoUrlAsync(org.LogoUrl, cancellationToken),
            Documents = documents,
        };

    private async Task<string?> ResolveLogoUrlAsync(string? raw, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return raw;

        var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
        var bucketId = _configuration["Appwrite:BucketId"] ?? "";
        var projectId = _configuration["Appwrite:ProjectId"] ?? "";
        var fileId = ToFileId(raw);
        return $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }

    private int GetUserIdOrThrow()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsImageContentType(string ct) =>
        ct.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        ct.Equals("image/png", StringComparison.OrdinalIgnoreCase) ||
        ct.Equals("image/webp", StringComparison.OrdinalIgnoreCase);

    private static bool IsAllowedContentType(string ct) =>
        ct.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) || IsImageContentType(ct);

    private static string NormalizeType(string? documentType)
    {
        var t = (documentType ?? "other").Trim().ToLowerInvariant();
        return t switch
        {
            "business_license" or "tax" or "authorization" or "other" => t,
            _ => "other",
        };
    }
}
