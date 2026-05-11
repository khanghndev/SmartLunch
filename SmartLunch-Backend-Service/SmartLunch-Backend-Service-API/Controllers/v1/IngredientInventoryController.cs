using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.IngredientInventory.CreateInternalStockIssue;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientInventory;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetIngredientInventoryDetail;
using SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssue;
using SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssues;
using SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetLowStockIngredientAlerts;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Nghiệp vụ quản lý tồn kho nguyên liệu: cảnh báo tồn thấp, chi tiết tồn, phiếu xuất kho nội bộ.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ingredient-inventory")]
[Authorize(Policy = "roles:Admin,WarehouseStaff,Manager")]
public class IngredientInventoryController : ControllerBase
{
    private readonly ILogger<IngredientInventoryController> _logger;
    private readonly IMediator _mediator;

    public IngredientInventoryController(ILogger<IngredientInventoryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Cảnh báo tồn kho thấp: nguyên liệu đang hoạt động có QuantityAvailable &lt;= ReorderLevel.
    /// </summary>
    [HttpGet("low-stock-alerts")]
    [Authorize(Policy = "permission:inventory.read")]
    [Authorize(Policy = "permission:ingredients.read")]
    public async Task<ActionResult<BaseApiResponse<GetLowStockIngredientAlertsResponse>>> GetLowStockAlerts()
    {
        try
        {
            var response = await _mediator.Send(new GetLowStockIngredientAlertsQuery());
            return Ok(BaseApiResponse<GetLowStockIngredientAlertsResponse>.SuccessResult(
                response,
                "Low stock alerts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading low stock alerts");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetLowStockIngredientAlertsResponse>.ErrorResult(
                    "An error occurred while loading low stock alerts",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chi tiết tồn kho một nguyên liệu (tồn hiện tại, ngưỡng, lô gần đây).
    /// </summary>
    [HttpGet("detail/{ingredientId:int}")]
    [Authorize(Policy = "permission:inventory.read")]
    [Authorize(Policy = "permission:ingredients.read")]
    public async Task<ActionResult<BaseApiResponse<GetIngredientInventoryDetailResponse>>> GetDetail(
        int ingredientId,
        [FromQuery] int recentBatches = 20)
    {
        try
        {
            var response = await _mediator.Send(new GetIngredientInventoryDetailQuery(ingredientId, recentBatches));
            return Ok(BaseApiResponse<GetIngredientInventoryDetailResponse>.SuccessResult(
                response,
                "Ingredient inventory detail retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetIngredientInventoryDetailResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory detail for {IngredientId}", ingredientId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetIngredientInventoryDetailResponse>.ErrorResult(
                    "An error occurred while loading inventory detail",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Danh sách phiếu xuất kho nội bộ (có lọc theo ngày phát hành).
    /// </summary>
    [HttpGet("internal-issues")]
    [Authorize(Policy = "permission:internal_stock_issues.read")]
    public async Task<ActionResult<BaseApiResponse<GetInternalStockIssuesResponse>>> GetInternalIssues(
        [FromQuery] GetInternalStockIssuesRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetInternalStockIssuesQuery(request));
            return Ok(BaseApiResponse<GetInternalStockIssuesResponse>.SuccessResult(
                response,
                "Internal stock issues retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetInternalStockIssuesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing internal stock issues");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetInternalStockIssuesResponse>.ErrorResult(
                    "An error occurred while listing internal stock issues",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chi tiết một phiếu xuất kho nội bộ.
    /// </summary>
    [HttpGet("internal-issues/{id:int}")]
    [Authorize(Policy = "permission:internal_stock_issues.read")]
    public async Task<ActionResult<BaseApiResponse<InternalStockIssueDetailDto>>> GetInternalIssue(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetInternalStockIssueQuery(id));
            if (dto == null)
            {
                return NotFound(BaseApiResponse<InternalStockIssueDetailDto>.NotFoundResult(
                    $"Internal stock issue {id} was not found."));
            }

            return Ok(BaseApiResponse<InternalStockIssueDetailDto>.SuccessResult(
                dto,
                "Internal stock issue retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading internal stock issue {IssueId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<InternalStockIssueDetailDto>.ErrorResult(
                    "An error occurred while loading the internal stock issue",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Lập phiếu xuất kho nội bộ và trừ tồn kho theo từng dòng nguyên liệu.
    /// </summary>
    [HttpPost("internal-issues")]
    [Authorize(Policy = "permission:internal_stock_issues.create")]
    public async Task<ActionResult<BaseApiResponse<CreateInternalStockIssueResponse>>> CreateInternalIssue(
        [FromBody] CreateInternalStockIssueRequest request)
    {
        try
        {
            int? createdBy = null;
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var uid))
                createdBy = uid;

            var response = await _mediator.Send(new CreateInternalStockIssueCommand(request, createdBy));
            return Ok(BaseApiResponse<CreateInternalStockIssueResponse>.SuccessResult(
                response,
                "Internal stock issue created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateInternalStockIssueResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateInternalStockIssueResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateInternalStockIssueResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating internal stock issue");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateInternalStockIssueResponse>.ErrorResult(
                    "An error occurred while creating the internal stock issue",
                    new[] { ex.Message }));
        }
    }
}
