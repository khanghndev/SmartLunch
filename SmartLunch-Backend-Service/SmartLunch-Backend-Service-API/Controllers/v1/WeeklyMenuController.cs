using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenus;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// WeeklyMenu management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
/// <remarks>Manager quản lý thực đơn tuần toàn hệ thống; Organization (B2B) có thể xem theo quyền weekly_menus.read.</remarks>
[Authorize(Policy = "roles:Admin,Manager,Organization,Customer")]
public class WeeklyMenuController : ControllerBase
{
    private readonly ILogger<WeeklyMenuController> _logger;
    private readonly IMediator _mediator;

    public WeeklyMenuController(ILogger<WeeklyMenuController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of weeklymenus with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:weekly_menus.read")]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenusResponse>>> GetWeeklyMenus([FromQuery] GetWeeklyMenusRequest request)
    {
        try
        {
            var query = new GetWeeklyMenusQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.CustomerTypeId,
                request.CustomerProfileKey);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetWeeklyMenusResponse>.SuccessResult(response, "WeeklyMenus retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetWeeklyMenusResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weeklymenus");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetWeeklyMenusResponse>.ErrorResult("An error occurred while retrieving weeklymenus", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get weeklymenu by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:weekly_menus.read")]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenuResponse>>> GetWeeklyMenu(int id)
    {
        try
        {
            var query = new GetWeeklyMenuQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetWeeklyMenuResponse>.SuccessResult(response, "WeeklyMenu retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetWeeklyMenuResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weeklymenu with ID: {WeeklyMenuId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetWeeklyMenuResponse>.ErrorResult("An error occurred while retrieving weeklymenu", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chi tiết weekly menu: thông tin header + toàn bộ <c>MenuSchedule</c> (theo Dish), sắp xếp theo ngày và meal slot.
    /// </summary>
    [HttpGet("{id:int}/detail")]
    [Authorize(Policy = "permission:weekly_menus.read")]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenuDetailResponse>>> GetWeeklyMenuDetail(int id)
    {
        try
        {
            var response = await _mediator.Send(new GetWeeklyMenuDetailQuery(id));

            return Ok(BaseApiResponse<GetWeeklyMenuDetailResponse>.SuccessResult(
                response,
                "Weekly menu detail retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetWeeklyMenuDetailResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weeklymenu detail with ID: {WeeklyMenuId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetWeeklyMenuDetailResponse>.ErrorResult(
                    "An error occurred while retrieving weeklymenu detail",
                    new[] { ex.Message }));
        }
    }
}
