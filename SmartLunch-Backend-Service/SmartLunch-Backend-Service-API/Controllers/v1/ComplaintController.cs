using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaint;
using SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaints;
using System.Net;

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
    public async Task<ActionResult<BaseApiResponse<GetComplaintsResponse>>> GetComplaints([FromQuery] GetComplaintsRequest request)
    {
        try
        {
            var query = new GetComplaintsQuery(request.Page, request.PageSize, request.SearchTerm);
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
}
