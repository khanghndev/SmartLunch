using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnit;
using SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnits;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.CreateUserUnit;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.UpdateUserUnit;
using SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.DeleteUserUnit;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UserUnitController : ControllerBase
{
    private readonly ILogger<UserUnitController> _logger;
    private readonly IMediator _mediator;

    public UserUnitController(ILogger<UserUnitController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:units.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserUnitsResponse>>> GetUserUnits([FromQuery] GetUserUnitsRequest request)
    {
        try
        {
            var query = new GetUserUnitsQuery(request.Page, request.PageSize, request.UserId, request.UnitId, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserUnitsResponse>.SuccessResult(response, "User units retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserUnitsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user units");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserUnitsResponse>.ErrorResult("An error occurred while retrieving user units", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:units.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserUnitResponse>>> GetUserUnit(Guid id)
    {
        try
        {
            var query = new GetUserUnitQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserUnitResponse>.SuccessResult(response, "User unit retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user unit {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserUnitResponse>.ErrorResult("An error occurred while retrieving user unit", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:units.update")]
    public async Task<ActionResult<BaseApiResponse<CreateUserUnitResponse>>> Create([FromBody] CreateUserUnitRequest request)
    {
        try
        {
            var command = new CreateUserUnitCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateUserUnitResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateUserUnitResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateUserUnitResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user unit");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateUserUnitResponse>.ErrorResult("An error occurred while creating user unit", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:units.update")]
    public async Task<ActionResult<BaseApiResponse<GetUserUnitResponse>>> Update(Guid id, [FromBody] UpdateUserUnitRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetUserUnitResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateUserUnitCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetUserUnitResponse>.SuccessResult(response, "User unit updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetUserUnitResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user unit");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserUnitResponse>.ErrorResult("An error occurred while updating user unit", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:units.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteUserUnitCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "User unit deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user unit");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting user unit", new[] { ex.Message }));
        }
    }
}
