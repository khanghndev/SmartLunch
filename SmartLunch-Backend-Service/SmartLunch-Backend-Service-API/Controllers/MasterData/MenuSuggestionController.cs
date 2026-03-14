using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestion;
using SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestions;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// MenuSuggestion management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class MenuSuggestionController : ControllerBase
{
    private readonly ILogger<MenuSuggestionController> _logger;
    private readonly IMediator _mediator;

    public MenuSuggestionController(ILogger<MenuSuggestionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of menusuggestions with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:menusuggestions.read")]
    public async Task<ActionResult<BaseApiResponse<GetMenuSuggestionsResponse>>> GetMenuSuggestions([FromQuery] GetMenuSuggestionsRequest request)
    {
        try
        {
            var query = new GetMenuSuggestionsQuery(request.Page, request.PageSize, request.SearchTerm);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetMenuSuggestionsResponse>.SuccessResult(response, "MenuSuggestions retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMenuSuggestionsResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menusuggestions");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMenuSuggestionsResponse>.ErrorResult("An error occurred while retrieving menusuggestions", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get menusuggestion by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:menusuggestions.read")]
    public async Task<ActionResult<BaseApiResponse<GetMenuSuggestionResponse>>> GetMenuSuggestion(Guid id)
    {
        try
        {
            var query = new GetMenuSuggestionQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetMenuSuggestionResponse>.SuccessResult(response, "MenuSuggestion retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetMenuSuggestionResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menusuggestion with ID: {MenuSuggestionId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMenuSuggestionResponse>.ErrorResult("An error occurred while retrieving menusuggestion", new[] { ex.Message }));
        }
    }
}
