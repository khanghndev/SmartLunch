using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;

namespace SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLog;

public class GetChatbotLogQuery : IRequest<GetChatbotLogResponse>
{
    public int ChatbotLogId { get; set; }

    public GetChatbotLogQuery(int chatbotLogId)
    {
        ChatbotLogId = chatbotLogId;
    }
}
