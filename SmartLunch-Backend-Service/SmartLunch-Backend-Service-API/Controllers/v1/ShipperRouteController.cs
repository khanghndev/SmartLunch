using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Routes;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Routes;
using SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Routes.OptimizeRoute;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>
/// Route utilities for Shipper role (optimize stop order).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shipper/routes")]
[Authorize(Policy = "roles:Shipper")]
public class ShipperRouteController : ControllerBase
{
    private readonly ILogger<ShipperRouteController> _logger;
    private readonly IMediator _mediator;

    public ShipperRouteController(ILogger<ShipperRouteController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Tự động sắp xếp tuyến giao hàng tối ưu (approx) từ tọa độ (lat/lng) do app cung cấp.
    /// </summary>
    [HttpPost("optimize")]
    [Authorize(Policy = "permission:deliveries.list")]
    public async Task<ActionResult<BaseApiResponse<OptimizeRouteResponse>>> Optimize([FromBody] OptimizeRouteRequest request)
    {
        try
        {
            var shipperId = RequireUserId();
            var response = await _mediator.Send(new OptimizeRouteQuery(shipperId, request));
            return Ok(BaseApiResponse<OptimizeRouteResponse>.SuccessResult(response, "Route optimized successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<OptimizeRouteResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing shipper route");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OptimizeRouteResponse>.ErrorResult("An error occurred while optimizing route", new[] { ex.Message }));
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

