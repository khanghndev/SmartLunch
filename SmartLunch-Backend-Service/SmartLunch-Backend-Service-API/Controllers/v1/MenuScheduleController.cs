using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MenuSchedules;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;
using SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedule;
using SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedules;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// MenuSchedule management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class MenuScheduleController : ControllerBase
{
    private readonly ILogger<MenuScheduleController> _logger;
    private readonly IMediator _mediator;

    public MenuScheduleController(ILogger<MenuScheduleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of menuschedules with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:menuschedules.read")]
    public async Task<ActionResult<BaseApiResponse<GetMenuSchedulesResponse>>> GetMenuSchedules([FromQuery] GetMenuSchedulesRequest request)
    {
        try
        {
            var query = new GetMenuSchedulesQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetMenuSchedulesResponse>.SuccessResult(response, "MenuSchedules retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMenuSchedulesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menuschedules");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMenuSchedulesResponse>.ErrorResult("An error occurred while retrieving menuschedules", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get menuschedule by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:menuschedules.read")]
    public async Task<ActionResult<BaseApiResponse<GetMenuScheduleResponse>>> GetMenuSchedule(int id)
    {
        try
        {
            var query = new GetMenuScheduleQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetMenuScheduleResponse>.SuccessResult(response, "MenuSchedule retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMenuScheduleResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menuschedule with ID: {MenuScheduleId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMenuScheduleResponse>.ErrorResult("An error occurred while retrieving menuschedule", new[] { ex.Message }));
        }
    }
}
