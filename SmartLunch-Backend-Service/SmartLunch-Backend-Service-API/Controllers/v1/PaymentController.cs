using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Payments;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Payments;
using SmartLunch.Backend.Service.Application.Queries.Payments.GetPayment;
using SmartLunch.Backend.Service.Application.Queries.Payments.GetPayments;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Payment management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;

    public PaymentController(ILogger<PaymentController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of payments with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:payments.read")]
    public async Task<ActionResult<BaseApiResponse<GetPaymentsResponse>>> GetPayments([FromQuery] GetPaymentsRequest request)
    {
        try
        {
            var query = new GetPaymentsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetPaymentsResponse>.SuccessResult(response, "Payments retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPaymentsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPaymentsResponse>.ErrorResult("An error occurred while retrieving payments", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:payments.read")]
    public async Task<ActionResult<BaseApiResponse<GetPaymentResponse>>> GetPayment(int id)
    {
        try
        {
            var query = new GetPaymentQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetPaymentResponse>.SuccessResult(response, "Payment retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with ID: {PaymentId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPaymentResponse>.ErrorResult("An error occurred while retrieving payment", new[] { ex.Message }));
        }
    }
}
