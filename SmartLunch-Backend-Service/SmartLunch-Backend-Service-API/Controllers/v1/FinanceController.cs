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
using SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPaymentReconciliation;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPayments;
using SmartLunch.Backend.Service.Application.Commands.Finance.CreateSupplierPayment;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetOrganizationReceivables;
using SmartLunch.Backend.Service.Application.Queries.Finance.GetSupplierContractsForPayment;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// Báo cáo thu chi và đối soát thanh toán (không phải CRUD master-data).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/finance")]
[Authorize(Policy = "roles:Admin,Manager,WarehouseStaff,Organization")]
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
    /// Thanh toán theo hợp đồng: tóm tắt &amp; chi tiết đơn (thu khách) + dòng chi nhà cung cấp (PartnerPayment).
    /// </summary>
    [HttpGet("contracts/{contractId:int}/payments")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:orders.read")]
    [Authorize(Policy = "permission:contracts.read")]
    public async Task<ActionResult<BaseApiResponse<GetContractPaymentsResponse>>> GetContractPayments(
        int contractId,
        [FromQuery] GetContractPaymentsRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetContractPaymentsQuery(contractId, request));
            return Ok(BaseApiResponse<GetContractPaymentsResponse>.SuccessResult(
                response,
                "Contract payments retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractPaymentsResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractPaymentsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading payments for contract {ContractId}", contractId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractPaymentsResponse>.ErrorResult(
                    "An error occurred while loading contract payments",
                    new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Đối soát theo hợp đồng: so khớp Payment (paid) với tổng đơn và trạng thái PaymentStatus trên đơn.
    /// </summary>
    [HttpGet("contracts/{contractId:int}/payment-reconciliation")]
    [Authorize(Policy = "permission:payments.read")]
    [Authorize(Policy = "permission:orders.read")]
    [Authorize(Policy = "permission:contracts.read")]
    public async Task<ActionResult<BaseApiResponse<GetContractPaymentReconciliationResponse>>> GetContractPaymentReconciliation(
        int contractId,
        [FromQuery] GetContractPaymentReconciliationRequest request)
    {
        try
        {
            var response = await _mediator.Send(new GetContractPaymentReconciliationQuery(contractId, request));
            return Ok(BaseApiResponse<GetContractPaymentReconciliationResponse>.SuccessResult(
                response,
                "Contract payment reconciliation retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetContractPaymentReconciliationResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetContractPaymentReconciliationResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building reconciliation for contract {ContractId}", contractId);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetContractPaymentReconciliationResponse>.ErrorResult(
                    "An error occurred while building contract payment reconciliation",
                    new[] { ex.Message }));
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
    /// <summary>Hợp đồng NCC (chưa hủy) kèm số đã chi — dùng khi ghi nhận thanh toán.</summary>
    [HttpGet("supplier-contracts")]
    [Authorize(Policy = "permission:contracts.read")]
    [Authorize(Policy = "permission:partner_payments.read")]
    public async Task<ActionResult<BaseApiResponse<GetSupplierContractsForPaymentResponse>>> GetSupplierContracts(
        [FromQuery] int partnerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _mediator.Send(new GetSupplierContractsForPaymentQuery(partnerId), cancellationToken);
            return Ok(BaseApiResponse<GetSupplierContractsForPaymentResponse>.SuccessResult(response, "Supplier contracts retrieved"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetSupplierContractsForPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading supplier contracts for partner {PartnerId}", partnerId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSupplierContractsForPaymentResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    /// <summary>Ghi nhận thanh toán cho nhà cung cấp (PartnerPayment).</summary>
    [HttpPost("supplier-payments")]
    [Authorize(Policy = "permission:partner_payments.create")]
    public async Task<ActionResult<BaseApiResponse<CreateSupplierPaymentResponse>>> CreateSupplierPayment(
        [FromBody] CreateSupplierPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _mediator.Send(new CreateSupplierPaymentCommand(request), cancellationToken);
            return Ok(BaseApiResponse<CreateSupplierPaymentResponse>.SuccessResult(response, response.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateSupplierPaymentResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateSupplierPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<CreateSupplierPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier payment");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateSupplierPaymentResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpGet("supplier-payables")]
    [Authorize(Policy = "permission:partner_payments.read")]
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
    [Authorize(Policy = "permission:partner_payments.read")]
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
