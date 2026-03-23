using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.CreateIngredientSource;
using SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.DeleteIngredientSource;
using SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.UpdateIngredientSource;
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
            var query = new GetIngredientSourcesQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.PartnerId,
                request.IngredientId);
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

    /// <summary>
    /// Ghi nhận nguồn / lô nguyên liệu và nhà cung cấp (đối tác).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permission:ingredientsources.update")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientSourceResponse>>> CreateIngredientSource(
        [FromBody] CreateIngredientSourceRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreateIngredientSourceCommand(request));
            return Ok(BaseApiResponse<GetIngredientSourceResponse>.SuccessResult(response, "Ingredient source created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetIngredientSourceResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ingredient source");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientSourceResponse>.ErrorResult("An error occurred while creating ingredient source", new[] { ex.Message }));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:ingredientsources.update")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientSourceResponse>>> UpdateIngredientSource(
        Guid id,
        [FromBody] UpdateIngredientSourceRequest request)
    {
        try
        {
            var response = await _mediator.Send(new UpdateIngredientSourceCommand(id, request));
            return Ok(BaseApiResponse<GetIngredientSourceResponse>.SuccessResult(response, "Ingredient source updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetIngredientSourceResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ingredient source {Id}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientSourceResponse>.ErrorResult("An error occurred while updating ingredient source", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "permission:ingredientsources.update")]
    public async Task<ActionResult<BaseApiResponse<DeleteIngredientSourceResponse>>> DeleteIngredientSource(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new DeleteIngredientSourceCommand(id));
            return Ok(BaseApiResponse<DeleteIngredientSourceResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<DeleteIngredientSourceResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ingredient source {Id}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<DeleteIngredientSourceResponse>.ErrorResult("An error occurred while deleting ingredient source", new[] { ex.Message }));
        }
    }
}
