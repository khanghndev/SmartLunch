using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSource;
using SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSources;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// IngredientSource management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class IngredientSourceController : ControllerBase
{
    private readonly ILogger<IngredientSourceController> _logger;
    private readonly IMediator _mediator;

    public IngredientSourceController(ILogger<IngredientSourceController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of ingredientsources with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:ingredientsources.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientSourcesResponse>>> GetIngredientSources([FromQuery] GetIngredientSourcesRequest request)
    {
        try
        {
            var query = new GetIngredientSourcesQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetIngredientSourcesResponse>.SuccessResult(response, "IngredientSources retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIngredientSourcesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ingredientsources");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientSourcesResponse>.ErrorResult("An error occurred while retrieving ingredientsources", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get ingredientsource by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:ingredientsources.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientSourceResponse>>> GetIngredientSource(Guid id)
    {
        try
        {
            var query = new GetIngredientSourceQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetIngredientSourceResponse>.SuccessResult(response, "IngredientSource retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIngredientSourceResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ingredientsource with ID: {IngredientSourceId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientSourceResponse>.ErrorResult("An error occurred while retrieving ingredientsource", new[] { ex.Message }));
        }
    }
}
