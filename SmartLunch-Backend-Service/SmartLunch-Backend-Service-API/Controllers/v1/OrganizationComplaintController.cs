using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.CreateOrganizationComplaint;
using SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.DeleteOrganizationComplaintEvidence;
using SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.SubmitOrganizationComplaint;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrderComplaintEligibility;
using SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaint;
using SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Khiếu nại đơn hàng — khách hàng doanh nghiệp.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organization/complaints")]
[Authorize(Policy = "roles:Organization,Khách hàng doanh nghiệp,Company")]
public class OrganizationComplaintController : ControllerBase
{
    private readonly ILogger<OrganizationComplaintController> _logger;
    private readonly IMediator _mediator;
    private readonly IComplaintRepository _complaints;
    private readonly IStorageService _storage;

    public OrganizationComplaintController(
        ILogger<OrganizationComplaintController> logger,
        IMediator mediator,
        IComplaintRepository complaints,
        IStorageService storage)
    {
        _logger = logger;
        _mediator = mediator;
        _complaints = complaints;
        _storage = storage;
    }

    [HttpGet("orders/{orderId:int}/eligibility")]
    [Authorize(Policy = "permission:complaints.read")]
    public async Task<ActionResult<BaseApiResponse<ComplaintEligibilityDto>>> GetEligibility(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new GetOrderComplaintEligibilityQuery(userId, orderId), cancellationToken);
            return Ok(BaseApiResponse<ComplaintEligibilityDto>.SuccessResult(dto, "Eligibility retrieved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ComplaintEligibilityDto>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResult<ComplaintEligibilityDto>(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking complaint eligibility for order {OrderId}", orderId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ComplaintEligibilityDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpGet]
    [Authorize(Policy = "permission:complaints.list")]
    public async Task<ActionResult<BaseApiResponse<PaginationResponse<OrganizationComplaintSummaryDto>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetOrganizationComplaintsQuery(userId, page, pageSize), cancellationToken);
            return Ok(BaseApiResponse<PaginationResponse<OrganizationComplaintSummaryDto>>.SuccessResult(response, "Complaints retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing organization complaints");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<PaginationResponse<OrganizationComplaintSummaryDto>>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "permission:complaints.read")]
    public async Task<ActionResult<BaseApiResponse<OrganizationComplaintDetailDto>>> Get(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new GetOrganizationComplaintQuery(userId, id), cancellationToken);
            return Ok(BaseApiResponse<OrganizationComplaintDetailDto>.SuccessResult(dto, "Complaint retrieved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<OrganizationComplaintDetailDto>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResult<OrganizationComplaintDetailDto>(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading complaint {ComplaintId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:complaints.create")]
    public async Task<ActionResult<BaseApiResponse<OrganizationComplaintDetailDto>>> Create(
        [FromBody] CreateOrganizationComplaintRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new CreateOrganizationComplaintCommand(userId, request), cancellationToken);
            return Ok(BaseApiResponse<OrganizationComplaintDetailDto>.SuccessResult(dto, "Complaint draft created"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResult<OrganizationComplaintDetailDto>(ex);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating complaint");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost("{id:int}/evidence")]
    [Authorize(Policy = "permission:complaints.create")]
    [RequestSizeLimit(26 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<OrganizationComplaintDetailDto>>> UploadEvidence(
        int id,
        [FromForm] IFormFile file,
        [FromForm] UploadComplaintEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("File is required", new[] { "Missing file." }));

            var kind = (request.Kind ?? string.Empty).Trim().ToLowerInvariant();
            if (!ComplaintEvidenceKind.All.Contains(kind))
                return BadRequest(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Invalid evidence kind", new[] { kind }));

            var complaint = await _complaints.GetByIdWithDetailsAsync(id, cancellationToken);
            if (complaint == null)
                return NotFound(BaseApiResponse<OrganizationComplaintDetailDto>.NotFoundResult("Khiếu nại không tồn tại."));
            if (complaint.UserId != userId)
                return StatusCode((int)HttpStatusCode.Forbidden,
                    BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Forbidden", new[] { "Forbidden" }));
            if (!OrganizationComplaintRules.IsEditable(complaint))
                return Conflict(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Complaint is not editable", new[] { "Not draft." }));

            var (mediaType, ext) = ComplaintMediaHelper.Resolve(file.ContentType ?? string.Empty, file.Length);
            var now = VietnamTime.Now;
            var objectName = $"complaints/{id:D}/evidence/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";

            await using (var stream = file.OpenReadStream())
                await _storage.UploadObjectAsync(objectName, stream, file.ContentType ?? "application/octet-stream", cancellationToken);

            var sortOrder = (complaint.Evidence?.Count ?? 0) + 1;
            var evidence = new ComplaintEvidence
            {
                ComplaintId = id,
                Kind = kind,
                MediaType = mediaType,
                StorageObjectName = objectName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                SortOrder = sortOrder,
                CreatedAt = now,
            };

            await _complaints.AddEvidenceAsync(evidence, cancellationToken);

            var reloaded = await _complaints.GetByIdWithDetailsAsync(id, cancellationToken) ?? complaint;
            var dto = await ComplaintMapper.ToDetailAsync(reloaded, _storage, cancellationToken);
            return Ok(BaseApiResponse<OrganizationComplaintDetailDto>.SuccessResult(dto, "Evidence uploaded"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading complaint evidence {ComplaintId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id:int}/evidence/{evidenceId:int}")]
    [Authorize(Policy = "permission:complaints.create")]
    public async Task<ActionResult<BaseApiResponse<OrganizationComplaintDetailDto>>> DeleteEvidence(
        int id,
        int evidenceId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new DeleteOrganizationComplaintEvidenceCommand(userId, id, evidenceId), cancellationToken);
            return Ok(BaseApiResponse<OrganizationComplaintDetailDto>.SuccessResult(dto, "Evidence deleted"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<OrganizationComplaintDetailDto>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResult<OrganizationComplaintDetailDto>(ex);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting evidence {EvidenceId}", evidenceId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost("{id:int}/submit")]
    [Authorize(Policy = "permission:complaints.create")]
    public async Task<ActionResult<BaseApiResponse<OrganizationComplaintDetailDto>>> Submit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new SubmitOrganizationComplaintCommand(userId, id), cancellationToken);
            return Ok(BaseApiResponse<OrganizationComplaintDetailDto>.SuccessResult(dto, "Complaint submitted for review"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<OrganizationComplaintDetailDto>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResult<OrganizationComplaintDetailDto>(ex);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting complaint {ComplaintId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }

    /// <summary>401 chỉ khi JWT/context lỗi; quyền nghiệp vụ (org/đơn) trả 403 để FE không nhầm refresh token.</summary>
    private ActionResult<BaseApiResponse<T>> ForbiddenResult<T>(UnauthorizedAccessException ex)
    {
        if (string.Equals(ex.Message, "Invalid user context.", StringComparison.Ordinal))
            return Unauthorized(BaseApiResponse<T>.ErrorResult(ex.Message, new[] { ex.Message }));

        return StatusCode(
            (int)HttpStatusCode.Forbidden,
            BaseApiResponse<T>.ErrorResult(ex.Message, new[] { ex.Message }));
    }
}
