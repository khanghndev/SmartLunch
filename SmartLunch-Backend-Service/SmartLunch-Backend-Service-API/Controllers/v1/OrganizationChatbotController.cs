using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.OrganizationChatbot.SendOrganizationChatbotMessage;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Chatbot CSKH — trả lời dựa trên dữ liệu hệ thống cho khách hàng doanh nghiệp.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organization/chatbot")]
[Authorize(Policy = "roles:Organization,Khách hàng doanh nghiệp,Company")]
public class OrganizationChatbotController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrganizationChatbotController> _logger;

    public OrganizationChatbotController(IMediator mediator, ILogger<OrganizationChatbotController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Gửi tin nhắn và nhận phản hồi tra cứu từ DB.</summary>
    [HttpPost("message")]
    [Authorize(Policy = "permission:chatbot_logs.create")]
    public async Task<ActionResult<BaseApiResponse<OrganizationChatbotMessageResponse>>> SendMessage(
        [FromBody] OrganizationChatbotMessageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(BaseApiResponse<OrganizationChatbotMessageResponse>.ErrorResult(
                    "Message is required", new[] { "Tin nhắn không được để trống." }));
            }

            if (request.Message.Length > 2000)
            {
                return BadRequest(BaseApiResponse<OrganizationChatbotMessageResponse>.ErrorResult(
                    "Message too long", new[] { "Tin nhắn tối đa 2000 ký tự." }));
            }

            var userId = RequireUserId();
            var result = await _mediator.Send(
                new SendOrganizationChatbotMessageCommand(userId, request.Message.Trim()),
                cancellationToken);

            return Ok(BaseApiResponse<OrganizationChatbotMessageResponse>.SuccessResult(result, "Chatbot reply generated"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseApiResponse<OrganizationChatbotMessageResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Organization chatbot error");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<OrganizationChatbotMessageResponse>.ErrorResult("Failed", new[] { ex.Message }));
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
