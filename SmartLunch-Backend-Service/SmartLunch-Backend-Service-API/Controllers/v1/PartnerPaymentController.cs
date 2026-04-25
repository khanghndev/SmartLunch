using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.PartnerPayments;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;
using SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayment;
using SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayments;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// PartnerPayment management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class PartnerPaymentController : ControllerBase
{
    private readonly ILogger<PartnerPaymentController> _logger;
    private readonly IMediator _mediator;

    public PartnerPaymentController(ILogger<PartnerPaymentController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of partnerpayments with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:partnerpayments.read")]
    public async Task<ActionResult<BaseApiResponse<GetPartnerPaymentsResponse>>> GetPartnerPayments([FromQuery] GetPartnerPaymentsRequest request)
    {
        try
        {
            var query = new GetPartnerPaymentsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetPartnerPaymentsResponse>.SuccessResult(response, "PartnerPayments retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPartnerPaymentsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partnerpayments");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPartnerPaymentsResponse>.ErrorResult("An error occurred while retrieving partnerpayments", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get partnerpayment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:partnerpayments.read")]
    public async Task<ActionResult<BaseApiResponse<GetPartnerPaymentResponse>>> GetPartnerPayment(int id)
    {
        try
        {
            var query = new GetPartnerPaymentQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetPartnerPaymentResponse>.SuccessResult(response, "PartnerPayment retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetPartnerPaymentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving partnerpayment with ID: {PartnerPaymentId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPartnerPaymentResponse>.ErrorResult("An error occurred while retrieving partnerpayment", new[] { ex.Message }));
        }
    }
}
