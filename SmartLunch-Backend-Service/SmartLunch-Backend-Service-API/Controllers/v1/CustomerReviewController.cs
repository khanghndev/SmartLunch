using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.CreateCustomerReview;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Customer.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>
/// Review features for individual customers
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer-reviews")]
[Authorize(Policy = "roles:Customer")]
public class CustomerReviewController : ControllerBase
{
    private readonly ILogger<CustomerReviewController> _logger;
    private readonly IMediator _mediator;

    public CustomerReviewController(ILogger<CustomerReviewController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a review (dish or order) by the authenticated customer.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BaseApiResponse<CreateCustomerReviewResponse>>> Create([FromBody] CreateCustomerReviewRequest request)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new CreateCustomerReviewCommand(userId, request));
            return Ok(BaseApiResponse<CreateCustomerReviewResponse>.SuccessResult(response, "Review created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<CreateCustomerReviewResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<CreateCustomerReviewResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<CreateCustomerReviewResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<CreateCustomerReviewResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer review");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<CreateCustomerReviewResponse>.ErrorResult("An error occurred while creating the review", new[] { ex.Message }));
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

