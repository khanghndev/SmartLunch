using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermission;
using SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermissions;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Permission management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly ILogger<PermissionController> _logger;
    private readonly IMediator _mediator;

    public PermissionController(ILogger<PermissionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of permissions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<GetPermissionsResponse>>> GetPermissions([FromQuery] GetPermissionsRequest request)
    {
        try
        {
            var query = new GetPermissionsQuery(request.IsActive, request.Resource, request.Action);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetPermissionsResponse>.SuccessResult(response, "Permissions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPermissionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPermissionsResponse>.ErrorResult("An error occurred while retrieving permissions", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BaseApiResponse<GetPermissionResponse>>> GetPermission(Guid id)
    {
        try
        {
            var query = new GetPermissionQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetPermissionResponse>.SuccessResult(response, "Permission retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPermissionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission with ID: {PermissionId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPermissionResponse>.ErrorResult("An error occurred while retrieving permission", new[] { ex.Message }));
        }
    }
}
