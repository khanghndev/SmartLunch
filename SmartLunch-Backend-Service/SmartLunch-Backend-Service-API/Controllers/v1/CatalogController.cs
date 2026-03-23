using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.Catalog;
using SmartLunch.Backend.Service.Application.Queries.Catalog.GetDishCategories;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Danh mục tham chiếu hệ thống (không map 1-1 một bảng CRUD).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/catalog")]
[Authorize(Policy = "roles:Admin")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly IMediator _mediator;

    public CatalogController(ILogger<CatalogController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Phân loại món chuẩn (mặn, xào, canh, tráng miệng) — dùng khi tạo/sửa món.
    /// </summary>
    [HttpGet("dish-categories")]
    [Authorize(Policy = "permission:dishes.read")]
    public async Task<ActionResult<BaseApiResponse<GetDishCategoriesResponse>>> GetDishCategories()
    {
        try
        {
            var response = await _mediator.Send(new GetDishCategoriesQuery());
            return Ok(BaseApiResponse<GetDishCategoriesResponse>.SuccessResult(response, "Dish categories retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dish categories");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetDishCategoriesResponse>.ErrorResult("An error occurred while retrieving dish categories", new[] { ex.Message }));
        }
    }
}
