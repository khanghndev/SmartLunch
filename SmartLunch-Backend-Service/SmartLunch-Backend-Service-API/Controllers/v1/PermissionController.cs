using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermission;
using SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermissions;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToUser;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromUser;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToRole;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromRole;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Permission management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin,SuperAdmin")]
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
    [Authorize(Policy = "permission:permissions.read")]
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
    [Authorize(Policy = "permission:permissions.read")]
    public async Task<ActionResult<BaseApiResponse<GetPermissionResponse>>> GetPermission(int id)
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

    /// <summary>
    /// Grant permission to user
    /// </summary>
    [HttpPost("grant-to-user")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<GrantPermissionToUserResponse>>> GrantPermissionToUser([FromBody] GrantPermissionToUserRequest request)
    {
        try
        {
            var command = new GrantPermissionToUserCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GrantPermissionToUserResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GrantPermissionToUserResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GrantPermissionToUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GrantPermissionToUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting permission to user");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GrantPermissionToUserResponse>.ErrorResult("An error occurred while granting permission", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Revoke permission from user
    /// </summary>
    [HttpPost("revoke-from-user")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<RevokePermissionFromUserResponse>>> RevokePermissionFromUser([FromBody] RevokePermissionFromUserRequest request)
    {
        try
        {
            var command = new RevokePermissionFromUserCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<RevokePermissionFromUserResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<RevokePermissionFromUserResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<RevokePermissionFromUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<RevokePermissionFromUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission from user");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<RevokePermissionFromUserResponse>.ErrorResult("An error occurred while revoking permission", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Grant permission to role
    /// </summary>
    [HttpPost("grant-to-role")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<GrantPermissionToRoleResponse>>> GrantPermissionToRole([FromBody] GrantPermissionToRoleRequest request)
    {
        try
        {
            var command = new GrantPermissionToRoleCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GrantPermissionToRoleResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GrantPermissionToRoleResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GrantPermissionToRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GrantPermissionToRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting permission to role");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GrantPermissionToRoleResponse>.ErrorResult("An error occurred while granting permission to role", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Revoke permission from role
    /// </summary>
    [HttpPost("revoke-from-role")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<RevokePermissionFromRoleResponse>>> RevokePermissionFromRole([FromBody] RevokePermissionFromRoleRequest request)
    {
        try
        {
            var command = new RevokePermissionFromRoleCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<RevokePermissionFromRoleResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<RevokePermissionFromRoleResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<RevokePermissionFromRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<RevokePermissionFromRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission from role");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<RevokePermissionFromRoleResponse>.ErrorResult("An error occurred while revoking permission from role", new[] { ex.Message }));
        }
    }
}
