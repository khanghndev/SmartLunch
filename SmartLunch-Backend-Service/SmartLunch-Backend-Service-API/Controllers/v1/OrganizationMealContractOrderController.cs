using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.CheckoutOrganizationMealPeriodContract;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.InitiateOrganizationMealPeriodPayment;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.PrepareOrganizationMealPeriodContract;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.SubmitOrganizationMealWeeklySelection;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Queries.OrganizationMealContractOrders.GetMealPortionPrices;
using SmartLunch.Backend.Service.Application.Queries.OrganizationMealContractOrders.GetOrganizationMealContractMainDishes;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Đặt suất ăn theo hợp đồng kỳ (Period-Based): thời hạn + ngày loại trừ, đặt cọc, chọn món theo tuần.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organization/meal-contract-order")]
[Authorize(Policy = "roles:Company,Organization,Khách hàng doanh nghiệp")]
public class OrganizationMealContractOrderController : ControllerBase
{
    private readonly ILogger<OrganizationMealContractOrderController> _logger;
    private readonly IMediator _mediator;

    public OrganizationMealContractOrderController(
        ILogger<OrganizationMealContractOrderController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>Mức giá suất ăn (bảng dish_values) — dùng khi chọn đơn giá HĐ kỳ.</summary>
    [HttpGet("portion-prices")]
    public async Task<ActionResult<BaseApiResponse<GetMealPortionPricesResponse>>> GetPortionPrices()
    {
        try
        {
            var response = await _mediator.Send(new GetMealPortionPricesQuery());
            return Ok(BaseApiResponse<GetMealPortionPricesResponse>.SuccessResult(
                response,
                "Meal portion prices retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Period meal contract: list portion prices");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMealPortionPricesResponse>.ErrorResult(
                    "An error occurred while retrieving portion prices",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Danh sách món chính (slot <c>main</c>) — dùng khi chọn món tuần.</summary>
    [HttpGet("dish/main")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationDishesByCategoryResponse>>> GetMainDishes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        try
        {
            var response = await _mediator.Send(new GetOrganizationMealContractMainDishesQuery(page, pageSize, search));
            return Ok(BaseApiResponse<GetOrganizationDishesByCategoryResponse>.SuccessResult(
                response,
                "Main dishes retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrganizationDishesByCategoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Period meal contract: list main dishes");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationDishesByCategoryResponse>.ErrorResult(
                    "An error occurred while retrieving main dishes",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Flow 1 — Chuẩn bị HĐ theo kỳ: thời hạn (bắt đầu ≥ hôm nay + 3 ngày, tối đa 1 tháng), ngày loại trừ, suất/ngày, giá/suất.
    /// </summary>
    [HttpPost("contract")]
    public async Task<ActionResult<BaseApiResponse<PrepareOrganizationMealPeriodContractResponse>>> PrepareContract(
        [FromBody] PrepareOrganizationMealPeriodContractRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new PrepareOrganizationMealPeriodContractCommand(userId, request));
            return Ok(BaseApiResponse<PrepareOrganizationMealPeriodContractResponse>.SuccessResult(
                response,
                "Period contract draft prepared successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<PrepareOrganizationMealPeriodContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<PrepareOrganizationMealPeriodContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Period meal contract: prepare");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<PrepareOrganizationMealPeriodContractResponse>.ErrorResult(
                    "An error occurred while preparing the period contract",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Flow 1b — Xác nhận HĐ: ghi đơn nguồn + đặt cọc chờ thanh toán.</summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>>> Checkout(
        [FromBody] CheckoutOrganizationMealPeriodContractRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CheckoutOrganizationMealPeriodContractCommand(userId, request));
            return Ok(BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>.SuccessResult(response, "Checkout successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Period meal contract: checkout");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CheckoutOrganizationMealPeriodContractResponse>.ErrorResult(
                    "An error occurred during checkout",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Flow 2 — Khởi tạo thanh toán PayOS đặt cọc HĐ theo kỳ.</summary>
    [HttpPost("pay")]
    public async Task<ActionResult<BaseApiResponse<InitiateOrganizationMealPaymentResponse>>> InitiatePayment(
        [FromBody] InitiateOrganizationMealPeriodPaymentRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new InitiateOrganizationMealPeriodPaymentCommand(userId, request));
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
            _logger.LogError(ex, "Period meal contract: pay order {OrderId}", request.OrderId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<InitiateOrganizationMealPaymentResponse>.ErrorResult(
                    "An error occurred while initiating payment",
                    new[] { ex.Message }));
        }
    }

    /// <summary>Flow 3 — Chọn món chính cho một tuần (weekStart = Thứ 2). Chỉ <c>mealPlan.main</c>.</summary>
    [HttpPost("{contractId:int}/weekly-meals")]
    public async Task<ActionResult<BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>>> SubmitWeeklyMeals(
        int contractId,
        [FromBody] SubmitOrganizationMealWeeklySelectionRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(
                new SubmitOrganizationMealWeeklySelectionCommand(userId, contractId, request));
            return Ok(BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>.SuccessResult(
                response,
                "Weekly meal selection saved"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Period meal contract: weekly meals contract {ContractId}", contractId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<SubmitOrganizationMealWeeklySelectionResponse>.ErrorResult(
                    "An error occurred while saving weekly meals",
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
