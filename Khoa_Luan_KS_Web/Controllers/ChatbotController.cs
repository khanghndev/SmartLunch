using System.Security.Claims;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers;

[Authorize(Policy = "CustomerArea")]
public class ChatbotController : Controller
{
    private readonly BackendOrganizationChatbotClient _chatbotClient;
    private readonly IApiTokenService _apiTokenService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(
        BackendOrganizationChatbotClient chatbotClient,
        IApiTokenService apiTokenService,
        ILogger<ChatbotController> logger)
    {
        _chatbotClient = chatbotClient;
        _apiTokenService = apiTokenService;
        _logger = logger;
    }

    private static bool IsOrganizationAccount(ClaimsPrincipal user) =>
        user.IsInRole("Organization") ||
        user.IsInRole("Company") ||
        user.IsInRole("Khách hàng doanh nghiệp");

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] ChatbotSendRequest request, CancellationToken ct)
    {
        if (!IsOrganizationAccount(User))
        {
            return BadRequest(new { error = "Chatbot dữ liệu chỉ dành cho tài khoản doanh nghiệp." });
        }

        var message = request.Message?.Trim();
        if (string.IsNullOrWhiteSpace(message))
            return BadRequest(new { error = "Tin nhắn không được để trống." });

        if (message.Length > 2000)
            return BadRequest(new { error = "Tin nhắn quá dài." });

        var accessToken = _apiTokenService.GetAccessToken();
        if (string.IsNullOrWhiteSpace(accessToken))
            return Unauthorized(new { error = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại." });

        try
        {
            var result = await _chatbotClient.SendMessageAsync(message, accessToken, ct);
            return Json(new
            {
                reply = result.Reply,
                intent = result.Intent,
                confidence = result.Confidence,
                suggestions = result.Suggestions,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Organization chatbot proxy failed");
            return StatusCode(500, new { error = "Không thể xử lý câu hỏi. Vui lòng thử lại hoặc liên hệ hotline." });
        }
    }
}

public class ChatbotSendRequest
{
    public string? Message { get; set; }
}
