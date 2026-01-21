using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;

namespace SmartLunch.Backend.Service.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<BaseApiResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _mediator.Send(new LoginCommand(request));
            return Ok(new BaseApiResponse(response));
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseApiResponse>> Register([FromBody] RegisterRequest request)
        {
            var response = await _mediator.Send(new RegisterRequest(request));
            return Ok(new BaseApiResponse(response));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<BaseApiResponse>> Logout([FromBody] LogoutRequest request)
        {
            var response = await _mediator.Send(new LogoutCommand(request));
            return Ok(new BaseApiResponse(response));
        }
    }
}