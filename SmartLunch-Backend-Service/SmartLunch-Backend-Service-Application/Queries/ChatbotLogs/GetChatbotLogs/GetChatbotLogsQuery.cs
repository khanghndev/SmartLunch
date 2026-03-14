using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;

namespace SmartLunch.Backend.Service.Application.Queries.ChatbotLogs.GetChatbotLogs;

public class GetChatbotLogsQuery : IRequest<GetChatbotLogsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetChatbotLogsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
