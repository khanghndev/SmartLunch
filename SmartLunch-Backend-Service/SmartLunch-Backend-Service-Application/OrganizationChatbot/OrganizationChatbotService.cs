using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public interface IOrganizationChatbotService
{
    Task<OrganizationChatbotMessageResponse> ProcessAsync(int userId, string message, CancellationToken cancellationToken = default);
}

public sealed class OrganizationChatbotService : IOrganizationChatbotService
{
    private readonly IOrganizationChatbotContextBuilder _contextBuilder;
    private readonly IOrganizationChatbotLlmClient _llmClient;
    private readonly OrganizationChatbotRuleFallback _ruleFallback;
    private readonly IChatbotLogRepository _chatbotLogs;
    private readonly OrganizationChatbotOptions _options;
    private readonly ILogger<OrganizationChatbotService> _logger;

    public OrganizationChatbotService(
        IOrganizationChatbotContextBuilder contextBuilder,
        IOrganizationChatbotLlmClient llmClient,
        OrganizationChatbotRuleFallback ruleFallback,
        IChatbotLogRepository chatbotLogs,
        IOptions<OrganizationChatbotOptions> options,
        ILogger<OrganizationChatbotService> logger)
    {
        _contextBuilder = contextBuilder;
        _llmClient = llmClient;
        _ruleFallback = ruleFallback;
        _chatbotLogs = chatbotLogs;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<OrganizationChatbotMessageResponse> ProcessAsync(
        int userId,
        string message,
        CancellationToken cancellationToken = default)
    {
        var trimmed = message.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return await LogAndReturnAsync(
                userId, trimmed,
                "Vui lòng nhập câu hỏi. Tôi tra cứu đơn hàng, hợp đồng, thực đơn và thanh toán từ hệ thống.",
                OrganizationChatbotIntents.Help, 0.2, cancellationToken);
        }

        var knowledge = await _contextBuilder.BuildAsync(userId, cancellationToken);

        // Thực đơn: trả lời trực tiếp từ DB (theo thứ T2–T7), không để LLM bịa / hướng dẫn chung chung
        if (OrganizationChatbotIntentClassifier.IsMenuQuestion(
                OrganizationChatbotIntentClassifier.NormalizeForMatch(trimmed)))
        {
            var menuReply = OrganizationChatbotMenuFormatter.FormatReply(trimmed, knowledge);
            return await LogAndReturnAsync(
                userId, trimmed, menuReply,
                OrganizationChatbotIntents.Menu, 0.96, cancellationToken,
                OrganizationChatbotMenuFormatter.DefaultSuggestions(knowledge));
        }

        // Primary: RAG + Gemini (trả lời tự nhiên, không cứng nhắc)
        if (_options.Enabled)
        {
            try
            {
                var llm = await _llmClient.GenerateReplyAsync(trimmed, knowledge, cancellationToken);
                if (llm != null && !string.IsNullOrWhiteSpace(llm.Reply))
                {
                    _logger.LogDebug("Chatbot LLM reply via {Model}", llm.Model);
                    return await LogAndReturnAsync(
                        userId, trimmed, llm.Reply,
                        "llm_rag", 0.92, cancellationToken, llm.Suggestions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "LLM chatbot failed, using rule fallback");
            }
        }

        // Fallback: rule engine trên knowledge pack (không cần gọi DB lại)
        var (reply, intent, confidence, suggestions) = _ruleFallback.Reply(trimmed, knowledge);
        return await LogAndReturnAsync(userId, trimmed, reply, intent, confidence, cancellationToken, suggestions);
    }

    private async Task<OrganizationChatbotMessageResponse> LogAndReturnAsync(
        int userId,
        string message,
        string reply,
        string intent,
        double confidence,
        CancellationToken cancellationToken,
        List<string>? suggestions = null)
    {
        var log = await _chatbotLogs.CreateAsync(new ChatbotLog
        {
            UserId = userId,
            Message = message,
            Response = reply,
            CreatedAt = VietnamTime.Now,
        }, cancellationToken);

        return new OrganizationChatbotMessageResponse
        {
            Reply = OrganizationChatbotTextFormatter.CleanReply(reply),
            Intent = intent,
            Confidence = confidence,
            Suggestions = suggestions ?? OrganizationChatbotRuleFallback.DefaultSuggestions(),
            LogId = log.Id,
        };
    }
}
