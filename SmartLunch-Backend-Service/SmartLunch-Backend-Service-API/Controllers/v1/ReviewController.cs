using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;
using SmartLunch.Backend.Service.Application.Queries.Reviews.GetReview;
using SmartLunch.Backend.Service.Application.Queries.Reviews.GetReviews;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Review management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class ReviewController : ControllerBase
{
    private readonly ILogger<ReviewController> _logger;
    private readonly IMediator _mediator;

    public ReviewController(ILogger<ReviewController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of reviews with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:reviews.read")]
    public async Task<ActionResult<BaseApiResponse<GetReviewsResponse>>> GetReviews([FromQuery] GetReviewsRequest request)
    {
        try
        {
            var query = new GetReviewsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetReviewsResponse>.SuccessResult(response, "Reviews retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetReviewsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetReviewsResponse>.ErrorResult("An error occurred while retrieving reviews", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get review by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:reviews.read")]
    public async Task<ActionResult<BaseApiResponse<GetReviewResponse>>> GetReview(int id)
    {
        try
        {
            var query = new GetReviewQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetReviewResponse>.SuccessResult(response, "Review retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetReviewResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review with ID: {ReviewId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetReviewResponse>.ErrorResult("An error occurred while retrieving review", new[] { ex.Message }));
        }
    }
}
