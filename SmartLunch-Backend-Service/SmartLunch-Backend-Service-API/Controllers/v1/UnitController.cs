using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Units;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;
using SmartLunch.Backend.Service.Application.Queries.Units.GetUnit;
using SmartLunch.Backend.Service.Application.Queries.Units.GetUnits;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Unit (customer/school/company) management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UnitController : ControllerBase
{
    private readonly ILogger<UnitController> _logger;
    private readonly IMediator _mediator;

    public UnitController(ILogger<UnitController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of units with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:units.read")]
    public async Task<ActionResult<BaseApiResponse<GetUnitsResponse>>> GetUnits([FromQuery] GetUnitsRequest request)
    {
        try
        {
            var query = new GetUnitsQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUnitsResponse>.SuccessResult(response, "Units retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUnitsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving units");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUnitsResponse>.ErrorResult("An error occurred while retrieving units", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:units.read")]
    public async Task<ActionResult<BaseApiResponse<GetUnitResponse>>> GetUnit(Guid id)
    {
        try
        {
            var query = new GetUnitQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetUnitResponse>.SuccessResult(response, "Unit retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUnitResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unit with ID: {UnitId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUnitResponse>.ErrorResult("An error occurred while retrieving unit", new[] { ex.Message }));
        }
    }
}
