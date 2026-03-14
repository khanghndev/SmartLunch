using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;
using SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDelivery;
using SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDeliveries;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Delivery management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class DeliveryController : ControllerBase
{
    private readonly ILogger<DeliveryController> _logger;
    private readonly IMediator _mediator;

    public DeliveryController(ILogger<DeliveryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of deliveries with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<GetDeliveriesResponse>>> GetDeliveries([FromQuery] GetDeliveriesRequest request)
    {
        try
        {
            var query = new GetDeliveriesQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetDeliveriesResponse>.SuccessResult(response, "Deliveries retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDeliveriesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deliveries");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDeliveriesResponse>.ErrorResult("An error occurred while retrieving deliveries", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get delivery by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<GetDeliveryResponse>>> GetDelivery(Guid id)
    {
        try
        {
            var query = new GetDeliveryQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetDeliveryResponse>.SuccessResult(response, "Delivery retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery with ID: {DeliveryId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDeliveryResponse>.ErrorResult("An error occurred while retrieving delivery", new[] { ex.Message }));
        }
    }
}
