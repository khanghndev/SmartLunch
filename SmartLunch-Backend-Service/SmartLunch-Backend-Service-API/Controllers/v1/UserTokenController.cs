using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserTokens;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;
using SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserToken;
using SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserTokens;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// UserToken management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UserTokenController : ControllerBase
{
    private readonly ILogger<UserTokenController> _logger;
    private readonly IMediator _mediator;

    public UserTokenController(ILogger<UserTokenController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of usertokens with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:usertokens.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserTokensResponse>>> GetUserTokens([FromQuery] GetUserTokensRequest request)
    {
        try
        {
            var query = new GetUserTokensQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUserTokensResponse>.SuccessResult(response, "UserTokens retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserTokensResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usertokens");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserTokensResponse>.ErrorResult("An error occurred while retrieving usertokens", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get usertoken by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:usertokens.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserTokenResponse>>> GetUserToken(Guid id)
    {
        try
        {
            var query = new GetUserTokenQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetUserTokenResponse>.SuccessResult(response, "UserToken retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserTokenResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usertoken with ID: {UserTokenId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserTokenResponse>.ErrorResult("An error occurred while retrieving usertoken", new[] { ex.Message }));
        }
    }
}
