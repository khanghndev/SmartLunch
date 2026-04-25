using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Sentiments;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;
using SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiment;
using SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiments;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Sentiment management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class SentimentController : ControllerBase
{
    private readonly ILogger<SentimentController> _logger;
    private readonly IMediator _mediator;

    public SentimentController(ILogger<SentimentController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of sentiments with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:sentiments.read")]
    public async Task<ActionResult<BaseApiResponse<GetSentimentsResponse>>> GetSentiments([FromQuery] GetSentimentsRequest request)
    {
        try
        {
            var query = new GetSentimentsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetSentimentsResponse>.SuccessResult(response, "Sentiments retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetSentimentsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiments");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSentimentsResponse>.ErrorResult("An error occurred while retrieving sentiments", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get sentiment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:sentiments.read")]
    public async Task<ActionResult<BaseApiResponse<GetSentimentResponse>>> GetSentiment(int id)
    {
        try
        {
            var query = new GetSentimentQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetSentimentResponse>.SuccessResult(response, "Sentiment retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetSentimentResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment with ID: {SentimentId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetSentimentResponse>.ErrorResult("An error occurred while retrieving sentiment", new[] { ex.Message }));
        }
    }
}
