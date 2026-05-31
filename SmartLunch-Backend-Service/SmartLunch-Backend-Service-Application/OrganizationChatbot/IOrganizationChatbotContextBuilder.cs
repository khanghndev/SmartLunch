namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public interface IOrganizationChatbotContextBuilder
{
    Task<OrganizationChatbotKnowledgePack> BuildAsync(int userId, CancellationToken cancellationToken = default);
}
