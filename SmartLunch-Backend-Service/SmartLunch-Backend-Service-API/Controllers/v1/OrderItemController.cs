using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.OrderItems;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;
using SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItem;
using SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItems;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// OrderItem management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class OrderItemController : ControllerBase
{
    private readonly ILogger<OrderItemController> _logger;
    private readonly IMediator _mediator;

    public OrderItemController(ILogger<OrderItemController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of orderitems with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:orderitems.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrderItemsResponse>>> GetOrderItems([FromQuery] GetOrderItemsRequest request)
    {
        try
        {
            var query = new GetOrderItemsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetOrderItemsResponse>.SuccessResult(response, "OrderItems retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderItemsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orderitems");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderItemsResponse>.ErrorResult("An error occurred while retrieving orderitems", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get orderitem by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:orderitems.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrderItemResponse>>> GetOrderItem(int id)
    {
        try
        {
            var query = new GetOrderItemQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetOrderItemResponse>.SuccessResult(response, "OrderItem retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderItemResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orderitem with ID: {OrderItemId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderItemResponse>.ErrorResult("An error occurred while retrieving orderitem", new[] { ex.Message }));
        }
    }
}
