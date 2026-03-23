using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.CreateDish;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.UpdateDish;
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
    public async Task<ActionResult<BaseApiResponse<GetDishResponse>>> GetDish(
        Guid id,
        [FromQuery] bool includeIngredientQuotas = false)
    {
        try
        {
            var query = new GetDishQuery(id, includeIngredientQuotas);
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

    /// <summary>
    /// Tạo món (phân loại: man / xao / canh / trang_mieng — xem GET /catalog/dish-categories).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<GetDishResponse>>> CreateDish([FromBody] CreateDishRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreateDishCommand(request));
            return Ok(BaseApiResponse<GetDishResponse>.SuccessResult(response, "Dish created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDishResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dish");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishResponse>.ErrorResult("An error occurred while creating dish", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Sửa món; ẩn món bằng IsActive = false.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<GetDishResponse>>> UpdateDish(Guid id, [FromBody] UpdateDishRequest request)
    {
        try
        {
            var response = await _mediator.Send(new UpdateDishCommand(id, request));
            return Ok(BaseApiResponse<GetDishResponse>.SuccessResult(response, "Dish updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetDishResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetDishResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dish {DishId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishResponse>.ErrorResult("An error occurred while updating dish", new[] { ex.Message }));
        }
    }
}
