using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredient;
using SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredients;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Ingredient management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class IngredientController : ControllerBase
{
    private readonly ILogger<IngredientController> _logger;
    private readonly IMediator _mediator;

    public IngredientController(ILogger<IngredientController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of ingredients with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:ingredients.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientsResponse>>> GetIngredients([FromQuery] GetIngredientsRequest request)
    {
        try
        {
            var query = new GetIngredientsQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetIngredientsResponse>.SuccessResult(response, "Ingredients retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIngredientsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ingredients");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientsResponse>.ErrorResult("An error occurred while retrieving ingredients", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get ingredient by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:ingredients.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientResponse>>> GetIngredient(Guid id)
    {
        try
        {
            var query = new GetIngredientQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetIngredientResponse>.SuccessResult(response, "Ingredient retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetIngredientResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ingredient with ID: {IngredientId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientResponse>.ErrorResult("An error occurred while retrieving ingredient", new[] { ex.Message }));
        }
    }
}
