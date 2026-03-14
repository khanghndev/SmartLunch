using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLogs;

public class GetChatbotLogsQueryHandler : IRequestHandler<GetChatbotLogsQuery, GetChatbotLogsResponse>
{
    private readonly IChatbotLogRepository _chatbotLogRepository;
    private readonly ILogger<GetChatbotLogsQueryHandler> _logger;

    public GetChatbotLogsQueryHandler(IChatbotLogRepository chatbotLogRepository, ILogger<GetChatbotLogsQueryHandler> logger)
    {
        _chatbotLogRepository = chatbotLogRepository;
        _logger = logger;
    }

    public async Task<GetChatbotLogsResponse> Handle(GetChatbotLogsQuery request, CancellationToken cancellationToken)
    {
        var (chatbotLogs, totalCount) = await _chatbotLogRepository.GetChatbotLogsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var chatbotLogDtos = chatbotLogs.Select(chatbotLog => new ChatbotLogDto
        {
                Id = chatbotLog.Id,
                UserId = chatbotLog.UserId,
                Message = chatbotLog.Message,
                Response = chatbotLog.Response,
                CreatedAt = chatbotLog.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} chatbotlogs (Page {Page}, PageSize {PageSize})",
            chatbotLogDtos.Count, request.Page, request.PageSize);

        return new GetChatbotLogsResponse
        {
            Data = chatbotLogDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
