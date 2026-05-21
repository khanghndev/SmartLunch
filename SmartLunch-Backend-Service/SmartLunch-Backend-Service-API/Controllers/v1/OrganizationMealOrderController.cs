using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.ListEligiblePromotions;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Promotions.PreviewPromotion;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.CheckoutOrganizationMeal;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.InitiateOrganizationMealPayment;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.PrepareOrganizationMealContract;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishCategories;
using SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishesByCategory;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Đặt suất ăn theo đơn vị (Organization): danh mục món, lập hợp đồng (ghi contracts), checkout PayOS.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organization/meal-order")]
[Authorize(Policy = "roles:Company,Organization,Khách hàng doanh nghiệp")]
public class OrganizationMealOrderController : ControllerBase
{
    private readonly ILogger<OrganizationMealOrderController> _logger;
    private readonly IMediator _mediator;

    public OrganizationMealOrderController(ILogger<OrganizationMealOrderController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>Lấy thể loại món (bảng dish_categories) + cửa sổ ngày được phép đặt (VN).</summary>
    [AllowAnonymous]
    [HttpGet("dish-category")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationDishCategoriesResponse>>> GetDishCategories()
    {
        try
        {
            var response = await _mediator.Send(new GetOrganizationDishCategoriesQuery());
            return Ok(BaseApiResponse<GetOrganizationDishCategoriesResponse>.SuccessResult(
                response,
                "Dish categories retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: list dish categories");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationDishCategoriesResponse>.ErrorResult(
                    "An error occurred while retrieving dish categories",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Lấy món theo thể loại (Id bản ghi dish_categories).</summary>
    [AllowAnonymous]
    [HttpGet("dish/category")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationDishesByCategoryResponse>>> GetDishesByCategory(
        [FromQuery] int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var response = await _mediator.Send(new GetOrganizationDishesByCategoryQuery(categoryId, page, pageSize));
            return Ok(BaseApiResponse<GetOrganizationDishesByCategoryResponse>.SuccessResult(
                response,
                "Dishes retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrganizationDishesByCategoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: list dishes by category {CategoryId}", categoryId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationDishesByCategoryResponse>.ErrorResult(
                    "An error occurred while retrieving dishes",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Danh sách mã khuyến mãi đủ điều kiện cho đơn đặt suất (kênh b2b_org).</summary>
    [HttpPost("promotions/eligible")]
    public async Task<ActionResult<BaseApiResponse<ListEligiblePromotionsResponse>>> ListEligiblePromotions(
        [FromBody] PreviewPromotionRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new ListEligiblePromotionsCommand(request, userId));
            return Ok(BaseApiResponse<ListEligiblePromotionsResponse>.SuccessResult(
                response,
                "Eligible promotions retrieved"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<ListEligiblePromotionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ListEligiblePromotionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: list eligible promotions");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ListEligiblePromotionsResponse>.ErrorResult(
                    "An error occurred while listing eligible promotions",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Xem trước áp dụng mã khuyến mãi cho đơn đặt suất.</summary>
    [HttpPost("promotions/preview")]
    public async Task<ActionResult<BaseApiResponse<PreviewPromotionResponse>>> PreviewPromotion(
        [FromBody] PreviewPromotionRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new PreviewPromotionCommand(request, userId));
            return Ok(BaseApiResponse<PreviewPromotionResponse>.SuccessResult(
                response,
                "Promotion preview completed"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<PreviewPromotionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<PreviewPromotionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: preview promotion");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<PreviewPromotionResponse>.ErrorResult(
                    "An error occurred while previewing promotion",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Chuẩn bị hợp đồng / đơn hàng nháy: giá/suất do FE (<c>price</c>), mealPlan theo slot <c>main|side|soup</c> khớp <c>dish_categories.SlotKey</c>.
    /// Tổng tiền = <c>price × (tổng quantity các dòng main)</c> — không dùng Dish.Price. Chỉ lưu Redis.
    /// </summary>
    [HttpPost("contract")]
    public async Task<ActionResult<BaseApiResponse<PrepareOrganizationMealContractResponse>>> PrepareContract(
        [FromBody] PrepareOrganizationMealContractRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new PrepareOrganizationMealContractCommand(userId, request));
            return Ok(BaseApiResponse<PrepareOrganizationMealContractResponse>.SuccessResult(
                response,
                "Draft prepared successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<PrepareOrganizationMealContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<PrepareOrganizationMealContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: prepare contract draft");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<PrepareOrganizationMealContractResponse>.ErrorResult(
                    "An error occurred while preparing the contract draft",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Xác nhận đơn: ghi đơn + hợp đồng (nếu cần) vào DB, tạo bản ghi đặt cọc chờ thanh toán (chưa gọi PayOS).
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<BaseApiResponse<CheckoutOrganizationMealResponse>>> Checkout(
        [FromBody] CheckoutOrganizationMealRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CheckoutOrganizationMealCommand(userId, request));
            return Ok(BaseApiResponse<CheckoutOrganizationMealResponse>.SuccessResult(response, "Checkout successful"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CheckoutOrganizationMealResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CheckoutOrganizationMealResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: checkout");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CheckoutOrganizationMealResponse>.ErrorResult(
                    "An error occurred during checkout",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Khởi tạo thanh toán PayOS cho đơn đã ký phụ lục (trạng thái awaiting_payment).
    /// </summary>
    [HttpPost("pay")]
    public async Task<ActionResult<BaseApiResponse<InitiateOrganizationMealPaymentResponse>>> InitiatePayment(
        [FromBody] InitiateOrganizationMealPaymentRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new InitiateOrganizationMealPaymentCommand(userId, request));
            return Ok(BaseApiResponse<InitiateOrganizationMealPaymentResponse>.SuccessResult(
                response,
                "Payment link created"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Org meal order: initiate payment for order {OrderId}", request.OrderId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(
                    "An error occurred while initiating payment",
                    new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
