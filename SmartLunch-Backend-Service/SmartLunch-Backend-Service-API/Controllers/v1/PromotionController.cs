using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.CreatePromotion;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.DeletePromotion;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.PreviewPromotion;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.UpdatePromotion;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotion;
using SmartLunch.Backend.Service.Application.Queries.Promotions.GetPromotions;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
public class PromotionController : ControllerBase
{
    private readonly ILogger<PromotionController> _logger;
    private readonly IMediator _mediator;

    public PromotionController(ILogger<PromotionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:promotions.read")]
    public async Task<ActionResult<BaseApiResponse<GetPromotionsResponse>>> GetPromotions([FromQuery] GetPromotionsRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetPromotionsQuery(
                request.Page, request.PageSize, request.SearchTerm, request.IsActive, request.ScopeType));
            return Ok(BaseApiResponse<GetPromotionsResponse>.SuccessResult(response, "Promotions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPromotionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving promotions");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPromotionsResponse>.ErrorResult("An error occurred while retrieving promotions", new[] { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:promotions.read")]
    public async Task<ActionResult<BaseApiResponse<GetPromotionResponse>>> GetPromotion(int id)
    {
        try
        {
            var response = await _mediator.Send(new GetPromotionQuery(id));
            return Ok(BaseApiResponse<GetPromotionResponse>.SuccessResult(response, "Promotion retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetPromotionResponse>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving promotion {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPromotionResponse>.ErrorResult("An error occurred while retrieving promotion", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:promotions.create")]
    public async Task<ActionResult<BaseApiResponse<GetPromotionResponse>>> CreatePromotion([FromBody] UpsertPromotionRequest request)
    {
        try
        {
            var response = await _mediator.Send(new CreatePromotionCommand(request));
            return Ok(BaseApiResponse<GetPromotionResponse>.SuccessResult(response, "Promotion created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPromotionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating promotion");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPromotionResponse>.ErrorResult("An error occurred while creating promotion", new[] { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:promotions.update")]
    public async Task<ActionResult<BaseApiResponse<GetPromotionResponse>>> UpdatePromotion(int id, [FromBody] UpsertPromotionRequest request)
    {
        try
        {
            var response = await _mediator.Send(new UpdatePromotionCommand(id, request));
            return Ok(BaseApiResponse<GetPromotionResponse>.SuccessResult(response, "Promotion updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetPromotionResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPromotionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating promotion {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPromotionResponse>.ErrorResult("An error occurred while updating promotion", new[] { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "roles:Admin,Manager")]
    [Authorize(Policy = "permission:promotions.delete")]
    public async Task<ActionResult<BaseApiResponse<object>>> DeletePromotion(int id)
    {
        try
        {
            await _mediator.Send(new DeletePromotionCommand(id));
            return Ok(BaseApiResponse<object>.SuccessResult(null!, "Promotion deactivated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<object>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting promotion {Id}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while deleting promotion", new[] { ex.Message }));
        }
    }

    /// <summary>Xem trước KM áp dụng (Manager + khách đặt hàng).</summary>
    [HttpPost("preview")]
    [Authorize]
    public async Task<ActionResult<BaseApiResponse<PreviewPromotionResponse>>> PreviewPromotion([FromBody] PreviewPromotionRequest request)
    {
        try
        {
            var userId = GetUserId();
            var response = await _mediator.Send(new PreviewPromotionCommand(request, userId));
            return Ok(BaseApiResponse<PreviewPromotionResponse>.SuccessResult(response, "Promotion preview completed"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<PreviewPromotionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing promotion");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<PreviewPromotionResponse>.ErrorResult("An error occurred while previewing promotion", new[] { ex.Message }));
        }
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
