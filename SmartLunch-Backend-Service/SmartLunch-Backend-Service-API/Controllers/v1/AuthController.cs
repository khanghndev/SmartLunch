using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using System.Net;
using System.Security.Claims;
using FirebaseLoginRequest = SmartLunch.Backend.Service.Application.DTOs.Request.Auth.FirebaseLoginRequest;
using FirebaseLoginCommand = SmartLunch.Backend.Service.Application.Commands.Auth.FirebaseLoginCommand;
using SmartLunch.Backend.Service.Application.Commands.Auth.LoginAdmin;
using SmartLunch.Backend.Service.Application.Commands.Auth.LoginUser;

namespace SmartLunch.Backend.Service.API.Controllers
{
    /// <summary>
    /// Authentication controller for user login, registration, and token management
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("login-admin")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> LoginAdmin(LoginAdminRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LoginAdminCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> LoginUser(LoginUserRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LoginUserCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("firebase-login")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> FirebaseLogin([FromBody] FirebaseLoginRequest request)
        {
            // Check model validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogWarning("Firebase login validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult("Invalid request", errors));
            }

            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
            {
                _logger.LogWarning("Firebase login request is null or IdToken is empty");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult("IdToken is required", new[] { "IdToken is required" }));
            }

            try
            {
                var response = await _mediator.Send(new FirebaseLoginCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Firebase login successful"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Firebase login argument exception");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Firebase login unauthorized");
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Firebase login invalid operation");
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseApiResponse<RegisterResponse>>> Register(RegisterRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RegisterCommand(request));
                return Ok(BaseApiResponse<RegisterResponse>.SuccessResult(response, "User registered successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<RegisterResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseApiResponse<RegisterResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<LogoutResponse>>> Logout(LogoutRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LogoutCommand(request));
                return Ok(BaseApiResponse<LogoutResponse>.SuccessResult(response, "Logout successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LogoutResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LogoutResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<RefreshTokenResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RefreshTokenCommand(request));
                return Ok(BaseApiResponse<RefreshTokenResponse>.SuccessResult(response, "Token refreshed successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<RefreshTokenResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<RefreshTokenResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseApiResponse<RefreshTokenResponse>.NotFoundResult(ex.Message));
            }
        }

        [HttpPut("reset-password")]
        [Authorize]
        public async Task<ActionResult<BaseApiResponse<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult("Invalid request", errors));
            }

            try
            {
                var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
                    throw new UnauthorizedAccessException("Invalid user context.");

                var response = await _mediator.Send(new ResetPasswordCommand(request, userId));
                return Ok(BaseApiResponse<ResetPasswordResponse>.SuccessResult(response, "Password reset successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<ResetPasswordResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseApiResponse<ResetPasswordResponse>.NotFoundResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<ResetPasswordResponse>.ErrorResult("An error occurred while resetting password", new[] { ex.Message }));
            }
        }

        // [HttpGet("profile")]
        // [Authorize]
        // public async Task<ActionResult<BaseApiResponse<UserProfileResponse>>> GetProfile()
        // {
        //     try
        //     {
        //         var response = await _mediator.Send(new GetUserProfileCommand());
        //         return Ok(BaseApiResponse<UserProfileResponse>.SuccessResult(response, "Profile retrieved successfully"));
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(BaseApiResponse<UserProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        //     }
        //     catch (UnauthorizedAccessException ex)
        //     {
        //         return Unauthorized(BaseApiResponse<UserProfileResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        //     }
        // }
    }
}