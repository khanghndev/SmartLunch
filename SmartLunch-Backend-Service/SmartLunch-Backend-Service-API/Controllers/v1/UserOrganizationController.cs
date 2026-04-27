using MediatR;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganization;
using SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganizations;
using SmartLunch.Backend.Service.Application.Commands.UserOrganizations.CreateUserOrganization;
using SmartLunch.Backend.Service.Application.Commands.UserOrganizations.UpdateUserOrganization;
using SmartLunch.Backend.Service.Application.Commands.UserOrganizations.DeleteUserOrganization;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UserOrganizationController : ControllerBase
{
    private readonly ILogger<UserOrganizationController> _logger;
    private readonly IMediator _mediator;

    public UserOrganizationController(ILogger<UserOrganizationController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:organizations.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserOrganizationsResponse>>> GetUserOrganizations([FromQuery] GetUserOrganizationsRequest request)
    {
        try
        {
            var query = new GetUserOrganizationsQuery(request.Page, request.PageSize, request.UserId, request.OrganizationId, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserOrganizationsResponse>.SuccessResult(response, "User organizations retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserOrganizationsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user organizations");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserOrganizationsResponse>.ErrorResult("An error occurred while retrieving user organizations", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:organizations.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserOrganizationResponse>>> GetUserOrganization(int id)
    {
        try
        {
            var query = new GetUserOrganizationQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserOrganizationResponse>.SuccessResult(response, "User organization retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user organization {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserOrganizationResponse>.ErrorResult("An error occurred while retrieving user organization", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:organizations.update")]
    public async Task<ActionResult<BaseApiResponse<CreateUserOrganizationResponse>>> Create([FromBody] CreateUserOrganizationRequest request)
    {
        try
        {
            var command = new CreateUserOrganizationCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateUserOrganizationResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateUserOrganizationResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateUserOrganizationResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user organization");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateUserOrganizationResponse>.ErrorResult("An error occurred while creating user organization", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:organizations.update")]
    public async Task<ActionResult<BaseApiResponse<GetUserOrganizationResponse>>> Update(int id, [FromBody] UpdateUserOrganizationRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetUserOrganizationResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateUserOrganizationCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetUserOrganizationResponse>.SuccessResult(response, "User organization updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetUserOrganizationResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user organization");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserOrganizationResponse>.ErrorResult("An error occurred while updating user organization", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:organizations.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var command = new DeleteUserOrganizationCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "User organization deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user organization");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting user organization", new[] { ex.Message }));
        }
    }
}
