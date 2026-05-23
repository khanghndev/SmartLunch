using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Manager.Deliveries.AssignManagerDelivery;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerDeliveries;
using SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerShippers;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Điều phối giao hàng cho Quản lý (Manager).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/manager/deliveries")]
[Authorize(Policy = "roles:Admin,Manager")]
public class ManagerDeliveryController : ControllerBase
{
    private readonly ILogger<ManagerDeliveryController> _logger;
    private readonly IMediator _mediator;

    public ManagerDeliveryController(ILogger<ManagerDeliveryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<GetManagerDeliveriesResponse>>> GetDeliveries(
        [FromQuery] GetManagerDeliveriesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new GetManagerDeliveriesQuery(
                request.Page,
                request.PageSize,
                request.Status,
                request.ScheduledOn,
                request.SearchTerm,
                request.UnassignedOnly), cancellationToken);

            return Ok(BaseApiResponse<GetManagerDeliveriesResponse>.SuccessResult(response, "Deliveries loaded"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manager deliveries");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetManagerDeliveriesResponse>.ErrorResult("Failed to load deliveries", new[] { ex.Message }));
        }
    }

    [HttpGet("shippers")]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<GetManagerShippersResponse>>> GetShippers(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new GetManagerShippersQuery(), cancellationToken);
            return Ok(BaseApiResponse<GetManagerShippersResponse>.SuccessResult(response, "Shippers loaded"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading shippers");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetManagerShippersResponse>.ErrorResult("Failed to load shippers", new[] { ex.Message }));
        }
    }

    [HttpPut("{id:int}/assign")]
    [Authorize(Policy = "permission:deliveries.update")]
    public async Task<ActionResult<BaseApiResponse<ManagerDeliveryListItemDto>>> Assign(
        int id,
        [FromBody] AssignManagerDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new AssignManagerDeliveryCommand(id, request), cancellationToken);
            return Ok(BaseApiResponse<ManagerDeliveryListItemDto>.SuccessResult(response, "Delivery assigned"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ManagerDeliveryListItemDto>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ManagerDeliveryListItemDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<ManagerDeliveryListItemDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning delivery {DeliveryId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ManagerDeliveryListItemDto>.ErrorResult("Assign failed", new[] { ex.Message }));
        }
    }
}
