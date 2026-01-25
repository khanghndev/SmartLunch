using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Queries.Users;
using SmartLunch.Backend.Service.Application.Queries.Users.GetUsers;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// User management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Admin")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get list of users with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:users.read")]
    public async Task<ActionResult<BaseApiResponse<GetUsersResponse>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        try
        {
            var query = new GetUsersQuery(request.Page, request.PageSize, request.SearchTerm, request.IsActive);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetUsersResponse>.SuccessResult(response, "Users retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUsersResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUsersResponse>.ErrorResult("An error occurred while retrieving users", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "permission:users.read")]
    public async Task<ActionResult<BaseApiResponse<GetUserResponse>>> GetUser(Guid id)
    {
        try
        {
            var query = new GetUserQuery(id);
            var response = await _mediator.Send(query);

            return Ok(BaseApiResponse<GetUserResponse>.SuccessResult(response, "User retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserResponse>.ErrorResult("An error occurred while retrieving user", new[] { ex.Message }));
        }
    }
}
