using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLog;

public class GetChatbotLogQueryHandler : IRequestHandler<GetChatbotLogQuery, GetChatbotLogResponse>
{
    private readonly IChatbotLogRepository _chatbotLogRepository;
    private readonly ILogger<GetChatbotLogQueryHandler> _logger;

    public GetChatbotLogQueryHandler(IChatbotLogRepository chatbotLogRepository, ILogger<GetChatbotLogQueryHandler> logger)
    {
        _chatbotLogRepository = chatbotLogRepository;
        _logger = logger;
    }

    public async Task<GetChatbotLogResponse> Handle(GetChatbotLogQuery request, CancellationToken cancellationToken)
    {
        var chatbotLog = await _chatbotLogRepository.GetByIdAsync(request.ChatbotLogId);

        if (chatbotLog == null)
        {
            _logger.LogWarning("ChatbotLog not found with ID: {ChatbotLogId}", request.ChatbotLogId);
            return new GetChatbotLogResponse { ChatbotLog = new ChatbotLogDto() };
        }

        return new GetChatbotLogResponse
        {
            ChatbotLog = new ChatbotLogDto
            {
                Id = chatbotLog.Id,
                UserId = chatbotLog.UserId,
                Message = chatbotLog.Message,
                Response = chatbotLog.Response,
                CreatedAt = chatbotLog.CreatedAt
            }
        };
    }
}
