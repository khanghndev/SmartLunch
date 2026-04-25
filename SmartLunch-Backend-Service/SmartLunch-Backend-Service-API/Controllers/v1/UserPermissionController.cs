using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermission;
using SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermissions;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.CreateUserPermission;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.UpdateUserPermission;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.DeleteUserPermission;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin,SuperAdmin")]
public class UserPermissionController : ControllerBase
{
    private readonly ILogger<UserPermissionController> _logger;
    private readonly IMediator _mediator;

    public UserPermissionController(ILogger<UserPermissionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:permissions.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserPermissionsResponse>>> GetUserPermissions([FromQuery] GetUserPermissionsRequest request)
    {
        try
        {
            var query = new GetUserPermissionsQuery(request.Page, request.PageSize, request.UserId, request.PermissionId, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserPermissionsResponse>.SuccessResult(response, "User permissions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserPermissionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user permissions");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserPermissionsResponse>.ErrorResult("An error occurred while retrieving user permissions", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:permissions.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserPermissionResponse>>> GetUserPermission(int id)
    {
        try
        {
            var query = new GetUserPermissionQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserPermissionResponse>.SuccessResult(response, "User permission retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user permission {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserPermissionResponse>.ErrorResult("An error occurred while retrieving user permission", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<CreateUserPermissionResponse>>> Create([FromBody] CreateUserPermissionRequest request)
    {
        try
        {
            var command = new CreateUserPermissionCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateUserPermissionResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateUserPermissionResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateUserPermissionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateUserPermissionResponse>.ErrorResult("An error occurred while creating user permission", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<GetUserPermissionResponse>>> Update(int id, [FromBody] UpdateUserPermissionRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetUserPermissionResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateUserPermissionCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetUserPermissionResponse>.SuccessResult(response, "User permission updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetUserPermissionResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserPermissionResponse>.ErrorResult("An error occurred while updating user permission", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var command = new DeleteUserPermissionCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "User permission deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting user permission", new[] { ex.Message }));
        }
    }
}
