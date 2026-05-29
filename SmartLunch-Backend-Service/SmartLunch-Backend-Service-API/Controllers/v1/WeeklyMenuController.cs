using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.WeeklyMenus.CreateWeeklyMenu;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenus;
using System.Net;
using System.Security.Claims;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// WeeklyMenu (master-data). Đọc: Admin, Organization, Customer — Customer dùng filter (customerTypeId, effectiveDate).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
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
    /// Danh sách weekly menu (phân trang). Dùng EffectiveDate trong query để chỉ trả các kỳ có Start–End bao ngày đó (app Customer).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenusResponse>>> GetWeeklyMenus([FromQuery] GetWeeklyMenusRequest request)
    {
        try
        {
            var query = new GetWeeklyMenusQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.CustomerTypeId,
                request.CustomerProfileKey,
                request.EffectiveDate,
                request.NotEndedBefore);
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

    /// <summary>Lấy thông tin header weekly menu theo Id.</summary>
    [HttpGet("{id}")]
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
    /// Tạo thực đơn tuần cố định kèm lịch món (Manager / Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenuDetailResponse>>> CreateWeeklyMenu(
        [FromBody] CreateWeeklyMenuRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CreateWeeklyMenuCommand(request, userId));
            return Ok(BaseApiResponse<GetWeeklyMenuDetailResponse>.SuccessResult(
                response,
                "Weekly menu created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetWeeklyMenuDetailResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetWeeklyMenuDetailResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating weekly menu");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetWeeklyMenuDetailResponse>.ErrorResult(
                    "An error occurred while creating weekly menu",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chi tiết weekly menu: header + toàn bộ <c>MenuSchedule</c> (theo Dish), sắp xếp theo ngày và meal slot (app Customer).
    /// </summary>
    [HttpGet("{id:int}/detail")]
    public async Task<ActionResult<BaseApiResponse<GetWeeklyMenuDetailResponse>>> GetWeeklyMenuDetail(
        int id,
        [FromQuery] DateTime? scheduleFrom = null)
    {
        try
        {
            var response = await _mediator.Send(new GetWeeklyMenuDetailQuery(id, scheduleFrom));

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

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
