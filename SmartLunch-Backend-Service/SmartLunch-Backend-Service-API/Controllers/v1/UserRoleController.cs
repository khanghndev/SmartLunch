using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRole;
using SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRoles;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.CreateUserRole;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.UpdateUserRole;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.DeleteUserRole;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UserRoleController : ControllerBase
{
    private readonly ILogger<UserRoleController> _logger;
    private readonly IMediator _mediator;

    public UserRoleController(ILogger<UserRoleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:roles.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserRolesResponse>>> GetUserRoles([FromQuery] GetUserRolesRequest request)
    {
        try
        {
            var query = new GetUserRolesQuery(request.Page, request.PageSize, request.UserId, request.RoleId, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserRolesResponse>.SuccessResult(response, "User roles retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserRolesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user roles");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserRolesResponse>.ErrorResult("An error occurred while retrieving user roles", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:roles.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserRoleResponse>>> GetUserRole(int id)
    {
        try
        {
            var query = new GetUserRoleQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserRoleResponse>.SuccessResult(response, "User role retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user role {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserRoleResponse>.ErrorResult("An error occurred while retrieving user role", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:roles.update")]
    public async Task<ActionResult<BaseApiResponse<CreateUserRoleResponse>>> Create([FromBody] CreateUserRoleRequest request)
    {
        try
        {
            var command = new CreateUserRoleCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateUserRoleResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateUserRoleResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateUserRoleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user role");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateUserRoleResponse>.ErrorResult("An error occurred while creating user role", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:roles.update")]
    public async Task<ActionResult<BaseApiResponse<GetUserRoleResponse>>> Update(int id, [FromBody] UpdateUserRoleRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetUserRoleResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateUserRoleCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetUserRoleResponse>.SuccessResult(response, "User role updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetUserRoleResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserRoleResponse>.ErrorResult("An error occurred while updating user role", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:roles.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var command = new DeleteUserRoleCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "User role deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user role");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting user role", new[] { ex.Message }));
        }
    }
}
