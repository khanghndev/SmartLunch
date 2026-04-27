using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateSalesInvoice;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Hóa đơn / đơn bán do nhân viên bán tạo (POS).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales/invoices")]
[Authorize(Policy = "roles:SalesStaff,Admin")]
public class SalesInvoiceController : ControllerBase
{
    private readonly ILogger<SalesInvoiceController> _logger;
    private readonly IMediator _mediator;

    public SalesInvoiceController(ILogger<SalesInvoiceController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Tạo hóa đơn (đơn ăn) — ghi nhận nhân viên bán, mã hóa đơn, dòng món và tổng tiền.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permission:orders.create")]
    public async Task<ActionResult<BaseApiResponse<GetOrderResponse>>> Create([FromBody] CreateSalesInvoiceRequest request)
    {
        try
        {
            var salesUserId = RequireUserId();
            var command = new CreateSalesInvoiceCommand(request, salesUserId);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetOrderResponse>.SuccessResult(response, "Sales invoice created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetOrderResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales invoice");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrderResponse>.ErrorResult(
                    "An error occurred while creating the sales invoice",
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
