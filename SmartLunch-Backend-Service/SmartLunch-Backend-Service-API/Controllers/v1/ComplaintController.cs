using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.ManagerComplaints.ResolveManagerComplaint;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaint;
using SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaints;
using SmartLunch.Backend.Service.Application.Queries.ManagerComplaints.GetManagerComplaintDetail;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Complaint management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin, Manager")]
public class ComplaintController : ControllerBase
{
    private readonly ILogger<ComplaintController> _logger;
    private readonly IMediator _mediator;

    public ComplaintController(ILogger<ComplaintController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of complaints with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:complaints.read")]
    public async Task<ActionResult<BaseApiResponse<GetComplaintsResponse>>> GetComplaints(
        [FromQuery] GetComplaintsRequest request,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new GetComplaintsQuery(request.Page, request.PageSize, request.SearchTerm, status);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetComplaintsResponse>.SuccessResult(response, "Complaints retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetComplaintsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaints");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetComplaintsResponse>.ErrorResult("An error occurred while retrieving complaints", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get complaint by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:complaints.read")]
    public async Task<ActionResult<BaseApiResponse<GetComplaintResponse>>> GetComplaint(int id)
    {
        try
        {
            var query = new GetComplaintQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetComplaintResponse>.SuccessResult(response, "Complaint retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetComplaintResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaint with ID: {ComplaintId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetComplaintResponse>.ErrorResult("An error occurred while retrieving complaint", new[] { ex.Message }));
        }
    }

    /// <summary>Chi tiết khiếu nại kèm bằng chứng shipper + đơn hàng (quản lý xử lý tranh chấp).</summary>
    [HttpGet("{id:int}/review")]
    [Authorize(Policy = "permission:complaints.read")]
    public async Task<ActionResult<BaseApiResponse<ManagerComplaintDetailDto>>> GetForReview(int id, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _mediator.Send(new GetManagerComplaintDetailQuery(id), cancellationToken);
            return Ok(BaseApiResponse<ManagerComplaintDetailDto>.SuccessResult(dto, "Complaint review context retrieved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ManagerComplaintDetailDto>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading complaint review {ComplaintId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ManagerComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    /// <summary>Quyết định: hoàn tiền (theo suất, có thể chỉnh tay) hoặc từ chối.</summary>
    [HttpPatch("{id:int}/resolve")]
    [Authorize(Policy = "permission:complaints.update")]
    public async Task<ActionResult<BaseApiResponse<ManagerComplaintDetailDto>>> Resolve(
        int id,
        [FromBody] ResolveManagerComplaintRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var managerId = RequireUserId();
            var dto = await _mediator.Send(new ResolveManagerComplaintCommand(managerId, id, request), cancellationToken);
            return Ok(BaseApiResponse<ManagerComplaintDetailDto>.SuccessResult(dto, "Complaint resolved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ManagerComplaintDetailDto>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ManagerComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<ManagerComplaintDetailDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving complaint {ComplaintId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ManagerComplaintDetailDto>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
