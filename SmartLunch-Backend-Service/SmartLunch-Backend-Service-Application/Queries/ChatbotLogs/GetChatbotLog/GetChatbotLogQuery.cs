using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;

namespace SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLog;

public class GetChatbotLogQuery : IRequest<GetChatbotLogResponse>
{
    public Guid ChatbotLogId { get; set; }

    public GetChatbotLogQuery(Guid chatbotLogId)
    {
        ChatbotLogId = chatbotLogId;
    }
}
