using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;
using SmartLunch.Backend.Service.Application.Queries.Dishes.GetDishes;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Dish (menu item) management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class DishController : ControllerBase
{
    private readonly ILogger<DishController> _logger;
    private readonly IMediator _mediator;

    public DishController(ILogger<DishController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of dishes with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:dishes.read")]
    public async Task<ActionResult<BaseApiResponse<GetDishesResponse>>> GetDishes([FromQuery] GetDishesRequest request)
    {
        try
        {
            var query = new GetDishesQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive, request.Category);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetDishesResponse>.SuccessResult(response, "Dishes retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDishesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dishes");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishesResponse>.ErrorResult("An error occurred while retrieving dishes", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get dish by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:dishes.read")]
    public async Task<ActionResult<BaseApiResponse<GetDishResponse>>> GetDish(Guid id)
    {
        try
        {
            var query = new GetDishQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetDishResponse>.SuccessResult(response, "Dish retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDishResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dish with ID: {DishId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishResponse>.ErrorResult("An error occurred while retrieving dish", new[] { ex.Message }));
        }
    }
}
