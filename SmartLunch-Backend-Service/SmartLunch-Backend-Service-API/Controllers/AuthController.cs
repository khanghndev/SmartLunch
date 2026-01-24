using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using System.Net;

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

        [HttpPost("login")]
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> Login(LoginRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LoginCommand(request));
                return Ok(CreateSuccessResponse(response, "Login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse<LoginResponse>(ex.Message, (int)HttpStatusCode.BadRequest));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(CreateErrorResponse<LoginResponse>(ex.Message, (int)HttpStatusCode.Unauthorized));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseApiResponse<RegisterResponse>>> Register(RegisterRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RegisterCommand(request));
                return Ok(CreateSuccessResponse(response, "User registered successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse<RegisterResponse>(ex.Message, (int)HttpStatusCode.BadRequest));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(CreateErrorResponse<RegisterResponse>(ex.Message, (int)HttpStatusCode.BadRequest));
            }
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<BaseApiResponse<LogoutResponse>>> Logout(LogoutRequest request)
        {
            try
            {
                var response = await _mediator.Send(new LogoutCommand(request));
                return Ok(CreateSuccessResponse(response, "Logout successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse<LogoutResponse>(ex.Message, (int)HttpStatusCode.BadRequest));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(CreateErrorResponse<LogoutResponse>(ex.Message, (int)HttpStatusCode.Unauthorized));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<BaseApiResponse<RefreshTokenResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var response = await _mediator.Send(new RefreshTokenCommand(request));
                return Ok(CreateSuccessResponse(response, "Token refreshed successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateErrorResponse<RefreshTokenResponse>(ex.Message, (int)HttpStatusCode.BadRequest));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(CreateErrorResponse<RefreshTokenResponse>(ex.Message, (int)HttpStatusCode.Unauthorized));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(CreateErrorResponse<RefreshTokenResponse>(ex.Message, (int)HttpStatusCode.NotFound));
            }
        }

        /// <summary>
        /// Helper method to create a successful API response
        /// </summary>
        private BaseApiResponse<T> CreateSuccessResponse<T>(T data, string message) where T : class
        {
            return new BaseApiResponse<T>
            {
                Success = true,
                ResponseStatus = "Success",
                ResponseMessage = message,
                ResponseData = data,
                ResponseTimestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Helper method to create an error API response
        /// </summary>
        private BaseApiResponse<T> CreateErrorResponse<T>(string message, int statusCode) where T : class
        {
            return BaseApiResponse<T>.ErrorResult(message, new[] { message }, statusCode);
        }
    }
}