using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;
using SmartLunch.Backend.Service.Application.Queries.CustomerFeatures.WeeklyMenu;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>
/// Menu features specifically for individual customers
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer-menu")]
public class CustomerMenuController : ControllerBase
{
    private readonly ILogger<CustomerMenuController> _logger;
    private readonly IMediator _mediator;

    public CustomerMenuController(ILogger<CustomerMenuController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get the weekly menu (including daily schedules and dishes) for a given date
    /// </summary>
    /// <param name="date">Optional date. Defaults to current UTC date if not provided.</param>
    /// <returns>Weekly Menu with detailed schedules and dishes</returns>
    [HttpGet("weekly")]
    public async Task<ActionResult<BaseApiResponse<GetCustomerWeeklyMenuResponse>>> GetWeeklyMenu(
        [FromQuery] DateTime? date,
        [FromQuery] int? customerTypeId)
    {
        try
        {
            var query = new GetCustomerWeeklyMenuQuery(date, customerTypeId);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetCustomerWeeklyMenuResponse>.SuccessResult(response, "Weekly menu retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer weekly menu for date: {Date}", date);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetCustomerWeeklyMenuResponse>.ErrorResult("An error occurred while retrieving the weekly menu", new[] { ex.Message }));
        }
    }
}
