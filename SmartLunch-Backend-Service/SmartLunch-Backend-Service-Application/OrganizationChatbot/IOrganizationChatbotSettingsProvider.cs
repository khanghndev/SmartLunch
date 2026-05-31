using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public interface IOrganizationChatbotSettingsProvider
{
    OrganizationChatbotOptions GetCurrent();
    Task<OrganizationChatbotAdminConfigDto> GetAdminConfigAsync(CancellationToken cancellationToken = default);
    Task<OrganizationChatbotAdminConfigDto> SaveAsync(
        UpdateOrganizationChatbotConfigRequest request,
        int? updatedByUserId,
        CancellationToken cancellationToken = default);
    Task<OrganizationChatbotTestResultDto> TestConnectionAsync(CancellationToken cancellationToken = default);
}
