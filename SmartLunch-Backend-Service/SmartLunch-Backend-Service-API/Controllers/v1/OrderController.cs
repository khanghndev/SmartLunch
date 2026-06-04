using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CancelCustomerOrder;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateCustomerMealOrder;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.SignOrderAnnex;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.UpdateOrderStatus;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetOrder;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetOrderAnnexPreview;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetOrders;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetMealStatistics;
using SmartLunch.Backend.Service.Application.Queries.Orders.GetDetailedMealStatistics;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Order management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize]
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
            int? restrictUser = null;
            if (User.IsInRole("Organization") && !User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Super Admin"))
                restrictUser = RequireUserId();

            var query = new GetOrdersQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.ScheduledOn,
                request.Status,
                request.PaymentStatus,
                restrictUser);
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

            if (response.Order.Id == 0)
                return NotFound(BaseApiResponse<GetOrderResponse>.ErrorResult("Order not found", new[] { "Order not found" }));

            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Super Admin"))
            {
                var uid = RequireUserId();
                if (response.Order.UserId != uid)
                    return Forbid();
            }

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
    [Authorize(Policy = "roles:Admin,Manager,Super Admin")]
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

    /// <summary>Xem trước PDF phụ lục đặt hàng (cùng bố cục bản ký chính thức).</summary>
    [HttpGet("{id:int}/annex-preview")]
    [Authorize(Policy = "roles:Organization,Company,Khách hàng cá nhân,Khách hàng doanh nghiệp")]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<IActionResult> GetOrderAnnexPreview(int id)
    {
        try
        {
            var userId = RequireUserId();
            var pdf = await _mediator.Send(new GetOrderAnnexPreviewQuery(id, userId));
            return File(pdf, "application/pdf", $"phu-luc-don-{id}-preview.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating annex preview for order {OrderId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>Ký phụ lục đặt hàng (mô phỏng): ghép chữ ký vào PDF, upload cloud, cập nhật AnnexPdfUrl.</summary>
    [HttpPost("{id:int}/sign-annex")]
    [Authorize(Policy = "roles:Organization,Company,Khách hàng cá nhân,Khách hàng doanh nghiệp")]
    [Authorize(Policy = "permission:orders.create")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> SignOrderAnnex(int id, [FromBody] SignOrderAnnexRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new SignOrderAnnexCommand(id, request, userId));
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Order annex signed and PDF stored."));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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
            _logger.LogError(ex, "Error signing order annex for {OrderId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("An error occurred while signing the order annex", new[] { ex.Message }));
        }
    }

    /// <summary>Khách hủy đơn của mình khi còn chờ xác nhận và chưa thanh toán cọc/đủ.</summary>
    [HttpPost("{id:int}/cancel")]
    [Authorize(Policy = "roles:Organization,Company,Customer,Khách hàng cá nhân,Khách hàng doanh nghiệp")]
    [Authorize(Policy = "permission:orders.create")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> CancelCustomerOrder(int id)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CancelCustomerOrderCommand(id, userId));
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Đã hủy đơn hàng thành công."));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("Không thể hủy đơn hàng", new[] { ex.Message }));
        }
    }

    /// <summary>Đặt món trực tuyến (B2C + B2B đại diện đơn vị): tạo đơn chờ xác nhận.</summary>
    [HttpPost("customer")]
    [Authorize(Policy = "roles:Organization,Company,Khách hàng cá nhân,Khách hàng doanh nghiệp")]
    [Authorize(Policy = "permission:orders.create")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> CreateCustomerOrder([FromBody] CreateCustomerMealOrderRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CreateCustomerMealOrderCommand(request, userId));
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Order created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer order");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult("An error occurred while creating the order", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }

    /// <summary>
    /// Thống kê suất ăn theo ngày / ca / bộ phận (Dành cho Admin và Công ty).
    /// </summary>
    [HttpGet("statistics/meal-count")]
    [Authorize(Policy = "roles:Admin,Company,Manager,Organization")]
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
    [Authorize(Policy = "roles:Admin,Company,Manager,Organization")]
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
