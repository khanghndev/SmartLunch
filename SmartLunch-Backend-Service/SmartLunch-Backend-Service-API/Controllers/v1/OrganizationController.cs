using MediatR;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Organizations;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;
using SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganization;
using SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganizations;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Organization (customer/school/company) management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin,Manager")]
public class OrganizationController : ControllerBase
{
    private readonly ILogger<OrganizationController> _logger;
    private readonly IMediator _mediator;

    public OrganizationController(ILogger<OrganizationController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of organizations with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:organizations.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationsResponse>>> GetOrganizations([FromQuery] GetOrganizationsRequest request)
    {
        try
        {
            var query = new GetOrganizationsQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetOrganizationsResponse>.SuccessResult(response, "Organizations retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrganizationsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organizations");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationsResponse>.ErrorResult("An error occurred while retrieving organizations", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get organization by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:organizations.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationResponse>>> GetOrganization(int id)
    {
        try
        {
            var query = new GetOrganizationQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetOrganizationResponse>.SuccessResult(response, "Organization retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrganizationResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving organization with ID: {OrganizationId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationResponse>.ErrorResult("An error occurred while retrieving organization", new[] { ex.Message }));
        }
    }
}
