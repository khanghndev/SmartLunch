using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermission;
using SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermissions;
using SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.CreateRolePermission;
using SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.UpdateRolePermission;
using SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.DeleteRolePermission;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class RolePermissionController : ControllerBase
{
    private readonly ILogger<RolePermissionController> _logger;
    private readonly IMediator _mediator;

    public RolePermissionController(ILogger<RolePermissionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:permissions.read")]
    public async Task<ActionResult<BaseApiResponse<GetRolePermissionsResponse>>> GetRolePermissions([FromQuery] GetRolePermissionsRequest request)
    {
        try
        {
            var query = new GetRolePermissionsQuery(request.Page, request.PageSize, request.RoleId, request.PermissionId, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetRolePermissionsResponse>.SuccessResult(response, "Role permissions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetRolePermissionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role permissions");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetRolePermissionsResponse>.ErrorResult("An error occurred while retrieving role permissions", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:permissions.read")]
    public async Task<ActionResult<BaseApiResponse<GetRolePermissionResponse>>> GetRolePermission(Guid id)
    {
        try
        {
            var query = new GetRolePermissionQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetRolePermissionResponse>.SuccessResult(response, "Role permission retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role permission {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetRolePermissionResponse>.ErrorResult("An error occurred while retrieving role permission", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<CreateRolePermissionResponse>>> Create([FromBody] CreateRolePermissionRequest request)
    {
        try
        {
            var command = new CreateRolePermissionCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateRolePermissionResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateRolePermissionResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateRolePermissionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateRolePermissionResponse>.ErrorResult("An error occurred while creating role permission", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<GetRolePermissionResponse>>> Update(Guid id, [FromBody] UpdateRolePermissionRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetRolePermissionResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateRolePermissionCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetRolePermissionResponse>.SuccessResult(response, "Role permission updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetRolePermissionResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetRolePermissionResponse>.ErrorResult("An error occurred while updating role permission", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:permissions.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteRolePermissionCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "Role permission deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role permission");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting role permission", new[] { ex.Message }));
        }
    }
}
