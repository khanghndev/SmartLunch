using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Inventories;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;
using SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventory;
using SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventories;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Inventory management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private readonly IMediator _mediator;

    public InventoryController(ILogger<InventoryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of inventories with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:inventories.read")]
    public async Task<ActionResult<BaseApiResponse<GetInventoriesResponse>>> GetInventories([FromQuery] GetInventoriesRequest request)
    {
        try
        {
            var query = new GetInventoriesQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetInventoriesResponse>.SuccessResult(response, "Inventories retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetInventoriesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventories");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetInventoriesResponse>.ErrorResult("An error occurred while retrieving inventories", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get inventory by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:inventories.read")]
    public async Task<ActionResult<BaseApiResponse<GetInventoryResponse>>> GetInventory(Guid id)
    {
        try
        {
            var query = new GetInventoryQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetInventoryResponse>.SuccessResult(response, "Inventory retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetInventoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory with ID: {InventoryId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetInventoryResponse>.ErrorResult("An error occurred while retrieving inventory", new[] { ex.Message }));
        }
    }
}
