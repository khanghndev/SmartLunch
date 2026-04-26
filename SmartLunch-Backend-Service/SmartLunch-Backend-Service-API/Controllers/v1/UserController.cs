using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Users.CreateUser;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Users.UpdateUser;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Queries.Users;
using SmartLunch.Backend.Service.Application.Queries.Users.GetUsers;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// User management controller for CRUD operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/[controller]")]
[Authorize(Policy = "roles:Super Admin, Admin")]
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
            var query = new GetUsersQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.IsActive,
                request.RoleName);
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
    public async Task<ActionResult<BaseApiResponse<GetUserResponse>>> GetUser(int id)
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

    /// <summary>Tạo tài khoản mới; có thể gán <see cref="CreateUserRequest.InitialRoleId"/> (một role).</summary>
    [HttpPost]
    [Authorize(Policy = "permission:users.create")]
    public async Task<ActionResult<BaseApiResponse<GetUserResponse>>> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var actorId = RequireUserId();
            var response = await _mediator.Send(new CreateUserCommand(request, actorId));
            return Ok(BaseApiResponse<GetUserResponse>.SuccessResult(response, "User created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserResponse>.ErrorResult("An error occurred while creating user", new[] { ex.Message }));
        }
    }

    /// <summary>Cập nhật thông tin, đặt lại mật khẩu, khóa/mở (IsActive).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "permission:users.update")]
    public async Task<ActionResult<BaseApiResponse<GetUserResponse>>> Update(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var actorId = RequireUserId();
            var response = await _mediator.Send(new UpdateUserCommand(id, request, actorId));
            return Ok(BaseApiResponse<GetUserResponse>.SuccessResult(response, "User updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetUserResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, BaseApiResponse<GetUserResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetUserResponse>.ErrorResult("An error occurred while updating user", new[] { ex.Message }));
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
