using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Admin — cấu hình chatbot CSKH doanh nghiệp (API key, model, prompt).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/organization-chatbot")]
[Authorize(Policy = "roles:Super Admin,Admin")]
public class OrganizationChatbotAdminController : ControllerBase
{
    private readonly IOrganizationChatbotSettingsProvider _settings;
    private readonly ILogger<OrganizationChatbotAdminController> _logger;

    public OrganizationChatbotAdminController(
        IOrganizationChatbotSettingsProvider settings,
        ILogger<OrganizationChatbotAdminController> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    [HttpGet("config")]
    public async Task<ActionResult<BaseApiResponse<OrganizationChatbotAdminConfigDto>>> GetConfig(CancellationToken cancellationToken)
    {
        var config = await _settings.GetAdminConfigAsync(cancellationToken);
        return Ok(BaseApiResponse<OrganizationChatbotAdminConfigDto>.SuccessResult(config, "OK"));
    }

    [HttpPut("config")]
    public async Task<ActionResult<BaseApiResponse<OrganizationChatbotAdminConfigDto>>> SaveConfig(
        [FromBody] UpdateOrganizationChatbotConfigRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = TryGetUserId();
            var saved = await _settings.SaveAsync(request, userId, cancellationToken);
            return Ok(BaseApiResponse<OrganizationChatbotAdminConfigDto>.SuccessResult(saved, "Đã lưu cấu hình chatbot."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Save chatbot config failed");
            return BadRequest(BaseApiResponse<OrganizationChatbotAdminConfigDto>.ErrorResult(
                "Save failed", new[] { ex.Message }));
        }
    }

    [HttpPost("test-connection")]
    public async Task<ActionResult<BaseApiResponse<OrganizationChatbotTestResultDto>>> TestConnection(CancellationToken cancellationToken)
    {
        var result = await _settings.TestConnectionAsync(cancellationToken);
        if (!result.Success)
        {
            return Ok(BaseApiResponse<OrganizationChatbotTestResultDto>.SuccessResult(
                result, result.Message));
        }
        return Ok(BaseApiResponse<OrganizationChatbotTestResultDto>.SuccessResult(result, "Kết nối thành công."));
    }

    private int? TryGetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : null;
    }
}
