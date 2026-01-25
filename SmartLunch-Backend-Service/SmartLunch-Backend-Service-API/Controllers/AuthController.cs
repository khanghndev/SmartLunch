using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using FirebaseLoginRequest = SmartLunch.Backend.Service.Application.DTOs.Request.Auth.FirebaseLoginRequest;
using FirebaseLoginCommand = SmartLunch.Backend.Service.Application.Commands.Auth.FirebaseLoginCommand;
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
        public async Task<ActionResult<BaseApiResponse<LoginResponse>>> FirebaseLogin(FirebaseLoginRequest request)
        {
            try
            {
                var response = await _mediator.Send(new FirebaseLoginCommand(request));
                return Ok(BaseApiResponse<LoginResponse>.SuccessResult(response, "Firebase login successful"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BaseApiResponse<LoginResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }
            catch (InvalidOperationException ex)
            {
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
    }
}