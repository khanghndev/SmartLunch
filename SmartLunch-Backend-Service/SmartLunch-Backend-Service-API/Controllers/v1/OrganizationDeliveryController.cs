using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationDeliveries;
using SmartLunch.Backend.Service.Application.Queries.OrganizationDeliveries.GetOrderDeliveryOtp;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Mã OTP xác nhận nhận hàng — Organization.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organization/deliveries")]
[Authorize(Policy = "roles:Organization,Khách hàng doanh nghiệp,Company")]
public class OrganizationDeliveryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrganizationDeliveryController> _logger;

    public OrganizationDeliveryController(IMediator mediator, ILogger<OrganizationDeliveryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Lấy mã OTP giao hàng (sau khi shipper chuyển sang đang giao).</summary>
    [HttpGet("orders/{orderId:int}/otp")]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<OrganizationDeliveryOtpResponse>>> GetOtp(int orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var dto = await _mediator.Send(new GetOrderDeliveryOtpQuery(userId, orderId), cancellationToken);
            return Ok(BaseApiResponse<OrganizationDeliveryOtpResponse>.SuccessResult(dto, "Delivery OTP retrieved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<OrganizationDeliveryOtpResponse>.NotFoundResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<OrganizationDeliveryOtpResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<OrganizationDeliveryOtpResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delivery OTP for order {OrderId}", orderId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationDeliveryOtpResponse>.ErrorResult("Failed", new[] { ex.Message }));
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
