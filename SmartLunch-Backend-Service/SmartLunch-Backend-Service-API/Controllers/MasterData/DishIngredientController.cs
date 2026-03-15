using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredient;
using SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredients;
using SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.CreateDishIngredient;
using SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.UpdateDishIngredient;
using SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.DeleteDishIngredient;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class DishIngredientController : ControllerBase
{
    private readonly ILogger<DishIngredientController> _logger;
    private readonly IMediator _mediator;

    public DishIngredientController(ILogger<DishIngredientController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "permission:dishes.read")]
    public async Task<ActionResult<BaseApiResponse<GetDishIngredientsResponse>>> GetDishIngredients([FromQuery] GetDishIngredientsRequest request)
    {
        try
        {
            var query = new GetDishIngredientsQuery(request.Page, request.PageSize, request.DishId, request.IngredientId);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetDishIngredientsResponse>.SuccessResult(response, "Dish ingredients retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDishIngredientsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dish ingredients");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishIngredientsResponse>.ErrorResult("An error occurred while retrieving dish ingredients", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "permission:dishes.read")]
    public async Task<ActionResult<BaseApiResponse<GetDishIngredientResponse>>> GetDishIngredient(Guid id)
    {
        try
        {
            var query = new GetDishIngredientQuery(id);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetDishIngredientResponse>.SuccessResult(response, "Dish ingredient retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dish ingredient {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishIngredientResponse>.ErrorResult("An error occurred while retrieving dish ingredient", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<CreateDishIngredientResponse>>> Create([FromBody] CreateDishIngredientRequest request)
    {
        try
        {
            var command = new CreateDishIngredientCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<CreateDishIngredientResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateDishIngredientResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateDishIngredientResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dish ingredient");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateDishIngredientResponse>.ErrorResult("An error occurred while creating dish ingredient", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<GetDishIngredientResponse>>> Update(Guid id, [FromBody] UpdateDishIngredientRequest request)
    {
        if (id != request.Id)
            return BadRequest(BaseApiResponse<GetDishIngredientResponse>.ErrorResult("Id mismatch", new[] { "Id in URL and body must match" }));
        try
        {
            var command = new UpdateDishIngredientCommand(request);
            var response = await _mediator.Send(command);
            return Ok(BaseApiResponse<GetDishIngredientResponse>.SuccessResult(response, "Dish ingredient updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetDishIngredientResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dish ingredient");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishIngredientResponse>.ErrorResult("An error occurred while updating dish ingredient", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<bool>>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteDishIngredientCommand(id);
            await _mediator.Send(command);
            return Ok(BaseApiResponse<bool>.SuccessResult(true, "Dish ingredient deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<bool>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dish ingredient");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<bool>.ErrorResult("An error occurred while deleting dish ingredient", new[] { ex.Message }));
        }
    }
}
