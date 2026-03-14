using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.ChatbotLogs;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;
using SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLog;
using SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLogs;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// ChatbotLog management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class ChatbotLogController : ControllerBase
{
    private readonly ILogger<ChatbotLogController> _logger;
    private readonly IMediator _mediator;

    public ChatbotLogController(ILogger<ChatbotLogController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of chatbotlogs with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:chatbotlogs.read")]
    public async Task<ActionResult<BaseApiResponse<GetChatbotLogsResponse>>> GetChatbotLogs([FromQuery] GetChatbotLogsRequest request)
    {
        try
        {
            var query = new GetChatbotLogsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetChatbotLogsResponse>.SuccessResult(response, "ChatbotLogs retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetChatbotLogsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chatbotlogs");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetChatbotLogsResponse>.ErrorResult("An error occurred while retrieving chatbotlogs", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get chatbotlog by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:chatbotlogs.read")]
    public async Task<ActionResult<BaseApiResponse<GetChatbotLogResponse>>> GetChatbotLog(Guid id)
    {
        try
        {
            var query = new GetChatbotLogQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetChatbotLogResponse>.SuccessResult(response, "ChatbotLog retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetChatbotLogResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chatbotlog with ID: {ChatbotLogId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetChatbotLogResponse>.ErrorResult("An error occurred while retrieving chatbotlog", new[] { ex.Message }));
        }
    }
}
