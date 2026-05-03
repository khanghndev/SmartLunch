using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.UpdateOrderStatus;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetOrder;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetOrders;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetMealStatistics;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetDetailedMealStatistics;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Order management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin,Sales")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IMediator _mediator;

    public OrderController(ILogger<OrderController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of orders with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrdersResponse>>> GetOrders([FromQuery] GetOrdersRequest request)
    {
        try
        {
            var query = new GetOrdersQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.ScheduledOn,
                request.Status);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetOrdersResponse>.SuccessResult(response, "Orders retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrdersResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrdersResponse>.ErrorResult("An error occurred while retrieving orders", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> GetOrder(int id)
    {
        try
        {
            var query = new GetOrderQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Order retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("An error occurred while retrieving order", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Update meal-order workflow status: pending → confirmed → delivered. Creates or completes a delivery record when status becomes delivered.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "roles:Admin")]
    [Authorize(Policy = "permission:orders.update")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var command = new UpdateOrderStatusCommand(id, request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Order status updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for ID: {OrderId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("An error occurred while updating order status", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Thống kê suất ăn theo ngày / ca / bộ phận (Dành cho Admin và Công ty).
    /// </summary>
    [HttpGet("statistics/meal-count")]
    [Authorize(Policy = "roles:Admin,Company")]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<ActionResult<BaseApiResponse<GetMealStatisticsResponse>>> GetMealStatistics([FromQuery] GetMealStatisticsRequest request)
    {
        try
        {
            var query = new GetMealStatisticsQuery(request);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetMealStatisticsResponse>.SuccessResult(response, "Meal statistics retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMealStatisticsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving meal statistics");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMealStatisticsResponse>.ErrorResult("An error occurred while retrieving meal statistics", new[] { ex.Message }));
        }
    }
    /// <summary>
    /// Thống kê chi tiết suất ăn (theo từng món) cho ngày / ca / bộ phận.
    /// </summary>
    [HttpGet("statistics/details")]
    [Authorize(Policy = "roles:Admin,Company")]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<ActionResult<BaseApiResponse<GetDetailedMealStatisticsResponse>>> GetDetailedMealStatistics([FromQuery] GetMealStatisticsRequest request)
    {
        try
        {
            var query = new GetDetailedMealStatisticsQuery(request);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetDetailedMealStatisticsResponse>.SuccessResult(response, "Detailed meal statistics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detailed meal statistics");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDetailedMealStatisticsResponse>.ErrorResult("An error occurred while retrieving detailed meal statistics", new[] { ex.Message }));
        }
    }
}
