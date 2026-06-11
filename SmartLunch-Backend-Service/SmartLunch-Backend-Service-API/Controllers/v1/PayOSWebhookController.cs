using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLunch.Backend.Service.Application.Commands.PayOS.ProcessPayOSWebhook;
using SmartLunch.Backend.Service.Application.DTOs;

namespace SmartLunch.Backend.Service.API.Controllers;

/// <summary>
/// PayOS gọi POST từ server của họ — không JWT; cần URL HTTPS public (ngrok/cloudflared) khi dev.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payos")]
[AllowAnonymous]
[DisableRateLimiting]
public class PayOSWebhookController : ControllerBase
{
    private readonly ILogger<PayOSWebhookController> _logger;
    private readonly IMediator _mediator;

    public PayOSWebhookController(ILogger<PayOSWebhookController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("webhook")]
    [Consumes("application/json")]
    public async Task<IActionResult> Webhook([FromBody] System.Text.Json.JsonElement body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new ProcessPayOSWebhookCommand(body), cancellationToken);
            return result.Status switch
            {
                PayOSWebhookProcessStatus.Ok or PayOSWebhookProcessStatus.AlreadyProcessed or PayOSWebhookProcessStatus.IgnoredNonSuccess
                    or PayOSWebhookProcessStatus.AcknowledgedNoUpdate
                    => Ok(BaseApiResponse<object>.SuccessResult(
                        new { error = 0, message = result.Message },
                        result.Message)),
                PayOSWebhookProcessStatus.InvalidSignature
                    => StatusCode((int)HttpStatusCode.Unauthorized,
                        BaseApiResponse<object>.ErrorResult(result.Message, new[] { result.Message })),
                PayOSWebhookProcessStatus.InvalidPayload or PayOSWebhookProcessStatus.PaymentNotFound
                    or PayOSWebhookProcessStatus.AmountMismatch
                    => BadRequest(BaseApiResponse<object>.ErrorResult(result.Message, new[] { result.Message })),
                _ => BadRequest(BaseApiResponse<object>.ErrorResult(result.Message, new[] { result.Message })),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS webhook processing failed");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("Webhook processing failed", new[] { ex.Message }));
        }
    }
}
