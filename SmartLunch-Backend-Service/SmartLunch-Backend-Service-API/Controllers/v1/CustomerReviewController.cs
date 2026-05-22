using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.CreateCustomerReview;
using SmartLunch.Backend.Service.Application.Commands.Customer.Reviews.ReplyToReview;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Customer.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetManagerReviews;
using SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetPublicReviews;
using SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetReviewMeContext;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Đánh giá suất ăn — khách doanh nghiệp (đã đặt đơn) gửi; mọi người xem công khai; quản lý trả lời.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer-reviews")]
public class CustomerReviewController : ControllerBase
{
    private readonly ILogger<CustomerReviewController> _logger;
    private readonly IMediator _mediator;

    public CustomerReviewController(ILogger<CustomerReviewController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseApiResponse<GetPublicReviewsResponse>>> GetPublic(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _mediator.Send(new GetPublicReviewsQuery(page, pageSize), cancellationToken);
            return Ok(BaseApiResponse<GetPublicReviewsResponse>.SuccessResult(response, "Public reviews retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading public reviews");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetPublicReviewsResponse>.ErrorResult("Failed to load reviews", new[] { ex.Message }));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<BaseApiResponse<GetReviewMeContextResponse>>> GetMeContext(CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetReviewMeContextQuery(userId), cancellationToken);
            return Ok(BaseApiResponse<GetReviewMeContextResponse>.SuccessResult(response, "Review context"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<GetReviewMeContextResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading review context");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetReviewMeContextResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Policy = "roles:Organization,Khách hàng doanh nghiệp,Company")]
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

    [HttpGet("manager")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<GetManagerReviewsResponse>>> GetForManager(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? maxRating = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _mediator.Send(
                new GetManagerReviewsQuery(page, pageSize, searchTerm, maxRating), cancellationToken);
            return Ok(BaseApiResponse<GetManagerReviewsResponse>.SuccessResult(response, "Manager reviews retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manager reviews");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetManagerReviewsResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost("{id:int}/reply")]
    [Authorize(Policy = "roles:Admin,Manager")]
    public async Task<ActionResult<BaseApiResponse<ManagerReviewListItemDto>>> Reply(
        int id,
        [FromBody] ReplyToReviewRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var managerId = RequireUserId();
            var dto = await _mediator.Send(new ReplyToReviewCommand(id, managerId, request.Reply), cancellationToken);
            return Ok(BaseApiResponse<ManagerReviewListItemDto>.SuccessResult(dto, "Reply saved"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<ManagerReviewListItemDto>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<ManagerReviewListItemDto>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to review {ReviewId}", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<ManagerReviewListItemDto>.ErrorResult("Failed", new[] { ex.Message }));
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
