namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;

public sealed class OrganizationChatbotMessageResponse
{
    public string Reply { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public int? LogId { get; set; }
}
