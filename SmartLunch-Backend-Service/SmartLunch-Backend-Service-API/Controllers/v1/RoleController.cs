using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;
using SmartLunch.Backend.Service.Application.Queries.Roles.GetRole;
using SmartLunch.Backend.Service.Application.Queries.Roles.GetRoles;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.GrantRoleToUser;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.RevokeRoleFromUser;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Role management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IMediator _mediator;

    public RoleController(ILogger<RoleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of roles
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:roles.read")]
    public async Task<ActionResult<BaseApiResponse<GetRolesResponse>>> GetRoles([FromQuery] GetRolesRequest request)
    {
        try
        {
            var query = new GetRolesQuery(request.IsActive);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetRolesResponse>.SuccessResult(response, "Roles retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetRolesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetRolesResponse>.ErrorResult("An error occurred while retrieving roles", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:roles.read")]
    public async Task<ActionResult<BaseApiResponse<GetRoleResponse>>> GetRole(int id)
    {
        try
        {
            var query = new GetRoleQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetRoleResponse>.SuccessResult(response, "Role retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with ID: {RoleId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetRoleResponse>.ErrorResult("An error occurred while retrieving role", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Grant role to user
    /// </summary>
    [HttpPost("grant")]
    [Authorize(Policy = "permission:roles.grant")]
    public async Task<ActionResult<BaseApiResponse<GrantRoleToUserResponse>>> GrantRoleToUser([FromBody] GrantRoleToUserRequest request)
    {
        try
        {
            var command = new GrantRoleToUserCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GrantRoleToUserResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GrantRoleToUserResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GrantRoleToUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GrantRoleToUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting role to user");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GrantRoleToUserResponse>.ErrorResult("An error occurred while granting role", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Revoke role from user
    /// </summary>
    [HttpPost("revoke")]
    [Authorize(Policy = "permission:roles.revoke")]
    public async Task<ActionResult<BaseApiResponse<RevokeRoleFromUserResponse>>> RevokeRoleFromUser([FromBody] RevokeRoleFromUserRequest request)
    {
        try
        {
            var command = new RevokeRoleFromUserCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<RevokeRoleFromUserResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<RevokeRoleFromUserResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<RevokeRoleFromUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<RevokeRoleFromUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking role from user");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<RevokeRoleFromUserResponse>.ErrorResult("An error occurred while revoking role", new[] { ex.Message }));
        }
    }
}
