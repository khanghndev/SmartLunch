namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public interface IOrganizationChatbotLlmClient
{
    Task<LlmChatbotResult?> GenerateReplyAsync(
        string userMessage,
        OrganizationChatbotKnowledgePack knowledge,
        CancellationToken cancellationToken = default);
}

public sealed class LlmChatbotResult
{
    public string Reply { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public string Model { get; set; } = string.Empty;
}
