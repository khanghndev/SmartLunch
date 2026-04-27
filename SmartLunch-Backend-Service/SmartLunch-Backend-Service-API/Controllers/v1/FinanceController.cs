using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetCashflowSummary;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetPartnerPayables;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentHistory;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentReconciliation;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetOrganizationReceivables;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Báo cáo thu chi và đối soát thanh toán (không phải CRUD master-data).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/finance")]
[Authorize(Policy = "roles:Admin")]
public class FinanceController : ControllerBase
{
    private readonly ILogger<FinanceController> _logger;
    private readonly IMediator _mediator;

    public FinanceController(ILogger<FinanceController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Tổng hợp thu chi theo ngày, tuần (ISO) hoặc tháng: tiền thu từ thanh toán đơn (paid), thu/chi từ bảng giao dịch.
    /// </summary>
    [HttpGet("cashflow-summary")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:transactions.read")]
    public async Task<ActionResult<BaseApiResponse<GetCashflowSummaryResponse>>> GetCashflowSummary(
        [FromQuery] GetCashflowSummaryRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetCashflowSummaryQuery(request));
            return Ok(BaseApiResponse<GetCashflowSummaryResponse>.SuccessResult(response, "Cashflow summary retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetCashflowSummaryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building cashflow summary");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetCashflowSummaryResponse>.ErrorResult("An error occurred while building cashflow summary", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Đối soát: so khớp tổng thanh toán trạng thái paid với tổng tiền đơn và trạng thái PaymentStatus trên đơn.
    /// </summary>
    [HttpGet("payment-reconciliation")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:orders.read")]
    public async Task<ActionResult<BaseApiResponse<GetPaymentReconciliationResponse>>> GetPaymentReconciliation(
        [FromQuery] GetPaymentReconciliationRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetPaymentReconciliationQuery(request));
            return Ok(BaseApiResponse<GetPaymentReconciliationResponse>.SuccessResult(response, "Payment reconciliation retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPaymentReconciliationResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building payment reconciliation");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPaymentReconciliationResponse>.ErrorResult("An error occurred while building payment reconciliation", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Công nợ khách hàng doanh nghiệp (theo tổ chức / Organization): tổng đơn chưa hủy − đã thu (Payment paid).
    /// </summary>
    [HttpGet("organization-receivables")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:orders.read")]
    [Authorize(Policy = "permission:organizations.read")]
    public async Task<ActionResult<BaseApiResponse<GetOrganizationReceivablesResponse>>> GetOrganizationReceivables(
        [FromQuery] GetOrganizationReceivablesRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetOrganizationReceivablesQuery(request));
            return Ok(BaseApiResponse<GetOrganizationReceivablesResponse>.SuccessResult(response, "Organization receivables retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetOrganizationReceivablesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building organization receivables");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetOrganizationReceivablesResponse>.ErrorResult("An error occurred while building organization receivables", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Công nợ nhà cung cấp (Partner): tổng giá trị hợp đồng (chưa hủy) − đã chi (PartnerPayment completed).
    /// </summary>
    [HttpGet("supplier-payables")]
    [Authorize(Policy = "permission:partnerpayments.read")]
    [Authorize(Policy = "permission:partners.read")]
    [Authorize(Policy = "permission:contracts.read")]
    public async Task<ActionResult<BaseApiResponse<GetPartnerPayablesResponse>>> GetSupplierPayables(
        [FromQuery] GetPartnerPayablesRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetPartnerPayablesQuery(request));
            return Ok(BaseApiResponse<GetPartnerPayablesResponse>.SuccessResult(response, "Supplier payables retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPartnerPayablesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building supplier payables");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPartnerPayablesResponse>.ErrorResult("An error occurred while building supplier payables", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Lịch sử thanh toán: thanh toán đơn hàng (khách) và/hoặc thanh toán NCC trong khoảng ngày.
    /// </summary>
    [HttpGet("payment-history")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:partnerpayments.read")]
    public async Task<ActionResult<BaseApiResponse<GetPaymentHistoryResponse>>> GetPaymentHistory(
        [FromQuery] GetPaymentHistoryRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetPaymentHistoryQuery(request));
            return Ok(BaseApiResponse<GetPaymentHistoryResponse>.SuccessResult(response, "Payment history retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPaymentHistoryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building payment history");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPaymentHistoryResponse>.ErrorResult("An error occurred while building payment history", new[] { ex.Message }));
        }
    }
}
